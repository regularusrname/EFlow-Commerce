namespace Orders.API.Domain.Orders;

public enum OrderStatus
{
    Pending           = 1, // order was created but payment has not started yet
    PaymentProcessing = 2, // payment event was published (payment is in progress)
    Paid              = 3, // payment succeeded
    PaymentFailed     = 4, // payment failed
    Cancelled         = 5, // order was cancelled
}
