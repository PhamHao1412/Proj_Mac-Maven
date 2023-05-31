using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace System.Web.Mvc.Html
{
    public static partial class HtmlHelpers
    {
        /// <summary>
        /// HTML Helper for rendering a DatePicker using 'Bootstrap.Datepicker' NuGet's package 1.3.0
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="self"></param>
        /// <param name="expression">Lambda that will evaluate the object.</param>
        /// <param name="DataAPIParameters">Custom data API parameters to use in the main input.</param>
        /// <returns></returns>
        public static MvcHtmlString DatePickerFor<TModel, TValue>
            (this HtmlHelper<TModel> self, Expression<Func<TModel, TValue>> expression, [Optional] params string[] DataAPIParameters)
        {
            // Get the Metadata from Model's DataAnnotations.
            var metadata = ModelMetadata.FromLambdaExpression(expression, self.ViewData);

            // Main input.
            var input = new TagBuilder("input");

            // Setting attributes.
            input.Attributes.Add("id", metadata.PropertyName);
            input.Attributes.Add("name", metadata.PropertyName);
            input.Attributes.Add("type", "text");
            input.Attributes.Add("data-provide", "datepicker"); // Bootstrap Markup API.
            input.AddCssClass("form-control"); // Bootstrap's 3.1.1 input CSS class.

            // Set Date's format according to the current culture.
            if (Helpers.GetCurrentCulture() != null)
            {
                input.Attributes.Add("data-date-language", Helpers.GetCurrentCulture().ToString());
                input.Attributes.Add("data-date-format", Helpers.GetCurrentCulture().DateTimeFormat.ShortDatePattern.ToLower());
            }

            // If the current Model's value is not null, set the input's value to correspond the call. If it is, set to the current date.
            if (metadata.Model != null)
            {
                input.Attributes.Add("value", ((DateTime)metadata.Model).ToShortDateString());
            }
            else
            {
                input.Attributes.Add("value", DateTime.Now.ToShortDateString());
            }

            // Datepicker API custom parameters specified optionally by the user.
            foreach (var p in DataAPIParameters)
            {
                input.Attributes.Add("data-date-" + p.Split('=')[0], p.Split('=')[1]);
            }

            return new MvcHtmlString(input.ToString());
        }
    }
}