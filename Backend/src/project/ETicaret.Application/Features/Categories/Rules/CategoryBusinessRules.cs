using Core.Application.Abstractions.Repositories;
using Core.Application.Common.Exceptions;
using ETicaret.Application.Features.Categories.Specifications;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Categories.Rules;

#region İş Kuralları Sınıfı (Business Rules Class)

/// <summary>
/// Kategori entity'si için iş kurallarını yöneten sınıf.
/// Bu sınıf, kategori işlemleri sırasında uygulanması gereken iş mantığı kontrollerini içerir.
/// Veritabanı bütünlüğünü ve iş gereksinimlerini korumak için kullanılır.
/// </summary>
public class CategoryBusinessRules
{
    #region Alan Tanımlamaları (Field Declarations)

    private readonly IRepository<Category, int> _categoryRepository;

    #endregion

    #region Yapıcı Metot (Constructor)

    /// <summary>
    /// CategoryBusinessRules sınıfının yeni bir örneğini oluşturur.
    /// </summary>
    /// <param name="unitOfWork">Veritabanı işlemleri için Unit of Work deseni implementasyonu.</param>
    public CategoryBusinessRules(IUnitOfWork unitOfWork)
    {
        _categoryRepository = unitOfWork.GetRepository<Category, int>();
    }

    #endregion

    #region Ekleme İşlemi İş Kuralları (Insert Business Rules)

    /// <summary>
    /// Yeni kategori eklenirken kategori adının tekrar etmediğini kontrol eder.
    /// </summary>
    /// <param name="name">Kontrol edilecek kategori adı.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <exception cref="BusinessException">Aynı isimde kategori zaten varsa fırlatılır.</exception>
    public async Task CheckCategoryNameCanNotBeDuplicatedWhenInsertedAsync(string name, CancellationToken cancellationToken)
    {
        var spec = new CategorySpecifications.ByName(name);
        bool isDuplicate = await _categoryRepository.AnyAsync(spec, cancellationToken);

        if (isDuplicate)
        {
            throw new BusinessException(
                message: $"A category with the name '{name}' already exists in the database.",
                userFriendlyMessage: $"'{name}' isminde bir kategori zaten mevcut. Lütfen farklı bir kategori adı seçiniz.",
                errorCode: "DUPLICATE_CATEGORY_NAME"
            );
        }
    }

    /// <summary>
    /// Üst kategori atanırken, belirtilen üst kategorinin var olup olmadığını kontrol eder.
    /// </summary>
    /// <param name="parentId">Kontrol edilecek üst kategori ID'si.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <param name="checkIfActive">Üst kategorinin aktif olup olmadığının da kontrol edilip edilmeyeceği.</param>
    /// <exception cref="BusinessException">Üst kategori bulunamazsa veya aktif değilse fırlatılır.</exception>
    public async Task CheckParentCategoryExistsAsync(int? parentId, CancellationToken cancellationToken, bool checkIfActive = true)
    {
        if (!parentId.HasValue) return;

        var spec = checkIfActive
            ? new CategorySpecifications.ById(parentId.Value) // Sadece ID kontrolü, aktif kontrolü sonra yapılacak
            : new CategorySpecifications.ById(parentId.Value);

        var parentCategory = await _categoryRepository.GetAsync(spec, cancellationToken);

        if (parentCategory == null)
        {
            throw new BusinessException(
                message: $"Parent category with ID {parentId.Value} was not found.",
                userFriendlyMessage: $"Belirtilen üst kategori (ID: {parentId.Value}) bulunamadı.",
                errorCode: "PARENT_CATEGORY_NOT_FOUND"
            );
        }

        if (checkIfActive && !parentCategory.IsActive)
        {
            throw new BusinessException(
                message: $"Parent category with ID {parentId.Value} is not active.",
                userFriendlyMessage: $"Belirtilen üst kategori (ID: {parentId.Value}) aktif değil.",
                errorCode: "PARENT_CATEGORY_NOT_ACTIVE"
            );
        }
    }

    #endregion

    #region Güncelleme İşlemi İş Kuralları (Update Business Rules)

    /// <summary>
    /// Kategori güncellenirken kategori adının (kendisi hariç) tekrar etmediğini kontrol eder.
    /// </summary>
    /// <param name="categoryId">Güncellenen kategorinin ID'si.</param>
    /// <param name="name">Kontrol edilecek kategori adı.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <exception cref="BusinessException">Aynı isimde başka bir kategori varsa fırlatılır.</exception>
    public async Task CheckCategoryNameCanNotBeDuplicatedWhenUpdatedAsync(int categoryId, string name, CancellationToken cancellationToken)
    {
        var spec = new CategorySpecifications.ByNameExcludingId(categoryId, name);
        bool isDuplicate = await _categoryRepository.AnyAsync(spec, cancellationToken);

        if (isDuplicate)
        {
            throw new BusinessException(
                message: $"Another category with the name '{name}' already exists.",
                userFriendlyMessage: $"'{name}' isminde başka bir kategori zaten mevcut. Lütfen farklı bir kategori adı seçiniz.",
                errorCode: "DUPLICATE_CATEGORY_NAME_UPDATE"
            );
        }
    }

    /// <summary>
    /// Kategorinin kendisinin üst kategorisi olmasını engeller.
    /// </summary>
    /// <param name="categoryId">Kontrol edilecek kategori ID'si.</param>
    /// <param name="parentId">Atanmak istenen üst kategori ID'si.</param>
    /// <exception cref="BusinessException">Kategori kendisinin üst kategorisi olarak atanırsa fırlatılır.</exception>
    public void CheckCategoryCanNotBeItsOwnParent(int categoryId, int? parentId)
    {
        if (parentId.HasValue && parentId.Value == categoryId)
        {
            throw new BusinessException(
                message: $"Category with ID {categoryId} cannot be its own parent.",
                userFriendlyMessage: "Bir kategori kendisinin üst kategorisi olamaz.",
                errorCode: "CATEGORY_SELF_PARENT"
            );
        }
    }

    /// <summary>
    /// Kategori güncellenirken döngüsel bağımlılık oluşup oluşmadığını kontrol eder.
    /// Bir kategorinin kendi alt kategorilerinden biri üst kategori olarak atanamaz.
    /// </summary>
    /// <param name="categoryToUpdateId">Güncellenen kategorinin ID'si.</param>
    /// <param name="newParentId">Yeni üst kategori ID'si.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <exception cref="BusinessException">Döngüsel bağımlılık tespit edilirse fırlatılır.</exception>
    public async Task CheckNoCyclicDependencyWhenUpdatingParentAsync(int categoryToUpdateId, int? newParentId, CancellationToken cancellationToken)
    {
        if (!newParentId.HasValue) return; // Ana kategori yapılıyorsa döngüsel kontrol gerekmez.

        // Yeni ebeveynin, güncellenen kategorinin mevcut alt kategorilerinden biri olup olmadığını kontrol et.
        bool isNewParentAChild = await IsNewParentACurrentChildAsync(categoryToUpdateId, newParentId.Value, cancellationToken);

        if (isNewParentAChild)
        {
            throw new BusinessException(
                message: $"Cannot set category {newParentId.Value} as parent of category {categoryToUpdateId} because it would create a cyclic dependency.",
                userFriendlyMessage: "Geçersiz üst kategori ataması. Döngüsel bir bağımlılık oluşturulamaz (bir kategori kendi alt kategorisi olamaz).",
                errorCode: "CATEGORY_CYCLIC_DEPENDENCY"
            );
        }
    }

    #endregion

    #region Silme İşlemi İş Kuralları (Delete Business Rules)

    /// <summary>
    /// Kategorinin aktif alt kategorilere sahip olup olmadığını kontrol eder.
    /// Aktif alt kategorileri olan kategoriler silinemez.
    /// </summary>
    /// <param name="categoryId">Kontrol edilecek kategori ID'si.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <exception cref="BusinessException">Aktif alt kategoriler varsa fırlatılır.</exception>
    public async Task CheckIfCategoryHasActiveChildCategoriesAsync(int categoryId, CancellationToken cancellationToken)
    {
        var spec = new CategorySpecifications.Children(categoryId, onlyActive: true);
        bool hasActiveChildren = await _categoryRepository.AnyAsync(spec, cancellationToken);

        if (hasActiveChildren)
        {
            throw new BusinessException(
                message: $"Category with ID {categoryId} cannot be deleted because it has active child categories.",
                userFriendlyMessage: $"Bu kategori (ID: {categoryId}) silinemez çünkü aktif alt kategorileri bulunmaktadır.",
                errorCode: "CATEGORY_HAS_ACTIVE_CHILDREN"
            );
        }
    }

    #endregion

    #region Yardımcı Metotlar (Helper Methods)

    /// <summary>
    /// Potansiyel yeni ebeveynin, güncellenen kategorinin mevcut alt kategorilerinden
    /// biri olup olmadığını özyineli (recursive) olarak kontrol eder.
    /// </summary>
    /// <param name="categoryToUpdateId">Güncellenen kategorinin ID'si.</param>
    /// <param name="newParentId">Yeni ebeveyn adayının ID'si.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    /// <returns>Yeni ebeveyn mevcut alt kategorilerden biriyse true, değilse false.</returns>
    private async Task<bool> IsNewParentACurrentChildAsync(int categoryToUpdateId, int newParentId, CancellationToken cancellationToken)
    {
        // Önce tüm alt kategorileri (ve onların altlarını) bulalım.
        var childrenOfCategoryToUpdate = new List<Category>();
        await GetAllChildrenRecursiveAsync(categoryToUpdateId, childrenOfCategoryToUpdate, cancellationToken);

        // Eğer bu alt kategori listesi içinde yeni ebeveyn adayı varsa, bu döngüsel bir bağımlılıktır.
        return childrenOfCategoryToUpdate.Any(child => child.Id == newParentId);
    }

    /// <summary>
    /// Belirli bir kategorinin tüm alt kategorilerini (ve onların alt kategorilerini) özyineli olarak getirir.
    /// </summary>
    /// <param name="parentId">Ana kategorinin ID'si.</param>
    /// <param name="allChildren">Tüm alt kategorileri toplamak için kullanılacak liste.</param>
    /// <param name="cancellationToken">İptal token'ı.</param>
    private async Task GetAllChildrenRecursiveAsync(int parentId, List<Category> allChildren, CancellationToken cancellationToken)
    {
        // Mevcut CategorySpecifications.Children kullanarak doğrudan alt kategorileri getir
        var spec = new CategorySpecifications.Children(parentId);
        var directChildren = await _categoryRepository.GetListAsync(spec, cancellationToken);

        if (!directChildren.Any())
        {
            return;
        }

        foreach (var child in directChildren)
        {
            allChildren.Add(child);
            // Her bir çocuk için aynı işlemi tekrarla.
            await GetAllChildrenRecursiveAsync(child.Id, allChildren, cancellationToken);
        }
    }

    #endregion
}

#endregion