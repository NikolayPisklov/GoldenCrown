using FluentValidation;
using GoldenCrown.Api.Database;
using GoldenCrown.Api.Middlewares;
using GoldenCrown.Api.RabbitMQ;
using GoldenCrown.Api.Services.BackgroundServices;
using GoldenCrown.Application;
using GoldenCrown.Application.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<GoldenCrownDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("GoldenCrownDbConnection"));
});

builder.Services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<GoldenCrownDbContext>());

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Authorization", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Description = "Введите значение заголовка Authorization"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Authorization", document)] = []
    });
});

builder.Services.AddHostedService<SessionCleanupService>();

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);
builder.Services.AddApplication();
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));
builder.Services.AddSingleton<IRabbitMqConnectionManager, RabbitMqConnectionManager>();
builder.Services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetService<GoldenCrownDbContext>();
db.Database.Migrate();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<GoldenCrownAuthMiddleware>();

app.MapControllers();

app.Run();
