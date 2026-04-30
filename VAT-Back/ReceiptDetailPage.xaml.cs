using VAT_Back.Models;

namespace VAT_Back
{
    public partial class ReceiptDetailPage : ContentPage
    {
        public ReceiptDetailPage(Receipt receipt)
        {
            InitializeComponent();

            // This connects the UI {Binding} tags to the receipt object data
            BindingContext = receipt;

            // Logically update the progress stepper colors based on the status
            UpdateStepper(receipt.Status);
        }

        private void UpdateStepper(string status)
        {
            // Reset colors to gray first
            Step2Circle.Fill = Color.FromArgb("#E0E0E0");
            Step3Circle.Fill = Color.FromArgb("#E0E0E0");

            if (status == "Validated" || status == "Refunded")
            {
                // Turn the "Digital Validation" step blue
                Step2Circle.Fill = Color.FromArgb("#283593");
            }

            if (status == "Refunded")
            {
                // Turn the "Payment Issued" step blue
                Step3Circle.Fill = Color.FromArgb("#283593");
            }
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            // Navigation.PopAsync returns the user to the previous screen (MainPage)
            await Navigation.PopAsync();
        }
    }
}