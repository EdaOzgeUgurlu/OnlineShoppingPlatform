using Microsoft.EntityFrameworkCore;
using OnlineShopping.Business.DataProtection;
using OnlineShopping.Business.Operations.User.Dtos;
using OnlineShopping.Business.Types;
using OnlineShopping.Data.Entities;
using OnlineShopping.Data.Enums;
using OnlineShopping.Data.Repositories;
using OnlineShopping.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Business.Operations.User
{
    // Kullanıcı işlemleri yöneticisi
    public class UserManager : IUserService
    {
        private readonly IUnitOfWork _unitOfWork; // İşlem birliği (Unit of Work)
        private readonly IRepository<UserEntity> _userRepository; // Kullanıcı repository'si
        private readonly IDataProtection _protector; // Şifreleme/şifre çözme işlemleri için IDataProtection

        // Constructor ile bağımlılıkların çözülmesi
        public UserManager(IUnitOfWork unitOfWork, IRepository<UserEntity> userRepository, IDataProtection protector)
        {
            _unitOfWork = unitOfWork; // İşlem birliği
            _userRepository = userRepository; // Kullanıcı repository'si
            _protector = protector; // Şifreleme/şifre çözme işlemleri için IDataProtection
        }

        // Yeni bir kullanıcı ekler
        public async Task<ServiceMessage> AddUser(AddUserDto user)
        {
            var hasMail = _userRepository.GetAll(x => x.Email.ToLower() == user.Email.ToLower()); // Aynı email ile kullanıcı olup olmadığını kontrol eder

            if (hasMail.Any()) // Eğer email mevcutsa
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Email adresi zaten mevcut." // E-posta zaten mevcutsa hata mesajı döner
                };
            }

            // Yeni kullanıcıyı temsil eden entity nesnesi oluşturulur
            var userEntity = new UserEntity()
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Password = _protector.Protect(user.Password), // Şifreyi şifreler
                PhoneNumber = user.PhoneNumber,
                UserType = UserType.Customer // Varsayılan olarak müşteri tipi atanır
            };

            _userRepository.Add(userEntity); // Yeni kullanıcı eklenir

            try
            {
                await _unitOfWork.SaveChangesAsync(); // Değişiklikler kaydedilir
            }
            catch (Exception)
            {
                throw new UserException("Kayıt sırasında bir hata oluştu."); // Hata durumunda özel istisna fırlatılır
            }

            return new ServiceMessage
            {
                IsSucceed = true
            };
        }

        // Kullanıcı tipini değiştirir
        public async Task<ServiceMessage> AdjustUserType(int id, UserType changeTo)
        {
            var user = _userRepository.GetById(id); // Kullanıcıyı id'ye göre alır
            if (user is null) // Kullanıcı bulunamazsa
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Id ile kullanıcı bulunamadı" // Hata mesajı döner
                };
            }

            user.UserType = changeTo; // Kullanıcı tipini değiştirir
            _userRepository.Update(user); // Kullanıcı güncellenir

            try
            {
                await _unitOfWork.SaveChangesAsync(); // Değişiklikler kaydedilir
            }
            catch (Exception)
            {
                throw new Exception("Kullanıcı türü değiştirilirken bir hata oluştu."); // Hata durumunda istisna fırlatılır
            }

            return new ServiceMessage
            {
                IsSucceed = true
            };
        }

        // Belirli bir kullanıcıyı siler
        public async Task<ServiceMessage> DeleteUser(int id)
        {
            var user = _userRepository.GetById(id); // Kullanıcıyı id'ye göre alır
            if (user is null) // Kullanıcı bulunamazsa
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Silinmek istenen kullanıcı bulunamadı" // Hata mesajı döner
                };
            }

            _userRepository.Delete(id); // Kullanıcı silinir

            try
            {
                await _unitOfWork.SaveChangesAsync(); // Değişiklikler kaydedilir
            }
            catch (Exception)
            {
                throw new Exception("Silme işlemi sırasında bir hata oluştu."); // Hata durumunda istisna fırlatılır
            }

            return new ServiceMessage
            {
                IsSucceed = true
            };
        }

        public async Task<UserInfoDto> GetUser(int id)
        {
            var user = await _userRepository.GetAll(x => x.Id == id) // Kullanıcıyı id'ye göre alır
                .Select(x => new UserInfoDto // Kullanıcıyı DTO'ya dönüştürür
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    UserType = x.UserType
                }).FirstOrDefaultAsync();

            if (user == null)
            {
                throw new KeyNotFoundException("Kullanıcı bulunamadı"); // Kullanıcı bulunamadığında hata fırlat
            }

            return user; // Kullanıcı bilgilerini döner
        }

        // Tüm kullanıcıları alır
        public async Task<List<UserInfoDto>> GetUsers()
        {
            var users = await _userRepository.GetAll() // Tüm kullanıcıları alır
                .Select(x => new UserInfoDto // Kullanıcıları DTO'ya dönüştürür
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    UserType = x.UserType
                }).ToListAsync();

            return users; // Tüm kullanıcı bilgilerini döner
        }

        // Kullanıcı giriş işlemi yapar ve kullanıcı bilgilerini döndürür
        public ServiceMessage<UserInfoDto> LoginUser(LoginUserDto user)
        {
            var userEntity = _userRepository.Get(x => x.Email.ToLower() == user.Email.ToLower()); // E-posta ile kullanıcıyı bulur
            if (userEntity == null) // Kullanıcı bulunamazsa
            {
                throw new InvalidCredentialsException("Kullanıcı adı veya şifre hatalı"); // Geçersiz giriş
            }

            var unprotectedPassword = _protector.UnProtect(userEntity.Password); // Şifreyi çözerek kontrol eder

            if (unprotectedPassword == user.Password) // Şifre doğruysa
            {
                return new ServiceMessage<UserInfoDto>
                {
                    IsSucceed = true,
                    Data = new UserInfoDto // Başarılı girişte kullanıcı bilgilerini döner
                    {
                        Email = userEntity.Email,
                        FirstName = userEntity.FirstName,
                        LastName = userEntity.LastName,
                        UserType = userEntity.UserType
                    }
                };
            }
            else // Şifre yanlışsa
            {
                throw new InvalidCredentialsException("Kullanıcı adı veya şifre hatalı"); // Geçersiz giriş
            }
        }

        // Mevcut bir kullanıcıyı günceller
        public async Task<ServiceMessage> UpdateUser(UpdateUserDto user)
        {
            var userEntity = _userRepository.GetById(user.Id); // Kullanıcıyı id'ye göre alır
            if (userEntity is null) // Kullanıcı bulunamazsa
            {
                return new ServiceMessage
                {
                    IsSucceed = false,
                    Message = "Kullanıcı bulunamadı" // Hata mesajı döner
                };
            }

            await _unitOfWork.BeginTransaction(); // Yeni bir işlem başlatılır
            userEntity.FirstName = user.FirstName;
            userEntity.LastName = user.LastName;
            userEntity.PhoneNumber = user.PhoneNumber; // Kullanıcı bilgileri güncellenir

            _userRepository.Update(userEntity); // Kullanıcı güncellenir

            try
            {
                await _unitOfWork.SaveChangesAsync(); // Değişiklikler kaydedilir
                await _unitOfWork.CommitTransaction(); // İşlem onaylanır
            }
            catch (Exception)
            {
                await _unitOfWork.RollBackTransaction(); // Hata durumunda işlem geri alınır
                throw new Exception("Güncelleme sırasında bir hata oldu."); // Hata mesajı döner
            }

            return new ServiceMessage
            {
                IsSucceed = true
            };
        }
    }
}

    