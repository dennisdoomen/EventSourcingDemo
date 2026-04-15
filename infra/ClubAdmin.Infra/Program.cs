using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Sql;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.ContainerInstance;
using System.Collections.Generic;

return await Deployment.RunAsync<ClubAdminStack>();

internal sealed class ClubAdminStack : Stack
{
    public ClubAdminStack()
    {
        var config = new Config();
        var location = config.Require("location");
        var environment = config.Require("environment");
        var sqlAdminLogin = config.Require("sqlAdminLogin");
        var sqlAdminPassword = config.RequireSecret("sqlAdminPassword");

        var resourceGroup = new ResourceGroup("clubadmin-rg", new ResourceGroupArgs
        {
            ResourceGroupName = $"clubadmin-{environment}-rg",
            Location = location,
        });

        var sqlServer = new Server("clubadmin-sql", new ServerArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Location = resourceGroup.Location,
            ServerName = $"clubadmin-{environment}-sql",
            AdministratorLogin = sqlAdminLogin,
            AdministratorLoginPassword = sqlAdminPassword,
            MinimalTlsVersion = "1.2",
            Version = "12.0",
        });

        var sqlDatabase = new Database("clubadmin-db", new DatabaseArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Location = resourceGroup.Location,
            ServerName = sqlServer.Name,
            DatabaseName = $"clubadmin-{environment}",
            Sku = new Pulumi.AzureNative.Sql.Inputs.SkuArgs { Name = "S1" },
        });

        var membersApi = new WebApp("clubadmin-members-api", new WebAppArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Location = resourceGroup.Location,
            Name = $"clubadmin-{environment}-members-api",
            Kind = "functionapp",
            SiteConfig = new Pulumi.AzureNative.Web.Inputs.SiteConfigArgs
            {
                AppSettings =
                [
                    new Pulumi.AzureNative.Web.Inputs.NameValuePairArgs
                    {
                        Name = "FUNCTIONS_WORKER_RUNTIME",
                        Value = "dotnet-isolated",
                    },
                    new Pulumi.AzureNative.Web.Inputs.NameValuePairArgs
                    {
                        Name = "FUNCTIONS_EXTENSION_VERSION",
                        Value = "~4",
                    },
                ],
            },
        });

        var financesApi = new WebApp("clubadmin-finances-api", new WebAppArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Location = resourceGroup.Location,
            Name = $"clubadmin-{environment}-finances-api",
            Kind = "functionapp",
            SiteConfig = new Pulumi.AzureNative.Web.Inputs.SiteConfigArgs
            {
                AppSettings =
                [
                    new Pulumi.AzureNative.Web.Inputs.NameValuePairArgs
                    {
                        Name = "FUNCTIONS_WORKER_RUNTIME",
                        Value = "dotnet-isolated",
                    },
                    new Pulumi.AzureNative.Web.Inputs.NameValuePairArgs
                    {
                        Name = "FUNCTIONS_EXTENSION_VERSION",
                        Value = "~4",
                    },
                ],
            },
        });

        var eventStoreDb = new ContainerGroup("clubadmin-eventstoredb", new ContainerGroupArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Location = resourceGroup.Location,
            ContainerGroupName = $"clubadmin-{environment}-eventstoredb",
            OsType = "Linux",
            Containers =
            [
                new Pulumi.AzureNative.ContainerInstance.Inputs.ContainerArgs
                {
                    Name = "eventstoredb",
                    Image = "eventstore/eventstore:latest",
                    Resources = new Pulumi.AzureNative.ContainerInstance.Inputs.ResourceRequirementsArgs
                    {
                        Requests = new Pulumi.AzureNative.ContainerInstance.Inputs.ResourceRequestsArgs
                        {
                            Cpu = 1.0,
                            MemoryInGB = 1.5,
                        },
                    },
                    Ports =
                    [
                        new Pulumi.AzureNative.ContainerInstance.Inputs.ContainerPortArgs { Port = 2113 },
                        new Pulumi.AzureNative.ContainerInstance.Inputs.ContainerPortArgs { Port = 1113 },
                    ],
                    EnvironmentVariables =
                    [
                        new Pulumi.AzureNative.ContainerInstance.Inputs.EnvironmentVariableArgs
                        {
                            Name = "EVENTSTORE_CLUSTER_SIZE",
                            Value = "1",
                        },
                        new Pulumi.AzureNative.ContainerInstance.Inputs.EnvironmentVariableArgs
                        {
                            Name = "EVENTSTORE_RUN_PROJECTIONS",
                            Value = "All",
                        },
                        new Pulumi.AzureNative.ContainerInstance.Inputs.EnvironmentVariableArgs
                        {
                            Name = "EVENTSTORE_INSECURE",
                            Value = "true",
                        },
                    ],
                },
            ],
            IpAddress = new Pulumi.AzureNative.ContainerInstance.Inputs.IpAddressArgs
            {
                Type = ContainerGroupIpAddressType.Public,
                Ports =
                [
                    new Pulumi.AzureNative.ContainerInstance.Inputs.PortArgs { Port = 2113 },
                    new Pulumi.AzureNative.ContainerInstance.Inputs.PortArgs { Port = 1113 },
                ],
            },
        });

        var staticWebApp = new StaticSite("clubadmin-ui", new StaticSiteArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Location = resourceGroup.Location,
            Name = $"clubadmin-{environment}-ui",
            Sku = new Pulumi.AzureNative.Web.Inputs.SkuDescriptionArgs { Name = "Standard" },
        });

        ResourceGroupName = resourceGroup.Name;
        MembersApiUrl = membersApi.DefaultHostName;
        FinancesApiUrl = financesApi.DefaultHostName;
        FrontendUrl = staticWebApp.DefaultHostname;
    }

    [Output] public Output<string> ResourceGroupName { get; set; }
    [Output] public Output<string> MembersApiUrl { get; set; }
    [Output] public Output<string> FinancesApiUrl { get; set; }
    [Output] public Output<string> FrontendUrl { get; set; }
}
