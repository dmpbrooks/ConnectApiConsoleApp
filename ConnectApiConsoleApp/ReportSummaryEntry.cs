using System;
using CsvHelper.Configuration.Attributes;


namespace ConnectApiConsoleApp
{
    public class ReportSummaryEntry
    {
        public string Provider { get; set; }
        [Name("Provider Country")]
        public string ProviderCountry { get; set; }
        public string SKU { get; set; }
        public string Developer { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        [Name("Product Type Identifier")]
        public string ProductType { get; set; }
        public decimal Units { get; set; }
        [Name("Developer Proceeds")]
        public decimal DeveloperProceeds { get; set; }
        [Name("Begin Date")]
        public DateTime BeginDate { get; set; } 
        [Name("End Date")]
        public DateTime EndDate { get; set; }
        [Name("Customer Currency")]
        public string CustomerCurrency { get; set; }
        [Name("Country Code")]
        public string CountryCode { get; set; }
        [Name("Currency of Proceeds")]
        public string CurrencyOfProceeds { get; set; }
        [Name("Apple Identifier")]
        public decimal AppleIdentifier { get; set; } 
        [Name("Customer Price")]
        public decimal CustomerPrice { get; set; }
        [Name("Promo Code")]
        public string PromoCode { get; set; }
        [Name("Parent Identifier")]
        public string ParentIdentifier { get; set; }
        public string Subscription { get; set; }
        public string Period { get; set; }
        public string Category { get; set; }
        public string CMB { get; set; }
        public string Device { get; set; }
        [Name("Supported Platforms")]
        public string SupportedPlatforms { get; set; }
        [Name("Proceeds Reason")]
        public string ProceedsReason { get; set; }
        [Name("Preserved Pricing")]
        public string PreservedPricing { get; set; }
        public string Client { get; set; }
        [Name("Order Type")]
        public string OrderType { get; set; }
    }



}
