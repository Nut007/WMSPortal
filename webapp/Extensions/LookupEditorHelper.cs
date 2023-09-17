using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace WMSPortal.Extensions
{
    public enum LookupEditorType
    {
        None,
        Customer
    }
    public static class LookupEditorHelper
    {
        public static MvcHtmlString LookupEditorFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, string DisplayProperty, string actionUrl, bool? isRequired = false, IDictionary<string, object> viewhtmlAttributes = null, string onselectfunction = "")
        {
            return GetLookupEditorForString(helper, expression, DisplayProperty, LookupEditorType.None, actionUrl, isRequired, viewhtmlAttributes, onselectfunction: onselectfunction);
        }
        public static MvcHtmlString LookupEditorFor<TModel, TValue>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, string DisplayProperty, LookupEditorType lookupEditorType, bool? isRequired = false, IDictionary<string, object> viewhtmlAttributes = null, string onselectfunction = "")
        {
            string actionUrl = string.Empty;
            UrlHelper url = new UrlHelper(helper.ViewContext.RequestContext);
            string acpath = url.Content("~/Customer/");
            if (lookupEditorType == LookupEditorType.Customer)
                actionUrl = acpath + "GetCustomers";


            return GetLookupEditorForString(helper, expression, DisplayProperty, lookupEditorType, actionUrl, isRequired: isRequired, viewhtmlAttributes: viewhtmlAttributes, onselectfunction: onselectfunction);
        }
        private static MvcHtmlString GetLookupEditorForString<TModel, TValue>(HtmlHelper<TModel> helper, Expression<Func<TModel, TValue>> expression, string DisplayText, LookupEditorType lookupEditorTypeType, string actionUrl = "", bool? isRequired = false, IDictionary<string, object> viewhtmlAttributes = null, string onselectfunction = "")
        {
            if (viewhtmlAttributes == null)
                viewhtmlAttributes = new Dictionary<string, object>();

            viewhtmlAttributes.Add("data-autocomplete", true);

            viewhtmlAttributes.Add("data-autocompletetype", lookupEditorTypeType.ToString().ToLower());

            viewhtmlAttributes.Add("data-sourceurl", actionUrl);


            if (!string.IsNullOrEmpty(onselectfunction))
            {
                viewhtmlAttributes.Add("data-electfunction", onselectfunction);
            }
            Func<TModel, TValue> method = expression.Compile();
            object value = null;
            if (helper.ViewData.Model != null)
                value = method((TModel)helper.ViewData.Model);

            string modelpropname = ((MemberExpression)expression.Body).ToString();

            modelpropname = modelpropname.Substring(modelpropname.IndexOf('.') + 1);

            viewhtmlAttributes.Add("data-valuetarget", modelpropname);


            if (isRequired.HasValue && isRequired.Value)
            {
                viewhtmlAttributes.Add("data-val", "true");
                viewhtmlAttributes.Add("data-val-required", modelpropname + " is required");
            }


            MvcHtmlString hidden = helper.HiddenFor(expression);

            MvcHtmlString textBox = helper.TextBox(modelpropname + "_AutoComplete", DisplayText, viewhtmlAttributes);

            var builder = new StringBuilder();

            builder.AppendLine(hidden.ToHtmlString());

            builder.AppendLine(textBox.ToHtmlString());

            return new MvcHtmlString(builder.ToString());
        }
    }
}