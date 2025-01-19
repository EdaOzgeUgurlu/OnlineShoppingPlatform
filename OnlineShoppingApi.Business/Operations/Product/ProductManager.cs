using Microsoft.EntityFrameworkCore;
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

namespace OnlineShopping.Business.Operations.Product
{
    public class ProductManager : IProductService
    {
        // Bağımlılıkları içeren alanlar
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ProductEntity> _productRepository;
        private readonly IRepository<OrderProductEntity> _orderProductRepository;

        // Constructor ile bağımlılıkların çözülmesi
        public ProductManager(IUnitOfWork unitOfWork, IRepository<ProductEntity> productRepository, IRepository<OrderProductEntity> orderProductRepository)
        {
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
            _orderProductRepository = orderProductRepository;
        }

        // Yeni ürün ekleme işlemi
        public async Task<ServiceMessage> AddProduct(AddProductDto product)
        {
            // Ürünün zaten var olup olmadığını kontrol et
            var hasProduct = _productRepository.GetAll(x => x.ProductName.ToLower() == product.ProductName.ToLower()).Any();
            if (hasProduct)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Bu ürün zaten mevcut." // Hata mesajı döndür
                };
            }

            // Yeni ürün nesnesi oluşturuluyor
            var productEntity = new ProductEntity
            {
                ProductName = product.ProductName,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
            };

            // Ürün veritabanına ekleniyor
            _productRepository.Add(productEntity);
            try
            {
                await _unitOfWork.SaveChangesAsync(); // Değişiklikler kaydediliyor
            }
            catch (Exception)
            {
                throw new ProductException("Kayıt sırasında bir hata oluştu."); // Hata durumunda istisna fırlatılıyor
            }

            return new ServiceMessage
            {
                IsSucceed = true,
            };
        }

        // ID ile tek bir ürün almak
        public async Task<ProductDto> GetProduct(int id)
        {
            var product = await _productRepository.GetAll(x => x.Id == id)
                .Select(x => new ProductDto
                {
                    Id = x.Id,
                    ProductName = x.ProductName,
                    Price = x.Price,
                    StockQuantity = x.StockQuantity
                }).FirstOrDefaultAsync();

            return product; // Ürün döndürülüyor
        }

        // Tüm ürünleri listelemek
        public async Task<List<ProductDto>> GetProducts()
        {
            var product = await _productRepository.GetAll()
                 .Select(x => new ProductDto
                 {
                     Id = x.Id,
                     ProductName = x.ProductName,
                     Price = x.Price,
                     StockQuantity = x.StockQuantity
                 }).ToListAsync();

            return product; // Ürün listesi döndürülüyor
        }

        // Ürün silme işlemi
        public async Task<ServiceMessage> DeleteProduct(int id)
        {
            var product = _productRepository.GetById(id); // Ürün ID ile alınıyor
            if (product is null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Silinmek istenen ürün bulunamadı" // Ürün bulunamadı hatası
                };
            }

            _productRepository.Delete(id); // Ürün siliniyor
            try
            {
                await _unitOfWork.SaveChangesAsync(); // Değişiklikler kaydediliyor
            }
            catch (Exception)
            {
                throw new Exception("Ürün silme sırasında bir hata oluştu."); // Hata durumunda istisna fırlatılıyor
            }

            return new ServiceMessage
            {
                IsSucceed = true
            };
        }

        // Ürün adı değiştirme işlemi
        public async Task<ServiceMessage> AdjustProductName(int id, string changeTo)
        {
            var product = _productRepository.GetById(id); // Ürün ID ile alınıyor
            if (product is null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Id ile eşleşen ürün bulunamadı" // Ürün bulunamadı hatası
                };
            }

            product.ProductName = changeTo; // Ürün adı değiştirilir
            _productRepository.Update(product); // Ürün güncellenir

            try
            {
                await _unitOfWork.SaveChangesAsync(); // Değişiklikler kaydediliyor
            }
            catch (Exception)
            {
                throw new Exception("Ürün Adı değiştirilirken bir hata oluştu."); // Hata durumunda istisna fırlatılıyor
            }

            return new ServiceMessage
            {
                IsSucceed = true
            };
        }

        // Ürün güncelleme işlemi
        public async Task<ServiceMessage> UpdateProduct(UpdateProductDto product)
        {
            var productEntity = _productRepository.GetById(product.Id); // Ürün ID ile alınır
            if (productEntity is null)
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Ürün bulunamadı" // Ürün bulunamadı hatası
                };
            }

            // Transaction başlatılır
            await _unitOfWork.BeginTransaction();
            productEntity.ProductName = product.ProductName;
            productEntity.Price = product.Price;
            productEntity.StockQuantity = product.StockQuantity;

            _productRepository.Update(productEntity); // Ürün güncellenir

            try
            {
                await _unitOfWork.SaveChangesAsync(); // Değişiklikler kaydedilir
                await _unitOfWork.CommitTransaction(); // Transaction tamamlanır
            }
            catch (Exception)
            {
                await _unitOfWork.RollBackTransaction(); // Hata durumunda işlem geri alınır
                throw new Exception("Güncelleme sırasında bir hata oldu."); // Hata mesajı döndürülür
            }

            return new ServiceMessage
            {
                IsSucceed = true,
            };
        }
    }
}