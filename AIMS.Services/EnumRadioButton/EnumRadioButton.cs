using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace AIMS.Services.EnumRadioButton
{
    public static class EnumRadioButton
    {
        public static MvcHtmlString EnumRadioButtons<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var metaData = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var listOfValues = Enum.GetNames(metaData.ModelType);
            var sb = new StringBuilder();
            if (listOfValues != null)
            {
                sb = sb.AppendFormat("<div>");
                foreach (var name in listOfValues)
                {
                    var label = name;
                    var memInfo = metaData.ModelType.GetMember(name);
                    if (memInfo != null)
                    {
                        var attributes = memInfo[0].GetCustomAttributes(typeof(DisplayAttribute), false);
                        if (attributes != null && attributes.Length > 0) label = ((DisplayAttribute)attributes[0]).Name;
                    }
                    var id = string.Format("{0}_{1}_{2}", htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix, metaData.PropertyName, name);
                    var radio = htmlHelper.RadioButtonFor(expression, name, new
                    {
                        id = id
                    }).ToHtmlString();
                    sb.AppendFormat("<span> <span> <span> </span></span></span> <span> {0}<span> </span>{1}</span>", radio, HttpUtility.HtmlEncode(label));
                }
                sb = sb.AppendFormat("</div>");
            }
            return MvcHtmlString.Create(sb.ToString());
        }
    }
}
