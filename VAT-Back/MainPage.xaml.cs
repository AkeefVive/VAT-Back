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
            // FIX: Removed the "Self_Focused_Action" error line
            base.OnAppearing();
            await LoadData();
        }

        private async Task LoadData()
        {
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

            double totalEur = _receipts.Sum(r => r.RefundInEur);

            string selected = CurrencyToggle.SelectedItem?.ToString() ?? "EUR (€)";
            string result;

            switch (selected)
            {
                case "MYR (RM)":
                    result = $"RM{(totalEur * 5.08):F2}";
                    break;
                case "GBP (£)":
                    result = $"£{(totalEur * 0.86):F2}";
                    break;
                case "USD ($)":
                    result = $"${(totalEur * 1.07):F2}";
                    break;
                default:
                    result = $"€{totalEur:F2}";
                    break;
            }

            TotalRefundLabel.Text = result;
        }

        private async void OnReceiptSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is Receipt selected)
            {
                // FIX: Changed to match your file name 'ReceiptDetailPage'
                await Navigation.PushAsync(new ReceiptDetailPage(selected));

                ((CollectionView)sender).SelectedItem = null;
            }
        }
    }
}