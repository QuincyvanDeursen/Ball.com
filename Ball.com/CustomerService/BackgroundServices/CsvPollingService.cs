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

        public CsvPollingService(
            IHttpClientFactory httpFactory,
            IConfiguration config,
            IServiceProvider serviceProvider)
        {
            _httpFactory = httpFactory;
            _serviceProvider = serviceProvider;
            _csvUrl = config["ExternalCsvUrl"];
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

                    foreach (var row in csvRows)
                    {
                        var cols = row.Split(',');

                        //Customer from CSV does not contain Email
                        var customer = new Customer
                        {
                            Id = Guid.NewGuid(),
                            FirstName = cols[1],
                            LastName = cols[2],
                            PhoneNumber = cols[3],
                            Address = cols[4]
                        };

                        var existing = await service.Get(customer.Id);
                        if (existing == null)
                        {
                            CustomerCreateDto createDto = new CustomerCreateDto
                            {
                                FirstName = customer.FirstName,
                                LastName = customer.LastName,
                                PhoneNumber = customer.PhoneNumber,
                                Address = customer.Address,
                                Email = customer.Email,
                            };
                            await service.Create(createDto);
                        }
                        else
                        {
                            CustomerUpdateDto updateDto = new CustomerUpdateDto
                            {
                                Id = customer.Id,
                                Address = customer.Address,
                                PhoneNumber = customer.PhoneNumber,
                                Email = customer.Email,
                            };
                            await service.Update(updateDto);
                        }
                    }
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
