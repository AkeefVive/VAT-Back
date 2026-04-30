using Firebase.Database;
using Firebase.Database.Query;
using VAT_Back.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VAT_Back.Services
{
    public class FirebaseService
    {
        private readonly string FireBaseUrl = "https://vat-back-db-default-rtdb.asia-southeast1.firebasedatabase.app/";
        private readonly FirebaseClient _client; // Consistent naming

        public FirebaseService()
        {
            _client = new FirebaseClient(FireBaseUrl);
        }

        // Saves new receipts (including Base64 images)
        public async Task AddReceipt(Receipt receipt)
        {
            await _client.Child("Receipts").PostAsync(receipt);
        }

        // The method the Dashboard (MainPage) is looking for
        public async Task<List<Receipt>> GetReceipts()
        {
            try
            {
                var collection = await _client.Child("Receipts").OnceAsync<Receipt>();

                if (collection == null) return new List<Receipt>();

                return collection.Select(item => new Receipt
                {
                    Key = item.Key,
                    StoreName = item.Object.StoreName,
                    Amount = item.Object.Amount,
                    Currency = item.Object.Currency,
                    RefundInEur = item.Object.RefundInEur,
                    Date = item.Object.Date,
                    Country = item.Object.Country,
                    VatRate = item.Object.VatRate,
                    Status = item.Object.Status ?? "Pending",
                    Category = item.Object.Category ?? "General",
                    ReceiptImageUrl = item.Object.ReceiptImageUrl
                }).ToList();
            }
            catch
            {
                return new List<Receipt>();
            }
        }

        // Used by AdminReviewPage to update Status to "Validated" or "Refunded"
        public async Task UpdateReceiptStatus(string key, string newStatus)
        {
            // Note: PutAsync needs the quotes for a plain string in Firebase
            await _client.Child("Receipts").Child(key).Child("Status").PutAsync($"\"{newStatus}\"");
        }
    }
}