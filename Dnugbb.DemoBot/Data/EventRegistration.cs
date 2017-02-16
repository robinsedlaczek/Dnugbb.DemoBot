using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Dnugbb.DemoBot.Data
{
    public enum TicketType
    {
        OneDay,
        TwoDay
    }

    [Serializable]
    public class EventRegistration
    {
        public string Event { get; set; }

        public int Seats { get; set; }

        public TicketType? TicketType{ get; set; }

        public static IForm<EventRegistration> BuildForm()
        {
            return new FormBuilder<EventRegistration>()
                    .Message("Willkommen bei der .NET User Group Berlin-Brandenburg Event-Anmeldung!")
                    .Build();
        }
    }
}