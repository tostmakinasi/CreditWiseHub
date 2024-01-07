using CreditWiseHub.API.Extensions;
using CreditWiseHub.API.Middlewares;
using CreditWiseHub.BackgroundJob.Schedules;
using CreditWiseHub.Core.Configurations;
using CreditWiseHub.Repository.Contexts;
using CreditWiseHub.Service.Mapping;
using CreditWiseHub.Service.Seeds;
using CreditWiseHub.Service.Validations.User;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql"));

});

builder.Services.Configure<CustomTokenOption>(builder.Configuration.GetSection("TokenOptions"));
var tokenOptions = builder.Configuration.GetSection("TokenOptions").Get<CustomTokenOption>();
builder.Services.AddAuthenticationWithJwtOptionsExtension(tokenOptions);

builder.Services.AddHangfire(configuration =>
            configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(builder.Configuration.GetConnectionString("HangfireConnection")));


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

//Seed Db
//using (var scope = app.Services.CreateScope())
//{
//    var seedService = scope.ServiceProvider.GetRequiredService<SeedService>();
//    await seedService.SeedAsync();
//}


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

app.UseHangfireDashboard("/hangfire", new DashboardOptions()
{
    DashboardTitle = "Hangfire Dashboard",
    Authorization = new[]{
    new HangfireBasicAuthenticationFilter.HangfireCustomBasicAuthenticationFilter{
        User = builder.Configuration.GetSection("HangfireCredentials:UserName").Value,
        Pass = builder.Configuration.GetSection("HangfireCredentials:Password").Value
    }}
});

RecurringJobs.AutomaticPatmentOperation();

app.MapControllers();

app.Run();

public partial class Program { }