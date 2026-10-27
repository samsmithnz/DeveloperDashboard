# Azure deployment

The Bicep template provisions a Linux App Service plan and web app, an Azure
Storage account and `Snapshots` table, and a system-assigned managed identity.
The plan capacity is fixed at one instance. No autoscale setting is deployed.
The web app receives the Table endpoint and table name as application settings;
storage account keys are disabled and are not used. The identity receives
**Storage Table Data Contributor** on the provisioned table only.

## Provision the infrastructure

Install the Azure CLI and sign in with an identity that can create the resources
and role assignment in the target subscription (for example, Contributor plus
User Access Administrator on the resource group).

```sh
az login
az account set --subscription "<subscription-id>"
az group create --name developer-dashboard --location eastus
az deployment group create \
  --resource-group developer-dashboard \
  --template-file infra/main.bicep \
  --parameters appName="<globally-unique-app-name>" \
               storageAccountName="<3-to-24-character-unique-name>"
```

The storage account name must be globally unique, lowercase, and contain only
letters and numbers. The App Service name must also be globally unique. Wait for
the role assignment to propagate before using the app to access the table.

## Publish the application

```sh
dotnet publish src/DeveloperDashboard.Web/DeveloperDashboard.Web.csproj \
  --configuration Release --output ./artifacts/publish
(cd ./artifacts/publish && zip -qr ../developer-dashboard.zip .)
az webapp deploy \
  --resource-group developer-dashboard \
  --name "<globally-unique-app-name>" \
  --src-path ./artifacts/developer-dashboard.zip \
  --type zip
```

The application readiness endpoint is `/health/ready`. It reports application
readiness only and does not contact Azure Storage. This foundation provisions
and configures Table Storage but does not write or read snapshots.

## Local configuration and credentials

The app runs without Azure credentials while the prototype is being developed.
When testing the configured Table client locally, set its endpoint and table
name in .NET User Secrets (or environment variables), then authenticate with
Azure CLI. The SDK uses `DefaultAzureCredential`; deployed access uses the
App Service managed identity.

```sh
az login
dotnet user-secrets set --project src/DeveloperDashboard.Web \
  "AzureTableStorage:Endpoint" "https://<storage-account>.table.core.windows.net"
dotnet user-secrets set --project src/DeveloperDashboard.Web \
  "AzureTableStorage:TableName" "Snapshots"
```

Do not add credentials, private keys, or storage keys to `appsettings*.json` or
source control. Supply any future application secrets through .NET User Secrets
for local development or protected App Service settings in Azure.
