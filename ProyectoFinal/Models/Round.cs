using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProyectoFinal.Models;

public class Round : INotifyPropertyChanged
{
    private readonly DartThrow[] _throws;
    private int _currentThrowIndex;
    private string _playerName;
    private int _roundNumber;

    public Round(string playerName, int roundNumber)
    {
        _playerName = playerName;
        _roundNumber = roundNumber;
        _throws = new DartThrow[3];
        _currentThrowIndex = 0;
        
        // Inicializar los tres lanzamientos
        for (int i = 0; i < 3; i++)
        {
            _throws[i] = new DartThrow();
        }
    }

    public string PlayerName
    {
        get => _playerName;
        set
        {
            if (_playerName != value)
            {
                _playerName = value;
                OnPropertyChanged();
            }
        }
    }

    public int RoundNumber
    {
        get => _roundNumber;
        set
        {
            if (_roundNumber != value)
            {
                _roundNumber = value;
                OnPropertyChanged();
            }
        }
    }

    public DartThrow[] Throws => _throws;

    public DartThrow Throw1 => _throws[0];
    public DartThrow Throw2 => _throws[1];
    public DartThrow Throw3 => _throws[2];

    public int CurrentThrowIndex
    {
        get => _currentThrowIndex;
        private set
        {
            if (_currentThrowIndex != value)
            {
                _currentThrowIndex = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentThrow));
                OnPropertyChanged(nameof(IsComplete));
            }
        }
    }

    public DartThrow CurrentThrow => _currentThrowIndex < 3 ? _throws[_currentThrowIndex] : null;

    public bool IsComplete => _currentThrowIndex >= 3;

    public int TotalPoints
    {
        get
        {
            int total = 0;
            foreach (var dartThrow in _throws)
            {
                if (dartThrow.IsValid)
                {
                    total += dartThrow.Points;
                }
            }
            return total;
        }
    }

    public string DisplayText
    {
        get
        {
            if (IsComplete)
            {
                return $"Ronda {_roundNumber}: {TotalPoints} puntos";
            }
            else
            {
                return $"Ronda {_roundNumber}: En progreso";
            }
        }
    }

    public bool AddThrow(int number, int marks)
    {
        if (IsComplete) return false;

        var currentThrow = _throws[_currentThrowIndex];
        currentThrow.Number = number;
        currentThrow.Marks = marks;

        if (currentThrow.IsValid)
        {
            CurrentThrowIndex++;
            OnPropertyChanged(nameof(TotalPoints));
            OnPropertyChanged(nameof(DisplayText));
            return true;
        }
        return false;
    }

    public void ClearCurrentThrow()
    {
        if (!IsComplete)
        {
            _throws[_currentThrowIndex].Clear();
        }
    }

    public void UndoLastThrow()
    {
        if (_currentThrowIndex > 0)
        {
            _currentThrowIndex--;
            _throws[_currentThrowIndex].Clear();
            OnPropertyChanged(nameof(TotalPoints));
            OnPropertyChanged(nameof(DisplayText));
        }
    }

    public void ClearRound()
    {
        for (int i = 0; i < 3; i++)
        {
            _throws[i].Clear();
        }
        _currentThrowIndex = 0;
        OnPropertyChanged(nameof(TotalPoints));
        OnPropertyChanged(nameof(DisplayText));
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

