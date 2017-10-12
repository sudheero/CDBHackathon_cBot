using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CDBHackathon_cBot.Dialogs
{
    [Serializable]
    public class PaymentsDialog : IDialog<object>
    {
        private const string CardsOption = "Cards";

        private const string PaymentsOption = "Payments";

        private const string DealsOffersOption = "Offers and Deals";

        private const string QnAOption = "Customer Support";

        private const string FeedbackOption = "Feedback";

        private const string MobileRechargeOption = "Mobile Recharge";

        private const string ElecticityRechargeOption = "Electricity Bill Pay";

        private const string DTHRechargeOption = "DTH Recharge";

        private const string DataCardRechargeOption = "Data Card Recharge";

        private const string GoHomeOption = "Main Menu";
        
        public async Task StartAsync(IDialogContext context)
        {
            //PromptDialog.Choice(context, this.OnPaymentOptionSelected, new List<string>() { MobileRechargeOption, ElecticityRechargeOption }, "Welcome to Payments Section. I currently provide below services. Please choose an option OR type:", "Not a valid option", 2);

            var reply = context.MakeMessage();

            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = GetCardsAttachments();

            await context.PostAsync(reply);
            context.Wait(this.OnPaymentOptionSelected);
            //this.ShowPaymentsOptions(context);
        }

        private async Task OnPaymentOptionSelected(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            try
            {
                var optionSelected = await result;

                switch (optionSelected.Text)
                {
                    case MobileRechargeOption:
                        await context.PostAsync($"Please Enter Mobile number to Recharge in the format (Mobile Number : Amount)");
                        context.Wait(this.MessageMobileRechargeAsync);
                        break;

                    case ElecticityRechargeOption:
                        await context.PostAsync($"Please Enter Electricity Connection number to Recharge in the format (EB Card : Amount)");
                        context.Wait(this.MessageElectricityBillPayAsync);
                        break;

                    case DTHRechargeOption:
                        await context.PostAsync($"Please Enter DTH Connection number to Recharge in the format (DTH Number : Amount)");
                        context.Wait(this.MessageDTHRechargeAsync);
                        break;

                    case DataCardRechargeOption:
                        await context.PostAsync($"Please Enter Data card number to Recharge in the format (Data card Number : Amount)");
                        context.Wait(this.MessageDataCardRechargeAsync);
                        break;

                    case GoHomeOption:
                        this.ShowOptions(context);
                        break;

                }
            }
            catch (TooManyAttemptsException ex)
            {
                await context.PostAsync($"Ooops! Too many attemps :(. But don't worry, I'm handling that exception and you can try again!");

                context.Wait(this.MessageReceivedAsync);
            }
        }

        private async Task MessageMobileRechargeAsync(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync($"Mobile number is recharge successfully.");
            this.ShowPaymentsOptions(context);
        }

        private async Task MessageElectricityBillPayAsync(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync($"Electricity Payment is done successfully.");
            this.ShowPaymentsOptions(context);
        }

        private async Task MessageDTHRechargeAsync(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync($"DTH Recharge is done successfully.");
            this.ShowPaymentsOptions(context);
        }
        private async Task MessageDataCardRechargeAsync(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync($"Data Card Recharge is done successfully.");
            this.ShowPaymentsOptions(context);
        }


        private async Task ShowPaymentsOptions(IDialogContext context)
        {
            context.Wait(this.OnPaymentOptionSelected);
            //PromptDialog.Choice(context, this.OnPaymentOptionSelected, new List<string>() { MobileRechargeOption, ElecticityRechargeOption, GoHomeOption}, "Choose an option:", "Not a valid option", 3);
            var reply = context.MakeMessage();

            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = GetCardsAttachments();

            await context.PostAsync(reply);
            context.Wait(this.OnPaymentOptionSelected);
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
            //context.Wait(new Dialogs.LUISDialogBox(), this.ResumeAfterOptionDialog);
        }

        private static IList<Attachment> GetCardsAttachments()
        {
            return new List<Attachment>()
            {
                GetThumbnailCard(
                    "Mobile Recharge",
                    "cBOT providing feature for simple mobile Recharge",
                    "You can recharge for Prepaid or Postpaid numbers accross India for all service providers",
                    new CardImage(url: "http://cbots.azurewebsites.net/images/mobilerecharge.png"),
                    new CardAction(ActionTypes.ImBack, "Mobile Recharge", value: "Mobile Recharge")),
                GetThumbnailCard(
                    "Electricity Bill Pay",
                    "cBOT providing feature for simple Electricity Bill Payments",
                    "You can Pay Electricity Bills accross all states in India",
                    new CardImage(url: "http://cbots.azurewebsites.net/images/electricitybillpay.jpg"),
                    new CardAction(ActionTypes.ImBack, "Electricity Bill Pay", value: "Electricity Bill Pay")),
                GetThumbnailCard(
                    "DTH Recharge",
                    "cBOT providing feature for simple DTH Recharge",
                    "You can recharge your DTH number for all service providers accross India",
                    new CardImage(url: "http://cbots.azurewebsites.net/images/dthrecharge.png"),
                    new CardAction(ActionTypes.ImBack, "DTH Recharge", value: "DTH Recharge")),
                GetThumbnailCard(
                    "Data Card Recharge",
                    "cBOT providing feature for simple Data Card Recharge",
                    "You can recharge for Prepaid or Postpaid Data Card numbers accross India for all service providers",
                    new CardImage(url: "http://cbots.azurewebsites.net/images/datacardrecharge.jpg"),
                    new CardAction(ActionTypes.ImBack, "Data Card Recharge", value: "Data Card Recharge")),
                GetThumbnailCard(
                    "Main Menu",
                    "cBOT - I am your virtual assistant to help you on your financial information",
                    "I maintain all your financial information and act as a one-stop place.",
                    new CardImage(url: "http://cbots.azurewebsites.net/images/cbotlogo.png"),
                    new CardAction(ActionTypes.ImBack, "Main Menu", value: "Main Menu")),
            };
        }

        private static Attachment GetThumbnailCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction)
        {
            var heroCard = new ThumbnailCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Images = new List<CardImage>() { cardImage },
                Buttons = new List<CardAction>() { cardAction },
            };

            return heroCard.ToAttachment();
        }
    }
}