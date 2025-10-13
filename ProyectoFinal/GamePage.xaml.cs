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
    }

    public GamePage(List<User> selectedUsers)
    {
        InitializeComponent();
        _selectedUsers = selectedUsers ?? new List<User>();
        StartNewGame();
    }

    private void StartNewGame()
    {
        var currentMode = _game?.GameMode ?? GameMode.X01;
        _game = new DartsGame(currentMode);
        
        // Limpiar jugadores existentes y agregar los usuarios seleccionados
        _game.ClearPlayers();
        
        if (_selectedUsers.Any())
        {
            foreach (var user in _selectedUsers)
            {
                int initialScore = currentMode == GameMode.X01 ? 501 : 0;
                _game.AddPlayer(user.Username, initialScore, user.Id);
            }
        }
        else
        {
            // Si no hay usuarios seleccionados, usar jugadores por defecto
            _game.AddPlayer("Jugador 1");
            _game.AddPlayer("Jugador 2");
        }
        
        BindingContext = _game;
        
        // Debug: Verificar que los jugadores se crearon
        System.Diagnostics.Debug.WriteLine($"Jugadores creados: {_game.Players.Count}");
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