using ProyectoFinal.Services;
using ProyectoFinal.Models;

namespace ProyectoFinal;

public partial class UserManagementPage : ContentPage
{
    private UserService _userService;

    public UserManagementPage()
    {
        InitializeComponent();
        _userService = UserService.Instance;
        BindingContext = _userService;
        UpdateUI();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateUI();
    }

    private void UpdateUI()
    {
        // Actualizar estado de sesión
        if (_userService.IsUserLoggedIn)
        {
            SessionStatusLabel.Text = "Usuario logueado";
            SessionStatusLabel.TextColor = Colors.Green;
            CurrentUserLabel.Text = $"Usuario: {_userService.CurrentUser.Username}";
            CurrentUserLabel.IsVisible = true;
            LogoutButton.IsVisible = true;
        }
        else
        {
            SessionStatusLabel.Text = "No hay usuario logueado";
            SessionStatusLabel.TextColor = Colors.Red;
            CurrentUserLabel.IsVisible = false;
            LogoutButton.IsVisible = false;
        }

        // Actualizar picker de usuarios
        LoginUserPicker.ItemsSource = _userService.Users.ToList();
        LoginUserPicker.SelectedItem = null;
        LoginButton.IsEnabled = false;
    }

    private void OnRegisterTextChanged(object sender, TextChangedEventArgs e)
    {
        RegisterButton.IsEnabled = !string.IsNullOrWhiteSpace(RegisterUsernameEntry.Text) && 
                                   !string.IsNullOrWhiteSpace(RegisterEmailEntry.Text);
    }

    private void OnRegisterClicked(object sender, EventArgs e)
    {
        string username = RegisterUsernameEntry.Text.Trim();
        string email = RegisterEmailEntry.Text.Trim();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email))
        {
            RegisterMessageLabel.Text = "Por favor, complete todos los campos";
            RegisterMessageLabel.TextColor = Colors.Red;
            return;
        }

        if (_userService.RegisterUser(username, email))
        {
            RegisterMessageLabel.Text = "Usuario registrado exitosamente";
            RegisterMessageLabel.TextColor = Colors.Green;
            RegisterUsernameEntry.Text = "";
            RegisterEmailEntry.Text = "";
            RegisterButton.IsEnabled = false;
            UpdateUI();
        }
        else
        {
            RegisterMessageLabel.Text = "Error: Usuario o email ya existe";
            RegisterMessageLabel.TextColor = Colors.Red;
        }
    }

    private void OnLoginUserChanged(object sender, EventArgs e)
    {
        LoginButton.IsEnabled = LoginUserPicker.SelectedItem != null;
    }

    private void OnLoginClicked(object sender, EventArgs e)
    {
        if (LoginUserPicker.SelectedItem is User selectedUser)
        {
            if (_userService.LoginUser(selectedUser.Username))
            {
                LoginMessageLabel.Text = $"Sesión iniciada como {selectedUser.Username}";
                LoginMessageLabel.TextColor = Colors.Green;
                UpdateUI();
            }
            else
            {
                LoginMessageLabel.Text = "Error al iniciar sesión";
                LoginMessageLabel.TextColor = Colors.Red;
            }
        }
    }

    private void OnLogoutClicked(object sender, EventArgs e)
    {
        _userService.LogoutUser();
        LoginMessageLabel.Text = "Sesión cerrada";
        LoginMessageLabel.TextColor = Colors.Orange;
        UpdateUI();
    }

    private async void OnDeleteUserClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string userId)
        {
            var user = _userService.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                bool confirm = await DisplayAlert("Confirmar eliminación", 
                    $"¿Está seguro de que desea eliminar el usuario '{user.Username}'?", 
                    "Sí", "No");
                
                if (confirm)
                {
                    if (_userService.DeleteUser(userId))
                    {
                        await DisplayAlert("Éxito", "Usuario eliminado correctamente", "OK");
                        UpdateUI();
                    }
                    else
                    {
                        await DisplayAlert("Error", "No se pudo eliminar el usuario", "OK");
                    }
                }
            }
        }
    }

    private async void OnGoToStatsClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new StatsPage());
    }

    private async void OnGoToMainClicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}

