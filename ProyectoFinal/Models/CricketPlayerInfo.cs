using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProyectoFinal.Models;

public class CricketPlayerInfo : INotifyPropertyChanged
{
    private Player _player;
    private DartsGame _game;
    private readonly int[] CRICKET_NUMBERS = { 0, 15, 16, 17, 18, 19, 20, 25 };
    private readonly int[] CRICKET_CLOSABLE_NUMBERS = { 15, 16, 17, 18, 19, 20, 25 }; // Números que se pueden cerrar (sin el 0)

    // Campos privados para las marcas
    private int _cricketMarks0;
    private int _cricketMarks15;
    private int _cricketMarks16;
    private int _cricketMarks17;
    private int _cricketMarks18;
    private int _cricketMarks19;
    private int _cricketMarks20;
    private int _cricketMarks25;
    private string _closedNumbersText;

    public CricketPlayerInfo(Player player, DartsGame game)
    {
        _player = player;
        _game = game;
        UpdateAllMarks();
    }

    public string Name => _player.Name;

    // Propiedades para mostrar marcas de Cricket en la UI
    public int CricketMarks0 
    { 
        get => _cricketMarks0; 
        private set 
        { 
            if (_cricketMarks0 != value) 
            { 
                _cricketMarks0 = value; 
                OnPropertyChanged(); 
            } 
        } 
    }
    
    public int CricketMarks15 
    { 
        get => _cricketMarks15; 
        private set 
        { 
            if (_cricketMarks15 != value) 
            { 
                _cricketMarks15 = value; 
                OnPropertyChanged(); 
            } 
        } 
    }
    
    public int CricketMarks16 
    { 
        get => _cricketMarks16; 
        private set 
        { 
            if (_cricketMarks16 != value) 
            { 
                _cricketMarks16 = value; 
                OnPropertyChanged(); 
            } 
        } 
    }
    
    public int CricketMarks17 
    { 
        get => _cricketMarks17; 
        private set 
        { 
            if (_cricketMarks17 != value) 
            { 
                _cricketMarks17 = value; 
                OnPropertyChanged(); 
            } 
        } 
    }
    
    public int CricketMarks18 
    { 
        get => _cricketMarks18; 
        private set 
        { 
            if (_cricketMarks18 != value) 
            { 
                _cricketMarks18 = value; 
                OnPropertyChanged(); 
            } 
        } 
    }
    
    public int CricketMarks19 
    { 
        get => _cricketMarks19; 
        private set 
        { 
            if (_cricketMarks19 != value) 
            { 
                _cricketMarks19 = value; 
                OnPropertyChanged(); 
            } 
        } 
    }
    
    public int CricketMarks20 
    { 
        get => _cricketMarks20; 
        private set 
        { 
            if (_cricketMarks20 != value) 
            { 
                _cricketMarks20 = value; 
                OnPropertyChanged(); 
            } 
        } 
    }
    
    public int CricketMarks25 
    { 
        get => _cricketMarks25; 
        private set 
        { 
            if (_cricketMarks25 != value) 
            { 
                _cricketMarks25 = value; 
                OnPropertyChanged(); 
            } 
        } 
    }

    public string ClosedNumbersText 
    { 
        get => _closedNumbersText; 
        private set 
        { 
            if (_closedNumbersText != value) 
            { 
                _closedNumbersText = value; 
                OnPropertyChanged(); 
            } 
        } 
    }

    private int GetCricketMarks(int number)
    {
        if (_game == null) return 0;
        
        var scores = _game.GetPlayerCricketScores(_player.Name);
        return scores.ContainsKey(number) ? scores[number] : 0;
    }

    private string GetClosedNumbersText()
    {
        if (_game == null) return "";
        
        var closedNumbers = _game.GetPlayerClosedNumbers(_player.Name);
        if (closedNumbers.Count == 0) return "Sin números cerrados";
        
        var closedList = closedNumbers.OrderBy(n => n).Select(n => n.ToString()).ToArray();
        return $"Números cerrados: {string.Join(", ", closedList)}";
    }

    private void UpdateAllMarks()
    {
        CricketMarks0 = GetCricketMarks(0);
        CricketMarks15 = GetCricketMarks(15);
        CricketMarks16 = GetCricketMarks(16);
        CricketMarks17 = GetCricketMarks(17);
        CricketMarks18 = GetCricketMarks(18);
        CricketMarks19 = GetCricketMarks(19);
        CricketMarks20 = GetCricketMarks(20);
        CricketMarks25 = GetCricketMarks(25);
        ClosedNumbersText = GetClosedNumbersText();
    }

    public void Refresh()
    {
        // Actualizar todas las marcas desde el juego
        UpdateAllMarks();
        
        // Debug: Verificar que se está actualizando
        System.Diagnostics.Debug.WriteLine($"CricketPlayerInfo actualizado para {_player.Name}: 0={CricketMarks0}, 15={CricketMarks15}, 20={CricketMarks20}");
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
