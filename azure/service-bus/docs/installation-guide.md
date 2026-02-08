# Create resources
If you already have a resource group you want to use, proceed to the next step. Replace myResourceGroup with a name you want to use for the resource group. You can replace eastus with a region near you if needed.

```bash
az group create --name myResourceGroup --location eastus
```

Creating some variables will reduce the changes needed to the commands that create resources. Run the following commands to create the needed variables. Replace myResourceGroup with the name you're using for this exercise. If you changed the location in the previous step, make the same change in the location variable.

```bash
resourceGroup=myResourceGroup
location=eastus
namespaceName=svcbusns$RANDOM
```

## Create an Azure Service Bus namespace and queue

Create a Service Bus messaging namespace.

```bash
az servicebus namespace create \
    --resource-group $resourceGroup \
    --name $namespaceName \
    --location $location
```

Now that a namespace is created, you need to create a queue to hold the messages. 

```bash
az servicebus queue create --resource-group $resourceGroup \
    --namespace-name $namespaceName \
    --name myqueue
```