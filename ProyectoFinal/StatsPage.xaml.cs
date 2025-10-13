using ProyectoFinal.Services;
using ProyectoFinal.Models;

namespace ProyectoFinal;

public partial class StatsPage : ContentPage
{
    private UserService _userService;

    public StatsPage()
    {
        InitializeComponent();
        _userService = UserService.Instance;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        UpdateStatistics();
    }

    private void UpdateStatistics()
    {
        if (_userService.IsUserLoggedIn)
        {
            var userStats = _userService.GetCurrentUserStatistics();
            var currentUser = _userService.CurrentUser;

            if (userStats != null)
            {
                // Actualizar estado de usuario
                UserStatusLabel.Text = $"Usuario: {currentUser.Username}";
                UserStatusLabel.TextColor = Colors.Green;

                // Estadísticas generales
                TotalGamesLabel.Text = $"Partidas jugadas: {userStats.TotalGamesPlayed}";
                GamesWonLabel.Text = $"Partidas ganadas: {userStats.TotalGamesWon}";
                WinPercentageLabel.Text = $"Porcentaje de victorias: {userStats.WinPercentage:F1}%";
                BestScoreLabel.Text = $"Mejor puntuación: {userStats.BestScore}";
                AverageTurnsLabel.Text = $"Promedio de turnos por partida: {userStats.AverageTurnsPerGame:F1}";

                // Estadísticas de lanzamientos
                TotalThrowsLabel.Text = $"Total de lanzamientos: {userStats.TotalThrows}";
                TotalPointsLabel.Text = $"Total de puntos: {userStats.TotalPoints}";
                AverageScoreLabel.Text = $"Puntuación promedio: {userStats.AverageScore:F2}";
                TotalCricketMarksLabel.Text = $"Marcas de Cricket: {userStats.TotalCricketMarks}";
                AverageCricketMarksLabel.Text = $"Promedio de marcas Cricket: {userStats.AverageCricketMarks:F2}";

                // Estadísticas por modo de juego
                X01GamesLabel.Text = $"Partidas X01: {userStats.X01GamesPlayed}";
                X01WinsLabel.Text = $"Victorias X01: {userStats.X01GamesWon}";
                CricketGamesLabel.Text = $"Partidas Cricket: {userStats.CricketGamesPlayed}";
                CricketWinsLabel.Text = $"Victorias Cricket: {userStats.CricketGamesWon}";

                // Información adicional
                if (userStats.LastGameDate != DateTime.MinValue)
                {
                    LastGameLabel.Text = $"Última partida: {userStats.LastGameDate:dd/MM/yyyy HH:mm}";
                }
                else
                {
                    LastGameLabel.Text = "Última partida: Nunca";
                }
                RegistrationDateLabel.Text = $"Fecha de registro: {currentUser.RegistrationDate:dd/MM/yyyy}";
            }
        }
        else
        {
            // Usuario no logueado - mostrar valores por defecto
            UserStatusLabel.Text = "No hay usuario logueado";
            UserStatusLabel.TextColor = Colors.Red;

            TotalGamesLabel.Text = "Partidas jugadas: 0";
            GamesWonLabel.Text = "Partidas ganadas: 0";
            WinPercentageLabel.Text = "Porcentaje de victorias: 0%";
            BestScoreLabel.Text = "Mejor puntuación: --";
            AverageTurnsLabel.Text = "Promedio de turnos por partida: --";

            TotalThrowsLabel.Text = "Total de lanzamientos: 0";
            TotalPointsLabel.Text = "Total de puntos: 0";
            AverageScoreLabel.Text = "Puntuación promedio: 0.0";
            TotalCricketMarksLabel.Text = "Marcas de Cricket: 0";
            AverageCricketMarksLabel.Text = "Promedio de marcas Cricket: 0.0";

            X01GamesLabel.Text = "Partidas X01: 0";
            X01WinsLabel.Text = "Victorias X01: 0";
            CricketGamesLabel.Text = "Partidas Cricket: 0";
            CricketWinsLabel.Text = "Victorias Cricket: 0";

            LastGameLabel.Text = "Última partida: Nunca";
            RegistrationDateLabel.Text = "Fecha de registro: --";
        }
    }

    private async void OnManageUserClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new UserManagementPage());
    }

    private async void OnGoToMainClicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }
}