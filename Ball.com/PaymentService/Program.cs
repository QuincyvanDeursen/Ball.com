using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using PaymentService.Configuration;
using PaymentService.Database;
using PaymentService.Domain;
using PaymentService.Handlers;
using PaymentService.Repository;
using PaymentService.Repository.Interfaces;
using PaymentService.Services;
using PaymentService.Services.Interfaces;
using Shared.Infrastructure.Messaging;
using Shared.Infrastructure.Messaging.Configuration;
using Shared.Infrastructure.Messaging.Events;
using Shared.Infrastructure.Messaging.Events.Interfaces;
using Shared.Infrastructure.Messaging.Interfaces;



var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<PaymentDbContext>(
    options => options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddScoped<IPaymentRepo, PaymentRepo>();
builder.Services.AddScoped<IPaymentService, PaymentService.Services.PaymentService>();

//RabbitMq setup, zie configuration folder.
builder.Services.AddRabbitMqMessaging(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Swagger / OpenAPI setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Payment API microservice Ball.com",
        Description = "An ASP.NET Core Web API for managing payments for Ball.com",
    });

    // Optioneel: XML comments toevoegen als je die hebt
    /*
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
    */
});

var app = builder.Build();

// Swagger UI inschakelen
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
    dbContext.Database.Migrate();

    if (!dbContext.Payments.Any())
    {
        dbContext.AddRange(
            new Payment
            {
                Id = Guid.Parse("e5e5e5e5-e5e5-5555-eeee-555555555555"),
                TotalPrice = 99.99m,
                Status = PaymentStatus.Paid,

                Customer = new Customer
                {
                    Id = Guid.Parse("a1a1a1a1-a1a1-1111-aaaa-111111111111"),
                    FirstName = "Quincy",
                    LastName = "van Deursen",
                    PhoneNumber = "0612345678",
                    Email = "jvd@student.avans.nl",
                    Address = "Avansstraat 123, 1234AB Breda"
                },
            },
new Payment
{
    Id = Guid.Parse("b7b7b7b7-b7b7-7777-bbbb-777777777777"),
    TotalPrice = 49.50m,
    Status = PaymentStatus.Pending,

    Customer = new Customer
    {
        Id = Guid.Parse("b2b2b2b2-b2b2-2222-bbbb-222222222222"),
        FirstName = "Bart",
        LastName = "Kroeten",
        PhoneNumber = "0612345678",
        Email = "qvd@student.avans.nl",
        Address = "Avansstraat 456, 1234AB Breda"
    },
}
        );

        dbContext.SaveChanges();
    }
}




app.MapControllers();

app.Run();
