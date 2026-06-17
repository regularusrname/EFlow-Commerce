namespace Contracts.IntegrationEvents;

public record PaymentSucceededIntegrationEvent(string OrderId, string PaymentId, DateTime PaidAtUtc);
