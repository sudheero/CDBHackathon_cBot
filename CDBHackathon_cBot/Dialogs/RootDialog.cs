using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using System.Threading;

namespace CDBHackathon_cBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private const string CardsOption = "Cards";

        private const string PaymentsOption = "Payments";

        private const string DealsOffersOption = "Offers and Deals";

        private const string QnAOption = "Customer Support";

        private const string FeedbackOption = "Feedback";

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageSelectOptionAsync);
            //context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync($"Welcome to **cBOT**.");
            //context.Wait(new Dialogs.LUISDialogBox(), this.ResumeAfterOptionDialog);

            
            //await context.PostAsync("![duck](cBOTlogo.png)");
            //await context.PostAsync($"> New Welcome");

            context.Wait(MessageSelectOptionAsync);
        }

        public async Task MessageSelectOptionAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Text.ToLower().Contains("help") || message.Text.ToLower().Contains("support") || message.Text.ToLower().Contains("problem"))
            {
                await context.Forward(new Dialogs.SupportDialog(), this.ResumeAfterSupportDialog, message, CancellationToken.None);
            }
            else
            {
                this.ShowOptions(context);
            }



            /*var questionMessage = context.MakeMessage();

            var heroCard = new HeroCard
            {
                Subtitle = "Welcome to cBot. Please select option:",
                Buttons = new List<CardAction>() {
                        new CardAction { Title = "Cards", Type = "imBack", Value = "Cards/Payments" },
                        new CardAction { Title = "CustomerService", Type = "imBack", Value = "Customer Service" },
                    }
            };
            questionMessage.Attachments.Add(heroCard.ToAttachment());
            await context.PostAsync(questionMessage);

            context.Wait(MessageAfterOptionSelectionAsync);*/
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

       /* private async Task MessageAfterOptionSelectionAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            if (message.Text == "Cards/Payments")
            {
                await context.PostAsync($"Welcome to CBOT Cards. We are still working on this feature.");
                context.Wait(MessageSelectOptionAsync);
            }
            else
            {
                await context.PostAsync($"Welcome to cBOT customer service. Type your question/key word to get answered");
                context.Wait(MessageCustomerServiceAsync);
            }
        }

        private async Task MessageCustomerServiceAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            await context.PostAsync($"You have typed: " + message.Text);
        }*/

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

        private async Task ResumeAfterSupportDialog(IDialogContext context, IAwaitable<int> result)
        {
            var ticketNumber = await result;

            await context.PostAsync($"Thanks for contacting our support team. Your ticket number is {ticketNumber}.");
            context.Wait(this.MessageReceivedAsync);
        }
    }
}