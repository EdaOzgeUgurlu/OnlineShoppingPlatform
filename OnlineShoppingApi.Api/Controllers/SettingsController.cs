using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OnlineShopping.Business.Operations.Setting;

namespace OnlineShopping.Api.Controllers
{
    [Route("api/[controller]")] // API rotası "api/Settings" olacak şekilde ayarlanıyor.
    [ApiController]             // Controller'ın bir API Controller olduğunu belirtir.
    public class SettingsController : ControllerBase
    {
        private readonly ISettingService _settingService; // Ayarlar için servis bağımlılığı.

        public SettingsController(ISettingService settingService)
        {
            _settingService = settingService; // Servis enjekte ediliyor.
        }

        [HttpPatch] // Bakım modunu açma/kapatma işlemi için PATCH metodu.
        [Authorize(Roles = "Admin")] // Sadece "Admin" rolündeki kullanıcılar bu işleme erişebilir.
        public async Task<IActionResult> ToggleMaintenence()
        {
            await _settingService.ToggleMaintenance(); // Bakım modu durumu değiştirilir.

            return Ok(); // İşlem başarılıysa 200 OK döner.
        }
    }

}
