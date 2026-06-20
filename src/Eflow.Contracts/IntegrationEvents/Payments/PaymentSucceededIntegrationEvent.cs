namespace Eflow.Contracts.IntegrationEvents.Payments;

public record PaymentSucceededIntegrationEvent(Guid Id, Guid OrderId, Guid PaymentId, DateTime PaidAtUtc);
