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
using iCalNET.Model;

namespace Dnugbb.DemoBot
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                /////////////////////////////////////////////////////////////////////////////////////////////
                // Demo of Bot Framework Capabilities

                try
                {
                    ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                    if (string.IsNullOrEmpty(activity.Text))
                    {
                        var message = "Hi. Wie kann ich behilflich sein?";
                        var reply = activity.CreateReply(message);
                        await connector.Conversations.ReplyToActivityAsync(reply);
                    }

                    if (activity.Text.ToLower().Contains("bilder"))
                        await ReplyWithSomeImagesAsync(activity, connector);
                    else if (activity.Text.ToLower().Contains("events") ||
                             activity.Text.ToLower().Contains("treffen"))
                        await ReplyWithEventListAsync(activity, connector);
                    else if (activity.Text.ToLower().Contains("details") ||
                             activity.Text.ToLower().Contains("inhalt") ||
                             activity.Text.ToLower().Contains("inhalte"))
                        await ReplyWithEventDetailsAsync(activity, connector);
                    else if (activity.Text.ToLower().Contains("registriere") ||
                             activity.Text.ToLower().Contains("registrieren"))
                        await RegisterUserForEventAsync(activity, connector);
                    else if (activity.Text.ToLower().Contains("anmelden") ||
                             activity.Text.ToLower().Contains("anmeldung)"))
                        await Conversation.SendAsync(activity, () => new StartEventRegistrationDialog());
                    else
                        await ReplyToAllUnknownMessagesAsync(activity, connector);
                }
                catch (Exception exception)
                {
                    var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                    var reply = activity.CreateReply($"Error occurred: {exception.ToString()}.");
                    await connector.Conversations.ReplyToActivityAsync(reply);
                }

                /////////////////////////////////////////////////////////////////////////////////////////////
                // Demo of FormFlow 
                // Uncomment the code above before using this FormFlow demo!

                //await Conversation.SendAsync(activity, MakeRootDialog);
            }
            else
            {
                await HandleSystemMessageAsync(activity);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        internal static IDialog<EventRegistration> MakeRootDialog()
        {
            return Chain.From(() => FormDialog.FromForm(BuildEventRegistrationForm));
        }

        private static IForm<EventRegistration> BuildEventRegistrationForm()
        {
            return new FormBuilder<EventRegistration>()
                .Message("Gerne helfe ich bei der Anmeldung zu unseren Treffen. Ich benötige nur ein paar Information von Dir.")
                .OnCompletion(OnEventRegistrationCompleted)
                .Build();
        }

        private async static Task OnEventRegistrationCompleted(IDialogContext context, EventRegistration state)
        {
            // SaveRegistrationToDatabase(state);
            // SendEmailConfirmation(state.Email);
        }

        private async Task ReplyToAllUnknownMessagesAsync(Activity activity, ConnectorClient connector)
        {
            try
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
            catch (Exception exception)
            {
                var reply = activity.CreateReply($"Error occurred: {exception.ToString()}.");
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

            var client = new HttpClient();
            var iCal = await client.GetStringAsync("https://dnugbb.azurewebsites.net/events.ics");
            var calendar = new vCalendar(iCal);
            
            foreach (var meetup in calendar.vEvents)
            {
                DateTime start;
                string startTime = string.Empty;

                if (DateTime.TryParse(meetup.ContentLines.Where(line => line.Key.ToLower() == "dtstart").FirstOrDefault().Value.Value, out start))
                    startTime = start.ToString("dd.MM.yyyy hh:mm");

                var heroCard = new HeroCard()
                {
                    Title = meetup.ContentLines.Where(line => line.Key.ToLower() == "summary").FirstOrDefault().Value.Value,
                    Subtitle = startTime
                                + " | " 
                                + meetup.ContentLines.Where(line => line.Key.ToLower() == "location").FirstOrDefault().Value.Value,
                    Text = meetup.ContentLines.Where(line => line.Key.ToLower() == "description").FirstOrDefault().Value.Value,

                    //Images = new List<CardImage>
                    //{
                    //    new CardImage(nextEvent.SpeakerImage)
                    //},

                    Buttons = new List<CardAction>
                    {
                        new CardAction(type: ActionTypes.OpenUrl, title: "Jetzt anmelden", value: meetup.ContentLines.Where(line => line.Key.ToLower() == "url").FirstOrDefault().Value.Value)
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