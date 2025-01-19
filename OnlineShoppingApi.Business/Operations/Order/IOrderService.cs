using OnlineShopping.Business.Operations.Order.Dtos;
using OnlineShopping.Business.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Business.Operations.Order
{
    // IOrderService, sipariş işlemleri için kullanılan servis interface'idir.
    public interface IOrderService
    {
        Task<ServiceMessage> AddOrder(AddOrderDto order);  // Yeni bir sipariş ekler.
        Task<OrderDto> GetOrder(int id);  // Belirli bir siparişi getirir.
        Task<List<OrderDto>> GetOrders();  // Tüm siparişleri getirir.
        Task<ServiceMessage> AdjustOrderUser(int id, int changeTo);  // Siparişin kullanıcı bilgisini günceller.
        Task<ServiceMessage> DeleteOrder(int id);  // Belirli bir siparişi siler.
        Task<ServiceMessage> UpdateOrder(UpdateOrderDto order);  // Mevcut siparişi günceller.
    }
}
