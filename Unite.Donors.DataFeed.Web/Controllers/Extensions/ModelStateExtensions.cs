using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Unite.Donors.DataFeed.Web.Controllers.Extensions
{
    public static class ModelStateExtensions
    {
        public static bool IsValid(this ModelStateDictionary modelState, out string errorMessage)
        {
            if (modelState.IsValid)
            {
                errorMessage = null;

                return true;
            }
            else
            {
                errorMessage = GetModelStateErrorMessage(modelState);

                return false;
            }
        }

        private static string GetModelStateErrorMessage(ModelStateDictionary modelState)
        {
            var message = new StringBuilder();

            var errors = modelState
                .Where(field => field.Value.Errors.Any())
                .Select(field => new Error(field.Key, field.Value.Errors.Select(error => error.ErrorMessage)));

            message.AppendLine("Request data has invalid format");

            foreach (var error in errors)
            {
                message.AppendLine(error.ToString());
            }

            return message.ToString();
        }
    }

    internal class Error
    {
        public string Field { get; set; }
        public string[] Errors { get; set; }

        public Error(string field, IEnumerable<string> errors)
        {
            Field = field;
            Errors = errors.ToArray();
        }

        public override string ToString()
        {
            return $"'{Field}' - {string.Join(", ", Errors)}";
        }
    }
}
