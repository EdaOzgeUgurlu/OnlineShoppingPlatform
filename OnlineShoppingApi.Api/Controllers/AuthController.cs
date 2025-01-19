using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Api.Jwt;
using OnlineShopping.Api.Models;
using OnlineShopping.Business.Operations.User;
using OnlineShopping.Business.Operations.User.Dtos;
using OnlineShopping.Data.Enums;

namespace OnlineShopping.Api.Controllers
{

    [Route("api/[controller]")]     // API rotası "api/Auth" olacak şekilde ayarlanıyor.
    [ApiController]                  // Controller'ın bir API Controller olduğunu belirtiyor.
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService; // Kullanıcı işlemleri için bağımlılık enjeksiyonu.

        // Constructor: Kullanıcı servisini enjekte ediyor.
        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]       // "api/Auth/register" endpoint'i için POST metodu.
        public async Task<IActionResult> RegisterAsync(RegisterRequest request)
        {
            // ModelState kontrolü: Gelen veriler modelle uyumlu mu?
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Geçersiz veri gönderildiyse hata döner.
            }//TODO:ileride action filter olarak kodlanacak. 

            // Gelen request'i AddUserDto'ya dönüştürüyoruz.
            var addUserDto = new AddUserDto
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Password = request.Password,
                PhoneNumber = request.PhoneNumber,
            };

            // Kullanıcıyı ekleme işlemi yapılır.
            var result = await _userService.AddUser(addUserDto);

            if (result.IsSucceed) // İşlem başarılıysa 200 OK döner.
                return Ok();
            else                 // İşlem başarısızsa 400 BadRequest döner.
                return BadRequest(result.Message);
        }

        [HttpPost("login")]          // "api/Auth/login" endpoint'i için POST metodu.
        public IActionResult LoginAsync(LoginRequest request)
        {
            // ModelState kontrolü: Gelen veriler modelle uyumlu mu?
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Geçersiz veri gönderildiyse hata döner.
            }//TODO:ileride action filter olarak kodlanacak. 

            // Kullanıcının giriş bilgileriyle doğrulama yapılır.
            var result = _userService.LoginUser(new LoginUserDto { Email = request.Email, Password = request.Password });

            if (!result.IsSucceed) // Eğer giriş başarısızsa hata döner.
                return BadRequest(result.Message);

            var user = result.Data; // Giriş yapan kullanıcının bilgileri alınır.

            // JWT token oluşturulması için gerekli konfigürasyonlar alınıyor.
            var configuration = HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var token = JwtHelper.GenerateJwtToken(new JwtDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserType = user.UserType,
                SecretKey = configuration["Jwt:SecretKey"]!,
                Issuer = configuration["Jwt:Issuer"]!,
                Audience = configuration["Jwt:Audience"]!,
                ExpireMinutes = int.Parse(configuration["Jwt:ExpireMinutes"]!)
            });

            // Başarılı giriş yanıtı ve token döndürülür.
            return Ok(new LoginResponse
            {
                Message = "Giriş başarıyla tamamlandı",
                Token = token,
            });
        }

        [HttpPatch("{id}/UserTypeEdit")] // Kullanıcı tipi düzenleme için PATCH metodu.
        [Authorize(Roles = "Admin")]    // Sadece "Admin" rolündeki kullanıcılar erişebilir.
        public async Task<IActionResult> AdjustUserType(int id, UserType changeTo)
        {
            // Kullanıcı tipi düzenleme işlemi yapılır.
            var result = await _userService.AdjustUserType(id, changeTo);

            if (!result.IsSucceed) // İşlem başarısızsa NotFound döner.
                return NotFound(result.Message);
            else                  // İşlem başarılıysa 200 OK döner.
                return Ok();
        }

        [HttpDelete("{id}")]          // Kullanıcı silmek için DELETE metodu.
        [Authorize(Roles = "Admin")]  // Sadece "Admin" rolündeki kullanıcılar erişebilir.
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _userService.DeleteUser(id);
            if (!result.IsSucceed)     // İşlem başarısızsa NotFound döner.
                return NotFound(result.Message);
            else                      // İşlem başarılıysa 200 OK döner.
                return Ok();
        }

        [HttpGet("User/{id}")]        // Belirli bir kullanıcı bilgisi almak için GET metodu.
        [Authorize(Roles = "Admin")]  // Sadece "Admin" rolündeki kullanıcılar erişebilir.
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUser(id);
            if (user is null)         // Kullanıcı bulunamazsa NotFound döner.
                return NotFound();
            else                     // Kullanıcı bulunursa 200 OK döner.
                return Ok(user);
        }

        [HttpGet("AllUsers")]         // Tüm kullanıcıları listelemek için GET metodu.
        [Authorize(Roles = "Admin")]  // Sadece "Admin" rolündeki kullanıcılar erişebilir.
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetUsers(); // Tüm kullanıcılar alınır.
            return Ok(users);                         // 200 OK ile döndürülür.
        }

        [HttpPut("{id}")]            // Kullanıcı güncellemek için PUT metodu.
        [Authorize(Roles = "Admin")] // Sadece "Admin" rolündeki kullanıcılar erişebilir.
        public async Task<IActionResult> UpdateUser(int id, UpdateUserRequest request)
        {
            // Güncelleme işlemi için DTO oluşturulur.
            var updateUserDto = new UpdateUserDto
            {
                Id = id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber
            };
            var result = await _userService.UpdateUser(updateUserDto);
            if (!result.IsSucceed)   // İşlem başarısızsa NotFound döner.
                return NotFound(result.Message);
            else                    // İşlem başarılıysa güncellenen kullanıcı bilgileri döner.
                return await GetUser(id);
        }

        [HttpGet("me")]              // Giriş yapan kullanıcının bilgilerini almak için GET metodu.
        [Authorize]                   // Sadece yetkilendirilmiş kullanıcılar erişebilir.
        public IActionResult GetMyUser()
        {
            return Ok(); // TODO: Kullanıcıya ait bilgileri dönecek şekilde geliştirilebilir.
        }
    }
}
