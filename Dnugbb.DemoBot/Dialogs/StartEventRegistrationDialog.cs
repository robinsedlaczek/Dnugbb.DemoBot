using Microsoft.Bot.Builder.Dialogs;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Dnugbb.DemoBot.Data;
using System;

namespace Dnugbb.DemoBot.Dialogs
{
    [Serializable]
    public class StartEventRegistrationDialog : IDialog<string>
    {
        protected bool _isRegisteringToEvent = false;
        protected Event _selectedEvent;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            _isRegisteringToEvent = true;

            var prompt = "Für welches Event darf ich Dich anmelden?";
            var retryPrompt = "Das Event konnte nicht gefunden werden. Versuche ein anderes.";

            PromptDialog.Text(context, AfterEventSelectedAsync, prompt, retryPrompt);
        }

        private async Task AfterEventSelectedAsync(IDialogContext context, IAwaitable<string> argument)
        {
            var activity = await argument;
            
            var eventOfInterest = EventProvider.Events.Where(e => e.Topic.ToLower().Contains(activity.ToLower())).FirstOrDefault();
            _selectedEvent = eventOfInterest;

            var message = $"Ok, melde Dich zum Event '{eventOfInterest.Topic}' am {eventOfInterest.Date.ToString("dd.MM.yyyy")} an.";

            await context.PostAsync(message);
        }
    }
}