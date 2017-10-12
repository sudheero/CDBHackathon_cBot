using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace CDBHackathon_cBot.Dialogs
{
    [Serializable]
    public class ProactiveOffersDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Flipkart Big Billion Day exclusive offer for you. Buy one iPhone 7 and get one iPhone 7 free. Type \"cBOT\" to resume");

            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            if ((await result).Text == "cBOT")
            {
                await context.PostAsync("Great, back to cBOT. Type \"view\" to view your accounts");
                context.Done(String.Empty); //Finish this dialog
            }
            else
            {
                await context.PostAsync("I will show this offer 'Flipkart Big Billion Day exclusive offer for you. Buy one iPhone 7 and get one iPhone 7 free' until you type \"cBOT\"");
                context.Wait(MessageReceivedAsync); //Not done yet
            }
        }
    }
}