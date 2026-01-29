using System.Diagnostics;
using Conda_AI.AIAPI;
using Conda_AI.AIAPI.Ollama;
using Conda_AI.Model;
using Conda_AI.Model.Enums;
using Newtonsoft.Json;

namespace Conda_AI.Service;

public class ChatService(OpenAiService apiService, DatabaseService databaseService, UserService userService)
{
    private readonly string _prompt =
        "The context is a mobile app where people can create and manage their tasks using AI through chat-based input. As the AI assistant, you will receive natural language inputs from users describing what they want to do. " +
        "The date of today is " + System.DateTime.Now + " and the user name is " + userService.GetUser()?.Name +
        ". Your job is to analyze the intent, extract relevant details, and return a structured response that allows the system to take action. " +
        "You must always return a JSON object with the following top-level structure: { \"intent\": \"<IntentType>\", \"task\": { ... } } or { \"intent\": \"<IntentType>\", \"update\": { ... } } or { \"intent\": \"<IntentType>\", \"answer\": \"...\" } Supported intents: - \"TaskCreate\" – User wants to create a new task. - \"TaskUpdate\" – User wants to update an existing task. - \"SimpleMessage\" – User asks a general question or just writes a normal message that doesn't require DB actions. For TaskCreate: Return a task object based on the following model. " +
        "Task Model (C#): public class TaskModel\n    {\n        [PrimaryKey, AutoIncrement]\n        public int TaskModel_Id { get; set; }\n\n        private string _name;\n        private string _description;\n        \n        [NotNull]\n        public string Name\n        {\n            get => _name;\n            set\n            {\n                _name = value;\n                NormalizedName = value.ToLower();\n            }\n        }\n\n        [NotNull]\n        public string Description\n        {\n            get => _description;\n            set\n            {\n                _description = value;\n                NormalizedDescription = value.ToLower();\n            }\n        }\n\n        [NotNull]\n        public TaskStatusConda Status { get; set; } = TaskStatusConda.ToDo;\n        [NotNull]\n        public TaskPriority Priority { get; set; } = TaskPriority.Mittel;\n        public int Category_Id { get; set; }\n\n        public DateTime DueDate { get; set; }\n        public string Location { get; set; }\n        public int EstimatedEffort { get; set; }\n        \n        // Name in lower case letters for search\n        public string NormalizedName { get; set; }\n\n        // Description in lower case letters for search\n        public string NormalizedDescription { get; set; }\n        \n        private TaskCategory? _category;\n\n        [Ignore]\n        public TaskCategory? Category\n        {\n            get => _category;\n            set\n            {\n                _category = value;\n                if (value != null)\n                    Category_Id = value.TaskCategory_Id;\n            }\n        }\n    } Enums: public enum TaskPriority { Hoch, Mittel, Gering } public enum TaskStatusConda { ToDo, InProgress, Done } If a field is not provided, use the default value. If the task includes a category, include the correct Category_Id. For TaskUpdate: Use this when the user wants to change something about an existing task. Since users may not reference task IDs directly, provide a safe structured update object (not raw SQL). TaskUpdate format: { \"updates\": { \"field_name\": \"new_value\" }, \"target\": { \"match\": { \"field\": \"title\", \"operator\": \"ilike\", \"value\": \"project meeting\" } } } - updates: Fields to change. - target.match: How to identify the task (e.g. fuzzy match on title). Never return raw SQL. Return this object so the backend can construct a safe parameterized query. Never allow empty or unsafe WHERE clauses. For target.match.field, use the properties NormalizedName and NormalizedDescription instead of Name and Description for a more effective search. For SimpleMessage: When the user asks a general or informational question, return an answer in this form: { \"intent\": \"SimpleMessage\", \"answer\": \"Your response here.\" } Examples include “What does high priority mean?”, “How many tasks can I create?”, “Hello” or “What's the default status of a task?” etc. " +
        "Example 1 – TaskCreate. User input: I have a meeting in 3 days in Berlin for discussion about a project, create the corresponding to-do for it. Expected output: { \"intent\": \"TaskCreate\", \"task\": { \"Name\": \"Meeting about project\", \"Description\": \"Meeting in Berlin to discuss a project\", \"Status\": \"ToDo\", \"Priority\": \"Mittel\", \"Category_Id\": 0, \"DueDate\": \"2025-07-06T00:00:00\", \"Location\": \"Berlin\", \"EstimatedEffort\": 0 } } Example 2 – TaskUpdate. User input: Change the due date of the project meeting to next Monday. Expected output: { \"intent\": \"TaskUpdate\", \"update\": { \"updates\": { \"DueDate\": \"2025-07-07T00:00:00\" }, \"target\": { \"match\": { \"field\": \"title\", \"operator\": \"ilike\", \"value\": \"project meeting\" } } } } Example 3 – SimpleMessage. User input: What does high priority mean in this app? Expected output: { \"intent\": \"SimpleMessage\", \"answer\": \"Tasks with 'high priority' (Hoch) should be handled as soon as possible. They are typically urgent or very important.\" }. " +
        "Also, always ouput the Location as a combination of the region and the actual place (like Berlin Hellersdorf) and when trying to output the update payload, convert the location (if given) to the same format to ease search. " +
        "The Prompt that you should analyze and respond to is the previous message to this one";

    public event EventHandler<string> OnMessageReceived;
    public event EventHandler<TaskModel> OnTaskCreated;
    public event EventHandler<TaskModel> OnTaskUpdated;

    public async Task GetAiResponse(List<OllamaApiMessage> messages, string discussionContext = null)
    {
        messages.Add(new()
        {
            Content = _prompt,
            Role = "system"
        });

        var prompt = _prompt + JsonConvert.SerializeObject(messages);
        string resultMessage;

        try
        {
            var response = await apiService.ExecuteRequest("/v1/responses", new()
            {
                Apikey = DotNetEnv.Env.GetString("OPEN_AI_KEY"),
                ExtraOptions = new()
                {
                    ["input"] = prompt
                }
            });

            // Safely extract the AI message content
            var aiOutput = response.output?.FirstOrDefault();
            var aiContent = aiOutput?.content?.FirstOrDefault();
            var text = aiContent?.text;

            if (string.IsNullOrWhiteSpace(text))
            {
                OnMessageReceived?.Invoke(this, "Etwas ist schief gelaufen. Versuchen Sie es erneut.");
                return;
            }

            var resMessageContent = JsonConvert.DeserializeObject<ApiMessageContent>(text);

            Debug.WriteLine(aiOutput?.content);

            switch (resMessageContent?.Intent)
            {
                case ApiResponseIntent.SimpleMessage:
                    if (resMessageContent.Answer != null)
                        OnMessageReceived?.Invoke(this, resMessageContent.Answer);
                    break;
                case ApiResponseIntent.TaskCreate:
                    await databaseService.Tasks.CreateAsync(resMessageContent.Task);
                    OnMessageReceived?.Invoke(this, "Task wurde erfolgreich erstellt");
                    OnTaskCreated?.Invoke(this, resMessageContent.Task);
                    break;
                case ApiResponseIntent.TaskUpdate:
                    await ProcessTaskUpdate(resMessageContent.Update);
                    break;
                default:
                    OnMessageReceived?.Invoke(this, "Etwas ist schief gelaufen. Versuchen Sie es erneut.");
                    break;
            }
        }
        catch (ApiRequestException e)
        {
            Debug.WriteLine(e.ExceptionData);
            resultMessage = "Etwas ist schief gelaufen. Versuchen Sie es erneut.";
            OnMessageReceived?.Invoke(this, resultMessage);
        }
    }

    private async Task ProcessTaskUpdate(UpdatePayload updatePayload)
    {
        var searchValue = updatePayload.Target.Match.Value.ToLower();

        var matchingTasks = await databaseService.Connection.QueryAsync<TaskModel>(
            @"SELECT * FROM Task WHERE NormalizedName LIKE ? OR NormalizedDescription LIKE ? OR Location LIKE ?",
            $"%{searchValue}%"
        );
        
        var task = matchingTasks?.FirstOrDefault();

        if (task != null)
        {
            foreach (var update in updatePayload.Updates)
            {
                ApplyFieldUpdate(task, update.Key, update.Value);
            }

            await databaseService.Tasks.UpdateAsync(task);
        }

        if (task == null)
        {
            OnMessageReceived?.Invoke(this, "Task konnte nicht gefunden werden.");
        }
        else
        {
            OnMessageReceived?.Invoke(this, "Task wurde erfolgreich aktualisiert.");
            OnTaskUpdated?.Invoke(this, matchingTasks.First());
        }
    }

    private void ApplyFieldUpdate(TaskModel task, string fieldName, object newValue)
    {
        switch (fieldName.ToLower())
        {
            case "duedate":
                if (DateTime.TryParse(newValue.ToString(), out var dueDate))
                    task.DueDate = dueDate;
                break;
            case "priority":
                if (Enum.TryParse(typeof(TaskPriority), newValue.ToString(), out var priority))
                    task.Priority = (TaskPriority)priority;
                break;
            case "status":
                if (Enum.TryParse(typeof(TaskStatusConda), newValue.ToString(), out var status))
                    task.Status = (TaskStatusConda)status;
                break;
            case "location":
                task.Location = newValue.ToString();
                break;
            case "description":
                task.Description = newValue.ToString();
                break;
            case "name":
            case "title":
                task.Name = newValue.ToString();
                break;
            // Add more fields as needed
        }
    }
}