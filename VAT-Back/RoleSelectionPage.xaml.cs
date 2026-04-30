using VAT_Back.Services;

namespace VAT_Back
{
    public partial class RoleSelectionPage : ContentPage
    {
        public RoleSelectionPage()
        {
            InitializeComponent();
        }

        private void OnTravelerClicked(object sender, EventArgs e)
        {
            // SWAP THE ROOT: This is the secret to making the Hamburger Menu appear.
            // By setting the MainPage to AppShell, the Flyout menu is enabled.
            Application.Current.MainPage = new AppShell();
        }

        private async void OnOfficerClicked(object sender, EventArgs e)
        {
            var firebaseService = Handler.MauiContext?.Services.GetService<FirebaseService>();
            if (firebaseService != null)
            {
                // We keep the Admin portal simple and secure without a hamburger menu
                await Navigation.PushAsync(new AdminDashboardPage(firebaseService));
            }
        }
    }
}