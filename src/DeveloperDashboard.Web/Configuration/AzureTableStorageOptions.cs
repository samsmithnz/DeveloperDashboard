namespace DeveloperDashboard.Web.Configuration;

public sealed class AzureTableStorageOptions
{
    public const string SectionName = "AzureTableStorage";

    public string? Endpoint { get; set; }

    public string? TableName { get; set; }
}
