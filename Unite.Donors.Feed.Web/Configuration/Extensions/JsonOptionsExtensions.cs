using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Unite.Donors.Feed.Web.Configuration.Extensions
{
    public static class JsonOptionsExtensions
    {
        public static void AddJsonOptions(this JsonOptions options)
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumMemberConverter());
            options.JsonSerializerOptions.IgnoreNullValues = true;
        }
    }
}
