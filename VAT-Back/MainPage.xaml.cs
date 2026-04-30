using System.Collections.ObjectModel;
using VAT_Back.Models;
using VAT_Back.Services;

namespace VAT_Back
{
    public partial class MainPage : ContentPage
    {
        private readonly FirebaseService _firebaseService;
        private ObservableCollection<Receipt> _receipts;

        public MainPage(FirebaseService firebaseService)
        {
            InitializeComponent();
            _firebaseService = firebaseService;

            // Set default currency display
            CurrencyToggle.SelectedIndex = 0;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadData();
        }

        private async Task LoadData()
        {
            // Uses the GetReceipts method we added to the service
            var data = await _firebaseService.GetReceipts();
            _receipts = new ObservableCollection<Receipt>(data);
            ReceiptsListView.ItemsSource = _receipts;
            UpdateTotalRefund();
        }

        private void OnCurrencyToggleChanged(object sender, EventArgs e)
        {
            UpdateTotalRefund();
        }

        private void UpdateTotalRefund()
        {
            if (_receipts == null) return;

            // 1. Internal Standard: Sum everything by the Base Euro value
            double totalEur = _receipts.Sum(r => r.RefundInEur);

            // 2. Multi-Currency Display Logic using hardcoded demo rates
            string selected = CurrencyToggle.SelectedItem?.ToString() ?? "EUR (€)";
            string result;

            switch (selected)
            {
                case "MYR (RM)":
                    result = $"RM{(totalEur * 5.08):F2}"; // 1 EUR = 5.08 MYR
                    break;
                case "GBP (£)":
                    result = $"£{(totalEur * 0.86):F2}";  // 1 EUR = 0.86 GBP
                    break;
                case "USD ($)":
                    result = $"${(totalEur * 1.07):F2}";  // 1 EUR = 1.07 USD
                    break;
                default:
                    result = $"€{totalEur:F2}";
                    break;
            }

            TotalRefundLabel.Text = result;
        }

        // KEEP ONLY THIS VERSION (Fixes CS7036)
        private async void OnReceiptSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Receipt selected)
            {
                // Pass BOTH arguments required by AdminReviewPage
                await Navigation.PushAsync(new AdminReviewPage(selected, _firebaseService));

                // Clear selection to allow re-tapping
                ((CollectionView)sender).SelectedItem = null;
            }
        }
    }
}