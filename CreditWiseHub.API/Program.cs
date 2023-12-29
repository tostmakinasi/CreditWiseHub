using CreditWiseHub.API.Extensions;
using CreditWiseHub.API.Middlewares;
using CreditWiseHub.Core.Configurations;
using CreditWiseHub.Repository.Contexts;
using CreditWiseHub.Service.Mapping;
using CreditWiseHub.Service.Services;
using CreditWiseHub.Service.Validations.User;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql"));

});

builder.Services.Configure<CustomTokenOption>(builder.Configuration.GetSection("TokenOptions"));
var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<CustomTokenOption>();
builder.Services.AddAuthenticationWithJwtOptionsExtension(tokenOptions);


builder.Services.AddAutoMapper(typeof(UserAppMap));

builder.Services.AddControllers().AddFluentValidation(opt =>
{
    opt.RegisterValidatorsFromAssemblyContaining<CreateUserDtoValidation>();
});
builder.Services.AddCustomValidationResponseExtension();
builder.Services.AddScopedWithExtension();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomException();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
