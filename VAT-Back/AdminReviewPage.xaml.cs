using VAT_Back.Models;
using VAT_Back.Services;

namespace VAT_Back
{
    public partial class AdminReviewPage : ContentPage
    {
        private readonly Receipt _receipt;
        private readonly FirebaseService _firebaseService;

        public AdminReviewPage(Receipt receipt, FirebaseService firebaseService)
        {
            InitializeComponent();
            _receipt = receipt;
            _firebaseService = firebaseService;

            // Bind Data to UI
            StoreLabel.Text = _receipt.StoreName;
            AmountLabel.Text = $"Refund Amount: €{_receipt.RefundInEur:F2}";
            CountryLabel.Text = $"Source: {_receipt.Country}";
            ReceiptImage.Source = _receipt.ReceiptImageUrl;
        }

        private async void OnApproveClicked(object sender, EventArgs e)
        {
            _receipt.Status = "Validated";
            await UpdateStatus("Receipt Validated! You can now process the payment.");
            PaymentButton.IsVisible = true; // Show the final payment button
        }

        private async void OnRejectClicked(object sender, EventArgs e)
        {
            _receipt.Status = "Rejected";
            await UpdateStatus("Receipt Rejected.");
            await Navigation.PopAsync();
        }

        private async void OnConfirmPaymentClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Confirm", "Confirm that €" + _receipt.RefundInEur + " has been transferred?", "Yes", "No");
            if (confirm)
            {
                _receipt.Status = "Refunded";
                await UpdateStatus("Payment Confirmed. Process Complete.");
                await Navigation.PopAsync();
            }
        }

        private async Task UpdateStatus(string message)
        {
            // We need a specific Update method in FirebaseService
            await _firebaseService.UpdateReceiptStatus(_receipt.Key, _receipt.Status);
            await DisplayAlert("Admin Action", message, "OK");
        }
    }
}