using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace CDBHackathon_cBot.Dialogs
{
    [Serializable]
    public class SupportDialog : IDialog<int>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync($"Please enter the ticket description:");
            context.Wait(this.MessageReceivedAsync);
        }

        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            var ticketNumber = new Random().Next(0, 20000);

            await context.PostAsync($"Your message '{message.Text}' was registered. Once we resolve it; we will get back to you.");

            context.Done(ticketNumber);
        }
    }
}