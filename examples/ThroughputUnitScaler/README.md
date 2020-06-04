# Throughput unit scaler

This is an Azure function that sets the Throughput units of a pre-configured namespace to 2 every 30 seconds. 

## Who needs this.

This example shows how to use Azure Functions to change your configuration of Azure Event Hubs, scaling, throughput units etc. This specific example demonstrates how someone who is using auto-inflate could reset the throughput units to the baseline value at a given time or date.

The code expects the corresponding AppSettings in the app service or under Values in your local.settings.json file.

```json
    "subscriptionId": "00000000-0000-0000-0000-000000000000",
    "tenantId": "00000000-0000-0000-0000-000000000000",
    "clientSecret": "00000000-0000-0000-0000-000000000000",
    "clientId": "00000000-0000-0000-0000-000000000000",
    "resourcegroup": "00000000-0000-0000-0000-000000000000",
    "eventHubNamespaceName": "00000000-0000-0000-0000-000000000000"
```

Generate the above values with the command below or read [here](https://github.com/Azure/azure-libraries-for-net/blob/master/AUTH.md) for more information about how to authenticate a client to the Azure Management Libraries for .NET. Running the command in [the cloud shell](shell.azure.com) makes it very straight forward.

```
az ad sp create-for-rbac --sdk-auth
```

## Resources

 * https://github.com/Azure/azure-libraries-for-net/blob/master/AUTH.md - Authentication in Azure Management Libraries for .NET
 * https://github.com/Azure-Samples/eventhub-dotnet-manage-event-hub - Getting started on managing event hub and associated resources using C#
 * https://github.com/Azure/azure-libraries-for-net#other-code-samples - Azure Management Libraries for .NET
 
