using InventoryManagementService.Commands;
using InventoryManagementService.Commands.Handlers;
using InventoryManagementService.Configuration;
using InventoryManagementService.Data;
using InventoryManagementService.Models;
using InventoryManagementService.Queries;
using InventoryManagementService.Queries.Handlers;
using InventoryManagementService.Repositories;
using InventoryManagementService.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
// db connection
var connectionString = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(connectionString));
// Add services to the container.
builder.Services.AddRabbitMqMessaging(builder.Configuration);

builder.Services.AddScoped<IEventStore, EventStore>();
builder.Services.AddScoped<IReadModelUpdater, ReadModelUpdater>();
builder.Services.AddScoped<IEventReplayer, EventReplayer>();



//Commands
builder.Services.AddScoped<ICommandHandler<CreateItemCommand>, CreateItemCommandHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateItemCommand>, UpdateItemCommandHandler>();
builder.Services.AddScoped<ICommandHandler<UpdateStockCommand>, UpdateStockCommandHandler>();

//Queries
builder.Services.AddScoped<IQueryHandler<GetAllItemsQuery, IEnumerable<ItemReadModel>>, GetAllItemsQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetItemsByIdQuery, ItemReadModel?>, GetItemsByIdQueryHandler>();



builder.Services.AddControllers();


// Swagger / OpenAPI setup
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Invetory (Products) API microservice Ball.com",
        Description = "An ASP.NET Core Web API for managing inventory for Ball.com",
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

app.UseAuthorization();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();

    if (!dbContext.ItemReadModels.Any())
    {
        dbContext.AddRange(
                            new ItemReadModel
                            {
                                Id = Guid.Parse("1a1a1a1a-1a1a-1111-aaaa-111111111111"),
                                Name = "Laptop",
                                Description = "High performance laptop",
                                Price = 1200.00m,
                                Stock = 50
                            },
                            new ItemReadModel
                            {
                                Id = Guid.Parse("2b2b2b2b-2b2b-2222-bbbb-222222222222"),
                                Name = "Smartphone",
                                Description = "Latest model smartphone",
                                Price = 800.00m,
                                Stock = 100
                            },
                            new ItemReadModel
                            {
                                Id = Guid.Parse("3c3c3c3c-3c3c-3333-cccc-333333333333"),
                                Name = "Headphones",
                                Description = "Noise-cancelling headphones",
                                Price = 200.00m,
                                Stock = 75
                            });

        dbContext.SaveChanges();
    }

    if (!dbContext.Events.Any())
    {
        // Replay events to populate the read model
        dbContext.AddRange(
            new EventEntity
            {
                Id = Guid.NewGuid(),
                AggregateId = Guid.Parse("1a1a1a1a-1a1a-1111-aaaa-111111111111"),
                EventType = "ItemCreatedDomainEvent",
                Data = "{\"ItemId\":\"1a1a1a1a-1a1a-1111-aaaa-111111111111\",\"Name\":\"Laptop\",\"Description\":\"High performance laptop\",\"Price\":1200.00,\"Stock\":50}",
                Timestamp = DateTime.UtcNow
            },
            new EventEntity
            {
                Id = Guid.NewGuid(),
                AggregateId = Guid.Parse("2b2b2b2b-2b2b-2222-bbbb-222222222222"),
                EventType = "ItemCreatedDomainEvent",
                Data = "{\"ItemId\":\"2b2b2b2b-2b2b-2222-bbbb-222222222222\",\"Name\":\"Smartphone\",\"Description\":\"Latest model smartphone\",\"Price\":800.00,\"Stock\":100}",
                Timestamp = DateTime.UtcNow
            },
            new EventEntity
            {
                Id = Guid.NewGuid(),
                AggregateId = Guid.Parse("3c3c3c3c-3c3c-3333-cccc-333333333333"),
                EventType = "ItemCreatedDomainEvent",
                Data = "{\"ItemId\":\"3c3c3c3c-3c3c-3333-cccc-333333333333\",\"Name\":\"Headphones\",\"Description\":\"Noise-cancelling headphones\",\"Price\":200.00,\"Stock\":75}",
                Timestamp = DateTime.UtcNow
            });
        dbContext.SaveChanges();
    }
}

app.MapControllers();

app.Run();
