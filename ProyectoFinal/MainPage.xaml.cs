namespace ProyectoFinal
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnButton1Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PlayerSelectionPage());
        }

        private async void OnButton1bClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GamePage());
        }

        private async void OnButton2Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new StatsPage());
        }

        private async void OnButton3Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserManagementPage());
        }

        
    }

}
