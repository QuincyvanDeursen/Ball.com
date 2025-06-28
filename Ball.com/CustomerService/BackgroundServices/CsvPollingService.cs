using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CustomerService.Domain;
using CustomerService.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
                    var repo = scope.ServiceProvider.GetRequiredService<ICustomerRepo>();

                    foreach (var row in csvRows)
                    {
                        var cols = row.Split(',');

                        var customer = new Customer
                        {
                            Id = Guid.NewGuid(),
                            CompanyName = cols[0],
                            FirstName = cols[1],
                            LastName = cols[2],
                            PhoneNumber = cols[3],
                            Address = cols[4]
                        };

                        var existing = await repo.GetCustomer(customer.Id);
                        if (existing == null)
                            await repo.AddCustomer(customer);
                        else
                        {
                            existing.CompanyName = customer.CompanyName;
                            existing.FirstName = customer.FirstName;
                            existing.LastName = customer.LastName;
                            existing.PhoneNumber = customer.PhoneNumber;
                            existing.Address = customer.Address;
                            await repo.UpdateCustomer(existing);
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
