using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace Dnugbb.DemoBot.Dialogs
{
    [Serializable]
    public class NumberOfSeatsDialog : IDialog<long>
    {
        protected long _numberOfSeats = 1;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var prompt = "Wie viele Plätze soll ich für Dich reservieren?";
            var retryPrompt = "Leider verstehe ich Deine Eingabe nicht. Bitte gebe eine ganze Zahl ein.";

            PromptDialog.Number(context, AfterSeatsEnteredAsync, prompt, retryPrompt);
        }

        private async Task AfterSeatsEnteredAsync(IDialogContext context, IAwaitable<long> argument)
        {
            _numberOfSeats = await argument;
        }
    }
}