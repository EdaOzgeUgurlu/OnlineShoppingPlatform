using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Api.Filters;
using OnlineShopping.Api.Models;
using OnlineShopping.Business.Operations.Order;
using OnlineShopping.Business.Operations.Order.Dtos;

namespace OnlineShopping.Api.Controllers
{
    [Route("api/[controller]")] // API rotası "api/Orders" olacak şekilde ayarlanıyor.
    [ApiController]             // Controller'ın bir API Controller olduğunu belirtir.
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService; // Sipariş işlemleri için bağımlılık enjeksiyonu.

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService; // Servis enjekte ediliyor.
        }

        [HttpGet("{id}")] // Belirli bir sipariş bilgisi almak için GET metodu.
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _orderService.GetOrder(id); // Servisten sipariş bilgisi alınır.
            if (order is null)         // Sipariş bulunamazsa 404 NotFound döner.
                return NotFound();
            else                      // Sipariş bulunursa 200 OK döner.
                return Ok(order);
        }

        [HttpGet] // Tüm siparişleri listelemek için GET metodu.
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _orderService.GetOrders(); // Servisten tüm siparişler alınır.
            return Ok(orders); // 200 OK ile döndürülür.
        }

        [HttpPost] // Yeni sipariş eklemek için POST metodu.
        [Authorize(Roles = "Admin")] // Sadece "Admin" rolündeki kullanıcılar erişebilir.
        public async Task<IActionResult> AddOrder(AddOrderRequest request)
        {
            // Gelen veriler DTO'ya dönüştürülür.
            var addOrderDto = new AddOrderDto
            {
                UserId = request.UserId,
                OrderDate = request.OrderDate,
                TotalAmount = request.TotalAmount,
                ProductIds = request.ProductIds,
            };
            var result = await _orderService.AddOrder(addOrderDto); // Sipariş ekleme işlemi yapılır.

            if (!result.IsSucceed)     // İşlem başarısızsa 400 BadRequest döner.
            {
                return BadRequest(result.Message);
            }
            else                      // İşlem başarılıysa 200 OK döner.
            {
                return Ok();
            }
        }

        [HttpPatch("{id}/OrderUserEdit")] // Siparişin kullanıcı bilgisini düzenlemek için PATCH metodu.
        [Authorize(Roles = "Admin")]      // Sadece "Admin" rolündeki kullanıcılar erişebilir.
        public async Task<IActionResult> AdjustOrderUser(int id, int changeTo)
        {
            var result = await _orderService.AdjustOrderUser(id, changeTo); // Kullanıcı düzenleme işlemi yapılır.

            if (!result.IsSucceed)         // İşlem başarısızsa 404 NotFound döner.
                return NotFound(result.Message);
            else                          // İşlem başarılıysa 200 OK döner.
                return Ok();
        }

        [HttpDelete("{id}")]          // Sipariş silmek için DELETE metodu.
        [Authorize(Roles = "Admin")]  // Sadece "Admin" rolündeki kullanıcılar erişebilir.
        [TimeControlFilter]            // İşlemin zaman kontrolü yapılır.
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _orderService.DeleteOrder(id); // Sipariş silme işlemi yapılır.
            if (!result.IsSucceed)     // İşlem başarısızsa 404 NotFound döner.
                return NotFound(result.Message);
            else                      // İşlem başarılıysa 200 OK döner.
                return Ok();
        }

        [HttpPut("{id}")]             // Sipariş güncellemek için PUT metodu.
        [Authorize(Roles = "Admin")]  // Sadece "Admin" rolündeki kullanıcılar erişebilir.
        [TimeControlFilter]            // İşlemin zaman kontrolü yapılır.
        public async Task<IActionResult> UpdateOrder(int id, UpdateOrderRequest request)
        {
            // Gelen veriler DTO'ya dönüştürülür.
            var updateOrderDto = new UpdateOrderDto
            {
                Id = id,
                UserId = request.UserId,
                OrderDate = request.OrderDate,
                TotalAmount = request.TotalAmount,
                ProductIds = request.ProductIds,
            };
            var result = await _orderService.UpdateOrder(updateOrderDto); // Sipariş güncelleme işlemi yapılır.
            if (!result.IsSucceed)     // İşlem başarısızsa 404 NotFound döner.
                return NotFound(result.Message);
            else                      // İşlem başarılıysa güncellenen sipariş bilgileri döner.
                return await GetOrder(id);
        }
    }
}
