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

            var azureCredentials = new AzureCredentialsFactory().FromServicePrincipal(AppSettings.ClientId, AppSettings.ClientSecret, AppSettings.TenantId, AzureEnvironment.AzureGlobalCloud);
            var azureResourceManager = Azure.Authenticate(azureCredentials).WithSubscription(AppSettings.SubscriptionId);
            var eventHubNamespace = azureResourceManager.EventHubNamespaces.GetByResourceGroup(AppSettings.Resourcegroup, AppSettings.EventHubNamespaceName);

            await eventHubNamespace
                .Update()
                .WithCurrentThroughputUnits(2)
                .ApplyAsync()
                .ConfigureAwait(false);
        }


        private class AppSettings
        {
            internal static string ClientId => Environment.GetEnvironmentVariable("clientId");
            internal static string ClientSecret => Environment.GetEnvironmentVariable("clientSecret");
            internal static string TenantId => Environment.GetEnvironmentVariable("tenantId");
            internal static string SubscriptionId => Environment.GetEnvironmentVariable("subscriptionId");
            internal static string Resourcegroup => Environment.GetEnvironmentVariable("resourcegroup");
            internal static string EventHubNamespaceName => Environment.GetEnvironmentVariable("eventHubNamespaceName");
        }
    }
}
