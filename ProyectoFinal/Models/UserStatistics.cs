using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProyectoFinal.Models;

public class UserStatistics : INotifyPropertyChanged
{
    private string _userId;
    private int _totalGamesPlayed = 0;
    private int _totalGamesWon = 0;
    private double _winPercentage = 0.0;
    private int _totalThrows = 0;
    private int _totalPoints = 0;
    private double _averageScore = 0.0;
    private int _bestScore = 0;
    private double _averageTurnsPerGame = 0.0;
    private int _totalRounds = 0;
    
    // Estadísticas específicas de Cricket
    private int _totalCricketMarks = 0;
    private double _averageCricketMarks = 0.0;
    
    // Estadísticas por modo de juego
    private int _x01GamesPlayed = 0;
    private int _x01GamesWon = 0;
    private int _cricketGamesPlayed = 0;
    private int _cricketGamesWon = 0;
    
    private DateTime _lastGameDate;
    private DateTime _createdDate;

    public UserStatistics()
    {
        _createdDate = DateTime.Now;
        _lastGameDate = DateTime.MinValue;
    }

    public UserStatistics(string userId) : this()
    {
        _userId = userId;
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

    public int TotalGamesPlayed
    {
        get => _totalGamesPlayed;
        private set
        {
            if (_totalGamesPlayed != value)
            {
                _totalGamesPlayed = value;
                OnPropertyChanged();
                UpdateWinPercentage();
                UpdateAverageTurnsPerGame();
            }
        }
    }

    public int TotalGamesWon
    {
        get => _totalGamesWon;
        private set
        {
            if (_totalGamesWon != value)
            {
                _totalGamesWon = value;
                OnPropertyChanged();
                UpdateWinPercentage();
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

    public int TotalThrows
    {
        get => _totalThrows;
        private set
        {
            if (_totalThrows != value)
            {
                _totalThrows = value;
                OnPropertyChanged();
                UpdateAverageScore();
                UpdateAverageCricketMarks();
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
                UpdateAverageScore();
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

    public int BestScore
    {
        get => _bestScore;
        private set
        {
            if (_bestScore != value)
            {
                _bestScore = value;
                OnPropertyChanged();
            }
        }
    }

    public double AverageTurnsPerGame
    {
        get => _averageTurnsPerGame;
        private set
        {
            if (_averageTurnsPerGame != value)
            {
                _averageTurnsPerGame = value;
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
                UpdateAverageTurnsPerGame();
            }
        }
    }

    public int TotalCricketMarks
    {
        get => _totalCricketMarks;
        private set
        {
            if (_totalCricketMarks != value)
            {
                _totalCricketMarks = value;
                OnPropertyChanged();
                UpdateAverageCricketMarks();
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

    public int X01GamesPlayed
    {
        get => _x01GamesPlayed;
        private set
        {
            if (_x01GamesPlayed != value)
            {
                _x01GamesPlayed = value;
                OnPropertyChanged();
            }
        }
    }

    public int X01GamesWon
    {
        get => _x01GamesWon;
        private set
        {
            if (_x01GamesWon != value)
            {
                _x01GamesWon = value;
                OnPropertyChanged();
            }
        }
    }

    public int CricketGamesPlayed
    {
        get => _cricketGamesPlayed;
        private set
        {
            if (_cricketGamesPlayed != value)
            {
                _cricketGamesPlayed = value;
                OnPropertyChanged();
            }
        }
    }

    public int CricketGamesWon
    {
        get => _cricketGamesWon;
        private set
        {
            if (_cricketGamesWon != value)
            {
                _cricketGamesWon = value;
                OnPropertyChanged();
            }
        }
    }

    public DateTime LastGameDate
    {
        get => _lastGameDate;
        private set
        {
            if (_lastGameDate != value)
            {
                _lastGameDate = value;
                OnPropertyChanged();
            }
        }
    }

    public DateTime CreatedDate
    {
        get => _createdDate;
        set
        {
            if (_createdDate != value)
            {
                _createdDate = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    // Métodos para actualizar estadísticas
    public void RecordGamePlayed(GameMode gameMode, bool won, int finalScore, int roundsPlayed)
    {
        TotalGamesPlayed++;
        TotalRounds += roundsPlayed;
        LastGameDate = DateTime.Now;

        if (won)
        {
            TotalGamesWon++;
        }

        // Actualizar mejor puntuación
        if (finalScore > BestScore)
        {
            BestScore = finalScore;
        }

        // Actualizar estadísticas por modo de juego
        if (gameMode == GameMode.X01)
        {
            X01GamesPlayed++;
            if (won) X01GamesWon++;
        }
        else if (gameMode == GameMode.Cricket)
        {
            CricketGamesPlayed++;
            if (won) CricketGamesWon++;
        }
    }

    public void RecordThrows(int points, int cricketMarks = 0)
    {
        TotalThrows++;
        TotalPoints += points;
        TotalCricketMarks += cricketMarks;
    }

    private void UpdateWinPercentage()
    {
        if (TotalGamesPlayed > 0)
        {
            WinPercentage = Math.Round((double)TotalGamesWon / TotalGamesPlayed * 100, 1);
        }
        else
        {
            WinPercentage = 0.0;
        }
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
        if (TotalThrows > 0)
        {
            double rounds = TotalThrows / 3.0;
            AverageCricketMarks = Math.Round(TotalCricketMarks / rounds, 2);
        }
        else
        {
            AverageCricketMarks = 0.0;
        }
    }

    private void UpdateAverageTurnsPerGame()
    {
        if (TotalGamesPlayed > 0)
        {
            AverageTurnsPerGame = Math.Round((double)TotalRounds / TotalGamesPlayed, 1);
        }
        else
        {
            AverageTurnsPerGame = 0.0;
        }
    }

    public void ResetStats()
    {
        TotalGamesPlayed = 0;
        TotalGamesWon = 0;
        WinPercentage = 0.0;
        TotalThrows = 0;
        TotalPoints = 0;
        AverageScore = 0.0;
        BestScore = 0;
        AverageTurnsPerGame = 0.0;
        TotalRounds = 0;
        TotalCricketMarks = 0;
        AverageCricketMarks = 0.0;
        X01GamesPlayed = 0;
        X01GamesWon = 0;
        CricketGamesPlayed = 0;
        CricketGamesWon = 0;
        LastGameDate = DateTime.MinValue;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

