using System.Text.Json.Serialization;

namespace HtmlToPdfFile.Request
{
    // author: feldy judah k
    // .NET 8

    public class PdfParameter
    {
        [JsonPropertyName("company")]
        public string Company { get; set; }

        [JsonPropertyName("street")]
        public string Street { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("invoice_number")]
        public string InvoiceNumber { get; set; }

        [JsonPropertyName("invoice_date")]
        public string InvoiceDate { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

    }
}
