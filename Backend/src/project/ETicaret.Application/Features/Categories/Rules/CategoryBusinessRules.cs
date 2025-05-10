using Core.CrossCuttingConcerns.Exceptions;
using ETicaret.Application.Services.Repositories;
using ETicaret.Domain.Entities;

namespace ETicaret.Application.Features.Categories.Rules;

public class CategoryBusinessRules
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryBusinessRules(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    /// <summary>
    /// Yeni kategori oluşturulurken, kategori adının başka bir kategori tarafından kullanılıp kullanılmadığını kontrol eder.
    /// </summary>
    public async Task CheckCategoryNameCanNotBeDuplicatedWhenInsertedAsync(string name, CancellationToken cancellationToken)
    {
        var existingCategoryWithName = await _categoryRepository.GetAsync(
            filter: c => c.Name.Trim().ToLower() == name.Trim().ToLower(),
            enableTracking: false,
            cancellationToken: cancellationToken);

        if (existingCategoryWithName != null)
        {
            throw new BusinessException($"'{name}' isminde bir kategori zaten mevcut.");
        }
    }

    /// <summary>
    /// Silinmek istenen kategorinin alt kategorileri olup olmadığını kontrol eder.
    /// </summary>
    public async Task CheckIfCategoryHasActiveChildCategoriesAsync(int categoryId, CancellationToken cancellationToken)
    {
        // Sadece aktif alt kategorileri kontrol ediyoruz. Pasif olanlar silinmeye engel olmamalı.
        var directChildrenPaginate = await _categoryRepository.GetListAsync(
            filter: c => c.ParentId == categoryId && c.IsActive,
            enableTracking: false,
            cancellationToken: cancellationToken);

        if (directChildrenPaginate != null && directChildrenPaginate.Items.Any())
        {
            throw new BusinessException($"Bu kategori (ID: {categoryId}) silinemez çünkü aktif alt kategorileri bulunmaktadır. Önce alt kategorilerini silin veya pasif hale getirin.");
        }
    }
    /// <summary>
    /// Kategori güncellenirken, yeni kategori adının başka bir kategori tarafından (güncellenen hariç) kullanılıp kullanılmadığını kontrol eder.
    /// </summary>
    public async Task CheckCategoryNameCanNotBeDuplicatedWhenUpdatedAsync(int categoryId, string name, CancellationToken cancellationToken)
    {
        var existingCategoryWithName = await _categoryRepository.GetAsync(
            filter: c => c.Name.Trim().ToLower() == name.Trim().ToLower() && c.Id != categoryId,
            enableTracking: false,
            cancellationToken: cancellationToken);

        if (existingCategoryWithName != null)
        {
            throw new BusinessException($"'{name}' isminde başka bir kategori zaten mevcut.");
        }
    }

    /// <summary>
    /// Bir kategorinin kendisinin üst kategorisi olup olmadığını kontrol eder.
    /// </summary>
    public void CheckCategoryCanNotBeItsOwnParent(int categoryId, int? parentId)
    {
        if (parentId.HasValue && parentId.Value == categoryId)
        {
            throw new BusinessException("Bir kategori kendisinin üst kategorisi olamaz.");
        }
    }

    /// <summary>
    /// Belirtilen üst kategorinin var olup olmadığını (ve isteğe bağlı olarak aktif olup olmadığını) kontrol eder.
    /// </summary>
    public async Task CheckParentCategoryExistsAsync(int? parentId, CancellationToken cancellationToken, bool checkIfActive = true)
    {
        if (!parentId.HasValue) return;

        var parentCategory = await _categoryRepository.GetAsync(
            filter: c => c.Id == parentId.Value && (!checkIfActive || c.IsActive), // Eğer checkIfActive true ise aktiflik de kontrol edilir.
            enableTracking: false,
            cancellationToken: cancellationToken);

        if (parentCategory == null)
        {
            throw new BusinessException($"Belirtilen üst kategori (Id: {parentId.Value}) bulunamadı{(checkIfActive ? " veya aktif değil" : "")}.");
        }
    }

    /// <summary>
    /// Kategori güncellenirken, yeni üst kategori atamasının döngüsel bir bağımlılık oluşturup oluşturmadığını kontrol eder.
    /// </summary>
    public async Task CheckNoCyclicDependencyWhenUpdatingParentAsync(int categoryToUpdateId, int? newParentId, CancellationToken cancellationToken)
    {
        if (!newParentId.HasValue) return; // Ana kategori yapılıyorsa döngüsel kontrol gerekmez.

        // CheckCategoryCanNotBeItsOwnParent zaten bu durumu yakalar ama burada da teyit edilebilir.
        if (newParentId.Value == categoryToUpdateId)
        {
            throw new BusinessException("Bir kategori kendisinin üst kategorisi olamaz (döngüsel kontrol).");
        }

        if (await IsNewParentACurrentChildAsync(categoryToUpdateId, newParentId.Value, cancellationToken))
        {
            throw new BusinessException("Geçersiz üst kategori ataması. Döngüsel bir bağımlılık oluşturulamaz.");
        }
    }

    /// <summary>
    /// Belirtilen bir kategorinin, potansiyel yeni üst kategorisinin mevcut alt kategorilerinden biri olup olmadığını kontrol eder.
    /// </summary>
    private async Task<bool> IsNewParentACurrentChildAsync(int categoryToUpdateId, int newParentId, CancellationToken cancellationToken)
    {
        var childrenOfCategoryToUpdate = new List<Category>();
        await GetAllChildrenRecursiveAsync(categoryToUpdateId, childrenOfCategoryToUpdate, cancellationToken);

        return childrenOfCategoryToUpdate.Any(child => child.Id == newParentId);
    }

    /// <summary>
    /// Belirli bir kategorinin tüm alt kategorilerini (ve onların alt kategorilerini) özyineli olarak getirir.
    /// </summary>
    private async Task GetAllChildrenRecursiveAsync(int parentId, List<Category> allChildren, CancellationToken cancellationToken)
    {
        var directChildrenPaginate = await _categoryRepository.GetListAsync(
            filter: c => c.ParentId == parentId,
            enableTracking: false,
            cancellationToken: cancellationToken);

        if (directChildrenPaginate == null || !directChildrenPaginate.Items.Any())
        {
            return;
        }

        foreach (var child in directChildrenPaginate.Items)
        {
            allChildren.Add(child);
            await GetAllChildrenRecursiveAsync(child.Id, allChildren, cancellationToken);
        }
    }
}
