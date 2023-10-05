using Microsoft.AspNetCore.Mvc;
using BedAndBreakfastGroup16.Models;
using BedAndBreakfastGroup16.Data;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Configuration;
using System.IO;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;


namespace BedAndBreakfastGroup16.Controllers
{
    public class BookingController : Controller
    {
        private const string queueName = "BookingRoomSystemQueue";

        //connect the controller with the database using below statement
        private readonly BedAndBreakfastGroup16Context _context;

        public BookingController(BedAndBreakfastGroup16Context context) //constructor
        {
            _context = context;
        }


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

            var roomtype = await _context.RoomsTable.ToListAsync();
            ViewBag.Room = roomtype;
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

                // Save the booking information to the database
                _context.BookingInformationTable.Add(information);
                await _context.SaveChangesAsync();

                ViewBag.BookingId = information.BookingId;
                ViewBag.transactionID = response1.MessageId;
                return View(); //Need UI for the reserved page!
            }
            catch (AmazonSQSException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        //function 3: view back the message from the queue
        // admin dashboard type
        public async Task<IActionResult> viewMessages()
        {
            List<string> keys = getKeys();
            AmazonSQSClient agent = new AmazonSQSClient
            (keys[0], keys[1], keys[2], RegionEndpoint.USEast1);
            var response = await agent.GetQueueUrlAsync(new GetQueueUrlRequest { QueueName = queueName });

            List<KeyValuePair<BookingInformation, string>> msglist = new List<KeyValuePair<BookingInformation, string>>();
            try
            {
                ReceiveMessageRequest request = new ReceiveMessageRequest
                {
                    QueueUrl = response.QueueUrl,
                    MaxNumberOfMessages = 10,
                    WaitTimeSeconds = 10,
                    VisibilityTimeout = 10,
                };
                ReceiveMessageResponse response1 = await agent.ReceiveMessageAsync(request);
                if (response1.Messages.Count <= 0)
                {
                    ViewBag.errormsg = "No Booking in the waiting list now";
                }
                else
                {
                    for (int i = 0; i < response1.Messages.Count; i++)
                    {
                        BookingInformation info = JsonConvert.DeserializeObject<BookingInformation>(response1.Messages[i].Body);
                        string messageDeleteID = response1.Messages[i].ReceiptHandle;
                        msglist.Add(new KeyValuePair<BookingInformation, string>(info, messageDeleteID));
                    }
                }
                return View(msglist);
            }
            catch (AmazonSQSException ex)
            {
                return BadRequest(ex.Message);
            }

        }

        //function 4: Delete Message from the queue
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> deleteMessage(string deleteid, string word)
        {
            List<string> keys = getKeys();
            AmazonSQSClient agent = new AmazonSQSClient
            (keys[0], keys[1], keys[2], RegionEndpoint.USEast1);

            //generate the URL using dynamic queuename

            var response = await agent.GetQueueUrlAsync(new GetQueueUrlRequest { QueueName = queueName });

            try
            {
                if (word == "accept")
                {
                    Console.WriteLine("Linked to database after this!");

                }
                else
                {
                    Console.WriteLine("Only delete message, no need further action!");
                }

                DeleteMessageRequest request = new DeleteMessageRequest
                {
                    QueueUrl = response.QueueUrl,
                    ReceiptHandle = deleteid
                };
                await agent.DeleteMessageAsync(request);
            }
            catch (AmazonSQSException ex)
            {
                return BadRequest(ex.Message);
            }

            return RedirectToAction("ViewMessages", "SQSBooking");
        }

    }
}
