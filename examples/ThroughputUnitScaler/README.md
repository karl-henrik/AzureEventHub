# Throughput unit scaler

This is an Azure function that sets the Throughput units of a pre-configured namespace to 2 every 30 seconds. 

## Who needs this.

This is a basic example for how to use Azure Functions to change your configuration of Azure Event Hubs, scaling, throughput units etc. For this exact example it is ment for someone who are using auto-inflate and would like to reset the throughput units to a baseline value at a given time or date.