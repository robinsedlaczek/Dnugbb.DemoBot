using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;

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

        public static IForm<EventRegistration> BuildForm()
        {
            return new FormBuilder<EventRegistration>()
                    .Message("Willkommen! Gerne registriere ich Dich zu einem Event unser .NET User Group Berlin.")
                    .OnCompletion(OnCompleted)
                    .Build();
        }

        private static Task OnCompleted(IDialogContext context, EventRegistration state)
        {
            var message = context.MakeMessage();

            throw new NotImplementedException();
        }
    }
}