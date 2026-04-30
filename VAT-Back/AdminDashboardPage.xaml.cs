using VAT_Back.Models;
using VAT_Back.Services;
using System.Linq;

namespace VAT_Back
{
    public partial class AdminDashboardPage : ContentPage
    {
        private readonly FirebaseService _firebaseService;

        public AdminDashboardPage(FirebaseService firebaseService)
        {
            InitializeComponent();
            _firebaseService = firebaseService;
        }

        // Refreshes the list every time the page appears
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadPendingReceipts();
        }

        private async Task LoadPendingReceipts()
        {
            try
            {
                var allReceipts = await _firebaseService.GetReceipts();


                // Show ONLY items that are still 'Pending'
                var pending = allReceipts.Where(r => r.Status == "Pending").ToList();

                PendingReceiptsListView.ItemsSource = pending;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Could not load queue: " + ex.Message, "OK");
            }
        }

        private async void OnReceiptSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Receipt selected)
            {
                // NAVIGATION: Goes to the Review Page and passes the selected item
                await Navigation.PushAsync(new AdminReviewPage(selected, _firebaseService));

                // Deselect the item so it doesn't stay highlighted
                PendingReceiptsListView.SelectedItem = null;
            }
        }
    }
}