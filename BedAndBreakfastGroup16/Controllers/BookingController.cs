using Microsoft.AspNetCore.Mvc;
using BedAndBreakfastGroup16.Models;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json;


namespace BedAndBreakfastGroup16.Controllers
{
    public class BookingController : Controller
    {
        private const string queueName = "BookingRoomSystemQueue.fifo";

        private List<string> getKeys()
        {
            List<string> keys = new List<string>();

            //1. collect the keys from appsetting.json; link to appsettings.json and get back the values
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json");
            IConfigurationRoot conf = builder.Build(); //build the json file

            //2. read the info from json using configure instance
            keys.Add(conf["Keys:Key1"]);
            keys.Add(conf["Keys:Key2"]);
            keys.Add(conf["Keys:Key3"]);

            return keys;
        }

        //function 1: create a reservation table page
        public async Task<IActionResult> Index()
        {
            List<string> keys = getKeys();
            AmazonSQSClient agent = new AmazonSQSClient (keys[0], keys[1], keys[2], RegionEndpoint.USEast1);

            // generate URL using dynamic queuename
            var response = await agent.GetQueueUrlAsync(new GetQueueUrlRequest { QueueName = queueName });

            //get attribute request to receive message count from the queue
            GetQueueAttributesRequest request = new GetQueueAttributesRequest
            {
                QueueUrl = response.QueueUrl,
                AttributeNames = { "ApproximateNumberOfMessages" }
            };
            GetQueueAttributesResponse response1 = await agent.GetQueueAttributesAsync (request);
            ViewBag.count = response1.ApproximateNumberOfMessages;

            return View();
        }

        //function 2: send message to queue
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> send2Queue(BookingInformation information)
        {
            List<string> keys = getKeys();
            AmazonSQSClient agent = new AmazonSQSClient(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);

            // generate URL using dynamic queuename
            var response = await agent.GetQueueUrlAsync(new GetQueueUrlRequest { QueueName = queueName });

            //send message
            try
            {
                information.BookingId = Guid.NewGuid().ToString();
                SendMessageRequest request = new SendMessageRequest
                {
                    QueueUrl = response.QueueUrl,
                    MessageBody = JsonConvert.SerializeObject(information)
                };
                SendMessageResponse response1 = await agent.SendMessageAsync(request);
                ViewBag.reserveID = information.BookingId;
                ViewBag.transactionIS = response1.MessageId;
                return View(); //Need UI for the reserved page!
            }
            catch (AmazonSQSException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
