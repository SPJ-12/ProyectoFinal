using System.Collections.ObjectModel;
using System.Text.Json;
using ProyectoFinal.Models;

namespace ProyectoFinal.Services;

public class UserService
{
    private static UserService _instance;
    private static readonly object _lock = new object();
    
    private ObservableCollection<User> _users;
    private ObservableCollection<UserStatistics> _userStatistics;
    private User _currentUser;
    
    private const string USERS_FILE = "users.json";
    private const string STATISTICS_FILE = "user_statistics.json";

    private UserService()
    {
        _users = new ObservableCollection<User>();
        _userStatistics = new ObservableCollection<UserStatistics>();
        LoadUsers();
        LoadUserStatistics();
    }

    public static UserService Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new UserService();
                    }
                }
            }
            return _instance;
        }
    }

    public ObservableCollection<User> Users => _users;
    public ObservableCollection<UserStatistics> UserStatistics => _userStatistics;
    public User CurrentUser => _currentUser;

    public bool IsUserLoggedIn => _currentUser != null;

    public bool RegisterUser(string username, string email)
    {
        // Validar que el usuario no exista
        if (_users.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) || 
                           u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        var newUser = new User(username, email);
        _users.Add(newUser);
        
        // Crear estadísticas para el nuevo usuario
        var userStats = new UserStatistics(newUser.Id);
        _userStatistics.Add(userStats);
        
        SaveUsers();
        SaveUserStatistics();
        
        return true;
    }

    public bool LoginUser(string username)
    {
        var user = _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (user != null && user.IsActive)
        {
            _currentUser = user;
            return true;
        }
        return false;
    }

    public void LogoutUser()
    {
        _currentUser = null;
    }

    public UserStatistics GetCurrentUserStatistics()
    {
        if (_currentUser == null) return null;
        
        return _userStatistics.FirstOrDefault(s => s.UserId == _currentUser.Id);
    }

    public UserStatistics GetUserStatistics(string userId)
    {
        return _userStatistics.FirstOrDefault(s => s.UserId == userId);
    }

    public void RecordGameForCurrentUser(GameMode gameMode, bool won, int finalScore, int roundsPlayed)
    {
        var stats = GetCurrentUserStatistics();
        if (stats != null)
        {
            stats.RecordGamePlayed(gameMode, won, finalScore, roundsPlayed);
            SaveUserStatistics();
        }
    }

    public void RecordThrowsForCurrentUser(int points, int cricketMarks = 0)
    {
        var stats = GetCurrentUserStatistics();
        if (stats != null)
        {
            stats.RecordThrows(points, cricketMarks);
            SaveUserStatistics();
        }
    }

    public void RecordGameForUser(string userId, GameMode gameMode, bool won, int finalScore, int roundsPlayed)
    {
        var stats = GetUserStatistics(userId);
        if (stats != null)
        {
            System.Diagnostics.Debug.WriteLine($"Recording game for user {userId}: Mode={gameMode}, Won={won}, Score={finalScore}, Rounds={roundsPlayed}");
            stats.RecordGamePlayed(gameMode, won, finalScore, roundsPlayed);
            SaveUserStatistics();
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"Warning: Could not find statistics for user {userId}");
        }
    }

    public void RecordThrowsForUser(string userId, int points, int cricketMarks = 0)
    {
        var stats = GetUserStatistics(userId);
        if (stats != null)
        {
            System.Diagnostics.Debug.WriteLine($"Recording throw for user {userId}: Points={points}, CricketMarks={cricketMarks}");
            stats.RecordThrows(points, cricketMarks);
            SaveUserStatistics();
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"Warning: Could not find statistics for user {userId}");
        }
    }

    public bool DeleteUser(string userId)
    {
        var user = _users.FirstOrDefault(u => u.Id == userId);
        if (user != null)
        {
            _users.Remove(user);
            
            // Eliminar estadísticas del usuario
            var stats = _userStatistics.FirstOrDefault(s => s.UserId == userId);
            if (stats != null)
            {
                _userStatistics.Remove(stats);
            }
            
            // Si el usuario eliminado es el actual, cerrar sesión
            if (_currentUser?.Id == userId)
            {
                _currentUser = null;
            }
            
            SaveUsers();
            SaveUserStatistics();
            return true;
        }
        return false;
    }

    private void LoadUsers()
    {
        try
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, USERS_FILE);
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                var users = JsonSerializer.Deserialize<List<User>>(json);
                if (users != null)
                {
                    foreach (var user in users)
                    {
                        _users.Add(user);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading users: {ex.Message}");
        }
    }

    private void SaveUsers()
    {
        try
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, USERS_FILE);
            var json = JsonSerializer.Serialize(_users.ToList(), new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving users: {ex.Message}");
        }
    }

    private void LoadUserStatistics()
    {
        try
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, STATISTICS_FILE);
            System.Diagnostics.Debug.WriteLine($"Loading statistics from: {filePath}");
            
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                System.Diagnostics.Debug.WriteLine($"Statistics file content length: {json.Length} characters");
                
                var options = new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    IncludeFields = false
                };
                var statistics = JsonSerializer.Deserialize<List<UserStatistics>>(json, options);
                if (statistics != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Loaded {statistics.Count} user statistics");
                    foreach (var stat in statistics)
                    {
                        _userStatistics.Add(stat);
                        System.Diagnostics.Debug.WriteLine($"Loaded stats for user: {stat.UserId}, Games: {stat.TotalGamesPlayed}, Throws: {stat.TotalThrows}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Deserialized statistics list is null");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Statistics file does not exist at: {filePath}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading user statistics: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    private void SaveUserStatistics()
    {
        try
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, STATISTICS_FILE);
            System.Diagnostics.Debug.WriteLine($"Saving {_userStatistics.Count} statistics to: {filePath}");
            
            var options = new JsonSerializerOptions 
            { 
                WriteIndented = true,
                IncludeFields = false
            };
            var json = JsonSerializer.Serialize(_userStatistics.ToList(), options);
            
            // Asegurar que el directorio existe
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            File.WriteAllText(filePath, json);
            System.Diagnostics.Debug.WriteLine($"Statistics saved successfully. File size: {new FileInfo(filePath).Length} bytes");
            
            // Log de cada estadística guardada
            foreach (var stat in _userStatistics)
            {
                System.Diagnostics.Debug.WriteLine($"Saved stats for user: {stat.UserId}, Games: {stat.TotalGamesPlayed}, Throws: {stat.TotalThrows}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving user statistics: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}