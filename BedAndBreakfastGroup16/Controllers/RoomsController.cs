using Microsoft.AspNetCore.Mvc;
using BedAndBreakfastGroup16.Data;
using BedAndBreakfastGroup16.Models;
using Microsoft.EntityFrameworkCore;

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
        [ValidateAntiForgeryToken] //avoid cross-site attack

        public async Task<IActionResult> updateProcess(Rooms room)
        {
            if (room == null)
            {
                return NotFound();
            }
            _context.RoomsTable.Update(room);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Rooms");
        }
    }
}
