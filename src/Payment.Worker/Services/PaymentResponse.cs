namespace Payment.Worker.Services;

public record PaymentResponse(Guid PaymentId, bool IsSuccess, string? FailureReason);
