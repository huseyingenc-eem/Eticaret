using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ETicaret.Application.Features.Roles.Queries.RolesList;


public class RolesListQuery :IRequest <List<IdentityRole>>
{



    public class RolesListQueryHandler : IRequestHandler<RolesListQuery, List<IdentityRole>>
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesListQueryHandler(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<List<IdentityRole>> Handle(RolesListQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleManager.Roles.ToListAsync();

            return roles;
        }
    }
}
