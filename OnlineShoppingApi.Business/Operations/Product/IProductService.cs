using OnlineShopping.Business.Operations.Product.Dtos;
using OnlineShopping.Business.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Business.Operations.Product
{
    public interface IProductService
    {
     
            Task<ServiceMessage> AddProduct(AddProductDto product); // Ürün ekler
            Task<ProductDto> GetProduct(int id); // Ürünü ID'ye göre alır
            Task<List<ProductDto>> GetProducts(); // Tüm ürünleri alır
            Task<ServiceMessage> DeleteProduct(int id); // Ürünü siler
            Task<ServiceMessage> AdjustProductName(int id, string changeTo); // Ürün adını değiştirir
            Task<ServiceMessage> UpdateProduct(UpdateProductDto product); // Ürünü günceller
        
    }
}
