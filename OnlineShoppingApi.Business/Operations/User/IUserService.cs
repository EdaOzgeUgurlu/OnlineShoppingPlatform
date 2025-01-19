using OnlineShopping.Business.Operations.User.Dtos;
using OnlineShopping.Business.Types;
using OnlineShopping.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Business.Operations.User
{
    public interface IUserService
    {

        Task<ServiceMessage> AddUser(AddUserDto user); // Yeni bir kullanıcı ekler
        ServiceMessage<UserInfoDto> LoginUser(LoginUserDto user); // Kullanıcı giriş işlemi yapar ve kullanıcı bilgilerini döndürür
        Task<UserInfoDto> GetUser(int id); // Belirli bir kullanıcıyı id'ye göre alır
        Task<List<UserInfoDto>> GetUsers(); // Tüm kullanıcıları alır
        Task<ServiceMessage> DeleteUser(int id); // Belirli bir kullanıcıyı siler
        Task<ServiceMessage> UpdateUser(UpdateUserDto user); // Mevcut bir kullanıcıyı günceller
        Task<ServiceMessage> AdjustUserType(int id, UserType changeTo); // Kullanıcı tipini değiştirir
    }
}

