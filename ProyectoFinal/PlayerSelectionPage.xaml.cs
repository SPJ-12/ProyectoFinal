using System.Collections.ObjectModel;
using ProyectoFinal.Services;
using ProyectoFinal.Models;

namespace ProyectoFinal;

public partial class PlayerSelectionPage : ContentPage
{
    private UserService _userService;
    private ObservableCollection<User> _availableUsers;
    private ObservableCollection<User> _selectedPlayers;
    private bool _canStartGame = false;

    public PlayerSelectionPage()
    {
        InitializeComponent();
        _userService = UserService.Instance;
        _availableUsers = new ObservableCollection<User>();
        _selectedPlayers = new ObservableCollection<User>();
        
        BindingContext = this;
        LoadAvailableUsers();
    }

    public ObservableCollection<User> AvailableUsers => _availableUsers;
    public ObservableCollection<User> SelectedPlayers => _selectedPlayers;
    
    public bool CanStartGame
    {
        get => _canStartGame;
        set
        {
            if (_canStartGame != value)
            {
                _canStartGame = value;
                OnPropertyChanged();
            }
        }
    }

    private void LoadAvailableUsers()
    {
        _availableUsers.Clear();
        
        // Cargar usuarios que no están ya seleccionados
        foreach (var user in _userService.Users)
        {
            if (!_selectedPlayers.Any(p => p.Id == user.Id))
            {
                _availableUsers.Add(user);
            }
        }
        
        UpdateCanStartGame();
    }

    private void UpdateCanStartGame()
    {
        CanStartGame = _selectedPlayers.Count >= 2 && _selectedPlayers.Count <= 4;
    }

    private void OnSelectUserClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string userId)
        {
            var user = _userService.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null && _selectedPlayers.Count < 4)
            {
                _selectedPlayers.Add(user);
                _availableUsers.Remove(user);
                UpdateCanStartGame();
            }
            else if (_selectedPlayers.Count >= 4)
            {
                DisplayAlert("Error", "Máximo 4 jugadores permitidos", "OK");
            }
        }
    }

    private void OnRemovePlayerClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string userId)
        {
            var user = _selectedPlayers.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                _selectedPlayers.Remove(user);
                _availableUsers.Add(user);
                UpdateCanStartGame();
            }
        }
    }

    private async void OnAddGuestPlayerClicked(object sender, EventArgs e)
    {
        string guestName = GuestPlayerNameEntry.Text?.Trim();
        
        if (string.IsNullOrEmpty(guestName))
        {
            await DisplayAlert("Error", "Por favor ingrese un nombre para el jugador invitado", "OK");
            return;
        }

        if (_selectedPlayers.Count >= 4)
        {
            await DisplayAlert("Error", "Máximo 4 jugadores permitidos", "OK");
            return;
        }

        // Crear un usuario temporal para el jugador invitado
        var guestUser = new User($"Invitado: {guestName}", "guest@invitado.com");
        guestUser.Username = guestName; // Usar el nombre ingresado como username
        
        _selectedPlayers.Add(guestUser);
        GuestPlayerNameEntry.Text = "";
        UpdateCanStartGame();
    }

    private async void OnStartGameClicked(object sender, EventArgs e)
    {
        if (!CanStartGame)
        {
            await DisplayAlert("Error", "Seleccione entre 2 y 4 jugadores", "OK");
            return;
        }

        // Crear la página de juego con los jugadores seleccionados
        var gamePage = new GamePage(_selectedPlayers.ToList());
        await Navigation.PushAsync(gamePage);
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}

