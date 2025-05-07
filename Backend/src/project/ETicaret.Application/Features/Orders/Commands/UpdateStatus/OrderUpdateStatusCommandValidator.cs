using ETicaret.Domain.Enums;
using FluentValidation;

namespace ETicaret.Application.Features.Orders.Commands.UpdateStatus;

public class OrderUpdateStatusCommandValidator : AbstractValidator<OrderUpdateStatusCommand>
{
    public OrderUpdateStatusCommandValidator()
    {
        RuleFor(c => c.OrderId)
            .GreaterThan(0).WithMessage("Sipariş ID'si geçerli olmalıdır.");

        RuleFor(c => c.NewStatus)
            .IsInEnum().WithMessage("Geçerli bir sipariş durumu belirtilmelidir.")
            .NotEqual(default(OrderStatus)).WithMessage("Geçerli bir sipariş durumu belirtilmelidir.");
    }
}
