using Microsoft.AspNetCore.Mvc;
using Amazon; //write connnection to aws account
using Amazon.S3; //s3 bucket feature, put item
using Amazon.S3.Model; //read the content
using Microsoft.Extensions.Configuration;//appsettings.json - keys
using System.IO; //input output
using Microsoft.AspNetCore.Http;//for binary data transmission for MIME
using BedAndBreakfastGroup16.Data;
using BedAndBreakfastGroup16.Models;
using Microsoft.EntityFrameworkCore;

namespace BedAndBreakfastGroup16.Controllers
{
    public class RoomsController : Controller
    {
        //connect the controller with the database using below statement
        private readonly BedAndBreakfastGroup16Context _context;

        //decide which bucket you want to follow in this code
        private const string bucketname = "bedandbreakfastrooms";

        //function 1: to get all the key back from the appsetting.json file
        private List<string> getKeys()
        {
            List<string> keys = new List<string>();

            //1.link to appsetting.json to get back the value
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json"); ;
            IConfiguration conf = builder.Build();

            //2.read the info from json to configure instance
            keys.Add(conf["Keys:key1"]);
            keys.Add(conf["Keys:key2"]);
            keys.Add(conf["Keys:key3"]);

            //collect the keys from appsetting.json
            return keys;
        }

        public RoomsController(BedAndBreakfastGroup16Context context) //constructor
        {
            _context = context;
        }

        //index page: This page can be used to display the table information (read purpose)
        public async Task<IActionResult> Index()
        {
            List<Rooms> roomlist = await _context.RoomsTable.ToListAsync();
            return View(roomlist);
        }

        //function: Add new room to the database
        public IActionResult AddNewRoom()
        {
            return View();
        }



        //function 4: Worker to bring the flower information to the database

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRoom(Rooms roomobject, List<IFormFile> imagefile)
        {
            if (ModelState.IsValid) //if the form has no issue
            {
                // Upload images to S3
                List<string> keys = getKeys();
                AmazonS3Client agent = new AmazonS3Client(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);

                foreach (var image in imagefile)
                {
                    if (image.Length <= 0)
                    {
                        return BadRequest("File of" + image.FileName + "is an empty file. Unable to upload!");
                    }
                    else if (image.Length > 2097152) //not more than 2MB
                    {
                        return BadRequest("File of" + image.FileName + "is over 2MB limit of size. Unable to upload!");
                    }
                    else if (image.ContentType.ToLower() != "image/png" && image.ContentType.ToLower() != "image/jpeg")
                    {
                        return BadRequest("File of" + image.FileName + "It is not a valid image! Unable to upload!");
                    }
                    try
                    {
                        PutObjectRequest request = new PutObjectRequest
                        {
                            BucketName = bucketname,
                            Key = "images/" + image.FileName,
                            InputStream = image.OpenReadStream(),
                            CannedACL = S3CannedACL.PublicRead
                        };

                        await agent.PutObjectAsync(request);

                        // Assuming you have a property in your Rooms model to store the image URL
                        roomobject.RoomImage = "https://" + bucketname + ".s3.amazonaws.com/images/" + image.FileName;
                    }
                    catch (AmazonS3Exception ex)
                    {
                        return BadRequest("Technical Issue" + ex.Message);
                    }
                }

                _context.RoomsTable.Add(roomobject); //add item
                await _context.SaveChangesAsync(); //to confirm add item
                return RedirectToAction("Index", "Rooms");
            }
            return View("AddNewRoom", roomobject); //return back to previous page with data
        }

        //function 5: Delete Function
        public async Task<IActionResult> deletepage(int? fid)
        {
            if (fid == null)
            {
                return NotFound();
            }

            Rooms room = await _context.RoomsTable.FindAsync(fid);

            if (room == null)
            {
                return BadRequest("Room with ID" + fid + "is not found");
            }

            _context.RoomsTable.Remove(room);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Rooms");

        }

        //function 6: Edit Page
        public async Task<IActionResult> editpage(int? fid)
        {
            if (fid == null)
            {
                return NotFound();
            }

            Rooms room = await _context.RoomsTable.FindAsync(fid);

            if (room == null)
            {
                return BadRequest("Room with ID" + fid + "is not found");
            }

            return View(room);

        }

        //function 7: update the edited information

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> updateProcess(Rooms room, List<IFormFile> imagefile)
        {
            if (room == null)
            {
                return NotFound();
            }

            // Check if new images are provided
            if (imagefile != null && imagefile.Count > 0)
            {
                List<string> keys = getKeys();
                AmazonS3Client agent = new AmazonS3Client(keys[0], keys[1], keys[2], RegionEndpoint.USEast1);

                foreach (var image in imagefile)
                {
                    if (image.Length <= 0)
                    {
                        return BadRequest("File of" + image.FileName + "is an empty file. Unable to upload!");
                    }
                    else if (image.Length > 2097152) //not more than 2MB
                    {
                        return BadRequest("File of" + image.FileName + "is over 2MB limit of size. Unable to upload!");
                    }
                    else if (image.ContentType.ToLower() != "image/png" && image.ContentType.ToLower() != "image/jpeg")
                    {
                        return BadRequest("File of" + image.FileName + "It is not a valid image! Unable to upload!");
                    }
                    try
                    {
                        PutObjectRequest request = new PutObjectRequest
                        {
                            BucketName = bucketname,
                            Key = "images/" + image.FileName,
                            InputStream = image.OpenReadStream(),
                            CannedACL = S3CannedACL.PublicRead
                        };

                        await agent.PutObjectAsync(request);

                        // Update the room's image URL
                        room.RoomImage = "https://" + bucketname + ".s3.amazonaws.com/images/" + image.FileName;
                    }
                    catch (AmazonS3Exception ex)
                    {
                        return BadRequest("Technical Issue" + ex.Message);
                    }
                }
            }

            _context.RoomsTable.Update(room);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Rooms");
        }
    }
}
