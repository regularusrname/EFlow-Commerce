namespace Contracts.IntegrationEvents.Payments;

public record PaymentFailedIntegrationEvent(string Id, string OrderId, string Reason, DateTime FailedAtUtc);
