using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System.Collections.Generic;
using Dnugbb.DemoBot.Data;
using Microsoft.Bot.Builder.Dialogs;
using Dnugbb.DemoBot.Dialogs;
using System.Text.RegularExpressions;
using Microsoft.Bot.Builder.FormFlow;

namespace Dnugbb.DemoBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        internal static IDialog<EventRegistration> MakeRootDialog()
        {
            return Chain.From(() => FormDialog.FromForm(EventRegistration.BuildForm));
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                /////////////////////////////////////////////////////////////////////////////////////////////
                // Demo
                if (activity.Text.ToLower().Contains("bilder"))
                    await ReplyWithSomeImagesAsync(activity, connector);
                else if (activity.Text.ToLower().Contains("events") || activity.Text.ToLower().Contains("treffen"))
                    await ReplyWithEventListAsync(activity, connector);
                else if (activity.Text.ToLower().Contains("treffen") || activity.Text.ToLower().Contains("meetup") || activity.Text.ToLower().Contains("event"))
                    await ReplyWithEventDetailsAsync(activity, connector);
                else if (activity.Text.ToLower().Contains("anmelden") || activity.Text.ToLower().Contains("melde mich an") || activity.Text.ToLower().Contains("anmeldung)"))
                    await RegisterUserForEventAsync(activity, connector);
                else if (activity.Text.ToLower().Contains("registriere") || activity.Text.ToLower().Contains("registrieren"))
                    await Conversation.SendAsync(activity, () => new StartEventRegistrationDialog());
                else
                    await ReplyToAllUnknownMessagesAsync(activity, connector);

                /////////////////////////////////////////////////////////////////////////////////////////////
                // FormFlow
                //await Conversation.SendAsync(activity, MakeRootDialog);
            }
            else
            {
                await HandleSystemMessageAsync(activity);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task ReplyToAllUnknownMessagesAsync(Activity activity, ConnectorClient connector)
        {
            var stateClient = activity.GetStateClient();
            var userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);

            var isRegistering = userData.GetProperty<bool>("IsRegisteringToEvent");
            var selectedEvent = userData.GetProperty<string>("SelectedEvent");

            if (isRegistering && string.IsNullOrEmpty(selectedEvent))
            {
                var eventOfInterest = EventProvider.Events.Where(e => e.Topic.ToLower().Contains(activity.Text.ToLower())).FirstOrDefault();

                userData.SetProperty<string>("SelectedEvent", eventOfInterest.Topic);
                await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);

                var message = $"Ok, melde Dich zum Event '{eventOfInterest.Topic}' am {eventOfInterest.Date.ToString("dd.MM.yyyy")} an.{Environment.NewLine}{Environment.NewLine}Wieviele Plätze soll ich reservieren?";
                var reply = activity.CreateReply(message);

                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else if (isRegistering && !string.IsNullOrEmpty(selectedEvent))
            {
                var eventOfInterest = EventProvider.Events.Where(e => e.Topic.ToLower().Contains(selectedEvent.ToLower())).FirstOrDefault();
                var seats = int.Parse(activity.Text);

                await stateClient.BotState.DeleteStateForUserAsync(activity.ChannelId, activity.From.Id);

                var message = $"Ok, melde Dich zum Event '{eventOfInterest.Topic}' am {eventOfInterest.Date.ToString("dd.MM.yyyy")} an, und reserviere Dir {seats} Plätze.";
                var reply = activity.CreateReply(message);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                var message = "Diese Aussage verstehe ich leider nicht.";
                var reply = activity.CreateReply(message);
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
        }

        private async Task RegisterUserForEventAsync(Activity activity, ConnectorClient connector)
        {
            var message = "Zu welchem Event darf ich Dich anmelden?";

            var reply = activity.CreateReply(message);

            var response = await connector.Conversations.ReplyToActivityAsync(reply);

            var stateClient = activity.GetStateClient();
            var userData = await stateClient.BotState.GetUserDataAsync(activity.ChannelId, activity.From.Id);
            userData.SetProperty<bool>("IsRegisteringToEvent", true);
            await stateClient.BotState.SetUserDataAsync(activity.ChannelId, activity.From.Id, userData);
        }

        private async Task ReplyWithEventListAsync(Activity activity, ConnectorClient connector)
        {
            var message = "Das sind unserer nächsten Events:" + Environment.NewLine + Environment.NewLine;

            foreach (var nextEvent in EventProvider.Events)
            {
                message += nextEvent.Date.ToString("dd.MM.yyyy") + " - " + nextEvent.Topic + Environment.NewLine + Environment.NewLine;
            }

            var reply = activity.CreateReply(message);

            await connector.Conversations.ReplyToActivityAsync(reply);
        }

        private async Task ReplyWithEventDetailsAsync(Activity activity, ConnectorClient connector)
        {
            var message = "Hier die Details zu unseren nächsten Events:" + Environment.NewLine + Environment.NewLine;

            var reply = activity.CreateReply(message);
            reply.Attachments = new List<Attachment>();

            foreach (var nextEvent in EventProvider.Events)
            {
                var heroCard = new HeroCard()
                {
                    Title = nextEvent.Topic,
                    Subtitle = "Referent: " + nextEvent.Speaker + " Ort: " + nextEvent.Address,
                    Text = nextEvent.Abstract,
                    Images = new List<CardImage>
                    {
                        new CardImage(nextEvent.SpeakerImage)
                    },
                    Buttons = new List<CardAction>
                    {
                        new CardAction(type: ActionTypes.OpenUrl, title: "Jetzt anmelden", value: nextEvent.EventUrl),
                        new CardAction(type: ActionTypes.OpenUrl, title: "Speaker kontaktieren", value: nextEvent.EventUrl)
                    }
                };

                reply.Attachments.Add(heroCard.ToAttachment());
            }

            await connector.Conversations.ReplyToActivityAsync(reply);
        }

        private async Task ReplyWithSomeImagesAsync(Activity activity, ConnectorClient connector)
        {
            var reply = activity.CreateReply("Hier sind ein paar Bilder der letzten Treffen:");

            reply.Attachments = new List<Attachment>
            {
                new Attachment("image/png", ImageProvider.ImageUrls[1]),
                new Attachment("image/png", ImageProvider.ImageUrls[2]),
                //new Attachment("image/png", ImageProvider.ImageUrls[3]),
                new Attachment("image/png", ImageProvider.ImageUrls[4]),
            };

            await connector.Conversations.ReplyToActivityAsync(reply);
        }

        private async Task<Activity> HandleSystemMessageAsync(Activity activity)
        {
            if (activity.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (activity.MembersAdded.Count() == 0)
                    return null;

                if (activity.MembersAdded.First().Name == "Bot")
                    return null;

                var message = $@"
Hi {activity.MembersAdded.First().Name}!{Environment.NewLine}

Ich bin der Bot der .NET User Group Berlin-Brandenburg. Was kann ich für Dich tun?{Environment.NewLine}

Ich könnte Dir z.B. Bilder der letzten Treffen oder die nächsten Events zeigen. Gerne erledige ich auch Deine Anmeldung bei einem unserer bevorstehenden Events.";

                var reply = activity.CreateReply(message);
                var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                await connector.Conversations.SendToConversationAsync(reply);
                
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (activity.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (activity.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (activity.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }

    }
}