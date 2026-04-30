using System;
using Microsoft.Maui.Controls;

namespace VAT_Back
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }

        // We use 'public' here to ensure the XAML can definitely find the event
        public async void OnLogoutClicked(object sender, EventArgs e)
        {
            try
            {
                // Navigate back to the Portal Selection screen
                // Wrapping it in a NavigationPage ensures the buttons on that page work
                Application.Current.MainPage = new NavigationPage(new RoleSelectionPage());
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Logout failed: " + ex.Message, "OK");
            }
        }
    }
}