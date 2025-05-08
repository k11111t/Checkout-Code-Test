using Microsoft.Extensions.Options;

using PaymentGateway.Api.Interfaces;
using PaymentGateway.Api.Models.Data;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Services.Bank;
using PaymentGateway.Api.Services.PaymentGateway;
using PaymentGateway.Api.Services.Validation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug(); 

builder.Services.Configure<PaymentGatewayConfiguration>(
    builder.Configuration.GetSection("PaymentGatewayConfig"));

builder.Services.AddSingleton<IPaymentGatewayConfiguration>(sp =>
    sp.GetRequiredService<IOptions<PaymentGatewayConfiguration>>().Value);

builder.Services.AddScoped<IValidator<PostPaymentRequest>, AmountValidator>();
builder.Services.AddScoped<IValidator<PostPaymentRequest>, CardNumberValidator>();
builder.Services.AddScoped<IValidator<PostPaymentRequest>, CurrencyValidator>();
builder.Services.AddScoped<IValidator<PostPaymentRequest>, CvvValidator>();
builder.Services.AddScoped<IValidator<PostPaymentRequest>, ExpiryDateValidator>();
builder.Services.AddScoped<IValidator<PostPaymentRequest>, ExpiryMonthValidator>();
builder.Services.AddScoped<IValidator<PostPaymentRequest>, ExpiryYearValidator>();

builder.Services.AddScoped<IValidatorManager<PostPaymentRequest>, PaymentValidationManager>();

builder.Services.AddSingleton<IRepository<PaymentRecord>, PaymentsRepository>();

builder.Services.AddScoped<IBankRequestBuilder, BankRequestBuilder>();
builder.Services.AddScoped<IBankResponseParser, BankResponseParser>();
builder.Services.AddHttpClient<IBankPaymentManager, BankPaymentManager>(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(10);
    });
builder.Services.AddScoped<IPaymentManager, PaymentManager>();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
