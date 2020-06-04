using System;
using System.Threading.Tasks;
using Microsoft.Azure.Management.EventHub.Fluent;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Description;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ThroughputUnitScaler
{
    public static class ThroughputUnitScaler
    {
        [FunctionName("ThroughputUnitScaler")]
        public static async Task Run([TimerTrigger("*/30 * * * * *")]TimerInfo _, ILogger log, ExecutionContext context)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            AppSettings appSettings = Startup(context);
            var azureCredentials = new AzureCredentialsFactory().FromServicePrincipal(appSettings.ClientId, appSettings.ClientSecret, appSettings.TenantId, AzureEnvironment.AzureGlobalCloud);
            var azureResourceManager = Azure.Authenticate(azureCredentials).WithSubscription(appSettings.SubscriptionId);
            var eventHubNamespace = azureResourceManager.EventHubNamespaces.GetByResourceGroup(appSettings.Resourcegroup, appSettings.EventHubNamespaceName);

            await eventHubNamespace
                .Update()
                .WithCurrentThroughputUnits(2)
                .ApplyAsync()
                .ConfigureAwait(false);
        }

        private static AppSettings Startup(ExecutionContext context)
        {
            var config = new ConfigurationBuilder()
                            .SetBasePath(context.FunctionAppDirectory)
                            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                            .AddEnvironmentVariables()
                            .Build();

            return new AppSettings(clientId: config["clientId"],
                clientSecret: config["clientSecret"],
                tenantId: config["tenantId"],
                subscriptionId: config["subscriptionId"],
                resourcegroup: config["resourcegroup"],
                eventHubNamespaceName: config["eventHubNamespaceName"]);
        }

        private class AppSettings
        {
            internal AppSettings(string clientId, string clientSecret, string tenantId, string subscriptionId, string resourcegroup, string eventHubNamespaceName)
            {
                ClientId = clientId;
                ClientSecret = clientSecret;
                TenantId = tenantId;
                SubscriptionId = subscriptionId;
                Resourcegroup = resourcegroup;
                EventHubNamespaceName = eventHubNamespaceName;
            }

            internal string ClientId { get; }
            internal string ClientSecret { get; }
            internal string TenantId { get; }
            internal string SubscriptionId { get; }
            internal string Resourcegroup { get; }
            internal string EventHubNamespaceName { get; }
        }
    }
}

