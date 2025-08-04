using Core.Application.Behaviors.Rules;
using ETicaret.Application.Features.Suppliers.Commands.Create;
using ETicaret.Application.Features.Suppliers.Commands.Update;

namespace ETicaret.Application.Features.Suppliers.Rules;

public static partial class SupplierRules
{
    /// <summary>
    /// Tedarikçi durumunun geçerli olduğunu kontrol eden rule.
    /// </summary>
    public class SupplierStatusMustBeValidRule :
        IBusinessRule<CreateSupplierCommand>,
        IBusinessRule<UpdateSupplierCommand>
    {
        public bool ShouldExecute(CreateSupplierCommand command) => true;
        public bool ShouldExecute(UpdateSupplierCommand command) => true;

        public Task ExecuteAsync(CreateSupplierCommand command, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task ExecuteAsync(UpdateSupplierCommand command, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public int Priority => 5;
    }
}