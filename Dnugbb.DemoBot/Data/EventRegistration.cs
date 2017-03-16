using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dnugbb.DemoBot.Data
{
public class EventRegistration
{
    public string Event { get; set; }

    public int NumberOfSeats { get; set; }

    public string FullName { get; set; }

    public string Email { get; set; }

    public string Street { get; set; }

    public int PostalCode { get; set; }

    public string City { get; set; }
}
}