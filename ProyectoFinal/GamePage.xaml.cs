using ProyectoFinal.Models;

namespace ProyectoFinal;

public partial class GamePage : ContentPage
{
    private DartsGame _game;
    private List<User> _selectedUsers;

    public GamePage()
    {
        InitializeComponent();
        _selectedUsers = new List<User>();
        StartNewGame();
        
        // Subscribe to X01 variant picker changes
        if (X01VariantPicker != null)
        {
            X01VariantPicker.SelectedIndexChanged += OnX01VariantChanged;
        }
    }

    public GamePage(List<User> selectedUsers)
    {
        InitializeComponent();
        _selectedUsers = selectedUsers ?? new List<User>();
        StartNewGame();
        
        // Subscribe to X01 variant picker changes
        if (X01VariantPicker != null)
        {
            X01VariantPicker.SelectedIndexChanged += OnX01VariantChanged;
        }
    }
    
    private void OnX01VariantChanged(object sender, EventArgs e)
    {
        if (X01VariantPicker?.SelectedItem is int selectedScore && _game != null)
        {
            // Update the X01 initial score
            _game.X01InitialScore = selectedScore;
        }
    }

    private void StartNewGame()
    {
        var currentMode = _game?.GameMode ?? GameMode.X01;
        var currentX01Score = _game?.X01InitialScore ?? 501;
        _game = new DartsGame(currentMode);
        
        // Configurar la puntuación inicial de X01 ANTES de agregar jugadores
        if (currentMode == GameMode.X01)
        {
            _game.X01InitialScore = currentX01Score;
        }
        
        // Limpiar jugadores existentes y agregar los usuarios seleccionados
        _game.ClearPlayers();
        
        // Agregar jugadores con la puntuación correcta
        int initialScoreForNewPlayers = currentMode == GameMode.X01 ? currentX01Score : 0;
        
        if (_selectedUsers.Any())
        {
            foreach (var user in _selectedUsers)
            {
                _game.AddPlayer(user.Username, initialScoreForNewPlayers, user.Id);
            }
        }
        else
        {
            // Si no hay usuarios seleccionados, usar jugadores por defecto
            _game.AddPlayer("Jugador 1", initialScoreForNewPlayers);
            _game.AddPlayer("Jugador 2", initialScoreForNewPlayers);
        }
        
        BindingContext = _game;
        
        // Debug: Verificar que los jugadores se crearon
        System.Diagnostics.Debug.WriteLine($"Jugadores creados con puntuación inicial {initialScoreForNewPlayers}: {_game.Players.Count}");
        foreach (var player in _game.Players)
        {
            System.Diagnostics.Debug.WriteLine($"- {player.Name}: {player.Score}, Usuario ID: {player.UserId}, Media Cricket: {player.AverageCricketMarks:F2}, Rondas: {player.TotalRounds}");
        }
    }

    private async void OnX01ThrowSubmitted(object sender, EventArgs e)
    {
        if (X01NumberPicker.SelectedItem == null || X01MarksPicker.SelectedItem == null)
        {
            await DisplayAlert("Error", "Por favor seleccione un número y cantidad de marcas", "OK");
            return;
        }

        int number = (int)X01NumberPicker.SelectedItem;
        int marks = (int)X01MarksPicker.SelectedItem;

        if (!_game.AddThrow(number, marks))
        {
            await DisplayAlert("Error", "Lanzamiento no válido", "OK");
            return;
        }

        // Limpiar los pickers
        X01NumberPicker.SelectedItem = null;
        X01MarksPicker.SelectedItem = null;

        await CheckWinnerAndProceed();
    }

    private async void OnCricketThrowSubmitted(object sender, EventArgs e)
    {
        if (CricketNumberPicker.SelectedItem == null || CricketMarksPicker.SelectedItem == null)
        {
            await DisplayAlert("Error", "Por favor seleccione un número y cantidad de marcas", "OK");
            return;
        }

        int number = (int)CricketNumberPicker.SelectedItem;
        int marks = (int)CricketMarksPicker.SelectedItem;

        if (!_game.AddThrow(number, marks))
        {
            await DisplayAlert("Error", "Lanzamiento no válido", "OK");
            return;
        }

        // Limpiar los pickers
        CricketNumberPicker.SelectedItem = null;
        CricketMarksPicker.SelectedItem = null;

        await CheckWinnerAndProceed();
    }

    private async Task CheckWinnerAndProceed()
    {
        if (_game.CheckWinner(out string winner))
        {
            await DisplayAlert("¡Fin del juego!", $"¡{winner} ha ganado!", "OK");
            StartNewGame();
            return;
        }

        // No necesitamos cambiar de jugador manualmente ya que el juego lo hace automáticamente
        // cuando se completa una ronda
    }

    private void OnUndoLastThrow(object sender, EventArgs e)
    {
        _game.UndoLastThrow();
    }

    private void OnNewGameClicked(object sender, EventArgs e)
    {
        StartNewGame();
    }

    private async void OnAddPlayerClicked(object sender, EventArgs e)
    {
        string playerName = NewPlayerNameEntry.Text?.Trim();
        
        if (string.IsNullOrEmpty(playerName))
        {
            await DisplayAlert("Error", "Por favor ingrese un nombre de jugador", "OK");
            return;
        }

        if (!_game.AddPlayer(playerName))
        {
            if (_game.Players.Count >= 4)
            {
                await DisplayAlert("Error", "Máximo 4 jugadores permitidos", "OK");
            }
            else
            {
                await DisplayAlert("Error", "Ya existe un jugador con ese nombre", "OK");
            }
            return;
        }

        NewPlayerNameEntry.Text = "";
    }

    private async void OnRemovePlayerClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string playerName)
        {
            if (_game.Players.Count <= 2)
            {
                await DisplayAlert("Error", "Mínimo 2 jugadores requeridos", "OK");
                return;
            }

            bool confirm = await DisplayAlert("Confirmar", 
                $"¿Está seguro de que desea eliminar a {playerName}?", 
                "Sí", "No");

            if (confirm)
            {
                if (!_game.RemovePlayer(playerName))
                {
                    await DisplayAlert("Error", "No se pudo eliminar el jugador", "OK");
                }
            }
        }
    }

    private async void OnSelectUsersClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PlayerSelectionPage());
    }
}