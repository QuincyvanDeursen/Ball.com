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

app.MapControllers();

app.Run();
