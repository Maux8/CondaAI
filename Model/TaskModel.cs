using CommunityToolkit.Mvvm.ComponentModel;
using Conda_AI.Model.Enums;
using SQLite;

namespace Conda_AI.Model
{
    [Table(name: "Task")]
    public class TaskModel
    {
        [PrimaryKey, AutoIncrement]
        public int TaskModel_Id { get; set; }

        private string _name;
        private string _description;
        
        [NotNull]
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NormalizedName = value.ToLower();
            }
        }

        [NotNull]
        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                NormalizedDescription = value.ToLower();
            }
        }

        [NotNull]
        public TaskStatusConda Status { get; set; } = TaskStatusConda.ToDo;
        [NotNull]
        public TaskPriority Priority { get; set; } = TaskPriority.Mittel;
        public int Category_Id { get; set; }
        public DateTime DueDate { get; set; }
        public string Location { get; set; }
        public int EstimatedEffort { get; set; }
        
        // Name in lower case letters for search
        public string NormalizedName { get; set; }

        // Description in lower case letters for search
        public string NormalizedDescription { get; set; }

        public TaskCategoryEnum Category { get; set; }


    }
}
