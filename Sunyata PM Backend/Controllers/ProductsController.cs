using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sunyata_PM_Backend.DTOs;
using Sunyata_PM_Backend.Services.Interfaces;

namespace Sunyata_PM_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> CreateProduct(ProductDto productDto)
        {
            var createdProduct = await _productService.CreateProductAsync(productDto);
            return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(int id, ProductDto productDto)
        {
            var updatedProduct = await _productService.UpdateProductAsync(id, productDto);
            if (updatedProduct == null) return NotFound();
            return Ok(updatedProduct);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProductAsync(id);
            return NoContent();
        }

        [HttpGet("export-pdf")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ExportProductsToPdf()
        {
            try
            {
               
                var products = await _productService.GetAllProductsAsync();
                if (!products.Any())
                    return NotFound("No products found to export");

                using (MemoryStream ms = new MemoryStream())
                {
                    PdfWriter writer = new PdfWriter(ms);
                    PdfDocument pdf = new PdfDocument(writer);
                    Document document = new Document(pdf);

                  
                    document.Add(new Paragraph("Products Report")
                        .SetFontSize(20));

                    
                    document.Add(new Paragraph($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}")
                        .SetFontSize(10));

                    
                    Table table = new Table(4).UseAllAvailableWidth();

                  
                    table.AddHeaderCell("ID");
                    table.AddHeaderCell("Name");
                    table.AddHeaderCell("Description");
                    table.AddHeaderCell("Price");

                  
                    foreach (var product in products)
                    {
                        table.AddCell(product.Id.ToString());
                        table.AddCell(product.Name ?? "");
                        table.AddCell(product.Description ?? "");
                        table.AddCell($"${product.Price:F2}");
                    }

                    document.Add(table);

                   
                    document.Add(new Paragraph($"Total Products: {products.Count()}")
                        .SetFontSize(10));

                    document.Close();

                   
                    return File(ms.ToArray(),
                        "application/pdf",
                        $"Products_Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(500, "Error generating PDF report");
            }
        }
    }

}
