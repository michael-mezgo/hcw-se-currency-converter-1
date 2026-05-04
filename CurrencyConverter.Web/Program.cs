using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using CurrencyConverter.Web;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();

builder.Services.AddSingleton<ServiceDebugBehavior>(new ServiceDebugBehavior
{
    IncludeExceptionDetailInFaults = true
});

var app = builder.Build();

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<CurrencyService>();

    serviceBuilder.AddServiceEndpoint<CurrencyService, ICurrencyService>(
        new BasicHttpBinding(),
        "/CurrencyService.svc");
});

var smb = app.Services.GetRequiredService<ServiceMetadataBehavior>();
smb.HttpGetEnabled = true;

app.Run();