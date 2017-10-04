using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Threading.Tasks;

namespace Dnugbb.DemoBot.Dialogs
{
    [LuisModel("modelId", "subscriptionKey")]
    [Serializable]
    public class RegistrationDialog : LuisDialog<object>
    {
        [LuisIntent("RegisterForEvent")]
        public async Task RegisterForEvent(IDialogContext context, LuisResult result)
        {
            // Logik für die Anmeldung zu einem Event...
        }

        [LuisIntent("UnregisterFromEvent")]
        public async Task UnregisterFromEvent(IDialogContext context, LuisResult result)
        {
            
            // Logik für die Abmeldung zu einem Event...
        }
    }
}