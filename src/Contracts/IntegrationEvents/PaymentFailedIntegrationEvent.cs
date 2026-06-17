namespace Contracts.IntegrationEvents;

public record PaymentFailedIntegrationEvent(string OrderId, string Reason, DateTime FailedAtUtc);
