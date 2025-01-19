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
        // WebApplication nesnesi olu�turuluyor. Uygulama yap�land�rmas� burada ba�lar.
        var builder = WebApplication.CreateBuilder(args);

        // Servisleri konteynerimize ekliyoruz.
        // Bu servisler, uygulaman�n �al��ma zaman�nda kullan�lacak bile�enlerdir.
        builder.Services.AddControllers();  // MVC denetleyicilerini ekliyoruz.

        // Swagger/OpenAPI dok�mantasyonunu ekliyoruz. API'nin belgelendirilmesi i�in gereklidir.
        builder.Services.AddEndpointsApiExplorer();

        // Swagger yap�land�rmas�
        builder.Services.AddSwaggerGen(options =>
        {
            // JWT g�venlik �emas� tan�mlan�yor. Swagger i�in authentication i�lemini yap�land�r�yoruz.
            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                Scheme = "Bearer",   // Bearer token kullan�yoruz
                BearerFormat = "JWT",  // JWT format�n� belirtiyoruz
                Name = "Jwt Authentication", // Authentication ad�
                In = ParameterLocation.Header,  // Authorization header'�nda yer alacak
                Type = SecuritySchemeType.Http,  // HTTP g�venlik t�r�
                Description = "JWT Token'ini a�a��daki kutuya yap��t�r�n.", // Swagger UI a��klamas�
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,  // JWT authentication �emas�n� referans olarak belirliyoruz
                    Type = ReferenceType.SecurityScheme
                }
            };

            // G�venlik �emas� ekleniyor.
            options.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

            // G�venlik gereksinimi ekliyoruz. Bu, Swagger UI'da JWT'yi gerektiren API u� noktalar� i�in ge�erli olacak.
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {jwtSecurityScheme, Array.Empty<string>() }
        });
        });

        // Veri koruma servisini ekliyoruz. Bu, hassas verilerin g�venli bir �ekilde saklanmas�na olanak tan�r.
        builder.Services.AddScoped<IDataProtection, DataProtection>();

        // �ifreli veri koruma anahtarlar� i�in dizin belirliyoruz.
        var keysDirectory = new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "App_Data", "Keys"));

        // Veri koruma servisini yap�land�r�yoruz ve anahtarlar� dosya sistemine kaydediyoruz.
        builder.Services.AddDataProtection()
            .SetApplicationName("OnlineShoppingProject")  // Uygulama ismi
            .PersistKeysToFileSystem(keysDirectory);  // Anahtarlar� belirtilen dizine kaydediyoruz.

        // JWT authentication yap�land�rmas� ekleniyor.
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // JWT'nin ge�erlili�ini do�rulamak i�in gerekli parametreler
                    ValidateIssuer = true,  // Issuer'� do�rula
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],  // Ge�erli Issuer bilgisi

                    ValidateAudience = true,  // Audience'� do�rula
                    ValidAudience = builder.Configuration["Jwt:Audience"],  // Ge�erli Audience bilgisi

                    ValidateLifetime = true,  // Token s�resi ge�erliyse kabul et
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))  // Gizli anahtar
                };
            });

        // Veritaban� ba�lant� dizesi al�n�yor ve DbContext ekleniyor.
        var connectionString = builder.Configuration.GetConnectionString("default");
        builder.Services.AddDbContext<ShoppingAppDbContext>(options => options.UseSqlServer(connectionString));

        // Repository, UnitOfWork ve servis s�n�flar� ekleniyor. Bu s�n�flar ba��ml�l�k enjeksiyonuna dahil ediliyor.
        builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddScoped<IUserService, UserManager>();
        builder.Services.AddScoped<IProductService, ProductManager>();
        builder.Services.AddScoped<IOrderService, OrderManager>();
        builder.Services.AddScoped<ISettingService, SettingManager>();

        // Uygulama ba�lat�l�yor.
        var app = builder.Build();

        // HTTP istek boru hatt�n� yap�land�r�yoruz.
        if (app.Environment.IsDevelopment())
        {
            // E�er geli�tirme ortam�ndaysak Swagger ve Swagger UI'yi etkinle�tiriyoruz.
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Bak�m modu kontrol� ekleniyor. Uygulama bak�mda m� kontrol ediliyor.
       app.UseMaintenanceMode();

        // HTTPS y�nlendirmesini etkinle�tiriyoruz.
        app.UseHttpsRedirection();

        // Kimlik do�rulama ve yetkilendirme i�lemleri
        app.UseAuthentication();
        app.UseAuthorization();

        // Controller'lar� uygulamaya ba�l�yoruz. API u� noktalar�na eri�im sa�lanacak.
        app.MapControllers();

        // �zelle�tirilmi� hata i�leme (middleware) ekliyoruz. Hatalar� merkezi bir yerde y�netiyoruz.
        //app.UseMiddleware<ExceptionMiddleware>();

        // Uygulaman�n �al��mas�n� ba�lat�yoruz.
        app.Run();
    }
}