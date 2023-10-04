using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BedAndBreakfastGroup16.Areas.Identity.Data;

// Add profile data for application users by adding properties to the BedAndBreakfastGroup16User class
public class BedAndBreakfastGroup16User : IdentityUser
{
    [PersonalData]
    public string CustomerFullName { get; set; }

    [PersonalData]
    public int CustomerAge { get; set; }

    [PersonalData]
    public string CustomerAddress { get; set; }

    [PersonalData]
    public DateTime CustomerDoB { get; set; }

    [PersonalData]
    public String userrole{ get; set; }
}

