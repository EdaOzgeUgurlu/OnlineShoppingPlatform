using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace OnlineShopping.Api.Filters
{
    public class TimeControlFilter : ActionFilterAttribute
    {
        // İzin verilen zaman aralığı başlangıç saati.
        public string StartTime { get; set; } = "";

        // İzin verilen zaman aralığı bitiş saati.
        public string EndTime { get; set; } = "";

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Geçerli zamanı al.
            var now = DateTime.Now.TimeOfDay;

            // Varsayılan başlangıç ve bitiş zamanlarını ayarla.
            StartTime = "23:00";
            EndTime = "23:59";

            // Eğer mevcut zaman belirtilen aralıktaysa işlemi devam ettir.
            if (now >= TimeSpan.Parse(StartTime) && now <= TimeSpan.Parse(EndTime))
            {
                base.OnActionExecuting(context); // İşlem devam eder.
            }
            else
            {
                // Eğer zaman aralığı dışındaysa 403 Forbidden dön.
                context.Result = new ContentResult()
                {
                    Content = "Bu saatler arasında bu endpoint'e istek atılamaz.", // Kullanıcıya mesaj.
                    StatusCode = 403 // HTTP 403 durum kodu.
                };
            }
        }
    }
}

