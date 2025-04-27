# E-Ticaret Backend Projesi

##  GİRİŞ

Bu proje, modern e-ticaret uygulamaları için sağlam ve ölçeklenebilir bir backend altyapısı sunmayı amaçlamaktadır. Temiz Mimari (Clean Architecture) prensipleri üzerine kurulu olup, CQRS (Command Query Responsibility Segregation) ve Repository gibi tasarım desenlerini kullanmaktadır.

## KULLANILAN TEKNOLOJİLER

* **.NET 8:** Ana platform ve framework.
* **ASP.NET Core 8:** Web API altyapısı için.
* **Entity Framework Core 8:** Veri erişimi ve ORM için.
* **ASP.NET Core Identity:** Kullanıcı kimlik doğrulama ve yetkilendirme için.
* **MediatR:** CQRS desenini uygulamak ve Application katmanındaki işlemleri yönetmek için.
* **AutoMapper:** Nesneler arası otomatik dönüşüm (mapping) için.
* **Redis (Opsiyonel):** Performans artışı için caching (önbellekleme) amacıyla kullanılıyor.
* **JWT (JSON Web Tokens):** API güvenliği ve kimlik doğrulama token'ları için.
* **SQL Server / Diğer İlişkisel Veritabanı:** EF Core tarafından desteklenen bir veritabanı (Migrations dosyalarına göre).

## MİMARİ

Proje, sorumlulukların ayrılması ve test edilebilirliği artırmak amacıyla **Temiz Mimari (Clean Architecture)** prensiplerine uygun olarak katmanlara ayrılmıştır:

1.  **Domain:** Entity'ler ve temel iş kurallarını içerir. Bağımsızdır.
2.  **Application:** İş mantığını, CQRS komutlarını/sorgularını, DTO'ları, Validation'ları, Mapper profillerini ve servis interface'lerini içerir. Domain katmanına bağımlıdır.
3.  **Persistence:** Veri erişim katmanıdır. DbContext, Repository implementasyonları, EF Core Configuration'ları ve Migrations'ları içerir. Application ve Domain katmanlarına bağımlıdır.
4.  **Presentation:** API endpoint'lerini (Controllers), Middleware'leri (örn: Exception Handling) ve API ile ilgili diğer yapılandırmaları içerir. Application katmanına bağımlıdır.
5.  **Core Katmanları:**
    * `Core.Application`: Application katmanı için temel pipeline davranışları (Authorization, Performance) gibi soyutlamalar içerir.
    * `Core.Persistence`: Repository deseninin temel implementasyonları (IAsyncRepository, IRepository, EfRepositoryBase) ve base Entity gibi yapıları barındırır.
    * `Core.CrossCuttingConcerns`: Exception sınıfları ve ProblemDetails gibi katmanlar arası kullanılan yardımcıları içerir.

## ÖZELLİKLER (Mevcut ve Geliştirilen)

* **Kimlik Doğrulama & Yetkilendirme:**
    * Kullanıcı Kayıt (Register)
    * Kullanıcı Giriş (Login)
    * JWT ile Token tabanlı kimlik doğrulama
    * Rol Yönetimi (Temel)
    * Kullanıcı-Rol Ataması (Temel)
    * Rol tabanlı Yetkilendirme (Pipeline ve Attribute'lar ile)
* **Kategori Yönetimi:**
    * CRUD İşlemleri (Ekleme, Güncelleme, Silme)
    * Hiyerarşik Kategori Listeleme (Ağaç Yapısı)
    * Kategoriye Ait Ürünleri Listeleme (Geliştiriliyor)
* **Ürün Yönetimi:**
    * CRUD İşlemleri (Ekleme, Güncelleme, Silme)
    * Kategoriye Göre Ürün Listeleme
    * Fiyat Aralığına Göre Listeleme
    * İsme Göre Arama/Listeleme
    * Redis ile Liste Önbellekleme
* **Tedarikçi Yönetimi:**
    * CRUD İşlemleri (Geliştiriliyor)
* **Genel:**
    * Merkezi Hata Yönetimi (Exception Handling Middleware)
    * Repository Deseni (Generic)
    * CQRS (MediatR ile)

## BAŞLANGIÇ (Getting Started)

*(Bu bölüm projenin nasıl kurulup çalıştırılacağını anlatmalıdır. Örneğin:)*

1.  **Ön Gereksinimler:**
    * .NET 8 SDK
    * SQL Server (veya kullanılan başka bir veritabanı)
    * Redis (Eğer aktif olarak kullanılıyorsa)
2.  **Veritabanı Kurulumu:**
    * `ETicaret.Presentation` projesindeki `appsettings.Development.json` dosyasında veritabanı bağlantı string'ini (`ConnectionStrings`) kendi yerel ortamınıza göre güncelleyin.
    * Package Manager Console (PMC) veya `dotnet CLI` kullanarak migration'ları uygulayın:
        ```bash
        # PMC içinde Persistence projesi seçiliyken:
        Update-Database

        # veya dotnet CLI ile Presentation projesi dizinindeyken:
        dotnet ef database update --project ../ETicaret.Persistence/ETicaret.Persistence.csproj --startup-project .
        ```
3.  **Uygulamayı Çalıştırma:**
    * `ETicaret.Presentation` projesini Visual Studio veya `dotnet run` komutu ile başlatın.
    * API genellikle `https://localhost:xxxx` veya `http://localhost:yyyy` adresinde çalışmaya başlayacaktır.

## API ENDPOINTS

*(Swagger veya Controller listesi buraya eklenebilir)*

* `api/Auth`
* `api/Products`
* `api/Categories` (CategoryController.cs)
* `api/Roles`
* `api/UserRoles`
* `api/Suppliers` (Oluşturuldu)
