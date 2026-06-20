namespace Contracts.IntegrationEvents.Payments;

public record PaymentSucceededIntegrationEvent(string Id, string OrderId, string PaymentId, DateTime PaidAtUtc);
