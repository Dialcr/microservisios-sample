using MediatR;
using Microsoft.Extensions.Logging;
using PaymentService.Domain.Entities;
using PaymentService.Application.Interfaces;
using Shared.EventBus.Abstractions;
using Shared.EventBus.Events;

namespace PaymentService.Application.Commands.ProcessPayment;

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, Guid>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<ProcessPaymentCommandHandler> _logger;

    public ProcessPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IEventBus eventBus,
        ILogger<ProcessPaymentCommandHandler> logger)
    {
        _paymentRepository = paymentRepository;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<Guid> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = new Payment(request.OrderId, request.Amount);

        try
        {
            payment.Process();
            _logger.LogInformation("Payment processed for order {OrderId}", request.OrderId);
        }
        catch
        {
            payment.Fail();
            throw;
        }

        await _paymentRepository.AddAsync(payment, cancellationToken);

        await _eventBus.PublishAsync(new PaymentProcessedEvent(
            payment.OrderId, payment.Id, payment.Status, payment.ProcessedAt!.Value), cancellationToken);

        return payment.Id;
    }
}
