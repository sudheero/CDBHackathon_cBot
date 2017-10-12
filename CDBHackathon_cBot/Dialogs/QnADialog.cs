using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using QnAMakerDialog;
using System.Collections.Generic;
using System.Net.Http;

namespace CDBHackathon_cBot.Dialogs
{
    [Serializable]
    [QnAMakerService("e0118ee24bc04262ac9c20e285cc6232", "d79f280c-11b3-4608-87a8-c18a8299141f")]
    public class QnADialog : QnAMakerDialog<object>
    {
        private const string RaiseTicketOption = "Raise Ticket";

        private const string GoToHomeOption = "Main Menu";

        private const string ContinueChatOption = "Continue Chat";

        public override async Task NoMatchHandler(IDialogContext context, string originalQueryText)
        {
            using (HttpClient client = new HttpClient())
            {
                string query = originalQueryText;
                string RequestURI = "https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/7f4d2dc9-a54b-46e1-8557-859f5dd4a546?subscription-key=846edaa4a52c4286b15fcfc11e19ad40&verbose=true&timezoneOffset=0&q=" + query;
                HttpResponseMessage msg = await client.GetAsync(RequestURI);

                if (msg.IsSuccessStatusCode)
                {
                    var JsonDataResponse = await msg.Content.ReadAsStringAsync();
                }
            }
            await context.PostAsync($"Sorry, I am unable to answer your query '{originalQueryText}'. Please raise a ticket using the option given below for my team to handle it Offline.");
            this.ShowOptions(context);
        }

        private void ShowOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { RaiseTicketOption, GoToHomeOption, ContinueChatOption }, "Choose an option:", "Not a valid option", 4);
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string optionSelected = await result;

                switch (optionSelected)
                {
                    case RaiseTicketOption:
                        context.Call(new Dialogs.SupportDialog(), this.ResumeAfterSupportDialog);
                        break;

                    case GoToHomeOption:
                        await context.PostAsync($"Going out of Customer Support. Type go back to Home");
                        context.Call(new Dialogs.RootDialog(), this.MessageAfterOptionSelectionAsync);
                        break;

                    case ContinueChatOption:
                        await context.PostAsync($"Please type your question:");
                        context.Call(new Dialogs.QnADialog(), this.MessageAfterOptionSelectionAsync);
                        break;
                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");

                //context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task ResumeAfterSupportDialog(IDialogContext context, IAwaitable<int> result)
        {
            var ticketNumber = await result;

            await context.PostAsync($"Thanks for contacting our support team. Your ticket number is {ticketNumber}.");
            this.ShowOptions(context);
        }

        [QnAMakerResponseHandler(50)]
        public async Task LowScoreHandler(IDialogContext context, string originalQueryText, QnAMakerResult result)
        {
            await context.PostAsync($"I found an answer that might help...{result.Answer}.");
            context.Wait(MessageReceived);
        }

        private async Task MessageAfterOptionSelectionAsync(IDialogContext context, IAwaitable<object> result)
        {
            var message = await result;
        }
    }
}