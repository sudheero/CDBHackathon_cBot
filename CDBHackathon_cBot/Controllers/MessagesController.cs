using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CDBHackathon_cBot
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
                await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task HandleSystemMessage(Activity message)
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
                // Note: Add introduction here:
                //IConversationUpdateActivity update = message;
                //var client = new ConnectorClient(new Uri(message.ServiceUrl), new MicrosoftAppCredentials());
                //var reply = message.CreateReply();
                //reply.Text = $"Welcome User! This is cBOT which helps to manage all your Financial Info";
                //client.Conversations.ReplyToActivityAsync(reply);

                var heroCard = new ThumbnailCard
                {
                    Title = "cBOT",
                    Subtitle = "I am your virtual assistant to help you on your queries related to your Cards, Payments, Deals & Offers and Customer Support",
                    Text = "I maintain all your financial information and act as a one-stop place. You can Add your cards, check your Outstanding Balance, Pay Bills, Recharge Mobile number, Check Offers & Deals and ask any queries related to our Products. I also have a Feedback section to have your valuable feedback.",
                    Images = new List<CardImage> { new CardImage("http://cbots.azurewebsites.net/images/cbotlogo.png") },
                    Buttons = new List<CardAction> { new CardAction(ActionTypes.ImBack, "Get Started", value: "Get Started") }
                };


                //Activity replyToConversation = message.CreateReply("Welcome **" + message.From.Name +"!!**");
                Activity replyToConversation = message.CreateReply("Welcome **Sudheer!!**");

                Attachment plAttachment = heroCard.ToAttachment();
                replyToConversation.Attachments.Add(plAttachment);

                if (message.MembersAdded.Any(o => o.Id == message.Recipient.Id))
                {
                    //var reply = message.CreateReply("Welcome to cBOT. You can have all your financial info here.");

                    ConnectorClient connector = new ConnectorClient(new Uri(message.ServiceUrl));

                    await connector.Conversations.ReplyToActivityAsync(replyToConversation);
                }

                //if (update.MembersAdded != null && update.MembersAdded.Any())
                //{
                //    foreach (var newMember in update.MembersAdded)
                //    {
                //        if (newMember.Id != message.Recipient.Id)
                //        {
                //            var reply = message.CreateReply();
                //            reply.Text = $"Welcome {newMember.Name}! This is cBOT which helps to manage all your Financial Info";
                //            client.Conversations.ReplyToActivityAsync(reply);
                //        }
                //    }
                //}
                //else
                //{
                //    var reply = message.CreateReply();
                //    reply.Text = $"Welcome User! This is cBOT which helps to manage all your Financial Info";
                //    client.Conversations.ReplyToActivityAsync(reply);
                //}
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

            //return null;
        }
    }
}