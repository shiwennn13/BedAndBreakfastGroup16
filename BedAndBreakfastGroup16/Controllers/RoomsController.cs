using Microsoft.AspNetCore.Mvc;
using BedAndBreakfastGroup16.Data;
using BedAndBreakfastGroup16.Models;

namespace BedAndBreakfastGroup16.Controllers
{
    public class RoomsController : Controller
    {
        //connect the controller with the database using below statement
        private readonly BedAndBreakfastGroup16Context _context;

        public RoomsController(BedAndBreakfastGroup16Context context) //constructor
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        //function: Add new room to the database
        public IActionResult AddNewRoom()
        {
            return View();
        }

        //function 4: Worker to bring the flower information to the database

        public async Task<IActionResult> AddRoom(Rooms roomobject)
        {
            if (ModelState.IsValid) //if the form has no issue
            {
                _context.RoomsTable.Add(roomobject); //add item
                await _context.SaveChangesAsync(); //to confirm add item
                return RedirectToAction("Index", "Rooms");
            }
            return View("AddNewRoom", roomobject); //return back to previous page with data
        }
    }
}
