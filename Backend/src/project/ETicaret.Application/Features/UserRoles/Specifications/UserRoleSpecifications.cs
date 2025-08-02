using Core.Application.Abstractions.Specifications;
using ETicaret.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ETicaret.Application.Features.UserRoles.Specifications;

/// <summary>
/// UserRoles için specification'lar
/// Mevcut specification pattern'inizi kullanarak clean queries
/// </summary>
public static class UserRoleSpecifications
{
    /// <summary>
    /// ✅ User ID'sine göre kullanıcı getiren specification
    /// </summary>
    public class UserByIdSpecification : Specification<User>
    {
        public UserByIdSpecification(string userId)
            : base(user => user.Id == userId)
        {
        }
    }

    /// <summary>
    /// ✅ Birden fazla rol ID'sine göre rolleri getiren specification
    /// </summary>
    public class RolesByIdsSpecification : Specification<IdentityRole>
    {
        public RolesByIdsSpecification(List<string> roleIds)
            : base(role => roleIds.Contains(role.Id))
        {
            AddOrderBy(role => role.Name);
        }
    }

    /// <summary>
    /// ✅ Rol adına göre rol getiren specification
    /// </summary>
    public class RoleByNameSpecification : Specification<IdentityRole>
    {
        public RoleByNameSpecification(string roleName)
            : base(role => role.Name == roleName)
        {
        }
    }

    /// <summary>
    /// ✅ Aktif rolleri getiren specification
    /// </summary>
    public class ActiveRolesSpecification : Specification<IdentityRole>
    {
        public ActiveRolesSpecification()
            : base(role => role.Name != null && role.Name != "")
        {
            AddOrderBy(role => role.Name);
        }
    }
}