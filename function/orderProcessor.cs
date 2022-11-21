using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using System;

namespace partQueueFunc
{
    public class orderProcessor
    {
        [FunctionName("orderProcessor")]
        [ExponentialBackoffRetry(5, "00:00:30", "00:01:00")]
        [return: Table("MyTable")]
        public async Task Run(
            [ServiceBusTrigger("orders", Connection = "SBConnectionString", IsSessionsEnabled = true, AutoComplete = false)]Message myQueueItem, 
            IMessageSession messageSession,
            ILogger log)
        {
            //fail 1% of the time
            var fail = new Random().Next(100) == 77;
            if (fail) throw new Exception("Error!");

            var message = System.Text.Encoding.UTF8.GetString(myQueueItem.Body);
            log.LogInformation($"Processed: {message}");
            await messageSession.CompleteAsync(myQueueItem.SystemProperties.LockToken);
        }
    }
}
