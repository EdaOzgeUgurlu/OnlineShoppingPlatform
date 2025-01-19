# 🌐 **Online Alışveriş Platformu - Çok Katmanlı ASP.NET Core Web API Projesi** 🛍️

Bu proje, **ASP.NET Core Web API** teknolojisiyle geliştirilmiş, **çok katmanlı mimari** yapısına sahip bir **online alışveriş platformudur**. **Entity Framework Core (Code First)** kullanılarak veri tabanı etkileşimi sağlanmış, gelişmiş güvenlik özellikleri (JWT, Data Protection), kullanıcı yönetimi, yetkilendirme ve daha fazlasını içeren modern web geliştirme standartlarına sahip bir projedir. 🚀

## 📜 **Proje Hakkında** 🖥️

Online alışveriş platformu, **müşteri** ve **admin** rollerine sahip kullanıcıların, ürünleri görüntüleyip sipariş verebileceği ve yöneticilerin siparişleri yönetebileceği bir sistem sunmaktadır. Ayrıca, **JWT (JSON Web Token)** ile güvenli bir kimlik doğrulama ve yetkilendirme mekanizması entegre edilmiştir. Kullanıcı ve ürünlerin verileri **Entity Framework Core** ile yönetilmektedir.

## 🌟 **Proje Özellikleri** ✨

### 🔑 **Kimlik Doğrulama ve Yetkilendirme**

- **ASP.NET Core Identity** veya **Custom Identity Servisi** kullanılarak kullanıcı doğrulaması yapılmaktadır. 💡
- **JWT (JSON Web Token)** ile güvenli kimlik doğrulama sağlanır ve kullanıcıların rolleri üzerinden (Admin, Customer) yetkilendirme yapılır. 🔐
- Admin kullanıcıları için özel erişim yetkileri tanımlanmıştır. 👑

### 🧑‍💻 **Katmanlı Mimari** 📊

- **Presentation Layer (API Katmanı)**: API controller'ları ve uç noktalar burada yer almaktadır.
- **Business Layer (İş Katmanı)**: İş mantığı ve servisler burada yönetilmektedir.
- **Data Access Layer (Veri Erişim Katmanı)**: Entity Framework ile veritabanı etkileşimleri ve Repository, UnitOfWork pattern'ları bu katmanda yönetilmektedir.

### 🗂️ **Veri Modeli** 📦

- **User (Kullanıcı)**:  
  - `Id`, `FirstName`, `LastName`, `Email`, `PhoneNumber`, `Password`, `Role` (Enum) özelliklerine sahiptir.  
  - Şifreler **Data Protection** ile güvenli şekilde saklanır. 🔒

- **Product (Ürün)**:  
  - `Id`, `ProductName`, `Price`, `StockQuantity` gibi temel özelliklere sahiptir.  
  - Ürünler ve siparişler arasında **Çoktan Çoka** ilişki bulunmaktadır.

- **Order (Sipariş)**:  
  - `Id`, `OrderDate`, `TotalAmount`, `CustomerId` gibi özelliklere sahiptir.
  - **OrderProduct** ile ilişkilidir, yani bir sipariş birden fazla ürünü içerebilir ve bir ürün birden fazla siparişte yer alabilir. 📦

### 🛠️ **Özelleşmiş Araçlar ve Teknolojiler**

- **Middleware**:
  - **Loglama Middleware**: Gelen tüm isteklerin URL'si, zamanı ve kimliği kaydedilir. 📜
  - **Maintenance Middleware**: Uygulama bakımda olduğunda devreye girer ve kullanıcıları bilgilendirir. ⚙️

- **Action Filter**:
  - Zaman dilimi kontrollü erişim ve belirli API'lere kısıtlı erişim için **Action Filter** kullanılır. ⏳

- **Model Validation**:
  - Kullanıcı ve ürün modelleri üzerinde doğrulama yapılır: zorunlu alanlar, e-posta formatı, stok kontrolü vb. ✅

- **Dependency Injection (DI)**:
  - Servisler ve bağımlılıklar DI ile yönetilir. 🔄

- **Data Protection**:
  - Kullanıcı şifreleri **Data Protection API** ile güvenli bir şekilde şifrelenir. 🔐

- **Global Exception Handling**:
  - Uygulama genelinde meydana gelen tüm hatalar global olarak yakalanır ve uygun mesajlar döndürülür. ⚠️

## 🏗️ **API Endpoints** 📡

### **AuthController** - Kullanıcı Yönetimi 🔑
- **POST** `/register`: Yeni kullanıcı kaydı oluşturma.
- **POST** `/login`: Kullanıcı girişi ve JWT token oluşturma.
- **PATCH** `/{id}/UserTypeEdit`: Kullanıcı rolü düzenleme (Admin rolü ile yapılabilir).
- **DELETE** `/{id}`: Kullanıcı silme (Admin rolü ile yapılabilir).
- **GET** `/User/{id}`: Kullanıcı bilgilerini getir.
- **GET** `/AllUsers`: Tüm kullanıcıları listele.
- **PUT** `/{id}`: Kullanıcı bilgilerini güncelleme.
- **GET** `/me`: Giriş yapmış kullanıcının bilgilerini al.

### **OrdersController** - Sipariş Yönetimi 🛍️
- **GET** `/{id}`: Sipariş bilgilerini getir.
- **GET** `/`: Tüm siparişleri listele.
- **POST** `/`: Yeni sipariş ekle (Admin rolü ile yapılabilir).
- **PATCH** `/{id}/OrderUserEdit`: Siparişin kullanıcı bilgisini düzenle (Admin rolü ile yapılabilir).
- **DELETE** `/{id}`: Siparişi silme (Admin rolü ile yapılabilir).

### **ProductController** - Ürün Yönetimi 📦
- **GET** `/`: Tüm ürünleri listele.
- **GET** `/{id}`: Ürün bilgilerini getir.
- **POST** `/`: Yeni ürün ekle (Admin rolü ile yapılabilir).
- **PUT** `/{id}`: Ürün bilgilerini güncelle (Admin rolü ile yapılabilir).
- **DELETE** `/{id}`: Ürün silme (Admin rolü ile yapılabilir).

## 💻 **Projeyi Çalıştırma Adımları** 🚀

### 1️⃣ **Proje Kodunu Klonla**

Öncelikle projeyi GitHub üzerinden bilgisayarınıza indirin:

```bash
git clone https://github.com/<username>/online-shopping-platform.git
cd online-shopping-platform
```

### 2️⃣ **Gerekli Paketleri Yükleyin**

NuGet paketlerini yüklemek için:

```bash
dotnet restore
```

### 3️⃣ **Veritabanı Migration'larını Uygula**

Veritabanı şemasını oluşturmak için:

```bash
dotnet ef database update
```

### 4️⃣ **Projeyi Çalıştır**

Projeyi başlatmak için:

```bash
dotnet run
```

API'niz, **https://localhost:5001** adresinde çalışacaktır.

## 🔧 **Kullanılan Teknolojiler** 🛠️

- **ASP.NET Core** (Web API)
- **Entity Framework Core** (Code First)
- **JWT (JSON Web Token)** 🔐
- **ASP.NET Core Identity** (Kullanıcı Yönetimi)
- **Data Protection API** (Şifreleme)
- **Dependency Injection** (Bağımlılık Yönetimi)
- **Middleware & Action Filters** (Özelleştirilmiş istek loglama ve zaman dilimi kontrolü)
- **Swagger** (API dokümantasyonu)

## 🌍 **Proje Geliştirme Süreci** 📅

- **Zaman Çizelgesi**:
  - 1. Aşama: Gereksinimlerin belirlenmesi ve temel API yapısının kurulması.
  - 2. Aşama: Kimlik doğrulama, yetkilendirme, ve JWT entegrasyonu.
  - 3. Aşama: Katmanlı mimari, Entity Framework ve veritabanı yapısının oluşturulması.
  - 4. Aşama: Kullanıcı ve ürün yönetimi, sipariş işlemleri.
  - 5. Aşama: Hata yönetimi, middleware, action filter ve model doğrulama entegrasyonu.

## 📄 **Lisans ve Diğer Bilgiler** 📝

Bu proje **MIT Lisansı** altında lisanslanmıştır.  
**Geliştirici**: Eda Özge Uğurlu 🧑‍💻  
**Proje Tarihi**: 2025 📅

---

Bu proje, **online alışveriş** süreçlerinin güvenli ve verimli bir şekilde yönetilmesini sağlayan kapsamlı bir çözüm sunmaktadır. Hem **admin** kullanıcıları hem de **müşteriler** için güçlü bir yönetim paneli ve işlemler sunmaktadır. Her türlü geliştirme ve katkıya açıktır! 🎉
