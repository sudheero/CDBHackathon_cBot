using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis;
using System.Linq;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Net.Http;

namespace CDBHackathon_cBot.Dialogs
{
    [LuisModel("7f4d2dc9-a54b-46e1-8557-859f5dd4a546", "846edaa4a52c4286b15fcfc11e19ad40", LuisApiVersion.V2)]
    [Serializable]
    public class LUISDialogBox : LuisDialog<object>
    {
        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task CasualTalk(IDialogContext context, IAwaitable<IMessageActivity> activity, Microsoft.Bot.Builder.Luis.Models.LuisResult result)
        {
            var msg = await activity;
            await context.PostAsync($"Sorry I didn't understand your text. I have noted your text, I will get trained with this text soon.");
        }
    }
}