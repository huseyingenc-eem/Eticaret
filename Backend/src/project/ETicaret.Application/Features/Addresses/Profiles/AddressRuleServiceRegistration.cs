using Core.Application.Behaviors.Rules;
using ETicaret.Application.Features.Addresses.Commands.Create;
using ETicaret.Application.Features.Addresses.Commands.Delete;
using ETicaret.Application.Features.Addresses.Commands.Update;
using ETicaret.Application.Features.Addresses.Queries.GetById;
using ETicaret.Application.Features.Addresses.Queries.GetListByUserId;
using ETicaret.Application.Features.Addresses.Rules;
using Microsoft.Extensions.DependencyInjection;

namespace ETicaret.Application.Features.Addresses.Profiles;

/// <summary>
/// Address feature'ı için rule registration'larını yöneten sınıf.
/// Clean Architecture prensiplerine uygun feature-isolated approach.
/// </summary>
public class AddressRuleServiceRegistration : IRuleServiceRegistration
{
    public string FeatureName => "Addresses";

    public void RegisterRules(IServiceCollection services)
    {
        // Business Rules sınıfını kaydet
        services.AddScoped<AddressBusinessRules>();

        // Individual Rules'ları kaydet
        RegisterIndividualRules(services);

        // Rule Interface Mapping'lerini kaydet
        RegisterRuleInterfaceMappings(services);
    }

    #region Helper Methods

    private static void RegisterIndividualRules(IServiceCollection services)
    {
        services.AddScoped<UserExistsRule>();
        services.AddScoped<AddressLimitRule>();
        services.AddScoped<DuplicateAddressTitleRule>();
        services.AddScoped<DefaultAddressTypeRule>();
        services.AddScoped<MinimumAddressRule>();
    }

    private static void RegisterRuleInterfaceMappings(IServiceCollection services)
    {
        // Create Address Command Rules
        services.AddScoped<IRule<CreateAddressCommand>, UserExistsRule>();
        services.AddScoped<IRule<CreateAddressCommand>, AddressLimitRule>();
        services.AddScoped<IRule<CreateAddressCommand>, DuplicateAddressTitleRule>();
        services.AddScoped<IRule<CreateAddressCommand>, DefaultAddressTypeRule>();

        // Update Address Command Rules  
        services.AddScoped<IRule<UpdateAddressCommand>, UserExistsRule>();
        services.AddScoped<IRule<UpdateAddressCommand>, DuplicateAddressTitleRule>();
        services.AddScoped<IRule<UpdateAddressCommand>, DefaultAddressTypeRule>();

        // Delete Address Command Rules
        services.AddScoped<IRule<DeleteAddressCommand>, UserExistsRule>();
        services.AddScoped<IRule<DeleteAddressCommand>, MinimumAddressRule>();

        // Query Rules
        services.AddScoped<IRule<GetByIdAddressQuery>, UserExistsRule>();
        services.AddScoped<IRule<GetListByUserIdAddressQuery>, UserExistsRule>();
    }

    #endregion
}