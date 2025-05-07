using System.Threading.Tasks;

namespace ETicaret.Application.Services.Authorization;

public interface IAuthorizationRuleService
{
    Task<string[]> GetRequiredRolesAsync(string operationName);
}
