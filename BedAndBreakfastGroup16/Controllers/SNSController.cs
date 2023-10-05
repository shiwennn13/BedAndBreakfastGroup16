using Microsoft.AspNetCore.Mvc;
using Amazon;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace BedAndBreakfastGroup16.Controllers
{
    public class SNSController : Controller
    {
        private const string topicARN = "arn:aws:sns:us-east-1:107291066015:SNSsample";

        //function 1: to get all the keys back from the appsetting.json
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



        //function 3: use to subscribe the topics 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> regSubscription(string emailsubscribe)
        {
            List<string> keys = getKeys();
            AmazonSimpleNotificationServiceClient agent = new AmazonSimpleNotificationServiceClient(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);

            try
            {
                SubscribeRequest request = new SubscribeRequest
                {
                    TopicArn = topicARN,
                    Protocol = "email",
                    Endpoint = emailsubscribe
                };
                SubscribeResponse response = await agent.SubscribeAsync(request);
                ViewBag.recordid = response.ResponseMetadata.RequestId;
            }
            catch (AmazonSimpleNotificationServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            return View();
        }
        [Authorize(Roles = "Admin")]
        //function 4: admin broadcast personal message sample
        public IActionResult BroadcastMessagePage()
        {
            return View();
        }

        //function 5: Broadcast Message Action through SNS
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> broadcastmsg(string subject, string msgbody)
        {
            List<string> keys = getKeys();
            AmazonSimpleNotificationServiceClient agent = new AmazonSimpleNotificationServiceClient(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);
            try
            {
                PublishRequest request = new PublishRequest
                {
                    TopicArn = topicARN,
                    Subject = subject,
                    Message = msgbody
                };
                await agent.PublishAsync(request);
            }
            catch (AmazonSimpleNotificationServiceException ex)
            {
                return BadRequest(ex.Message);
            }
            return RedirectToAction("SuccessfulBroadcast");
        }

        public IActionResult SuccessfulBroadcast()
        {
            return View();
        }
    }
}
