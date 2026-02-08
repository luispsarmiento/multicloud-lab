using Azure.Messaging.ServiceBus;
using Azure.Identity;
using System.Timers;

string svcbusConnectionString = "Endpoint=sb://lps-poc-notifications.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=ZMfxncStp5YZvaMxEtZn1AiJ0FueOTbpy+ASbOqpZjQ=";
string queueName = "sms";

ServiceBusClient client = new(svcbusConnectionString);


ServiceBusSender sender = client.CreateSender(queueName);

using ServiceBusMessageBatch messageBatch = await sender.CreateMessageBatchAsync();

const int numOfMessages = 3;

for (int i = 1; i <= numOfMessages; i++)
{
    // try adding a message to the batch
    if (!messageBatch.TryAddMessage(new ServiceBusMessage($"Message {i}")))
    {
        // if it is too large for the batch
        throw new Exception($"The message {i} is too large to fit in the batch.");
    }
}

try
{
    // Use the producer client to send the batch of messages to the Service Bus queue
    await sender.SendMessagesAsync(messageBatch);
    Console.WriteLine($"A batch of {numOfMessages} messages has been published to the queue.");
}
finally
{
    // Calling DisposeAsync on client types is required to ensure that network
    // resources and other unmanaged objects are properly cleaned up.
    await sender.DisposeAsync();
}

Console.WriteLine("Press any key to continue");
Console.ReadKey();

// ADD CODE TO PROCESS MESSAGES FROM THE QUEUE



// Dispose client after use
await client.DisposeAsync();