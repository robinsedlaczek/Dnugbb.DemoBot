using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dnugbb.DemoBot.Data
{
    public enum TicketType
    {
        Standard = 1,
        CommunitySupporter
    }

    [Serializable]
    public class EventRegistration
    {
        public string Event { get; set; }

        public int NumberOfSeats { get; set; }

        public TicketType TicketType { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }
    }
}