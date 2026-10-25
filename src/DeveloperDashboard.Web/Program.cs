using Azure.Data.Tables;
using Azure.Identity;
using DeveloperDashboard.Web.Configuration;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();

builder.Services.AddControllersWithViews();
builder.Services.AddHealthChecks();
builder.Services.Configure<AzureTableStorageOptions>(
    builder.Configuration.GetSection(AzureTableStorageOptions.SectionName));
builder.Services.AddSingleton<TableClient>(serviceProvider =>
{
    var options = serviceProvider.GetRequiredService<IOptions<AzureTableStorageOptions>>().Value;
    if (!Uri.TryCreate(options.Endpoint, UriKind.Absolute, out var endpoint))
    {
        throw new InvalidOperationException("Azure Table Storage endpoint is not configured.");
    }

    if (string.IsNullOrWhiteSpace(options.TableName))
    {
        throw new InvalidOperationException("Azure Table Storage table name is not configured.");
    }

    return new TableClient(endpoint, options.TableName, new DefaultAzureCredential());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = _ => false
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

public partial class Program;
