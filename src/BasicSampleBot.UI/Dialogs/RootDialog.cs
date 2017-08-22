using BasicMultiDialogBot.Dialogs;
using BasicSampleBot.BusinessCore;
using BasicSampleBot.Forms;
using BasicSampleBot.Services;
using BasicSampleBot.UI;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace BasicSampleBot.Dialogs
{
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {
        private const int DelayBetweenMessages = 2000; // in ms
        private string _name;

        private const string EntityCRMActivity = "activity_log";
        private const string EntityPerson = "person";

        //private static ICustomerService CustomerData = ServiceFactory.CustomerData;
        private static ISearchService SearchData = ServiceFactory.SearchData;

        public RootDialog() : base(new LuisService(new LuisModelAttribute("a071206e-8b23-46e5-8191-5820115fae33", "7cb588afefbd4130b8cb059a78634c4c")))
        {


        }

        public static async Task SendMessageWithDelay(string message, IDialogContext context)
        {
            try
            {
                Thread.Sleep(DelayBetweenMessages);
            }
            catch (ThreadInterruptedException e)
            {
                Trace.WriteLine($"SendMessageWithDelay encountered a ThreadInterruptedException.");
                Trace.WriteLine(e);
            }
            await context.PostAsync(message);
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            // string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance.";

            string message = $"Empty or none intent established, turning over to search api";
            await SendMessageWithDelay(message, context);

            var queryToLower = result.Query.ToLower();     
            var results = await SearchData.GetSearchResults(queryToLower);

            await PresentSearchResults(results, context);

            context.Wait(this.MessageReceived);
        }

        async Task PresentSearchResults(IEnumerable<CRMSearchDto> results, IDialogContext context)
        {
            int idx = 1;
            foreach (CRMSearchDto searchResult in results)
            {
                string message = $"Result { idx }: {searchResult.Name}, { searchResult.Description }. Category { searchResult.Category }";
                await SendMessageWithDelay(message, context);
                idx++;
            }
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await SendMessageWithDelay("Greeting() entered", context);
            // await context.PostAsync("Hi! Try asking me things like 'search hotels in Seattle', 'search hotels near LAX airport' or 'show me the reviews of The Bot Resort'");
            await SendMessageWithDelay("Welcome, I am the the DI Sample Bot.", context);
            await SendMessageWithDelay("I'm a very basic bot. It's really nice to meet you!", context);

            // await SendMessageWithDelay("I would like to know your name. Let us traverse to the Name-dialogue.....", context);
            // context.Call(new NameDialog(), this.NameDialogResumeAfter);
            context.Wait(this.MessageReceived);
        }



        [LuisIntent("SignUp")] 
        public async Task SignUp(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await context.PostAsync("Great! I just need a few pieces of information to get you signed up.");

            var form = new FormDialog<SignupForm>(
                new SignupForm(),
                SignupForm.BuildForm,
                FormOptions.PromptInStart,
                PreprocessEntities(result.Entities));

            context.Call<SignupForm>(form, SignUpComplete);
        }

        /// <summary>
        /// Handles when the SignupForm is complete.
        /// </summary>
        /// <param name="context">The dialog context</param>
        /// <param name="result">The completed form</param>
        /// <returns></returns>
        private async Task SignUpComplete(IDialogContext context, IAwaitable<SignupForm> result)
        {
            SignupForm form = null;
            try
            {
                form = await result;
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
                // Here is where we could call our signup service here to complete the sign-up

                var message = $"Thanks! We signed up **{form.EmailAddress}** with title **{form.Title}**.";
                await context.PostAsync(message);
            }

            context.Wait(MessageReceived);
        }

        string GetCRMQueryTermFromUserMessage(string message)
        {
            var crmQueryStarts = new string[] {
                "tell me about ",
                "look up ",
                "show me information on ",
                "who is ",
                "find company ",
                "customer ",
                "crm "
            };

            try
            {
                var queryStart = crmQueryStarts.Where(q => message.StartsWith(q)).FirstOrDefault();
                return message.Substring(message.IndexOf(queryStart) + queryStart.Length);
            }
            catch (Exception ex)
            {
                // log the issue!
                Trace.TraceError(ex.Message, ex);
            }

            return string.Empty;
        }

        

        [LuisIntent("CRMEntityLookup")]
        public async Task CRMEntityLookup(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await SendMessageWithDelay("Thanks. You entered the CRM lookup dialog.", context);
            var crmEntityInferredName = "";

            if (result.TryFindEntity(EntityPerson, out EntityRecommendation personEntityRecommendation))
            {
                crmEntityInferredName = personEntityRecommendation.Entity;
                await SendMessageWithDelay($"I think you are looking for information about **{ crmEntityInferredName }**.", context);           
            }

            var queryToLower = result.Query.ToLower();
            string crmQuery = "";

            if (result.Entities.Count == 0)
            {
                await SendMessageWithDelay($"The NLU system could not infer the company or person name from the query.", context);
                await SendMessageWithDelay($"Evaluating intent based on exact wording.", context);

                crmQuery = GetCRMQueryTermFromUserMessage(queryToLower);

                if (!string.IsNullOrEmpty(crmQuery))
                {
                    await SendMessageWithDelay($"Based on how you worded your query, you are looking for **{ crmEntityInferredName }**", context);
                }
                else
                {
                    await SendMessageWithDelay($"Unfortunately, your message was effectively incomprehensible!", context);
                    await SendMessageWithDelay($"Reverting back to helper dialogue... (to do)", context);
                }
            }

            var searchResult = await SearchData.GetSearchResults(crmQuery);
            await PresentSearchResults(searchResult, context);


            // Direct SQL Server integration...

            //Customer customer = null;

            //if (!string.IsNullOrEmpty(crmEntityInferredName))
            //{
            //    string[] temp = crmEntityInferredName.Split(' ');
            //    customer = CustomerData.GetCustomerByName(temp[0], temp[1]);

            //    if (customer != null)
            //    {
            //        await SendMessageWithDelay($"We queried the CRM database and found **{customer.Title} {customer.FirstName} {customer.LastName}** at **{customer.CompanyName}**.", context);
            //    }
            //    else
            //    {
            //        await SendMessageWithDelay($"We queried the CRM database and failed to find a customer.. Falling back to external services.", context);
            //    }
            //}
            //else
            //{
            //    await SendMessageWithDelay($"Sorry, we could not resolve the person's name from your query.", context);

            //}

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("AddCRMActivity")]
        public async Task AddCRMActivity(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await SendMessageWithDelay("AddCRMActivity() entered", context);

            if (result.TryFindEntity(EntityCRMActivity, out EntityRecommendation activityEntityRecommendation))
            {
                await SendMessageWithDelay($"I think you are looking for a **{ activityEntityRecommendation.Entity }** activity.", context);
                await SendMessageWithDelay($"Next, I will try to infer the type of operation you want to perform on the activity.", context);
            }

            context.Wait(this.MessageReceived);
        }

        private async Task SendWelcomeMessageAsync(IDialogContext context)
        {
            await SendMessageWithDelay("Welcome, I am the the DI Sample Bot.", context);
            await SendMessageWithDelay("I'm a very basic bot. It's really nice to meet you!", context);

            await SendMessageWithDelay("I would like to know your name. Let us traverse to the Name-dialogue.....", context);
            context.Call(new NameDialog(), this.NameDialogResumeAfter);
        }

        private async Task NameDialogResumeAfter(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                this._name = await result;
                await context.PostAsync($"Yay! Through the Name-dialog, we have established that your name is: **{_name}**");
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("Oh noes. A 'TooManyAttempsException' was hit during the Name-dialog.");
            }
            finally
            {
                // await this.SendWelcomeMessageAsync(context);
            }
        }

        /// <summary>
        /// Preprocesses any entities sent from LUIS parsing.
        /// </summary>
        /// <param name="entities">The entities to processed</param>
        /// <returns>The processed entities</returns>
        private IList<EntityRecommendation> PreprocessEntities(IList<EntityRecommendation> entities)
        {
            // remove spaces from email address
            var emailEntity = entities.Where(e => e.Type == "EmailAddress").FirstOrDefault();
            if (emailEntity != null)
            {
                emailEntity.Entity = Regex.Replace(emailEntity.Entity, @"\s+", string.Empty);
            }
            return entities;
        }
    }
}