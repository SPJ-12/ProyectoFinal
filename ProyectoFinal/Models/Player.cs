using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProyectoFinal.Models;

public class Player : INotifyPropertyChanged
{
    private string _name;
    private int _score;
    private string _userId; // ID del usuario registrado asociado
    
    // Estadísticas de dardos
    private int _totalThrows = 0;
    private int _totalPoints = 0;
    private double _averageScore = 0.0;
    private int _legsWon = 0;
    private int _legsPlayed = 0;
    private double _winPercentage = 0.0;
    
    // Estadísticas específicas de Cricket
    private int _totalCricketMarks = 0;
    private double _averageCricketMarks = 0.0;
    private int _totalRounds = 0;

    public Player(string name, int initialScore = 501, string userId = null)
    {
        _name = name;
        _score = initialScore;
        _userId = userId;
    }

    public string Name
    {
        get => _name;
        set
        {
            if (_name != value)
            {
                _name = value;
                OnPropertyChanged();
            }
        }
    }

    public int Score
    {
        get => _score;
        set
        {
            if (_score != value)
            {
                _score = value;
                OnPropertyChanged();
            }
        }
    }

    // Estadísticas de dardos
    public int TotalThrows
    {
        get => _totalThrows;
        private set
        {
            if (_totalThrows != value)
            {
                _totalThrows = value;
                OnPropertyChanged();
            }
        }
    }

    public int TotalPoints
    {
        get => _totalPoints;
        private set
        {
            if (_totalPoints != value)
            {
                _totalPoints = value;
                OnPropertyChanged();
            }
        }
    }

    public double AverageScore
    {
        get => _averageScore;
        private set
        {
            if (_averageScore != value)
            {
                _averageScore = value;
                OnPropertyChanged();
            }
        }
    }

    public int LegsWon
    {
        get => _legsWon;
        private set
        {
            if (_legsWon != value)
            {
                _legsWon = value;
                OnPropertyChanged();
            }
        }
    }

    public int LegsPlayed
    {
        get => _legsPlayed;
        private set
        {
            if (_legsPlayed != value)
            {
                _legsPlayed = value;
                OnPropertyChanged();
            }
        }
    }

    public double WinPercentage
    {
        get => _winPercentage;
        private set
        {
            if (_winPercentage != value)
            {
                _winPercentage = value;
                OnPropertyChanged();
            }
        }
    }

    // Estadísticas específicas de Cricket
    public int TotalCricketMarks
    {
        get => _totalCricketMarks;
        private set
        {
            if (_totalCricketMarks != value)
            {
                _totalCricketMarks = value;
                OnPropertyChanged();
            }
        }
    }

    public double AverageCricketMarks
    {
        get => _averageCricketMarks;
        private set
        {
            if (_averageCricketMarks != value)
            {
                _averageCricketMarks = value;
                OnPropertyChanged();
            }
        }
    }

    public int TotalRounds
    {
        get => _totalRounds;
        private set
        {
            if (_totalRounds != value)
            {
                _totalRounds = value;
                OnPropertyChanged();
            }
        }
    }

    public string UserId
    {
        get => _userId;
        set
        {
            if (_userId != value)
            {
                _userId = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsRegisteredUser => !string.IsNullOrEmpty(_userId);

    public event PropertyChangedEventHandler PropertyChanged;

    // Métodos para actualizar estadísticas
    public void AddThrow(int points)
    {
        TotalThrows++;
        TotalPoints += points;
        UpdateAverageScore();
        UpdateAverageCricketMarks(); // Actualizar también la media de marcas de cricket
    }

    public void AddCricketMarks(int marks)
    {
        TotalCricketMarks += marks;
        UpdateAverageCricketMarks();
        
        // Debug: Verificar actualización
        System.Diagnostics.Debug.WriteLine($"Cricket Marks - {Name}: +{marks} marcas, Total: {TotalCricketMarks}, Media: {AverageCricketMarks:F2}");
    }

    public void CompleteRound()
    {
        TotalRounds++;
        UpdateAverageCricketMarks();
        
        // Debug: Verificar actualización de rondas
        System.Diagnostics.Debug.WriteLine($"Ronda completada - {Name}: Total rondas: {TotalRounds}, Media marcas: {AverageCricketMarks:F2}");
    }

    public void RecordLegWin()
    {
        LegsWon++;
        LegsPlayed++;
        UpdateWinPercentage();
    }

    public void RecordLegLoss()
    {
        LegsPlayed++;
        UpdateWinPercentage();
    }

    private void UpdateAverageScore()
    {
        if (TotalThrows > 0)
        {
            AverageScore = Math.Round((double)TotalPoints / TotalThrows, 2);
        }
        else
        {
            AverageScore = 0.0;
        }
    }

    private void UpdateAverageCricketMarks()
    {
        // Calcular media de marcas por ronda (3 lanzamientos)
        // TotalCricketMarks incluye todas las marcas registradas
        // TotalThrows incluye todos los lanzamientos (incluyendo fallidos)
        
        if (TotalThrows > 0)
        {
            // Calcular media de marcas por ronda (3 lanzamientos)
            double rounds = TotalThrows / 3.0;
            AverageCricketMarks = Math.Round(TotalCricketMarks / rounds, 2);
        }
        else
        {
            AverageCricketMarks = 0.0;
        }
    }

    private void UpdateWinPercentage()
    {
        if (LegsPlayed > 0)
        {
            WinPercentage = Math.Round((double)LegsWon / LegsPlayed * 100, 1);
        }
        else
        {
            WinPercentage = 0.0;
        }
    }

    public void ResetStats()
    {
        TotalThrows = 0;
        TotalPoints = 0;
        AverageScore = 0.0;
        LegsWon = 0;
        LegsPlayed = 0;
        WinPercentage = 0.0;
        TotalCricketMarks = 0;
        AverageCricketMarks = 0.0;
        TotalRounds = 0;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}