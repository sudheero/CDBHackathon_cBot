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
    public class DealsOffersDialog : IDialog<object>
    {
        private const string CardsOption = "Cards";

        private const string PaymentsOption = "Payments";

        private const string DealsOffersOption = "Offers and Deals";

        private const string QnAOption = "Customer Support";

        private const string FeedbackOption = "Feedback";

        private const string GoHomeOption = "Main Menu";
        public async Task StartAsync(IDialogContext context)
        {
            var reply = context.MakeMessage();

            reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;
            reply.Attachments = GetCardsAttachments();

            await context.PostAsync(reply);
            context.Wait(this.OnPaymentOptionSelected);

            //await context.PostAsync(
            //    "**I got these Hot Deals for you:**\n\n "+ 
            //    //"`MAX BUPA HEALTH INSURANCE`: SBI Credit Cards - 3 % Off on premium \n\n \n\n" +
            //    //"`INDIALENDS`: SBI Credit Cards - Get 100 Vantage Credits on E - approval \n\n" +
            //    "`PEPPERFRY`: Rs 1000 Off on Minimum Purchase of Rs 3, 500 \n\n" +
            //    //"`FERNS N PETALS`: Upto 7 % Vantage Points on Eid Gifts Store \n\n" +
            //    "`AMAZON`: Upto 8.4 % Vantage Points on Eid Special Store \n\n" +
            //    //"`EDIBUDDY`: Additional 10 % Off on Health Check \n\n" +
            //    "`SAMSUNG MOBILE & HOME APPLIANCES`: Upto 8000 Off + Free insurance & Zero cost EMI on Smartphones \n\n" +
            //    "`FLIPKART`: Upto 4.2 % Vantage Points on Large Appliance Store \n\n" 
            //    //"`OYO ROOMS`: Rs 500 Off on minimum Hotel Bookings of Rs 1799 \n\n"
            //    );
            //this.ShowOptions(context);
        }

        private async Task OnPaymentOptionSelected(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            try
            {
                var optionSelected = await result;

                switch (optionSelected.Text)
                {
                    //case MobileRechargeOption:
                    //    await context.PostAsync($"Please Enter Mobile number to Recharge in the format (Mobile Number : Amount)");
                    //    context.Wait(this.MessageMobileRechargeAsync);
                    //    break;

                    //case ElecticityRechargeOption:
                    //    await context.PostAsync($"Please Enter Electricity Connection number to Recharge in the format (EB Card : Amount)");
                    //    context.Wait(this.MessageElectricityBillPayAsync);
                    //    break;

                    //case DTHRechargeOption:
                    //    await context.PostAsync($"Please Enter DTH Connection number to Recharge in the format (DTH Number : Amount)");
                    //    context.Wait(this.MessageDTHRechargeAsync);
                    //    break;

                    //case DataCardRechargeOption:
                    //    await context.PostAsync($"Please Enter Data card number to Recharge in the format (Data card Number : Amount)");
                    //    context.Wait(this.MessageDataCardRechargeAsync);
                    //    break;

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
            //await context.PostAsync($"Welcome to cBOT. Type go to start your conversation.");

            //context.Wait(StartAsync);
        }

        private static IList<Attachment> GetCardsAttachments()
        {
            return new List<Attachment>()
            {
                GetThumbnailCard(
                    "Amazon",
                    "Get 22% Off on minimum purchase of Rs 1799",
                    "Exclusive deals in Amazon.in. Get 22% Off on minimum purchase of Rs 1799.",
                    new CardImage(url: "http://cbots.azurewebsites.net/images/amazon.png"),
                    new CardAction(ActionTypes.OpenUrl, "Get this offer", value: "http://amazon.in")),
                GetThumbnailCard(
                    "Flipkart",
                    "Upto 7% Vantage Points on Home Products ",
                    "Happy Shopping Days: The affordable HOME Makeover gives upto 7% Vantage Point. ",
                    new CardImage(url: "http://cbots.azurewebsites.net/images/flipkart.png"),
                    new CardAction(ActionTypes.OpenUrl, "Get this offer", value: "http://flipkart.com")),
                GetThumbnailCard(
                    "INDIALENDS",
                    "Offer on Personal Loan",
                    "Personal Loan Balance Transfer - Get 250 Vantage Credits on Successful Balance Transfer ",
                    new CardImage(url: "http://cbots.azurewebsites.net/images/indialends.png"),
                    new CardAction(ActionTypes.OpenUrl, "Get this offer", value: "http://indialends.com")),
                GetThumbnailCard(
                    "PepperFry",
                    "Rs 1000 Off on Minimum Purchase of Rs 3,500 ",
                    "Exclusive deals in Pepperfry.com. Rs 1000 Off on Minimum Purchase of Rs 3,500 ",
                    new CardImage(url: "http://cbots.azurewebsites.net/images/pepperfry.png"),
                    new CardAction(ActionTypes.OpenUrl, "Get this offer", value: "http://pepperfry.com")),
                GetThumbnailCard(
                    "Main Menu",
                    "cBOT - I am your financial assistant to help you",
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