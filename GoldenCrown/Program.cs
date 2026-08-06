using FluentValidation;
using GoldenCrown.Database;
using GoldenCrown.Middlewares;
using GoldenCrown.Services.BackgroundServices;
using GoldenCrown.Services.FinanceServices;
using GoldenCrown.Services.UserServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<GoldenCrownDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("GoldenCrownDbConnection"));
});

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

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IFinanceService, FinanceService>();

builder.Services.AddHostedService<SessionCleanupService>();

builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly, includeInternalTypes: true);

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
