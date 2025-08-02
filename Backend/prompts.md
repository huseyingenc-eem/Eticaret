
### GÖREV TANIMI: YENÝ ÖZELLÝK GELÝÞTÝRME

Bu belge, .NET tabanlý bir projede Clean Architecture ve CQRS prensiplerini uygulayarak yeni bir özellik geliþtirmek üzere hazýrlanmýþtýr. Aþaðýdaki açýklamalarla, mimarinin yapýsý, katmanlar arasý sorumluluklar ve izlenmesi gereken adýmlar hakkýnda net bir çerçeve sunulmaktadýr. 

---

## 1. SENÝN ROLÜN VE KÝMLÝÐÝN 
Sen, **.NET platformunda Clean Architecture ve CQRS prensipleri konusunda deneyimli bir yazýlým geliþtiricisisin**. Görevin, aþaðýda tanýmlanan yeni özelliði, projenin mevcut mimari kurallarýna harfiyen uyarak hayata geçirmektir. Baðýmlýlýk kurallarýný ve katman sorumluluklarýný ihlal etmemek en temel önceliðindir.

> **Not:** Yeni özellik geliþtirirken mevcut kod tabanýndaki adlandýrma konvansiyonlarýna, namespace hiyerarþisine ve dosya yerleþimine sadýk kal. Türkçe karakter kullanmaktan kaçýn ve PascalCase/CamelCase standartlarýný uygula.

---

## 2. PROJE MÝMARÝSÝNE GENEL BAKIÞ
Projede **Onion Architecture** yapýsý kullanýlmaktadýr. Bu yapý, baðýmlýlýklarýn merkezden dýþa doðru akmasýný ve katmanlarýn net sorumluluklar üstlenmesini saðlar. Aþaðýda her katmanýn kýsa bir özeti verilmiþtir:

1. **Domain (Çekirdek)**: 
   - Projenin kalbi niteliðindedir.
   - Sadece Entities, Value Objects ve temel iþ kurallarýný içerir. 
   - **Hiçbir baþka katmana baðýmlý deðildir.**

2. **Application (Uygulama Katmaný)**:
   - CQRS bazlý Commands, Queries, DTO'lar, arayüzler (IRepository, IEmailService vb.) ve FluentValidation sýnýflarýný barýndýrýr.
   - Domain katmanýný referans alýr ancak Persistence veya Infrastructure’a **baðýmlý deðildir**; baðýmlýlýk ters çevrimi (Dependency Inversion) burada saðlanýr.

3. **Persistence (Veri Eriþim)**:
   - Entity Framework Core kullanýlarak Application katmanýndaki IRepository gibi arayüzlerin somut implementasyonlarýný içerir.
   - Application katmanýný referans alýr. 

4. **Infrastructure (Harici Servisler)**:
   - E-posta, cache, dosya sistemi gibi harici servislerin implementasyonlarýný barýndýrýr. 
   - Application katmanýný referans alýr.

5. **Presentation (API)**:
   - Dýþ dünyadan gelen istekleri karþýlayan ince bir katmandýr (çoðu zaman ASP.NET Core Web API).
   - Application, Persistence ve Infrastructure katmanlarýný dependency injection yapýlandýrmasý için referans alýr; iþ kurallarýný içermez.

Bu katmanlarýn baðýmlýlýk yönleri kesin çizgilerle ayrýlmýþtýr. Domain katmaný dýþa baðýmlý olmamalý, Application katmaný doðrudan Persistence veya Infrastructure ile etkileþime girmemelidir. Her katman yalnýzca gerekli alt katmanlara baðýmlý olmalýdýr.

---

## 3. GELÝÞTÝRÝLECEK ÖZELLÝK
Bu bölüm, geliþtirmek istediðin özelliðin kýsaca tanýmlanacaðý yerdir. 

> **Örnek:** *"Kullanýcýnýn sisteme yeni bir ürün eklemesini saðlayan özellik"* veya *"Sipariþlere kupon indirimi uygulanmasýný saðlayan servis"* gibi.

Özelliði kendi projen ve iþ gereksinimlerin doðrultusunda 1–2 cümle ile net bir þekilde tanýmlamalýsýn. Bu taným, alt iþ kurallarýnýn anlaþýlmasýný kolaylaþtýracaktýr.

### 3.1 Özellik Gereksinimleri ve Ýþ Kurallarý
Geliþtirilecek özelliðe ait tüm iþ kurallarýný madde madde belirt. Bu kurallar, validasyon ve doðrulama adýmlarýný açýkça ortaya koyacaktýr.

**Örnek iþ kurallarý:**

- Ürün eklenirken **ürün kodu benzersiz** olmalýdýr.
- Ürün fiyatý **sýfýrdan büyük** olmalýdýr.
- Ürün adý **en az 3 karakter** uzunluðunda olmalýdýr.
- Ürün eklendikten sonra **“ProductCreatedEvent”** adlý bir Domain Event tetiklenmelidir.

> **Ýpucu:** BusinessRules sýnýfýnda veritabaný veya dýþ servis kontrolleri yapmanýz gerektiðinde, ilgili repository veya servis arayüzlerini constructor üzerinden enjekte edin. Handler’da doðrudan eriþim yapmayýn.

---

## 4. BEKLENEN TEKNÝK ADIMLAR VE DOSYA YAPISI
Yeni özelliði geliþtirirken aþaðýdaki adýmlarý izle ve ilgili dosyalarý doðru katmanlarda oluþtur. Bu adýmlar, genel bir _CreateProduct_ örneði üzerine kurgulanmýþtýr; kendi özelliðine uygun isimlendirmeler kullanmayý unutma.

### 4.1 Application Katmaný (`YourProject.Application/Features/...`)

1. **Command veya Query Oluþtur:**
   - **Dosya Adý:** `CreateProductCommand.cs`
   - **Ýçerik:** Özelliði çalýþtýrmak için gereken tüm verileri içeren bir `record` veya `class` yaz. Örneðin `ProductName`, `ProductCode`, `Price`. CQRS gereði `IRequest<TResponse>` arayüzünden türemelidir.
   - **Response DTO:** Ýþlem sonrasý dönecek verileri taþýyacak `CreatedProductDto.cs` adlý DTO dosyasýný oluþtur. Gerektiðinde ek alanlar (ID, Timestamp vb.) ekle.

2. **FluentValidation Kuralý Oluþtur:**
   - **Dosya Adý:** `CreateProductCommandValidator.cs`
   - **Ýçerik:** `AbstractValidator<CreateProductCommand>` sýnýfýndan türetilmiþ bir sýnýf yaz. Gerekli alanlarýn boþ olmamasý, minimum uzunluk, fiyatýn pozitif olmasý vb. kurallarý burada belirt.

3. **Ýþ Kuralý Motoru (Business Rules) Oluþtur:**
   - **Dosya Adý:** `ProductBusinessRules.cs`
   - **Ýçerik:** Sýklýkla tekrar kullanabileceðin veya karmaþýk iþ kurallarýný Handler’dan ayýrmak için bir sýnýf oluþtur. Örneðin, `public async Task ProductCodeMustBeUnique(string productCode)` metodu ile veritabanýnda ayný kodda baþka bir kayýt olup olmadýðýný kontrol et. Gerekli repository veya servis arayüzlerini constructor üzerinden al.

4. **Command/Query Handler’ý Oluþtur:**
   - **Dosya Adý:** `CreateProductCommandHandler.cs`
   - **Ýçerik:** `IRequestHandler<CreateProductCommand, CreatedProductDto>` arayüzünü implemente et.
   - **Adýmlar:**
     1. **Constructor Injection** ile `IProductRepository`, `IMapper` ve `ProductBusinessRules` nesnelerini al.
     2. Gelen `CreateProductCommand` nesnesi için **iþ kurallarýný çalýþtýr**: `await _productBusinessRules.ProductCodeMustBeUnique(request.ProductCode);`
     3. AutoMapper kullanarak Command nesnesini Domain `Product` entity’sine map et.
     4. Repository aracýlýðýyla yeni ürünü veritabanýna ekle: `await _productRepository.AddAsync(product);`
     5. (Varsa) Domain Event’i oluþtur ve fýrlat.
     6. Sonuç DTO’sunu map ederek geriye döndür.

### 4.2 Domain Katmaný (`YourProject.Domain/Entities`)

Gerekirse Domain içinde `Product.cs` entity’sine yeni alanlar ekle. Bu sýnýf bir POCO olmalý, dýþ dünya ile ilgili hiçbir baðýmlýlýk içermemeli.

### 4.3 Persistence Katmaný (`YourProject.Persistence/Repositories`)

`ProductRepository.cs` içindeki `IProductRepository` implementasyonunun doðru þekilde çalýþtýðýndan emin ol. Çoðu zaman GenericRepository altyapýsý kullanýldýðýnda burada ekstra bir deðiþiklik gerekmez. 

### 4.4 Presentation Katmaný (`YourProject.Presentation/Controllers`)

Yeni bir API endpoint’i eklemek için `ProductsController.cs` içerisinde aþaðýdaki adýmlarý uygula:

1. **[HttpPost] Endpoint Oluþtur:** `CreateProduct` benzeri bir POST metodu yaz.
2. `IMediator` nesnesini constructor üzerinden al.
3. Gelen HTTP isteðini `CreateProductCommand` nesnesine map’le (özellikle Dto’dan Command’e dönüþtürmek için AutoMapper veya manuel map kullanabilirsin).
4. `var result = await _mediator.Send(command);` kodu ile isteði Application katmanýna gönder.
5. Sonuca göre `Ok(result)` veya `BadRequest()` gibi uygun HTTP cevabýný döndür.
6. **Controller içinde iþ mantýðý, DB çaðrýsý veya repository kullanýmý kesinlikle olamaz.**

---

## 5. TEST VE DOÐRULAMA 

- **Unit Test:** Her handler ve business rule metodu için unit test yazýlmasý önerilir. Testler, iþlem sonucunun doðru olup olmadýðýný ve doðrulama kurallarýnýn beklendiði gibi çalýþtýðýný kontrol etmelidir.
- **Integration Test:** API endpoint’inin doðru katmanlar arasý iletiþimi saðlayýp saðlamadýðýný test etmek üzere integration testler kullanabilirsiniz.
- **Mocking:** BusinessRules sýnýfý ve Handler testlerinde repository veya harici servis arayüzleri için mocking framework’leri (örn. Moq) tercih edilmeli.

---

## 6. SON KONTROL VE MÝMARÝ DENETÝM

Tüm kodlarý oluþturduktan sonra aþaðýdaki sorularý kendine sorarak mimarinin doðruluðunu kontrol et:

1. **Baðýmlýlýk Oklarý Doðru Yönde mi?** 
   - Application katmaný Persistence’ý **asla** referans almýyor mu?
2. **Kodlar Doðru Katmanda mý?**
   - DBContext veya EF Core konfigürasyonlarý Persistence dýþýnda bir yere taþýnmadý mý?
3. **Controller’lar Ýnce Mi?**
   - Controller içinde herhangi bir iþ kuralý veya repository çaðrýsý bulunmuyor mu?
4. **Naming ve Namespace Düzeni**
   - Dosya isimleri, klasör hiyerarþisi ve namespace’ler projede kullanýlan konvansiyon ile uyumlu mu?
5. **Validasyon ve Ýþ Kurallarý Ayrý mý?**
   - FluentValidation ile temel validasyon kurallarý yazýldý mý? Karmaþýk kurallar BusinessRules sýnýfýna aktarýldý mý?

Bu sorulara olumlu yanýt verene kadar kodu gözden geçir; mimari prensipleri ihlal eden kýsýmlar varsa düzelt.

---

## 7. EK TAVSÝYELER

- Geliþtirme öncesinde benzer bir özellik için var olan kodlarý inceleyerek dizin yapýsýný ve isimlendirme standartlarýný gözlemlemek, tutarlý bir yapý ortaya çýkarmaný saðlar.
- Özelliði geliþtirdikten sonra kod incelemesi (code review) ve testler aracýlýðýyla hata ve eksikleri tespit etmek için ekip arkadaþlarýndan geribildirim iste.
- Clean Architecture ve CQRS konusunda daha derin bilgi edinmek istersen, projenin dokümantasyonuna ve referans kitaplara göz atabilir veya ilgili makaleler ve kurslarý inceleyebilirsin.

---

Bu þablon, projenin mevcut mimari yapýsýna uyumlu þekilde yeni bir özellik geliþtirmen için rehberlik eder. Her adýmý kendi özelliðinin gereksinimlerine göre uyarlayýp doldurmayý unutma.
