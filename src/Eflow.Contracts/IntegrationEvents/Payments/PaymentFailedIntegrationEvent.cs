namespace Eflow.Contracts.IntegrationEvents.Payments;

public record PaymentFailedIntegrationEvent(Guid Id, Guid OrderId, string Reason, DateTime FailedAtUtc);
