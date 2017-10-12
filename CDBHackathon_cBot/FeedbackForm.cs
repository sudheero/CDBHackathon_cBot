using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CDBHackathon_cBot
{
    public enum Category { UserExperience=1, Services, Others};
    public enum Rating { Horrible=1, Bad, Ok , Good, Awesome};

    [Serializable]
    public class FeedbackForm
    {
        [Prompt("Please select any of the below Category  {||}")]
        public Category CategoryName { get; set; }

        [Prompt("What are the areas you liked here? {||}")]
        public string Like { get; set; }

        [Prompt("What are the areas you feel that could be improved? {||}")]
        public string Improved { get; set; }

        [Prompt("Please rate your overall experience: {||}")]
        public Rating Rate { get; set; }

        public static IForm<FeedbackForm> BuildForm()
        {
            // Builds an IForm<T> based on BasicForm
            return new FormBuilder<FeedbackForm>().Build();
        }

        public static IFormDialog<FeedbackForm> BuildFormDialog(FormOptions options = FormOptions.PromptInStart)
        {
            // Generate a new FormDialog<T> based on IForm<BasicForm>
            return FormDialog.FromForm(BuildForm, options);
        }
    }
}