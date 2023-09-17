using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace WMSPortal.Extensions
{
    public static class HtmlExtensions
    {
        public static string ValidationMessage(string message, ModelStateDictionary modelState)
        {
            // Nothing to do if there aren't any errors
            if (modelState.IsValid)
            {
                return null;
            }
            StringBuilder htmlSummary = new StringBuilder();
            string messageSpan;
            if (!String.IsNullOrEmpty(message))
            {
                TagBuilder spanTag = new TagBuilder("span");
                //spanTag.MergeAttributes(htmlAttributes);
                spanTag.MergeAttribute("class", HtmlHelper.ValidationSummaryCssClassName);
                spanTag.SetInnerText(message);
                messageSpan = spanTag.ToString(TagRenderMode.Normal) + Environment.NewLine;
                htmlSummary.AppendLine(message);
            }
            else
            {
                messageSpan = null;
            }


            TagBuilder unorderedList = new TagBuilder("ul");
            //unorderedList.MergeAttributes(htmlAttributes);
            unorderedList.MergeAttribute("class", HtmlHelper.ValidationSummaryCssClassName);
            htmlSummary.AppendLine(unorderedList.ToString(TagRenderMode.StartTag));
            foreach (ModelState states in modelState.Values)
            {
                foreach (ModelError modelError in states.Errors)
                {
                    string errorText = modelError.ErrorMessage;// GetUserErrorMessageOrDefault(htmlHelper.ViewContext.HttpContext, modelError, null /* modelState */);
                    if (!String.IsNullOrEmpty(errorText))
                    {
                        TagBuilder listItem = new TagBuilder("li");
                        listItem.SetInnerText(errorText);
                        htmlSummary.AppendLine(listItem.ToString(TagRenderMode.Normal));
                    }
                }
            }
            htmlSummary.AppendLine(unorderedList.ToString(TagRenderMode.EndTag));
            return htmlSummary.ToString();
        }
        public static MvcHtmlString RadioButtonForEnum<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
                                                                          Expression<Func<TModel, TProperty>> expression)
        {
            return RadioButtonForEnum<TModel, TProperty>(htmlHelper, expression, null);
        }

        public static MvcHtmlString RadioButtonForEnum<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
                                                                          Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            ModelMetadata metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            string[] names = Enum.GetNames(metaData.ModelType);
            var sb = new StringBuilder();

            foreach (string name in names)
            {
                string id = String.Format("{0}_{1}_{2}", htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix,
                                       metaData.PropertyName, name);

                if (htmlAttributes == null)
                {
                    htmlAttributes = new object();
                }
                var routeValueDictionary = new RouteValueDictionary(htmlAttributes);
                routeValueDictionary["id"] = id;

                string radioButton = htmlHelper.RadioButtonFor(expression, name, routeValueDictionary).ToHtmlString();
                sb.AppendFormat("<label for=\"{0}\">{1}</label> {2}", id, HttpUtility.HtmlEncode(name), radioButton);
            }

            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString PartialFor<TModel, TProperty>(this HtmlHelper<TModel> helper, string partialViewName, Expression<Func<TModel, TProperty>> expression)
        {
            string name = ExpressionHelper.GetExpressionText(expression);
            object model = ModelMetadata.FromLambdaExpression(expression, helper.ViewData).Model;
            var viewData = new ViewDataDictionary(helper.ViewData)
            {
                TemplateInfo = new System.Web.Mvc.TemplateInfo
                {
                    HtmlFieldPrefix = name
                }
            };

            return helper.Partial(partialViewName, model, viewData);
        }
    }
}