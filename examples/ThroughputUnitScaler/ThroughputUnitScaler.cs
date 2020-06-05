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


        private static class AppSettings
        {
            private static string clientId;
            internal static string ClientId => clientId ?? Environment.GetEnvironmentVariable("clientId");

            private static string clientSecret;
            internal static string ClientSecret => clientSecret ?? Environment.GetEnvironmentVariable("clientSecret");

            private static string tenantId;
            internal static string TenantId => tenantId ?? Environment.GetEnvironmentVariable("tenantId");

            private static string subscriptionId;
            internal static string SubscriptionId => subscriptionId ?? Environment.GetEnvironmentVariable("subscriptionId");

            private static string resourcegroup;
            internal static string Resourcegroup => resourcegroup ?? Environment.GetEnvironmentVariable("resourcegroup");

            private static string eventHubNamespaceName;
            internal static string EventHubNamespaceName => eventHubNamespaceName ?? Environment.GetEnvironmentVariable("eventHubNamespaceName");
        }
    }
}
