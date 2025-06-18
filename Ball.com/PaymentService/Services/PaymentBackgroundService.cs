//using PaymentService.Repository.Interfaces;
//using Shared.Events;
//using Shared.Infrastructure.Messaging.Interfaces;

//namespace PaymentService.Services
//{
//    public class PaymentBackgroundService : BackgroundService
//    {
//        private readonly IServiceProvider _services;
//        private readonly ILogger<PaymentBackgroundService> _logger;

//        public PaymentBackgroundService(
//            IServiceProvider services,
//            ILogger<PaymentBackgroundService> logger)
//        {
//            _services = services;
//            _logger = logger;
//        }

//        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//        {
//            _logger.LogInformation("💰 Payment service background worker gestart");

//            using var scope = _services.CreateScope();
//            var consumer = scope.ServiceProvider.GetRequiredService<IMessageConsumer>();
//            var paymentRepo = scope.ServiceProvider.GetRequiredService<IPaymentRepo>();

//            // ENKEL luisteren naar customer updates
//            await consumer.StartConsumingAsync<CustomerUpdatedEvent>(
//                "customer_updates_queue",
//                async updateEvent =>
//                {
//                    try
//                    {
//                        _logger.LogInformation("🔄 Customer update ontvangen voor: {CustomerId}", updateEvent.CustomerId);

//                        // Alleen de naam bijwerken in bestaande payments
//                        var payments = await paymentRepo.GetByCustomerId(updateEvent.CustomerId);

//                        foreach (var payment in payments)
//                        {
//                            payment.Customer.Email = updateEvent.Email ?? payment.Customer.Email; 
//                            payment.Customer.FirstName = updateEvent.FirstName ?? payment.Customer.FirstName;
//                            payment.Customer.LastName = updateEvent.LastName ?? payment.Customer.LastName;
//                            payment.Customer.PhoneNumber = updateEvent.PhoneNumber ?? payment.Customer.PhoneNumber;
//                            payment.Customer.Address = updateEvent.Address ?? payment.Customer.Address;

//                            await paymentRepo.UpdateAsync(payment);
//                            _logger.LogInformation("Payments bijgewerkt voor customer: {PaymentId}", payment.Id);
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        _logger.LogError(ex, "Fout bij verwerken customer update");
//                    }
//                });

//            await Task.Delay(Timeout.Infinite, stoppingToken);
//        }
//    }
//}