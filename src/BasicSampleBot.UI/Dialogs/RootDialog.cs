using BasicMultiDialogBot.Dialogs;
using BasicSampleBot.BusinessCore;
using BasicSampleBot.Forms;
using BasicSampleBot.Services;
using BasicSampleBot.UI;
using BasicSampleBot.UI.Services;
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
    // [LuisModel("a071206e-8b23-46e5-8191-5820115fae33", "7cb588afefbd4130b8cb059a78634c4c")]
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
            Trace.WriteLine("Hello world");
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
            // string message = $"Sorry, I did not understand '{}'. Type 'help' if you need assistance.";

            await SendMessageWithDelay($"Luis ei ymmärtänyt lausettasi: { result.Query }", context);
            await SendMessageWithDelay($"Haetaan hyviä osumia asiakas- ja tuote-tietokannoista...", context);

            var query = result.Query.ToLower();     
            var cResults = await SearchData.CustomerLookup(query);
            var pResults = await SearchData.ProductLookup(query);

            if (cResults.Count() > 0) { await PresentCustomerResults(cResults, context); }
            else if (pResults.Count() > 0) { await PresentProductResults(pResults, context); }
            else { await SendMessageWithDelay($"Valitan, ei tuloksia.", context); }



            context.Wait(this.MessageReceived);
        }

        async Task PresentCustomerResults(IEnumerable<CustomerSearchDto> customers, IDialogContext context)
        {
            switch (customers.Count()) {
                case 0:
                    await SendMessageWithDelay("Emme löytäneet henkilöä joka vastaa hakuasi", context);
                    return;                
                case 1:
                    await SendMessageWithDelay("Löysimme yhden osuman", context);
                    break;
                default:
                    await SendMessageWithDelay($"Löysimme { customers.Count() } osumaa", context);
                    break;
            } 

            int idx = 1;
            foreach (CustomerSearchDto c in customers)
            {
                string message = $"{ c.Title} { c.Name} työskentelee yrityksessä { c.CompanyName }. Puhelinnumero: { c.Phone }";
                await SendMessageWithDelay(message, context);
                idx++;
            }
        }

        async Task PresentProductResults(IEnumerable<ProductSearchDto> products, IDialogContext context)
        {
            switch (products.Count())
            {
                case 0:
                    await SendMessageWithDelay("Emme löytäneet henkilöä joka vastaa hakuasi", context);
                    return;
                case 1:
                    await SendMessageWithDelay("Löysimme yhden osuman", context);
                    break;
                default:
                    await SendMessageWithDelay($"Löysimme { products.Count() } osumaa", context);
                    break;
            }

            int idx = 1;
            foreach (var p in products)
            {
                string message = $"{ p.Name }, hinta hyllyllä: { p.ListPrice }. Kategoria [{ p.Category }]";
                await SendMessageWithDelay(message, context);
                idx++;
            }
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            // await context.PostAsync("Hi! Try asking me things like 'search hotels in Seattle', 'search hotels near LAX airport' or 'show me the reviews of The Bot Resort'");
            await SendMessageWithDelay("Terve!", context);

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

        string TryGetCustomerNameFromQuery(string message)
        {
            var crmQueryStarts = new string[] {
                "look up person ",
                "who is ",
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
            // await SendMessageWithDelay("Haet t.", context);
            string customerName = "";

            if (result.TryFindEntity(EntityPerson, out EntityRecommendation personEntityRecommendation))
            {
                customerName = personEntityRecommendation.Entity;
                await SendMessageWithDelay($"Luulen, että haet asiakastietoja henkilöstä **{ customerName }** (nlu). Haetaan...", context);
                var searchResult = await SearchData.CustomerLookup(customerName);
                await PresentCustomerResults(searchResult, context);
                context.Wait(MessageReceived);
                return;
            }
 
            customerName = TryGetCustomerNameFromQuery(result.Query.ToLower());

            if (string.IsNullOrEmpty(customerName))
            {
                await SendMessageWithDelay($"En ymmärtänyt viestiäsi...", context);
                await SendMessageWithDelay($"Pullautanpa ohjeet vielä...", context);
            }
            else
            {
                await SendMessageWithDelay($"Luulen, että haet asiakastietoja henkilöstä **{ customerName }** (brute). Haetaan...", context);
                var searchResult = await SearchData.CustomerLookup(customerName);
                await PresentCustomerResults(searchResult, context);
                context.Wait(MessageReceived);
            }     
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