using System;
using Newtonsoft.Json;

namespace VAT_Back.Models
{
    public class Receipt
    {
        public string? Key { get; set; }
        public string StoreName { get; set; } = string.Empty; // Use ONLY StoreName
        public double Amount { get; set; }
        public string Currency { get; set; } = "RM"; // Use ONLY Currency
        public double RefundInEur { get; set; }
        public double VatRate { get; set; }
        public string Category { get; set; } = "General";
        public string Country { get; set; } = "Malaysia";
        public DateTime Date { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pending";
        public string? ReceiptImageUrl { get; set; } // Use ONLY ReceiptImageUrl
    }
}