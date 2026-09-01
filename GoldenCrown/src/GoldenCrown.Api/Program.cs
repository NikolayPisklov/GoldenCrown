using FluentValidation;
using GoldenCrown.Api.Middlewares;
using GoldenCrown.Application;
using GoldenCrown.Application.Abstractions;
using GoldenCrown.Infrastructure;
using GoldenCrown.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddOpenApi();
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
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();
await DatabaseInitializer.MigrateDatabaseAsync(app.Services);
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.UseMiddleware<GoldenCrownAuthMiddleware>();
app.MapControllers();
app.MapGet("/debug/rate", async (IExchangeRateProvider provider, CancellationToken ct) => 
{
    var rate = await provider.GetRateAsync("USD", "EUR", ct);
    if (rate)
    {
        Console.WriteLine("Successfull lock or rate is cached");
    }
    else
    {
        Console.WriteLine("Cant get lock");
    }
});
app.Run();
