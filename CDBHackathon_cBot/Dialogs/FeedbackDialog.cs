using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using System.Web;

namespace CDBHackathon_cBot.Dialogs
{
    [Serializable]
    public class FeedbackDialog : IDialog<object>
    {
        private const string CardsOption = "Cards";

        private const string PaymentsOption = "Payments";

        private const string DealsOffersOption = "Offers and Deals";

        private const string QnAOption = "Customer Support";

        private const string FeedbackOption = "Feedback";
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Thanks for selecting this section! Your Feedback is very important for us.");
            var form = new FormDialog<FeedbackForm>(
                new FeedbackForm(),
                FeedbackForm.BuildForm,
                FormOptions.PromptInStart);

            context.Call<FeedbackForm>(form, FormComplete);
        }

        public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            //await context.PostAsync("Please select your options for feedback");

            var form = new FormDialog<FeedbackForm>(
                new FeedbackForm(),
                FeedbackForm.BuildForm,
                FormOptions.PromptInStart);

            context.Call<FeedbackForm>(form, FormComplete);

        }

        private void ShowOptions(IDialogContext context)
        {
            PromptDialog.Choice(context, this.OnOptionSelected, new List<string>() { CardsOption, PaymentsOption, DealsOffersOption, QnAOption, FeedbackOption }, "Choose an option:", "Not a valid option", 5);
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

        private async Task FormComplete(IDialogContext context, IAwaitable<FeedbackForm> result)
        {
            FeedbackForm form = null;
            try
            {
                form = await result;

                ClientContext ctx = new ClientContext("https://wbsharepoint.sharepoint.com/sites/POCs/");
                Web web = ctx.Web;

                string pwd = "Welcome2017";
                SecureString passWord = new SecureString();
                foreach (char c in pwd.ToCharArray()) passWord.AppendChar(c);
                ctx.Credentials = new SharePointOnlineCredentials("demouser@wbsharepoint.onmicrosoft.com", passWord);

                List feedbackList = ctx.Web.Lists.GetByTitle("cBOTFeedback");
                ListItemCreationInformation feedbackCreateInfo = new ListItemCreationInformation();
                ListItem feedbackItem;

                feedbackItem = feedbackList.AddItem(feedbackCreateInfo);

                string[] arrCategory = { "Blank", "UX", "Services", "Others" };
                string[] arrRate = {"Blank", "Horrible", "Bad", "Ok", "Good", "Awesome" };

                feedbackItem["Title"] = arrCategory[(int)form.CategoryName];
                feedbackItem["WhatLikes"] = form.Like;
                feedbackItem["WhatNeedsImprove"] = form.Improved;
                feedbackItem["Rating"] = arrRate[(int)form.Rate];
                feedbackItem.Update();
                ctx.ExecuteQuery();
            }
            catch (OperationCanceledException)
            {
            }

            if (form == null)
            {
                await context.PostAsync("You canceled the form.");
            }
            else
            {
                // Here we could call our signup service to complete the sign-up

                var message = $"We have taken your inputs and Thanks for your valuable feedback.";
                await context.PostAsync(message);
            }
            this.ShowOptions(context);
            
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
    }
}