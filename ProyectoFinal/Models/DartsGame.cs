using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;
using System.Collections.ObjectModel;
using ProyectoFinal.Services;

namespace ProyectoFinal.Models;

public class DartsGame : INotifyPropertyChanged
{
    private readonly int[] CRICKET_NUMBERS = { 0, 15, 16, 17, 18, 19, 20, 25 }; // 25 = Bull
    private readonly int[] CRICKET_CLOSABLE_NUMBERS = { 15, 16, 17, 18, 19, 20, 25 }; // Números que se pueden cerrar (sin el 0)
    private GameMode _gameMode;
    private ObservableCollection<Player> _players = new();
    private ObservableCollection<CricketPlayerInfo> _cricketPlayerInfos = new();
    private int _currentPlayerIndex = 0;
    private Dictionary<string, Dictionary<int, int>> _playerCricketScores = new();
    private Dictionary<string, HashSet<int>> _playerClosedNumbers = new();
    
    // Sistema de registro de lanzamientos
    private Round _currentRound;
    private int _currentRoundNumber = 1;
    
    // Sistema de checkout para X01
    private List<CheckoutCombination> _checkoutCombinations = new();
    
    // Sistema de cooldown
    private bool _isInCooldown = false;
    private int _cooldownSeconds = 0;
    private System.Timers.Timer _cooldownTimer;

    public event PropertyChangedEventHandler PropertyChanged;

    public DartsGame(GameMode mode = GameMode.X01)
    {
        _gameMode = mode;
        // Initialize with 2 default players
        AddPlayer("Jugador 1");
        AddPlayer("Jugador 2");
        InitializeCricketScores();
        StartNewRound();
        
        // Debug: Verificar que CricketPlayerInfos se inicializó
        System.Diagnostics.Debug.WriteLine($"DartsGame creado - Modo: {_gameMode}, Jugadores: {_players.Count}, CricketInfos: {_cricketPlayerInfos.Count}");
        
        // Inicializar la UI de Cricket
        RefreshCricketUI();
    }

    private void InitializeCricketScores()
    {
        foreach (var player in _players)
        {
            _playerCricketScores[player.Name] = new Dictionary<int, int>();
            _playerClosedNumbers[player.Name] = new HashSet<int>();
            foreach (var number in CRICKET_NUMBERS)
            {
                _playerCricketScores[player.Name][number] = 0;
            }
        }
    }

    public GameMode GameMode
    {
        get => _gameMode;
        set
        {
            if (_gameMode != value)
            {
                _gameMode = value;
                ResetGame();
                OnPropertyChanged();
                
                // Si cambiamos a Cricket, asegurar que CricketPlayerInfos esté inicializado
                if (_gameMode == GameMode.Cricket)
                {
                    // Recrear CricketPlayerInfos para todos los jugadores existentes
                    _cricketPlayerInfos.Clear();
                    foreach (var player in _players)
                    {
                        var cricketInfo = new CricketPlayerInfo(player, this);
                        _cricketPlayerInfos.Add(cricketInfo);
                    }
                    System.Diagnostics.Debug.WriteLine($"Modo cambiado a Cricket - CricketInfos recreados: {_cricketPlayerInfos.Count}");
                }
                
                // Actualizar la UI de Cricket cuando cambia el modo de juego
                RefreshCricketUI();
                
                // Notificar cambio en CricketPlayerInfos
                OnPropertyChanged(nameof(CricketPlayerInfos));
            }
        }
    }

    public ObservableCollection<Player> Players
    {
        get => _players;
    }

    public ObservableCollection<CricketPlayerInfo> CricketPlayerInfos
    {
        get => _cricketPlayerInfos;
    }

    public Player CurrentPlayer
    {
        get => _players.Count > 0 ? _players[_currentPlayerIndex] : null;
    }

    public int CurrentPlayerIndex
    {
        get => _currentPlayerIndex;
        private set
        {
            if (_currentPlayerIndex != value)
            {
                _currentPlayerIndex = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CurrentPlayer));
            }
        }
    }


    public Round CurrentRound
    {
        get => _currentRound;
        private set
        {
            if (_currentRound != value)
            {
                _currentRound = value;
                OnPropertyChanged();
            }
        }
    }

    public int CurrentRoundNumber
    {
        get => _currentRoundNumber;
        private set
        {
            if (_currentRoundNumber != value)
            {
                _currentRoundNumber = value;
                OnPropertyChanged();
            }
        }
    }

    public List<CheckoutCombination> CheckoutCombinations
    {
        get => _checkoutCombinations;
        private set
        {
            if (_checkoutCombinations != value)
            {
                _checkoutCombinations = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsInCooldown
    {
        get => _isInCooldown;
        private set
        {
            if (_isInCooldown != value)
            {
                _isInCooldown = value;
                OnPropertyChanged();
            }
        }
    }

    public int CooldownSeconds
    {
        get => _cooldownSeconds;
        private set
        {
            if (_cooldownSeconds != value)
            {
                _cooldownSeconds = value;
                OnPropertyChanged();
            }
        }
    }

    public Dictionary<int, int> GetPlayerCricketScores(string playerName)
    {
        if (!_playerCricketScores.ContainsKey(playerName))
        {
            // Inicializar las puntuaciones si no existen
            _playerCricketScores[playerName] = new Dictionary<int, int>();
            _playerClosedNumbers[playerName] = new HashSet<int>();
            foreach (var number in CRICKET_NUMBERS)
            {
                _playerCricketScores[playerName][number] = 0;
            }
        }
        return _playerCricketScores[playerName];
    }

    public HashSet<int> GetPlayerClosedNumbers(string playerName)
    {
        if (!_playerClosedNumbers.ContainsKey(playerName))
        {
            // Inicializar las puntuaciones si no existen
            _playerCricketScores[playerName] = new Dictionary<int, int>();
            _playerClosedNumbers[playerName] = new HashSet<int>();
            foreach (var number in CRICKET_NUMBERS)
            {
                _playerCricketScores[playerName][number] = 0;
            }
        }
        return _playerClosedNumbers[playerName];
    }

    public bool AddPlayer(string playerName, int initialScore = -1, string userId = null)
    {
        if (_players.Count >= 4)
        {
            return false; // Maximum 4 players
        }

        if (_players.Any(p => p.Name == playerName))
        {
            return false; // Player name already exists
        }

        if (initialScore == -1)
        {
            initialScore = _gameMode == GameMode.X01 ? 501 : 0;
        }
        
        var newPlayer = new Player(playerName, initialScore, userId);
        _players.Add(newPlayer);
        
        // Crear CricketPlayerInfo para la UI
        var cricketInfo = new CricketPlayerInfo(newPlayer, this);
        _cricketPlayerInfos.Add(cricketInfo);
        
        // Initialize cricket scores for new player
        _playerCricketScores[playerName] = new Dictionary<int, int>();
        _playerClosedNumbers[playerName] = new HashSet<int>();
        foreach (var number in CRICKET_NUMBERS)
        {
            _playerCricketScores[playerName][number] = 0;
        }

        OnPropertyChanged(nameof(CurrentPlayer));
        
        // Actualizar la UI de Cricket para mostrar el nuevo jugador
        RefreshCricketUI();
        
        // Notificar cambio en CricketPlayerInfos
        OnPropertyChanged(nameof(CricketPlayerInfos));
        
        return true;
    }

    public bool RemovePlayer(string playerName)
    {
        if (_players.Count <= 2)
        {
            return false; // Minimum 2 players required
        }

        var playerToRemove = _players.FirstOrDefault(p => p.Name == playerName);
        if (playerToRemove == null)
        {
            return false; // Player not found
        }

        var playerIndex = _players.IndexOf(playerToRemove);
        _players.Remove(playerToRemove);
        
        // Remover también el CricketPlayerInfo correspondiente
        var cricketInfoToRemove = _cricketPlayerInfos.FirstOrDefault(c => c.Name == playerName);
        if (cricketInfoToRemove != null)
        {
            _cricketPlayerInfos.Remove(cricketInfoToRemove);
        }
        
        _playerCricketScores.Remove(playerName);
        _playerClosedNumbers.Remove(playerName);

        // Adjust current player index if necessary
        if (_currentPlayerIndex >= _players.Count)
        {
            _currentPlayerIndex = 0;
        }
        else if (_currentPlayerIndex > playerIndex)
        {
            _currentPlayerIndex--;
        }

        OnPropertyChanged(nameof(CurrentPlayer));
        OnPropertyChanged(nameof(CurrentPlayerIndex));
        
        // Actualizar la UI de Cricket después de eliminar el jugador
        RefreshCricketUI();
        
        // Notificar cambio en CricketPlayerInfos
        OnPropertyChanged(nameof(CricketPlayerInfos));
        
        return true;
    }

    public void ClearPlayers()
    {
        _players.Clear();
        _cricketPlayerInfos.Clear();
        _playerCricketScores.Clear();
        _playerClosedNumbers.Clear();
        _currentPlayerIndex = 0;
        
        // Limpiar cooldown si está activo
        if (_cooldownTimer != null)
        {
            _cooldownTimer.Stop();
            _cooldownTimer.Dispose();
            _cooldownTimer = null;
        }
        IsInCooldown = false;
        CooldownSeconds = 0;
        
        // Reiniciar la ronda
        _currentRoundNumber = 1;
        StartNewRound();
        
        OnPropertyChanged(nameof(CurrentPlayer));
        OnPropertyChanged(nameof(CurrentPlayerIndex));
        
        // Actualizar la UI de Cricket después de limpiar jugadores
        RefreshCricketUI();
        
        // Notificar cambio en CricketPlayerInfos
        OnPropertyChanged(nameof(CricketPlayerInfos));
    }

    public void SwitchPlayer()
    {
        if (_players.Count == 0) return;
        
        CurrentPlayerIndex = (_currentPlayerIndex + 1) % _players.Count;
    }

    private void StartNewRound()
    {
        if (CurrentPlayer != null && _players.Count > 0)
        {
            _currentRound = new Round(CurrentPlayer.Name, _currentRoundNumber);
            OnPropertyChanged(nameof(CurrentRound));
            UpdateCheckoutCombinations();
        }
    }

    public bool AddThrow(int number, int marks)
    {
        if (_currentRound == null || _currentRound.IsComplete || IsInCooldown || CurrentPlayer == null)
        {
            return false;
        }

        // Asegurar que las puntuaciones estén inicializadas para el jugador actual
        GetPlayerCricketScores(CurrentPlayer.Name);

        bool success = _currentRound.AddThrow(number, marks);
        
        if (success)
        {
            // Registrar estadísticas del lanzamiento
            RecordThrowStats(number, marks);
            
            // Actualizar combinaciones de checkout para X01
            if (_gameMode == GameMode.X01)
            {
                UpdateCheckoutCombinations();
            }
            
            // Si la ronda está completa, procesar la puntuación y activar cooldown
            if (_currentRound.IsComplete)
            {
                ProcessRoundScore();
                StartCooldown();
            }
        }
        
        return success;
    }

    private void RecordThrowStats(int number, int marks)
    {
        if (CurrentPlayer == null) return;

        // Manejar el caso especial del 50 (bull interior) como 2 marcas de 25
        if (number == 50)
        {
            // Registrar como 2 marcas de 25
            RecordThrowStats(25, 2);
            return;
        }

        // Calcular puntos del lanzamiento
        int points = CalculateThrowPoints(number, marks);
        
        // Registrar estadísticas del jugador
        CurrentPlayer.AddThrow(points);
        
        // Registrar estadísticas para el jugador actual si es un usuario registrado
        if (CurrentPlayer.IsRegisteredUser)
        {
            int cricketMarks = 0;
            if (_gameMode == GameMode.Cricket && CRICKET_NUMBERS.Contains(number))
            {
                // Asegurar que las puntuaciones estén inicializadas
                GetPlayerCricketScores(CurrentPlayer.Name);
                
                // Calcular marcas efectivas para la media
                int previousMarks = GetPlayerCricketScores(CurrentPlayer.Name)[number];
                int marksNeededToClose = 3 - previousMarks;
                
                // Verificar si otros jugadores tienen este número cerrado
                bool allOthersClosed = _players.Where(p => p.Name != CurrentPlayer.Name)
                                              .All(p => GetPlayerCricketScores(p.Name)[number] >= 3);
                
                if (allOthersClosed && marks > marksNeededToClose)
                {
                    // Si el número está cerrado por otros y hay overmarks, solo contar las marcas necesarias
                    cricketMarks = marksNeededToClose;
                }
                else
                {
                    // En caso contrario, contar todas las marcas
                    cricketMarks = marks;
                }
            }
            
            // Registrar estadísticas para este usuario específico
            var userStats = UserService.Instance.GetUserStatistics(CurrentPlayer.UserId);
            if (userStats != null)
            {
                userStats.RecordThrows(points, cricketMarks);
            }
        }
        
        // También registrar para el usuario actual si está logueado (para compatibilidad)
        if (UserService.Instance.IsUserLoggedIn && CurrentPlayer.UserId == UserService.Instance.CurrentUser.Id)
        {
            int cricketMarks = 0;
            if (_gameMode == GameMode.Cricket && CRICKET_NUMBERS.Contains(number))
            {
                // Asegurar que las puntuaciones estén inicializadas
                GetPlayerCricketScores(CurrentPlayer.Name);
                
                // Calcular marcas efectivas para la media
                int previousMarks = GetPlayerCricketScores(CurrentPlayer.Name)[number];
                int marksNeededToClose = 3 - previousMarks;
                
                // Verificar si otros jugadores tienen este número cerrado
                bool allOthersClosed = _players.Where(p => p.Name != CurrentPlayer.Name)
                                              .All(p => GetPlayerCricketScores(p.Name)[number] >= 3);
                
                if (allOthersClosed && marks > marksNeededToClose)
                {
                    // Si el número está cerrado por otros y hay overmarks, solo contar las marcas necesarias
                    cricketMarks = marksNeededToClose;
                }
                else
                {
                    // En caso contrario, contar todas las marcas
                    cricketMarks = marks;
                }
            }
            
            UserService.Instance.RecordThrowsForCurrentUser(points, cricketMarks);
        }
        
        // Registrar marcas específicas para Cricket
        if (_gameMode == GameMode.Cricket && CRICKET_NUMBERS.Contains(number))
        {
            // Asegurar que las puntuaciones estén inicializadas
            GetPlayerCricketScores(CurrentPlayer.Name);
            
            // Calcular marcas efectivas para la media
            int previousMarks = GetPlayerCricketScores(CurrentPlayer.Name)[number];
            int marksNeededToClose = 3 - previousMarks;
            
            // Verificar si otros jugadores tienen este número cerrado
            bool allOthersClosed = _players.Where(p => p.Name != CurrentPlayer.Name)
                                          .All(p => GetPlayerCricketScores(p.Name)[number] >= 3);
            
            int effectiveMarksForAverage;
            if (allOthersClosed && marks > marksNeededToClose)
            {
                // Si el número está cerrado por otros y hay overmarks, solo contar las marcas necesarias
                effectiveMarksForAverage = marksNeededToClose;
            }
            else
            {
                // En caso contrario, contar todas las marcas
                effectiveMarksForAverage = marks;
            }
            
            System.Diagnostics.Debug.WriteLine($"Registrando marcas de Cricket: {number} con {marks} marcas (efectivas para media: {effectiveMarksForAverage})");
            CurrentPlayer.AddCricketMarks(effectiveMarksForAverage);
            
            // Actualizar la UI para mostrar las nuevas marcas
            RefreshCricketUI();
        }
    }

    private int CalculateThrowPoints(int number, int marks)
    {
        if (_gameMode == GameMode.X01)
        {
            // En X01, los puntos son número * marcas
            return number * marks;
        }
        else // Cricket
        {
            // En Cricket, solo contar puntos si el número está cerrado por el jugador
            // y no está cerrado por todos los otros jugadores
            if (CurrentPlayer == null) return 0;
            
            // Asegurar que las puntuaciones estén inicializadas
            GetPlayerCricketScores(CurrentPlayer.Name);
            
            var currentClosedNumbers = GetPlayerClosedNumbers(CurrentPlayer.Name);
            bool allOthersClosed = _players.Where(p => p.Name != CurrentPlayer.Name)
                                          .All(p => GetPlayerCricketScores(p.Name)[number] >= 3);
            
            if (currentClosedNumbers.Contains(number) && !allOthersClosed)
            {
                return GetNumberValue(number) * marks;
            }
            
            return 0; // No se otorgan puntos en Cricket si no se cumplen las condiciones
        }
    }

    private void ProcessRoundScore()
    {
        if (_gameMode == GameMode.X01)
        {
            ProcessX01RoundScore();
        }
        else
        {
            ProcessCricketRoundScore();
        }
    }

    private void ProcessX01RoundScore()
    {
        int roundPoints = _currentRound.TotalPoints;
        
        if (CurrentPlayer != null)
        {
            if (CurrentPlayer.Score - roundPoints < 0)
            {
                // Puntuación inválida - excede el puntaje restante
                return;
            }
            CurrentPlayer.Score -= roundPoints;
        }
    }

    private void ProcessCricketRoundScore()
    {
        // Para Cricket, procesar cada lanzamiento individualmente
        foreach (var dartThrow in _currentRound.Throws)
        {
            if (dartThrow.IsValid)
            {
                // Solo procesar números válidos para Cricket
                if (CRICKET_NUMBERS.Contains(dartThrow.Number))
                {
                    ProcessCricketScore(dartThrow.Number, dartThrow.Marks);
                }
            }
        }
    }

    private void CompleteRound()
    {
        // Registrar que el jugador actual completó una ronda
        if (CurrentPlayer != null)
        {
            CurrentPlayer.CompleteRound();
        }
        
        // Cambiar de jugador y crear nueva ronda
        SwitchPlayer();
        _currentRoundNumber++;
        StartNewRound();
    }

    private void StartCooldown()
    {
        IsInCooldown = true;
        CooldownSeconds = 5;
        
        // Crear timer que se ejecute cada segundo
        _cooldownTimer = new System.Timers.Timer(1000); // 1000ms = 1 segundo
        _cooldownTimer.Elapsed += OnCooldownTick;
        _cooldownTimer.AutoReset = true;
        _cooldownTimer.Start();
    }

    private void OnCooldownTick(object sender, ElapsedEventArgs e)
    {
        // Este método se ejecuta en un hilo diferente, necesitamos usar Dispatcher
        Device.BeginInvokeOnMainThread(() =>
        {
            CooldownSeconds--;
            
            if (CooldownSeconds <= 0)
            {
                // Cooldown terminado
                _cooldownTimer?.Stop();
                _cooldownTimer?.Dispose();
                _cooldownTimer = null;
                
                IsInCooldown = false;
                CompleteRound();
            }
        });
    }

    public void UndoLastThrow()
    {
        if (_currentRound != null && !_currentRound.IsComplete && !IsInCooldown)
        {
            _currentRound.UndoLastThrow();
            
            // Actualizar combinaciones de checkout para X01
            if (_gameMode == GameMode.X01)
            {
                UpdateCheckoutCombinations();
            }
        }
    }


    private bool ProcessCricketScore(int number, int marks)
    {
        if (CurrentPlayer == null) return false;

        // Manejar el caso especial del 50 (bull interior) como 2 marcas de 25
        if (number == 50)
        {
            // Procesar como 2 marcas de 25
            return ProcessCricketScore(25, 2);
        }

        if (!CRICKET_NUMBERS.Contains(number)) return false;

        // Asegurar que las puntuaciones estén inicializadas
        GetPlayerCricketScores(CurrentPlayer.Name);
        
        var currentScores = GetPlayerCricketScores(CurrentPlayer.Name);
        var currentClosedNumbers = GetPlayerClosedNumbers(CurrentPlayer.Name);

        // Verificar si otros jugadores tienen este número cerrado
        bool allOthersClosed = _players.Where(p => p.Name != CurrentPlayer.Name)
                                      .All(p => GetPlayerCricketScores(p.Name)[number] >= 3);

        int previousMarks = currentScores[number];
        int marksNeededToClose = 3 - previousMarks;
        
        // Calcular marcas efectivas y overmarks
        int effectiveMarks = Math.Min(marks, marksNeededToClose);
        int overmarks = Math.Max(0, marks - marksNeededToClose);
        
        // Actualizar marcas (solo hasta 3)
        currentScores[number] = Math.Min(currentScores[number] + effectiveMarks, 3);
        
        // Actualizar la UI para mostrar las nuevas marcas
        RefreshCricketUI();

        // Verificar si el número se acaba de cerrar (llegó exactamente a 3)
        bool justClosed = previousMarks < 3 && currentScores[number] == 3;
        
        // Si el número se acaba de cerrar, agregarlo a los números cerrados
        if (justClosed && CRICKET_CLOSABLE_NUMBERS.Contains(number))
        {
            currentClosedNumbers.Add(number);
            
            // Actualizar la UI para mostrar el número cerrado
            RefreshCricketUI();
        }

        // Procesar puntos por marcas normales (si el número ya estaba cerrado)
        if (currentClosedNumbers.Contains(number) && !justClosed && !allOthersClosed)
        {
            var points = GetNumberValue(number) * effectiveMarks;
            CurrentPlayer.Score += points;
        }

        // Procesar overmarks (marcas extra que se convierten en puntos)
        if (overmarks > 0 && !allOthersClosed)
        {
            var overmarkPoints = GetNumberValue(number) * overmarks;
            CurrentPlayer.Score += overmarkPoints;
            
            // Debug: Mostrar overmarks
            System.Diagnostics.Debug.WriteLine($"Overmarks - {CurrentPlayer.Name}: {number} con {overmarks} marcas extra = {overmarkPoints} puntos");
        }

        return true;
    }

    private string GetNumberDisplayName(int number)
    {
        return number switch
        {
            25 => "Bull exterior",
            50 => "Bull interior",
            _ => number.ToString()
        };
    }

    private int GetNumberValue(int number)
    {
        return number switch
        {
            25 => 25, // Bull exterior vale 25 puntos
            50 => 50, // Bull interior vale 50 puntos
            _ => number
        };
    }

    public bool CheckWinner(out string winner)
    {
        winner = null;
        if (_gameMode == GameMode.X01)
        {
            var winnerPlayer = _players.FirstOrDefault(p => p.Score == 0);
            if (winnerPlayer != null)
            {
                winner = winnerPlayer.Name;
                RecordGameResults(winnerPlayer);
                return true;
            }
        }
        else // Cricket
        {
            var playersWithAllClosed = _players.Where(p => 
                CRICKET_CLOSABLE_NUMBERS.All(n => GetPlayerCricketScores(p.Name)[n] >= 3)).ToList();

            if (playersWithAllClosed.Any())
            {
                // El jugador con la puntuación más alta gana
                var winnerPlayer = playersWithAllClosed.OrderByDescending(p => p.Score).First();
                winner = winnerPlayer.Name;
                RecordGameResults(winnerPlayer);
                return true;
            }
        }
        return false;
    }

    private void RecordGameResults(Player winner)
    {
        // Registrar victoria para el ganador
        winner.RecordLegWin();
        
        // Registrar derrota para todos los otros jugadores
        foreach (var player in _players.Where(p => p != winner))
        {
            player.RecordLegLoss();
        }
        
        // Registrar estadísticas del juego para todos los usuarios registrados en la partida
        foreach (var player in _players.Where(p => p.IsRegisteredUser))
        {
            bool playerWon = player == winner;
            int finalScore = player.Score;
            int roundsPlayed = _currentRoundNumber - 1; // -1 porque _currentRoundNumber ya se incrementó
            
            // Crear estadísticas temporales para este usuario si no están en el servicio
            var userStats = UserService.Instance.GetUserStatistics(player.UserId);
            if (userStats != null)
            {
                userStats.RecordGamePlayed(_gameMode, playerWon, finalScore, roundsPlayed);
            }
        }
        
        // También registrar para el usuario actual si está logueado (para compatibilidad)
        if (UserService.Instance.IsUserLoggedIn)
        {
            var currentUserPlayer = _players.FirstOrDefault(p => p.UserId == UserService.Instance.CurrentUser.Id);
            if (currentUserPlayer != null)
            {
                bool userWon = currentUserPlayer == winner;
                int finalScore = currentUserPlayer.Score;
                int roundsPlayed = _currentRoundNumber - 1;
                
                UserService.Instance.RecordGameForCurrentUser(_gameMode, userWon, finalScore, roundsPlayed);
            }
        }
    }

    public void ResetGame()
    {
        if (_gameMode == GameMode.X01)
        {
            foreach (var player in _players)
            {
                player.Score = 501;
            }
        }
        else
        {
            foreach (var player in _players)
            {
                player.Score = 0;
            }
            InitializeCricketScores();
        }
        CheckoutCombinations.Clear();
        
        // Limpiar cooldown si está activo
        if (_cooldownTimer != null)
        {
            _cooldownTimer.Stop();
            _cooldownTimer.Dispose();
            _cooldownTimer = null;
        }
        IsInCooldown = false;
        CooldownSeconds = 0;
        
        _currentPlayerIndex = 0;
        _currentRoundNumber = 1;
        StartNewRound();
    }

    private void UpdateCheckoutCombinations()
    {
        if (_gameMode != GameMode.X01 || CurrentPlayer == null)
        {
            CheckoutCombinations = new List<CheckoutCombination>();
            return;
        }

        int currentScore = CurrentPlayer.Score;
        
        // Calcular el puntaje restante después de los lanzamientos actuales de la ronda
        int remainingScore = currentScore - _currentRound.TotalPoints;
        
        if (remainingScore <= 0)
        {
            CheckoutCombinations = new List<CheckoutCombination>();
            return;
        }

        var combinations = CheckoutCalculator.GetCheckoutCombinations(remainingScore);
        CheckoutCombinations = combinations.Take(5).ToList(); // Mostrar solo las 5 mejores combinaciones
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void RefreshCricketUI()
    {
        // Solo actualizar si estamos en modo Cricket
        if (_gameMode != GameMode.Cricket)
        {
            System.Diagnostics.Debug.WriteLine("RefreshCricketUI omitido - no es modo Cricket");
            return;
        }
        
        // Debug: Verificar que se está llamando
        System.Diagnostics.Debug.WriteLine($"RefreshCricketUI llamado - Modo: {_gameMode}, Jugadores: {_players.Count}, CricketInfos: {_cricketPlayerInfos.Count}");
        
        // Actualizar todas las instancias de CricketPlayerInfo
        foreach (var cricketInfo in _cricketPlayerInfos)
        {
            cricketInfo.Refresh();
        }
        
        // Debug: Verificar que se está actualizando
        System.Diagnostics.Debug.WriteLine($"Cricket UI actualizada para {_cricketPlayerInfos.Count} jugadores");
    }
}