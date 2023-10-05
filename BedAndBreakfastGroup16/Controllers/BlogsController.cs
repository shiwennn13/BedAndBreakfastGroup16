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
using Microsoft.AspNetCore.Authorization;

namespace BedAndBreakfastGroup16.Controllers
{
    public class BlogsController : Controller
    {
        //connect the controller with the database using below statement
        private readonly BedAndBreakfastGroup16Context _context;

        //decide which bucket you want to follow in this code
        private const string bucketname = "bedandbreakfastblogs";

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

        public BlogsController(BedAndBreakfastGroup16Context context) //constructor
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Blogs> bloglist = await _context.BlogsTable.ToListAsync();
            return View(bloglist);
        }

        [Authorize(Roles = "Admin")]
        //function: Add new blog to the database
        public IActionResult AddNewBlog()
        {
            return View();
        }

        //function 4: Worker to bring the flower information to the database

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBlog(Blogs blogobject, List<IFormFile> imagefile)
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

                        // Assuming you have a property in your Blogs model to store the image URL
                        blogobject.BlogImage = "https://" + bucketname + ".s3.amazonaws.com/images/" + image.FileName;
                    }
                    catch (AmazonS3Exception ex)
                    {
                        return BadRequest("Technical Issue" + ex.Message);
                    }
                }

                _context.BlogsTable.Add(blogobject); //add item
                await _context.SaveChangesAsync(); //to confirm add item
                return RedirectToAction("Index", "Blogs");
            }
            return View("AddNewBlog", blogobject); //return back to previous page with data
        }

        //function 5: Delete Function
        public async Task<IActionResult> deletepage(int? fid)
        {
            if (fid == null)
            {
                return NotFound();
            }

            Blogs blog = await _context.BlogsTable.FindAsync(fid);

            if (blog == null)
            {
                return BadRequest("Blog with ID" + fid + "is not found");
            }

            _context.BlogsTable.Remove(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Blogs");

        }

        //function 6: Edit Page
        public async Task<IActionResult> editpage(int? fid)
        {
            if (fid == null)
            {
                return NotFound();
            }

            Blogs blogs = await _context.BlogsTable.FindAsync(fid);

            if (blogs == null)
            {
                return BadRequest("Blog with ID" + fid + "is not found");
            }

            return View(blogs);

        }

        //function 7: update the edited information

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> updateProcess(Blogs blog, List<IFormFile> imagefile)
        {
            if (blog == null)
            {
                return NotFound();
            }

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
                    blog.BlogImage = "https://" + bucketname + ".s3.amazonaws.com/images/" + image.FileName;

                }
                catch (AmazonS3Exception ex)
                {
                    return BadRequest("Technical Issue" + ex.Message);
                }
            }

            _context.BlogsTable.Update(blog);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Blogs");
        }


        public async Task<IActionResult> BlogDetails(int? blogID)
        {
            if (blogID == null)
            {
                return NotFound();
            }
            Blogs blog = await _context.BlogsTable.FindAsync(blogID);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }


    }
}
