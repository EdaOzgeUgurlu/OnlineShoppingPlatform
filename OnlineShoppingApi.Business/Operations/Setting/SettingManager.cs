using OnlineShopping.Data.Entities;
using OnlineShopping.Data.Repositories;
using OnlineShopping.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Business.Operations.Setting
{
    public class SettingManager : ISettingService
    {
        // Bağımlılıkları içeren alanlar
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<SettingEntity> _settingRepository;

        // Constructor ile bağımlılıkların çözülmesi
        public SettingManager(IUnitOfWork unitOfWork, IRepository<SettingEntity> settingRepository)
        {
            _settingRepository = settingRepository; // Ayar repository'si
            _unitOfWork = unitOfWork; // Unit of work (işlem birliği)
        }

        // Bakım modunun durumunu alır (true/false)
        public bool GetMaintenanceState()
        {
            // ID'si 1 olan ayar kaydını alarak bakım modu bilgisini döndürür
            var maintenanceState = _settingRepository.GetById(1).MaintenanceMode;

            return maintenanceState; // Bakım modu durumunu döndürür
        }

        // Bakım modunu açar veya kapatır
        public async Task ToggleMaintenance()
        {
            // ID'si 1 olan ayar kaydını alır
            var setting = _settingRepository.GetById(1);

            // Bakım modunun durumunu tersine çevirir
            setting.MaintenanceMode = !setting.MaintenanceMode;

            /*_settingRepository.Update(setting);*/ // Ayar kaydını günceller

            try
            {
                // Değişiklikleri veritabanına kaydeder
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                // Hata durumunda özel SettingException fırlatılır
                throw new SettingException("Bakım durumu güncellenirken bir hata ile karşılaşıldı.");
            }
        }
    }
}

