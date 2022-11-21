using Azure.Messaging.ServiceBus;
using Azure.Identity;

// name of your Service Bus queue
// the client that owns the connection and can be used to create senders and receivers
ServiceBusClient client;

// the sender used to publish messages to the queue
ServiceBusSender sender;

// number of messages to be sent to the queue
const int numOfMessages = 5000;

client = new ServiceBusClient(Environment.GetEnvironmentVariable("SBSENDER"));
sender = client.CreateSender("orders");
var rand = new Random();

try
{
    for (int i = 1; i <= numOfMessages; i++)
    {
        // try adding a message to the batch
        var msg = new ServiceBusMessage($"Message {i}");
        msg.SessionId = rand.Next(100).ToString();
        await sender.SendMessageAsync(msg);
        Console.WriteLine($"Message {i} sent.");
    }
}
finally
{
    // Calling DisposeAsync on client types is required to ensure that network
    // resources and other unmanaged objects are properly cleaned up.
    await sender.DisposeAsync();
    await client.DisposeAsync();
}

Console.WriteLine("Press any key to end the application");
Console.ReadKey();