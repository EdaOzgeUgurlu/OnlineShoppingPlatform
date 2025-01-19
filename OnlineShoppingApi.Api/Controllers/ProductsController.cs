using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Api.Filters;
using OnlineShopping.Api.Models;
using OnlineShopping.Business.Operations.Product;
using OnlineShopping.Business.Operations.Product.Dtos;

namespace OnlineShopping.Api.Controllers
{

    [Route("api/[controller]")] // API rotası "api/Products" olacak şekilde ayarlanıyor.
    [ApiController]             // Controller'ın bir API Controller olduğunu belirtir.
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService; // Ürün işlemleri için bağımlılık enjeksiyonu.

        public ProductsController(IProductService productService)
        {
            _productService = productService; // Servis enjekte ediliyor.
        }

        [HttpGet("{id}")] // Belirli bir ürün bilgisi almak için GET metodu.
        public async Task<IActionResult> GetProduct(int id)
        {
            var order = await _productService.GetProduct(id); // Servisten ürün bilgisi alınır.
            if (order is null)         // Ürün bulunamazsa 404 NotFound döner.
                return NotFound();
            else                      // Ürün bulunursa 200 OK döner.
                return Ok(order);
        }

        [HttpGet] // Tüm ürünleri listelemek için GET metodu.
        public async Task<IActionResult> GetProducts()
        {
            var orders = await _productService.GetProducts(); // Servisten tüm ürünler alınır.
            return Ok(orders); // 200 OK ile döndürülür.
        }

        [HttpPost] // Yeni ürün eklemek için POST metodu.
        [Authorize(Roles = "Admin")] // Sadece "Admin" rolündeki kullanıcılar erişebilir.
        public async Task<IActionResult> AddProduct(AddProductRequest request)
        {
            // Gelen veriler DTO'ya dönüştürülür.
            var addProductDto = new AddProductDto
            {
                ProductName = request.ProductName,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
            };
            var result = await _productService.AddProduct(addProductDto); // Ürün ekleme işlemi yapılır.

            if (result.IsSucceed)     // İşlem başarılıysa 200 OK döner.
                return Ok();
            else                      // İşlem başarısızsa 400 BadRequest döner.
                return BadRequest(result.Message);
        }

        [HttpDelete("{id}")]          // Ürün silmek için DELETE metodu.
        [Authorize(Roles = "Admin")]  // Sadece "Admin" rolündeki kullanıcılar erişebilir.
        [TimeControlFilter]            // İşlemin zaman kontrolü yapılır.
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProduct(id); // Ürün silme işlemi yapılır.
            if (!result.IsSucceed)     // İşlem başarısızsa 404 NotFound döner.
                return NotFound(result.Message);
            else                      // İşlem başarılıysa 200 OK döner.
                return Ok();
        }

        [HttpPatch("{id}/ProductNameEdit")] // Ürün adını düzenlemek için PATCH metodu.
        [Authorize(Roles = "Admin")]        // Sadece "Admin" rolündeki kullanıcılar erişebilir.
        public async Task<IActionResult> AdjustProductName(int id, string changeTo)
        {
            var result = await _productService.AdjustProductName(id, changeTo); // Ürün adı düzenleme işlemi yapılır.

            if (!result.IsSucceed)         // İşlem başarısızsa 404 NotFound döner.
                return NotFound(result.Message);
            else                          // İşlem başarılıysa 200 OK döner.
                return Ok();
        }

        [HttpPut("{id}")]             // Ürün güncellemek için PUT metodu.
        [Authorize(Roles = "Admin")]  // Sadece "Admin" rolündeki kullanıcılar erişebilir.
        public async Task<IActionResult> UpdateProduct(int id, UpdateProductRequest request)
        {
            // Gelen veriler DTO'ya dönüştürülür.
            var updateProductDto = new UpdateProductDto
            {
                Id = id,
                ProductName = request.ProductName,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
            };
            var result = await _productService.UpdateProduct(updateProductDto); // Ürün güncelleme işlemi yapılır.
            if (!result.IsSucceed)     // İşlem başarısızsa 404 NotFound döner.
                return NotFound(result.Message);
            else                      // İşlem başarılıysa güncellenen ürün bilgileri döner.
                return await GetProduct(id);
        }
    }
}



