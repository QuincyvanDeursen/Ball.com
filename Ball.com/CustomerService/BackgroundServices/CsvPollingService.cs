using CustomerService.Controllers;
using CustomerService.Domain;
using CustomerService.Dto;
using CustomerService.Services.Interfaces;

namespace CustomerService.BackgroundServices
{
    public class CsvPollingService : BackgroundService
    {
        private readonly IHttpClientFactory _httpFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly string _csvUrl;
		private readonly ILogger<CsvPollingService> _logger;

		public CsvPollingService(
            IHttpClientFactory httpFactory,
            IConfiguration config,
            IServiceProvider serviceProvider, 
            ILogger<CsvPollingService> logger)
        {
            _httpFactory = httpFactory;
            _serviceProvider = serviceProvider;
            _csvUrl = config["ExternalCsvUrl"];
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var client = _httpFactory.CreateClient();

            // For testing, polls every 10s; switch to 24h in prod
            var delay = TimeSpan.FromSeconds(10);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var resp = await client.GetAsync(_csvUrl, stoppingToken);
                    resp.EnsureSuccessStatusCode();

                    var csvRows = (await resp.Content.ReadAsStringAsync(stoppingToken))
                                  .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                                  .Skip(1);

                    using var scope = _serviceProvider.CreateScope();
                    var service = scope.ServiceProvider.GetRequiredService<ICustomerService>();

                    List<Customer> csvCustomers = new List<Customer>();
                    foreach (var row in csvRows)
                    {
                        var cols = row.Split(',');

                        //Customer from CSV does not contain Email
                        var customer = new Customer
                        {
                            FirstName = cols[1],
                            LastName = cols[2],
                            PhoneNumber = cols[3],
                            Address = cols[4]
                        };
						csvCustomers.Add(customer);
					}

                    List<Customer> currentCustomers = new List<Customer>(await service.GetAll());

                    var count = 0;
                    foreach (var customer in csvCustomers)
                    {
                        //Check if the csv customer exists in our database (using phonenumber as uniqueness)
                        var existingCustomer = currentCustomers.FirstOrDefault(c => c.PhoneNumber == customer.PhoneNumber);
                        //If customer doesnt exist yet, create one
                        if (existingCustomer == null)
                        {
                            CustomerCreateDto createDto = new CustomerCreateDto
                            {
                                FirstName = customer.FirstName,
                                LastName = customer.LastName,
                                PhoneNumber = customer.PhoneNumber,
                                Address = customer.Address,
                            };
                            await service.Create(createDto);
                            count++;
                        }
                        else 
                        {
                            CustomerUpdateDto updateDto = new CustomerUpdateDto
                            {
                                Id = existingCustomer.Id,
                                Address = customer.Address,
                                PhoneNumber = customer.PhoneNumber,
                            };
                            await service.Update(updateDto);
                        }
                    }
					_logger.LogInformation("Added {count} new customers from csv", count);
				}
                catch
                {
                    // TODO: log error
                }

                await Task.Delay(delay, stoppingToken);
            }
        }
    }
}
