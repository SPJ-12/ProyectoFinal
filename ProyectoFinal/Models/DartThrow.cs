using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProyectoFinal.Models;

public class DartThrow : INotifyPropertyChanged
{
    private int _number;
    private int _marks;
    private bool _isValid;

    public DartThrow(int number = 0, int marks = 0)
    {
        _number = number;
        _marks = marks;
        _isValid = false;
    }

    public int Number
    {
        get => _number;
        set
        {
            if (_number != value)
            {
                _number = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Points));
                OnPropertyChanged(nameof(DisplayText));
                ValidateThrow();
            }
        }
    }

    public int Marks
    {
        get => _marks;
        set
        {
            if (_marks != value)
            {
                _marks = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Points));
                OnPropertyChanged(nameof(DisplayText));
                ValidateThrow();
            }
        }
    }

    public bool IsValid
    {
        get => _isValid;
        private set
        {
            if (_isValid != value)
            {
                _isValid = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Points));
                OnPropertyChanged(nameof(DisplayText));
            }
        }
    }

    public string DisplayText
    {
        get
        {
            if (!IsValid) return "Sin lanzar";
            
        if (_number == 0) return "Fallido";
        if (_number == 25) return $"Bull exterior ({_marks})";
        if (_number == 50) return $"Bull interior ({_marks})";
            
            string markText = _marks switch
            {
                1 => "Simple",
                2 => "Doble", 
                3 => "Triple",
                _ => _marks.ToString()
            };
            
            return $"{_number} - {markText}";
        }
    }

    public int Points
    {
        get
        {
            if (!IsValid) return 0;
            return _number * _marks;
        }
    }

    private void ValidateThrow()
    {
        // Validar que el número esté en el rango válido (1-20, 25 para bull exterior, 50 para bull interior, 0 para fallido)
        bool validNumber = (_number >= 0 && _number <= 20) || _number == 25 || _number == 50;
        
        // Validar que las marcas estén en el rango válido (1-3)
        bool validMarks = _marks >= 1 && _marks <= 3;
        
        // Para bull exterior (25), solo permitir marcas 1 o 2
        if (_number == 25)
        {
            validMarks = _marks == 1 || _marks == 2;
        }
        
        // Para bull interior (50), solo permitir marcas 1 o 2
        if (_number == 50)
        {
            validMarks = _marks == 1 || _marks == 2;
        }
        
        // Para fallido (número 0), las marcas deben ser 1
        if (_number == 0)
        {
            validMarks = _marks == 1;
        }
        
        IsValid = validNumber && validMarks;
    }

    public void Clear()
    {
        Number = 0;
        Marks = 0;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
