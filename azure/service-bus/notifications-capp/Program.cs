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
    var completeWord = GetCompleteWord(i);
    // try adding a message to the batch
    if (!messageBatch.TryAddMessage(new ServiceBusMessage($"{i} {completeWord}")))
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

ServiceBusProcessor processor = client.CreateProcessor(queueName, new ServiceBusProcessorOptions());

const int idleTimeoutMs = 3000;
System.Timers.Timer idleTimer = new(idleTimeoutMs);
idleTimer.Elapsed += async (s, e) =>
{
    Console.WriteLine($"No messages received for {idleTimeoutMs / 1000} seconds. Stopping processor...");
    await processor.StopProcessingAsync();
};

try
{
    // add handler to process messages
    processor.ProcessMessageAsync += MessageHandler;

    // add handler to process any errors
    processor.ProcessErrorAsync += ErrorHandler;

    // start processing 
    idleTimer.Start();
    await processor.StartProcessingAsync();

    Console.WriteLine($"Processor started. Will stop after {idleTimeoutMs / 1000} seconds of inactivity.");
    // Wait for the processor to stop
    while (processor.IsProcessing)
    {
        await Task.Delay(500);
    }
    idleTimer.Stop();
    Console.WriteLine("Stopped receiving messages");
}
finally
{
    // Dispose processor after use
    await processor.DisposeAsync();
}

// handle received messages
async Task MessageHandler(ProcessMessageEventArgs args)
{
    string body = args.Message.Body.ToString();
    Console.WriteLine($"Received: {body}");

    // Reset the idle timer on each message
    idleTimer.Stop();
    idleTimer.Start();

    // complete the message. message is deleted from the queue. 
    await args.CompleteMessageAsync(args.Message);
}

// handle any errors when receiving messages
Task ErrorHandler(ProcessErrorEventArgs args)
{
    Console.WriteLine(args.Exception.ToString());
    return Task.CompletedTask;
}

string GetCompleteWord(int count)
{
    return count switch
    {
        1 => "This",
        2 => "is a",
        3 => "hack",
        _ => "Message"
    };
}

// Dispose client after use
await client.DisposeAsync();