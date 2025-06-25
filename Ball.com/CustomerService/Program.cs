using CustomerService.Configuration;
using CustomerService.Database;
using CustomerService.Domain;
using CustomerService.Repository;
using CustomerService.Repository.Interfaces;
using CustomerService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// db connection
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<CustomerDbContext>(
    options => options.UseSqlServer(connectionString));

// Add services to the container.
builder.Services.AddScoped<ICustomerRepo, CustomerRepo>();
builder.Services.AddScoped<ICustomerService, CustomerService.Services.CustomerService>();

//RabbitMq setup, zie configuration folder.
builder.Services.AddRabbitMqMessaging(builder.Configuration);


builder.Services.AddControllers();

// Swagger / OpenAPI setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Customer API microservice Ball.com",
        Description = "An ASP.NET Core Web API for managing customers for Ball.com",
    });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();
app.UseCors();
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
    var dbContext = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
    dbContext.Database.Migrate();

    if (!dbContext.Customers.Any())
    {
        dbContext.AddRange(
                            new Customer
                            {
                                Id = Guid.Parse("a1a1a1a1-a1a1-1111-aaaa-111111111111"),
                                FirstName = "Quincy",
                                LastName = "van Deursen",
                                PhoneNumber = "0612345678",
                                Email = "jvd@student.avans.nl",
                                Address = "Avansstraat 123, 1234AB Breda"
                            },
                new Customer
                {
                    Id = Guid.Parse("b2b2b2b2-b2b2-2222-bbbb-222222222222"),
                    FirstName = "Bart",
                    LastName = "Kroeten",
                    PhoneNumber = "0612345678",
                    Email = "qvd@student.avans.nl",
                    Address = "Avansstraat 456, 1234AB Breda"
                },
                new Customer
                {
                    Id = Guid.Parse("c3c3c3c3-c3c3-3333-cccc-333333333333"),
                    FirstName = "Ruben",
                    LastName = "van Tilburg",
                    PhoneNumber = "0612345678",
                    Email = "svd@student.avans.nl",
                    Address = "Avansstraat 789, 1234AB Breda"
                },
                new Customer
                {
                    Id = Guid.Parse("d4d4d4d4-d4d4-4444-dddd-444444444444"),
                    FirstName = "Marco",
                    LastName = "Rietveld",
                    PhoneNumber = "0612345678",
                    Email = "bvd@student.avans.nl",
                    Address = "Avansstraat 159, 1234AB Breda"
                });
        dbContext.SaveChanges();
    }

}

app.MapControllers();

app.Run();
