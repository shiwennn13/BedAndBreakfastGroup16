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

        public IActionResult Index()
        {
            return View();
        }
    }
}
