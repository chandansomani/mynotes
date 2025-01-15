using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus.Administration;

class Program
{
    static async Task Main(string[] args)
    {
        string connectionString = "<Your-Service-Bus-Connection-String>";
        List<string> queueNamesToCheck = new List<string> { "queue1", "queue2", "queue3" };

        var adminClient = new ServiceBusAdministrationClient(connectionString);

        // Get the list of existing queues
        var existingQueues = await GetExistingQueuesAsync(adminClient);

        // Filter the queues to create
        var queuesToCreate = queueNamesToCheck
            .Where(queueName => !existingQueues.Contains(queueName))
            .ToList();

        Console.WriteLine($"Queues to create: {string.Join(", ", queuesToCreate)}");

        // Create the missing queues
        await CreateQueuesAsync(adminClient, queuesToCreate);
    }

    static async Task<List<string>> GetExistingQueuesAsync(ServiceBusAdministrationClient adminClient)
    {
        var existingQueues = new List<string>();

        // Fetch all queues in the namespace
        await foreach (var queue in adminClient.GetQueuesAsync())
        {
            existingQueues.Add(queue.Name);
        }

        return existingQueues;
    }

    static async Task CreateQueuesAsync(ServiceBusAdministrationClient adminClient, List<string> queuesToCreate)
    {
        foreach (var queueName in queuesToCreate)
        {
            try
            {
                var queueOptions = new CreateQueueOptions(queueName)
                {
                    MaxDeliveryCount = 10,
                    DefaultMessageTimeToLive = TimeSpan.FromDays(7)
                };

                await adminClient.CreateQueueAsync(queueOptions);
                Console.WriteLine($"Queue '{queueName}' created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating queue '{queueName}': {ex.Message}");
            }
        }
    }
}
