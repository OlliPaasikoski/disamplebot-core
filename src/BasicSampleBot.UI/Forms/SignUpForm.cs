using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BasicSampleBot.Forms
{
    [Serializable]
    public class SignupForm
    {   
        public string EmailAddress { get; set; }
        public string Title { get; set; }

        private const string EmailRegExPattern = @"^[a-zA-Z0-9.!#$%&'*+\/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$";

        /// <summary>
        /// Email address validator
        /// </summary>
        private static ValidateAsyncDelegate<SignupForm> EmailValidator = async (state, response) =>
        {
            var result = new ValidateResult { IsValid = true, Value = response };
            var email = (response as string).Trim();
            if (!Regex.IsMatch(email, EmailRegExPattern))
            {
                result.Feedback = "Sorry, that doesn't look like a valid email address.";
                result.IsValid = false;
            }

            return await Task.FromResult(result);
        };

        /// <summary>
        /// Builds the Signup form.
        /// </summary>
        /// <returns>An instance of the <see cref="SignupForm"/> form flow.</returns>
        public static IForm<SignupForm> BuildForm()
        {
            return new FormBuilder<SignupForm>()
                .Field(nameof(SignupForm.EmailAddress), validate: EmailValidator)
                .Field(nameof(SignupForm.Title))
                .Build();
        }

    }

}
