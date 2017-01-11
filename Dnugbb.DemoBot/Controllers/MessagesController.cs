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
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));

                await ReplyWithImageCards(activity, connector);
                await ReplyWithReceiptCard(activity, connector);
                await ReplyWithAttachment(activity, connector);
            }
            else
            {
                HandleSystemMessage(activity);
            }

            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private static async Task ReplyWithImageCards(Activity activity, ConnectorClient connector)
        {
            if (activity.Text.ToLower().Contains("bilder der dnugbb"))
            {
                var imageUrls = ImageProvider.GetImageUrls();

                var images = new List<CardImage>()
                    {
                        new CardImage(imageUrls[0]),
                        new CardImage(imageUrls[1]),
                        new CardImage(imageUrls[2]),
                    };

                var buttons = new List<CardAction>()
                    {
                        new CardAction("openUrl", "Open Image 1", imageUrls[0], imageUrls[0]),
                        new CardAction("openUrl", "Open Image 2", imageUrls[1], imageUrls[1]),
                        new CardAction("openUrl", "Open Image 3", imageUrls[2], imageUrls[2]),
                    };

                var heroCard = new HeroCard("Unsere Events", "Bilder der DNUGBB", "Hier sind ein paar Bilder unserer .NET User Group Berlin für Dich.", images, buttons);

                var thumbnailCard = new ThumbnailCard("Unsere Events", "Bilder der DNUGBB", "Hier sind ein paar Bilder unserer .NET User Group Berlin für Dich.", images, buttons);

                var reply = activity.CreateReply("Eine Heldenkarte...");
                reply.Attachments = new List<Attachment>();
                reply.Attachments.Add(heroCard.ToAttachment());
                reply.Attachments.Add(thumbnailCard.ToAttachment());

                var userReply = await connector.Conversations.ReplyToActivityAsync(reply);
            }
        }

        private static async Task ReplyWithReceiptCard(Activity activity, ConnectorClient connector)
        {
            if (activity.Text.ToLower().Contains("rechnung"))
            {
                var imageUrls = ImageProvider.GetImageUrls();

                var buttons = new List<CardAction>()
                    {
                        new CardAction("openUrl", "Open Image 1", imageUrls[0], imageUrls[0]),
                    };

                var lineItem1 = new ReceiptItem()
                {
                    Title = "Pork Shoulder",
                    Subtitle = "8 lbs",
                    Text = null,
                    Image = new CardImage(imageUrls[0]),
                    Price = "16.25",
                    Quantity = "1",
                    Tap = null
                };

                var lineItem2 = new ReceiptItem()
                {
                    Title = "Bacon",
                    Subtitle = "5 lbs",
                    Text = null,
                    Image = new CardImage(imageUrls[1]),
                    Price = "34.50",
                    Quantity = "2",
                    Tap = null
                };

                var receiptList = new List<ReceiptItem>()
                    {
                        lineItem1,
                        lineItem2
                    };

                var receiptCard = new ReceiptCard()
                {
                    Title = "I'm a receipt card, isn't this bacon expensive?",
                    Buttons = buttons,
                    Items = receiptList,
                    Total = "275.25",
                    Tax = "27.52"
                };

                var reply = activity.CreateReply("Eine Heldenkarte...");
                reply.Attachments = new List<Attachment>();
                reply.Attachments.Add(receiptCard.ToAttachment());

                var userReply = await connector.Conversations.ReplyToActivityAsync(reply);
            }
        }

        private static async Task ReplyWithAttachment(Activity activity, ConnectorClient connector)
        {
            if (activity.Text == "Du")
            {
                var reply = activity.CreateReply("Arsch!");

                var attachment = new Attachment()
                {
                    ContentUrl = "https://upload.wikimedia.org/wikipedia/en/a/a6/Bender_Rodriguez.png",
                    ContentType = "image/png",
                    Name = "Bender_Rodriguez.png"
                };

                reply.Attachments = new List<Attachment>();
                reply.Attachments.Add(attachment);

                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else if (activity.Text == "Sei lieb!")
            {
                var reply = activity.CreateReply("Na gut...");
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}