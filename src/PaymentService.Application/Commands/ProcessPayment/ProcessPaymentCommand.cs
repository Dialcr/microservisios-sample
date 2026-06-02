using MediatR;

namespace PaymentService.Application.Commands.ProcessPayment;

public record ProcessPaymentCommand(Guid OrderId, decimal Amount) : IRequest<Guid>;
