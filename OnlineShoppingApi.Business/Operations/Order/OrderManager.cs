using Microsoft.EntityFrameworkCore;
using OnlineShopping.Business.Operations.Order.Dtos;
using OnlineShopping.Business.Operations.Product.Dtos;
using OnlineShopping.Business.Types;
using OnlineShopping.Data.Entities;
using OnlineShopping.Data.Repositories;
using OnlineShopping.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Business.Operations.Order
{
    // IOrderService implementasyonu olan OrderManager sınıfı. Siparişle ilgili işlemleri gerçekleştirir.
    public class OrderManager : IOrderService
    {
        // UnitOfWork ve iki repository nesnesi kullanılarak işlemler gerçekleştirilir.
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<OrderEntity> _orderRepository;
        private readonly IRepository<OrderProductEntity> _orderProductRepository;

        // Constructor ile gerekli bağımlılıklar enjekte edilir.
        public OrderManager(IUnitOfWork unitOfWork, IRepository<OrderEntity> orderRepository, IRepository<OrderProductEntity> orderProductRepository)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _orderProductRepository = orderProductRepository;
        }

        // Yeni bir sipariş ekler.
        public async Task<ServiceMessage> AddOrder(AddOrderDto order)
        {
            await _unitOfWork.BeginTransaction();  // İşlem başlatılır.

            var orderEntity = new OrderEntity
            {
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount
            };

            _orderRepository.Add(orderEntity);  // Sipariş veritabanına eklenir.
            try
            {
                await _unitOfWork.SaveChangesAsync();  // Değişiklikler kaydedilir.
            }
            catch (Exception)
            {
                throw new OrderException("Kayıt sırasında bir sorun yaşandı");  // Hata durumunda özel istisna fırlatılır.
            }

            // Siparişe eklenen ürünler veritabanına eklenir.
            foreach (var productId in order.ProductIds)
            {
                var orderProduct = new OrderProductEntity
                {
                    OrderId = orderEntity.Id,
                    ProductId = productId,
                };
                _orderProductRepository.Add(orderProduct);
            }

            try
            {
                await _unitOfWork.SaveChangesAsync();  // Ürünler kaydedilir.
                await _unitOfWork.CommitTransaction();  // İşlem başarıyla tamamlanır.
            }
            catch (Exception)
            {
                await _unitOfWork.RollBackTransaction();  // Hata durumunda işlem geri alınır.
                throw new OrderProductException("Ürünler eklenirken bir hatayla karşılaşıldı, süreç başa sarıldı");
            }

            return new ServiceMessage
            {
                IsSucceed = true,  // İşlem başarılı
            };
        }

        // Belirli bir siparişi getirir.
        public async Task<OrderDto> GetOrder(int id)
        {
            var order = await _orderRepository.GetAll(x => x.Id == id)
        .Include(x => x.User)  // Siparişin kullanıcı bilgisi de dahil edilir.
        .Select(x => new OrderDto
        {
            Id = x.Id,
            UserId = x.UserId,
            UserName = x.User.FirstName,
            OrderDate = x.OrderDate,
            TotalAmount = x.TotalAmount,
            Products = x.OrderProducts.Select(f => new OrderProductDto
            {
                Id = f.Product.Id,
                ProductName = f.Product.ProductName,
            }).ToList()
        }).FirstOrDefaultAsync();

            if (order == null)
            {
                throw new KeyNotFoundException("Sipariş bulunamadı"); // Özel hata fırlatma
                                                                      // Alternatif olarak: return new OrderDto(); // Boş bir OrderDto döndür
            }

            return order;  // Sipariş döndürülür.
        }

        // Tüm siparişleri getirir.
        public async Task<List<OrderDto>> GetOrders()
        {
            var orders = await _orderRepository.GetAll()
                   .Include(x => x.User)
                   .Select(x => new OrderDto
                   {
                       Id = x.Id,
                       UserId = x.UserId,
                       UserName = x.User.FirstName + " " + x.User.LastName,
                       OrderDate = x.OrderDate,
                       TotalAmount = x.TotalAmount,
                       Products = x.OrderProducts.Select(f => new OrderProductDto
                       {
                           Id = f.Product.Id,
                           ProductName = f.Product.ProductName,
                       }).ToList()
                   }).ToListAsync();

            return orders;  // Sipariş listesi döndürülür.
        }

        // Siparişin kullanıcı bilgisini günceller.
        public async Task<ServiceMessage> AdjustOrderUser(int id, int changeTo)
        {
            var order = _orderRepository.GetById(id);  // Sipariş alınır.
            if (order is null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Id ile eşleşen sipariş bulunamadı"  // Sipariş bulunamazsa hata mesajı döndürülür.
                };
            }

            order.UserId = changeTo;  // Kullanıcı ID'si güncellenir.
            _orderRepository.Update(order);  // Sipariş güncellenir.

            try
            {
                await _unitOfWork.SaveChangesAsync();  // Değişiklikler kaydedilir.
            }
            catch (Exception)
            {
                throw new Exception("Id değiştirilirken bir hata oluştu");  // Hata durumunda istisna fırlatılır.
            }

            return new ServiceMessage
            {
                IsSucceed = true  // İşlem başarılı
            };
        }

        // Belirli bir siparişi siler.
        public async Task<ServiceMessage> DeleteOrder(int id)
        {
            var order = _orderRepository.GetById(id);  // Sipariş alınır.
            if (order is null)
            {
                throw new OrderDeleteException("Bu sipariş bulunamadı");  // Sipariş bulunamazsa hata fırlatılır.
            }

            _orderRepository.Delete(id);  // Sipariş silinir.
            try
            {
                await _unitOfWork.SaveChangesAsync();  // Değişiklikler kaydedilir.
            }
            catch (Exception)
            {
                throw new OrderDeleteException("Bu sipariş bulunamadı");  // Hata durumunda istisna fırlatılır.
            }

            return new ServiceMessage
            {
                IsSucceed = true  // İşlem başarılı
            };
        }

        // Mevcut siparişi günceller.
        public async Task<ServiceMessage> UpdateOrder(UpdateOrderDto order)
        {
            var orderEntity = _orderRepository.GetById(order.Id);  // Sipariş alınır.
            if (orderEntity is null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Sipariş bulunamadı"  // Sipariş bulunamazsa hata mesajı döndürülür.
                };
            }

            await _unitOfWork.BeginTransaction();  // İşlem başlatılır.

            orderEntity.UserId = order.UserId;
            orderEntity.OrderDate = order.OrderDate;
            orderEntity.TotalAmount = order.TotalAmount;

            _orderRepository.Update(orderEntity);  // Sipariş güncellenir.
            try
            {
                await _unitOfWork.SaveChangesAsync();  // Değişiklikler kaydedilir.
            }
            catch (Exception)
            {
                await _unitOfWork.RollBackTransaction();  // Hata durumunda işlem geri alınır.
                throw new Exception("Güncelleme sırasında bir hata oldu.");
            }

            // Eski ürünler silinir.
            var orderProducts = _orderProductRepository.GetAll(x => x.OrderId == order.Id).ToList();
            foreach (var orderProduct in orderProducts)
            {
                _orderProductRepository.Delete(orderProduct, false); // hard delete
            }

            // Yeni ürünler eklenir.
            foreach (var productId in order.ProductIds)
            {
                var orderProduct = new OrderProductEntity
                {
                    OrderId = orderEntity.Id,
                    ProductId = productId,
                };
                _orderProductRepository.Add(orderProduct);
            }

            try
            {
                await _unitOfWork.SaveChangesAsync();  // Yeni ürünler kaydedilir.
                await _unitOfWork.CommitTransaction();  // İşlem tamamlanır.
            }
            catch (Exception)
            {
                await _unitOfWork.RollBackTransaction();  // Hata durumunda işlem geri alınır.
                throw new Exception("Güncelleme sırasında bir hata oldu.İşlemler geri alınıyor");
            }

            return new ServiceMessage
            {
                IsSucceed = true,  // İşlem başarılı
            };
        }
    }
}

