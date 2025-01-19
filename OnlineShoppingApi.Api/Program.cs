using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OnlineShopping.Api.Middlewares;

//using OnlineShopping.Api.Middlewares;
using OnlineShopping.Business.DataProtection;
using OnlineShopping.Business.Operations.Order;
using OnlineShopping.Business.Operations.Product;
using OnlineShopping.Business.Operations.Setting;
using OnlineShopping.Business.Operations.User;
using OnlineShopping.Data.Context;
using OnlineShopping.Data.Repositories;
using OnlineShopping.Data.UnitOfWork;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        // WebApplication nesnesi oluþturuluyor. Uygulama yapýlandýrmasý burada baþlar.
        var builder = WebApplication.CreateBuilder(args);

        // Servisleri konteynerimize ekliyoruz.
        // Bu servisler, uygulamanýn çalýþma zamanýnda kullanýlacak bileþenlerdir.
        builder.Services.AddControllers();  // MVC denetleyicilerini ekliyoruz.

        // Swagger/OpenAPI dokümantasyonunu ekliyoruz. API'nin belgelendirilmesi için gereklidir.
        builder.Services.AddEndpointsApiExplorer();

        // Swagger yapýlandýrmasý
        builder.Services.AddSwaggerGen(options =>
        {
            // JWT güvenlik þemasý tanýmlanýyor. Swagger için authentication iþlemini yapýlandýrýyoruz.
            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                Scheme = "Bearer",   // Bearer token kullanýyoruz
                BearerFormat = "JWT",  // JWT formatýný belirtiyoruz
                Name = "Jwt Authentication", // Authentication adý
                In = ParameterLocation.Header,  // Authorization header'ýnda yer alacak
                Type = SecuritySchemeType.Http,  // HTTP güvenlik türü
                Description = "JWT Token'ini aþaðýdaki kutuya yapýþtýrýn.", // Swagger UI açýklamasý
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,  // JWT authentication þemasýný referans olarak belirliyoruz
                    Type = ReferenceType.SecurityScheme
                }
            };

            // Güvenlik þemasý ekleniyor.
            options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

            // Güvenlik gereksinimi ekliyoruz. Bu, Swagger UI'da JWT'yi gerektiren API uç noktalarý için geçerli olacak.
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {jwtSecurityScheme, Array.Empty<string>() }
        });
        });

        // Veri koruma servisini ekliyoruz. Bu, hassas verilerin güvenli bir þekilde saklanmasýna olanak tanýr.
        builder.Services.AddScoped<IDataProtection, DataProtection>();

        // Þifreli veri koruma anahtarlarý için dizin belirliyoruz.
        var keysDirectory = new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "App_Data", "Keys"));

        // Veri koruma servisini yapýlandýrýyoruz ve anahtarlarý dosya sistemine kaydediyoruz.
        builder.Services.AddDataProtection()
            .SetApplicationName("OnlineShoppingProject")  // Uygulama ismi
            .PersistKeysToFileSystem(keysDirectory);  // Anahtarlarý belirtilen dizine kaydediyoruz.

        // JWT authentication yapýlandýrmasý ekleniyor.
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // JWT'nin geçerliliðini doðrulamak için gerekli parametreler
                    ValidateIssuer = true,  // Issuer'ý doðrula
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],  // Geçerli Issuer bilgisi

                    ValidateAudience = true,  // Audience'ý doðrula
                    ValidAudience = builder.Configuration["Jwt:Audience"],  // Geçerli Audience bilgisi

                    ValidateLifetime = true,  // Token süresi geçerliyse kabul et
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))  // Gizli anahtar
                };
            });

        // Veritabaný baðlantý dizesi alýnýyor ve DbContext ekleniyor.
        var connectionString = builder.Configuration.GetConnectionString("default");
        builder.Services.AddDbContext<ShoppingAppDbContext>(options => options.UseSqlServer(connectionString));

        // Repository, UnitOfWork ve servis sýnýflarý ekleniyor. Bu sýnýflar baðýmlýlýk enjeksiyonuna dahil ediliyor.
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IUserService, UserManager>();
        builder.Services.AddScoped<IProductService, ProductManager>();
        builder.Services.AddScoped<IOrderService, OrderManager>();
        builder.Services.AddScoped<ISettingService, SettingManager>();

        // Uygulama baþlatýlýyor.
        var app = builder.Build();

        // HTTP istek boru hattýný yapýlandýrýyoruz.
        if (app.Environment.IsDevelopment())
        {
            // Eðer geliþtirme ortamýndaysak Swagger ve Swagger UI'yi etkinleþtiriyoruz.
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Bakým modu kontrolü ekleniyor. Uygulama bakýmda mý kontrol ediliyor.
       app.UseMaintenanceMode();

        // HTTPS yönlendirmesini etkinleþtiriyoruz.
        app.UseHttpsRedirection();

        // Kimlik doðrulama ve yetkilendirme iþlemleri
        app.UseAuthentication();
        app.UseAuthorization();

        // Controller'larý uygulamaya baðlýyoruz. API uç noktalarýna eriþim saðlanacak.
        app.MapControllers();

        // Özelleþtirilmiþ hata iþleme (middleware) ekliyoruz. Hatalarý merkezi bir yerde yönetiyoruz.
        //app.UseMiddleware<ExceptionMiddleware>();

        // Uygulamanýn çalýþmasýný baþlatýyoruz.
        app.Run();
    }
}