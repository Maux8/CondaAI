using Conda_AI.Database;
using Conda_AI.Model;
using Newtonsoft.Json;

namespace Conda_AI.Service;

public class UserService
{
    public User? GetUser()
    {
        var filePath = Constants.GetUserFilePath();
        if (!File.Exists(filePath)) return null;
        var json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<User>(json);
    }

    public async Task<UserPreferences> GetPreferencesAsync()
    {
        return null;
    }

    public async Task SavePreferencesAsync(UserPreferences prefs)
    {
    }
}