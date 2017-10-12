using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.ConnectorEx;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading;
using System.Web;

namespace CDBHackathon_cBot.Dialogs
{
    [Serializable]
    public class CardsDialog : IDialog<object>
    {
        [NonSerialized]
        Timer t;

        protected List<string> userCards = new List<string>();

        private const string CardsOption = "Cards";

        private const string PaymentsOption = "Payments";

        private const string DealsOffersOption = "Offers and Deals";

        private const string QnAOption = "Customer Support";

        private const string FeedbackOption = "Feedback";

        public async Task StartAsync(IDialogContext context)
        {
            //await context.PostAsync($"I am (cBOT) still dressing up with with Cards, come back later.");
            //this.ShowOptions(context);

            //await context.PostAsync($"Welcome to Cards! Type go to view Options:");
            //context.Wait(MessageReceivedStart);
            await context.PostAsync(MessageReceivedCardsHome(context));
            context.Wait(MessageReceivedChooseOption);
        }

        private async Task MessageReceivedStart(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (userCards.Count > 0)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();

                sb.Append("Your Cards are:\n\n");
                for (int i = 0; i < userCards.Count; i++)
                {
                    sb.Append("\n\n" + userCards[i]);
                }

                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                Activity replyMessage = activity.CreateReply(sb.ToString());
                await connector.Conversations.ReplyToActivityAsync(replyMessage);
            }


            //await context.PostAsync(activity.Recipient.Name + activity.ChannelId);
            
            await context.PostAsync(MessageReceivedCardsHome(context));
            context.Wait(MessageReceivedChooseOption);

        }

        private IMessageActivity MessageReceivedCardsHome(IDialogContext context)
        {

            //for adding buttons for Add, Remove and View
            var replyButtonsMessage = context.MakeMessage();
            replyButtonsMessage.Attachments = new List<Attachment>();
            List<CardAction> cardButtons = new List<CardAction>();
            cardButtons.Add(new CardAction
            {
                Title = "Add Card",
                Value = "Add",
                Type = "imBack"
            });
            cardButtons.Add(new CardAction
            {
                Title = "Remove Card",
                Value = "Remove",
                Type = "imBack"
            });
            cardButtons.Add(new CardAction
            {
                Title = "View Accounts",
                Value = "View",
                Type = "imBack"
            });
            HeroCard plCard = new HeroCard()
            {
                Title = "Welcome to Cards section.",
                Subtitle = "Please select the below operations or simply type",
                Buttons = cardButtons
            };
            Attachment plAttachment = plCard.ToAttachment();
            replyButtonsMessage.Attachments.Add(plAttachment);

            return replyButtonsMessage;
        }

        private async Task MessageReceivedChooseOption(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;




            var activity = message as Activity;


            if (message.Text.ToLower().Equals("add", StringComparison.InvariantCultureIgnoreCase))
            {
                await context.PostAsync("I need your card details to add to my list. Please provide your Debit/Credit/Savings Card details to Add:");
                context.Wait(MessageReceivedAddCard);
            }
            else if (message.Text.ToLower().Equals("home", StringComparison.InvariantCultureIgnoreCase))
            {
                this.ShowOptions(context);
            }

            else if (message.Text.ToLower().Equals("remove", StringComparison.InvariantCultureIgnoreCase))
            {
                if (userCards.Count > 0)
                {
                    await context.PostAsync("You have selected to remove your Card. Please provide Card details to Remove:");
                    context.Wait(MessageReceivedRemoveCard);
                }
                else
                {
                    await context.PostAsync($"No Cards are added");
                    context.Wait(MessageReceivedStart);
                }
            }
            else if (message.Text.ToLower().Equals("view", StringComparison.InvariantCultureIgnoreCase))
            {
                if (userCards.Count > 0)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    sb.Append("Your Account Statements:\n\n");
                    var replyMessage = context.MakeMessage();
                    List<ReceiptItem> lineItem = new List<ReceiptItem>();

                    lineItem.Add(new ReceiptItem() { Title = "SBI Savings", Price = "2,00,200" });
                    lineItem.Add(new ReceiptItem() { Title = "ICICI Savings", Price = "5,650" });
                    lineItem.Add(new ReceiptItem() { Title = "Citi Credit Card", Price = "-77,000" });

                    
                    List<CardAction> cardButtons = new List<CardAction>();
                    cardButtons.Add(new CardAction
                    {
                        Title = "Add Card",
                        Value = "Add",
                        Type = "imBack"
                    });
                    cardButtons.Add(new CardAction
                    {
                        Title = "Remove Card",
                        Value = "Remove",
                        Type = "imBack"
                    });
                    cardButtons.Add(new CardAction
                    {
                        Title = "View Accounts",
                        Value = "View",
                        Type = "imBack"
                    });
                    cardButtons.Add(new CardAction
                    {
                        Title = "Main Menu",
                        Value = "Home",
                        Type = "imBack"
                    });
                    ReceiptCard plCard = new ReceiptCard()
                    {
                        Title = "Your Account Statements:",
                        Items = lineItem,
                        Total = "128850",
                        Tax = "0",
                        
                    };

                    HeroCard hoCard = new HeroCard()
                    {
                        Subtitle = "Select Option:",
                        Buttons = cardButtons
                    };
                    Attachment plAttachment = plCard.ToAttachment();
                    replyMessage.Attachments.Add(plAttachment);
                    replyMessage.Attachments.Add(hoCard.ToAttachment());

                    await context.PostAsync(replyMessage);

                }
                else
                {
                    await context.PostAsync($"No Cards are added");
                    await context.PostAsync(MessageReceivedCardsHome(context));
                    context.Wait(MessageReceivedChooseOption);
                }
            }
            else
            {
                using (HttpClient client = new HttpClient())
                {
                    string query = message.Text;
                    string RequestURI = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/7f4d2dc9-a54b-46e1-8557-859f5dd4a546?subscription-key=846edaa4a52c4286b15fcfc11e19ad40&verbose=true&timezoneOffset=0&q=" + query;
                    HttpResponseMessage msg = await client.GetAsync(RequestURI);

                    if (msg.IsSuccessStatusCode)
                    {
                        var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                    }
                }
                await context.PostAsync($"Sorry I didn't understand your text. I have noted your text, I will get trained with this text soon.");

                await context.PostAsync(MessageReceivedCardsHome(context));
                context.Wait(MessageReceivedChooseOption);
            }
        }

        private async Task MessageReceivedAddCard(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            userCards.Add(message.Text.ToString().ToUpper());
            var replyMessage = context.MakeMessage();
            List<CardAction> cardButtons = new List<CardAction>();
            cardButtons.Add(new CardAction
            {
                Title = "Add Card",
                Value = "Add",
                Type = "imBack"
            });
            cardButtons.Add(new CardAction
            {
                Title = "Remove Card",
                Value = "Remove",
                Type = "imBack"
            });
            cardButtons.Add(new CardAction
            {
                Title = "View Accounts",
                Value = "View",
                Type = "imBack"
            });
            cardButtons.Add(new CardAction
            {
                Title = "Main Menu",
                Value = "Home",
                Type = "imBack"
            });
            HeroCard plCard = new HeroCard()
            {
                Title = "Card added successfully. We will add your card once verification is completed.",
                Subtitle = "Select Option:",
                Buttons = cardButtons
            };

            replyMessage.Attachments.Add(plCard.ToAttachment());
            await context.PostAsync(replyMessage);


            var conversationReference = message.ToConversationReference();
            ProactiveConversationStarter.conversationReference = JsonConvert.SerializeObject(conversationReference);

            //We will start a timer to fake a background service that will trigger the proactive message
            t = new Timer(new TimerCallback(timerEvent));
            t.Change(5000, Timeout.Infinite);

            var url = HttpContext.Current.Request.Url;
            await context.PostAsync("Hello, You have added a card. We want to interrupt your flow and directing to offer using Proactive message");
            //context.Wait(MessageReceivedAsync);


            context.Wait(MessageReceivedChooseOption);
        }

        private async Task MessageReceivedRemoveCard(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            userCards.Remove(message.Text.ToString().ToUpper());
            var replyMessage = context.MakeMessage();
            List<CardAction> cardButtons = new List<CardAction>();
            cardButtons.Add(new CardAction
            {
                Title = "Add Card",
                Value = "Add",
                Type = "imBack"
            });
            cardButtons.Add(new CardAction
            {
                Title = "Remove Card",
                Value = "Remove",
                Type = "imBack"
            });
            cardButtons.Add(new CardAction
            {
                Title = "View Accounts",
                Value = "View",
                Type = "imBack"
            });
            cardButtons.Add(new CardAction
            {
                Title = "Main Menu",
                Value = "Home",
                Type = "imBack"
            });
            HeroCard plCard = new HeroCard()
            {
                Title = "Card removed successfully",
                Subtitle = "Select Option:",
                Buttons = cardButtons
            };

            replyMessage.Attachments.Add(plCard.ToAttachment());
            //await context.PostAsync("Stock removed successfully");
            //await context.PostAsync($"Do you want to 'add' or 'remove' or 'view' your stocks");
            //await context.PostAsync(MessageReceivedUserButtons(context));
            await context.PostAsync(replyMessage);
            context.Wait(MessageReceivedChooseOption);
        }

        private void ShowOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { CardsOption, PaymentsOption, DealsOffersOption, QnAOption, FeedbackOption }, "Please choose any of the options below OR simply type them:", "Not a valid option", 5);
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;

                switch (optionSelected)
                {
                    case CardsOption:
                        context.Call(new Dialogs.CardsDialog(), this.ResumeAfterOptionDialog);
                        break;

                    case PaymentsOption:
                        context.Call(new Dialogs.PaymentsDialog(), this.ResumeAfterOptionDialog);
                        break;

                    case DealsOffersOption:
                        context.Call(new Dialogs.DealsOffersDialog(), this.ResumeAfterOptionDialog);
                        break;

                    case QnAOption:
                        await context.PostAsync($"I answer your general queries and given some samples below. Please type you query!! Eg:\n\n1. Is SMS Banking Charged?\n\n2. What are Banking Alerts?\n\n3. How can I subscribe to the Banking Alerts service?\n\n4. OR Anything Question related to Cards & Payments");
                        context.Call(new Dialogs.QnADialog(), this.ResumeAfterOptionDialog);
                        break;

                    case FeedbackOption:
                        context.Call(new Dialogs.FeedbackDialog(), this.ResumeAfterOptionDialog);
                        break;
                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");

                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var message = await result;
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
            finally
            {
                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            using (HttpClient client = new HttpClient())
            {
                string query = message.Text;
                string RequestURI = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/7f4d2dc9-a54b-46e1-8557-859f5dd4a546?subscription-key=846edaa4a52c4286b15fcfc11e19ad40&verbose=true&timezoneOffset=0&q=" + query;
                HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                }
            }
            await context.PostAsync($"Sorry I didn't understand your text. I have noted your text, I will get trained with this text soon.");

            await context.PostAsync(MessageReceivedCardsHome(context));
            context.Wait(MessageReceivedChooseOption);
        }

        public void timerEvent(object target)
        {
            t.Dispose();
            ProactiveConversationStarter.Resume(); //We don't need to wait for this, just want to start the interruption here
        }
    }
}