using VAT_Back.Models;
using VAT_Back.Services;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;

namespace VAT_Back
{
    public partial class AddReceiptPage : ContentPage
    {
        private readonly FirebaseService _firebaseService;
        private string _localFilePath = string.Empty;
        private string _currentSymbol = "RM";

        public AddReceiptPage(FirebaseService firebaseService)
        {
            InitializeComponent();
            _firebaseService = firebaseService;

            // Set default picker index to Malaysia
            CountryPicker.SelectedIndex = 0;
        }

        // 1. Logic to change currency label when Country changes
        private void OnCountryChanged(object sender, EventArgs e)
        {
            if (CountryPicker.SelectedIndex != -1)
            {
                string selectedCountry = CountryPicker.SelectedItem.ToString();

                _currentSymbol = selectedCountry switch
                {
                    "Malaysia" => "RM",
                    "Germany (EU)" => "€",
                    "United Kingdom" => "£",
                    "USA" => "$",
                    _ => "RM"
                };

                AmountLabel.Text = $"Total Amount ({_currentSymbol})";
            }
        }

        // 2. Camera Capture Logic
        private async void OnTakePhotoClicked(object sender, EventArgs e)
        {
            try
            {
                if (MediaPicker.Default.IsCaptureSupported)
                {
                    FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

                    if (photo != null)
                    {
                        string localFolder = FileSystem.CacheDirectory;
                        _localFilePath = Path.Combine(localFolder, photo.FileName);

                        using Stream sourceStream = await photo.OpenReadAsync();
                        using FileStream localStream = File.OpenWrite(_localFilePath);

                        await sourceStream.CopyToAsync(localStream);

                        // Update the preview image in your UI
                        ReceiptPreview.Source = ImageSource.FromFile(_localFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Camera failed: " + ex.Message, "OK");
            }
        }

        // 3. Submission Logic with Internal Tax Calculation
        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrWhiteSpace(StoreEntry.Text) ||
                    string.IsNullOrWhiteSpace(AmountEntry.Text) ||
                    string.IsNullOrEmpty(_localFilePath))
                {
                    await DisplayAlert("Error", "Please complete all fields and take a photo.", "OK");
                    return;
                }

                // --- NEW TAX & CONVERSION LOGIC START ---
                double rawAmount = double.Parse(AmountEntry.Text);
                string selectedCountry = CountryPicker.SelectedItem?.ToString() ?? "Malaysia";

                double taxRate = 0.0;
                double conversionToEur = 1.0;

                // Determine Tax Rate and Conversion based on selected country
                switch (selectedCountry)
                {
                    case "Germany (EU)":
                        taxRate = 0.19; // 19% VAT
                        conversionToEur = 1.0; // Already in Euro
                        break;
                    case "United Kingdom":
                        taxRate = 0.20; // 20% VAT
                        conversionToEur = 1.17; // 1 GBP to EUR approx
                        break;
                    case "USA":
                        taxRate = 0.08; // Approx Sales Tax
                        conversionToEur = 0.93; // 1 USD to EUR approx
                        break;
                    case "Malaysia":
                        taxRate = 0.0; // 0% for tourist refunds currently
                        conversionToEur = 0.20; // 1 MYR to EUR approx
                        break;
                    default:
                        taxRate = 0.10;
                        break;
                }

                // Calculate the refund in original currency, then convert to Euro for the dashboard
                double refundInOriginalCurrency = rawAmount * taxRate;
                double refundInEur = refundInOriginalCurrency * conversionToEur;
                // --- NEW TAX & CONVERSION LOGIC END ---

                // CONVERT IMAGE TO BASE64 STRING
                byte[] imageBytes = File.ReadAllBytes(_localFilePath);
                string base64Image = Convert.ToBase64String(imageBytes);

                // Create Final Receipt Object
                var newReceipt = new Receipt
                {
                    StoreName = StoreEntry.Text,
                    Amount = rawAmount,
                    Currency = _currentSymbol,
                    Country = selectedCountry,
                    Date = (DateTime)ReceiptDatePicker.Date,
                    Status = "Pending",
                    RefundInEur = refundInEur, // This now saves the actual value to Firebase!
                    ReceiptImageUrl = $"data:image/jpeg;base64,{base64Image}"
                };

                // Save Data to Firebase Realtime Database
                await _firebaseService.AddReceipt(newReceipt);

                await DisplayAlert("Success", "Receipt saved! Refund calculated.", "OK");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Save failed: " + ex.Message, "OK");
            }
        }
    }
}