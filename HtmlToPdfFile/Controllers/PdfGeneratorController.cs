using HtmlToPdfFile.Models;
using HtmlToPdfFile.Request;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text.Json;
using TheArtOfDev.HtmlRenderer.PdfSharp;

// author: feldy judah k
// .NET 8

namespace HtmlToPdfFile.Controllers
{
    [Route("pdf_generator")]
    [ApiController]
    public class PdfGeneratorController : ControllerBase
    {
        private readonly ILogger<PdfGeneratorController> _logger;

        public PdfGeneratorController(ILogger<PdfGeneratorController> logger)
        {
            _logger = logger;
        }

        [Consumes("application/json")]
        [Produces("application/json")]
        [HttpPost("generate_pdf")]
        public async Task<IActionResult> GeneratePdf(PdfParameter request)
        {
            string jsonRequest = JsonSerializer.Serialize(request);
            try
            {
                string inputPdfPath = AppContext.BaseDirectory.Replace("\\bin\\Debug\\net8.0", "") + "InputFile\\";
                string outputPdfPath = AppContext.BaseDirectory.Replace("\\bin\\Debug\\net8.0", "") + "OutputFile\\";

                //// read HTML from file path
                string htmlContent = "";
                using (FileStream fileStream = new FileStream(Path.Combine(inputPdfPath, "form.html"), FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        // Read all text from the stream
                        htmlContent = reader.ReadToEnd();
                    }
                }
                
                // header
                htmlContent = htmlContent.Replace("=[COMPANY]", request.Company);
                htmlContent = htmlContent.Replace("=[STREET]", request.Street);
                htmlContent = htmlContent.Replace("=[CITY]", request.City);
                htmlContent = htmlContent.Replace("=[COUNTRY]", request.Country);
                htmlContent = htmlContent.Replace("=[INVOICE_NUMBER]", request.InvoiceNumber);
                htmlContent = htmlContent.Replace("=[INVOICE_DATE]", request.InvoiceDate);
                htmlContent = htmlContent.Replace("=[NAME]", request.Name);
                htmlContent = htmlContent.Replace("=[PHONE]", request.Phone);
                htmlContent = htmlContent.Replace("=[EMAIL]", request.Email);

                // products dummy
                List<Product> listProducts = new List<Product>();
                string HTMLProducts = "";

                listProducts.Add(new Product()
                {
                    ProductName = "Women’s Bag",
                    Quantity = 1,
                    UnitPrice = 3899000,
                    TotalPrice = 1 * 3899000
                });
                listProducts.Add(new Product()
                {
                    ProductName = "Women’s Jeans",
                    Quantity = 2,
                    UnitPrice = 899000,
                    TotalPrice = 2 * 899000
                });

                foreach(var p in listProducts)
                {
                    HTMLProducts += $"<tr>";
                    HTMLProducts += $"<td>{p.ProductName}</td>";
                    HTMLProducts += $"<td class=\"text-right\">{p.Quantity.ToString()}</td>";
                    HTMLProducts += $"<td class=\"text-right\">{p.UnitPrice.ToString("N0")}</td>";
                    HTMLProducts += $"<td class=\"text-right\">{p.TotalPrice.ToString("N0")}</td>";
                    HTMLProducts += $"</tr>";
                }
                htmlContent = htmlContent.Replace("=[LISTPRODUCTS]", HTMLProducts);
                htmlContent = htmlContent.Replace("=[TOTALPRICE]", listProducts.Sum(p => p.TotalPrice).ToString("N0"));

                // file name by datetime now
                string datename = DateTime.Now.ToString("yyyyMMddHHmmss");

                // using PdfSharp, simple HTML no complex CSS
                using var document = new PdfSharp.Pdf.PdfDocument();

                // Optional: setting page
                PdfGenerator.AddPdfPages(
                    document,
                    htmlContent,
                    PdfSharp.PageSize.A4
                );

                using var stream = new FileStream($"{outputPdfPath}INV{datename}.pdf", FileMode.Create);
                document.Save(stream);

                // log the result
                _logger.LogInformation($"PDF saved to {$"{outputPdfPath}form{datename}.pdf"}");
                _logger.LogInformation($"GeneratePdf: success generate pdf file. Parameters: {jsonRequest}");

                return Ok(new { Status = "Ok", Message = $"GeneratePdf: success generate pdf file", Parameters = $"{jsonRequest}" });
            }
            catch (Exception ex)
            {
                // log the result
                _logger.LogError($"GeneratePdf: error generate pdf file. Parameters: {jsonRequest}");

                return BadRequest(new { Status = "BadRequest", Message = $"GeneratePdf: error generate pdf file", Parameters = $"{jsonRequest}" });
            }
        }

    }
}