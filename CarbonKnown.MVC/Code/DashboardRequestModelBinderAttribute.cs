using System;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using CarbonKnown.FileReaders;
using CarbonKnown.MVC.Models;

namespace CarbonKnown.MVC.Code
{
    public class DashboardRequestModelBinderAttribute : Attribute, IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var model = (DashboardRequest) bindingContext.Model ?? new DashboardRequest();

            var hasPrefix = bindingContext
                .ValueProvider
                .ContainsPrefix(bindingContext.ModelName);

            var searchPrefix = (hasPrefix) ? bindingContext.ModelName + "." : string.Empty;

            Dimension dimension;
            Section section;

            Enum.TryParse(actionContext.ActionDescriptor.ActionName, out dimension);
            Enum.TryParse(GetValue(bindingContext, searchPrefix, "Section"), out section);

            model.ActivityGroupId =
                TryParser.Nullable<Guid>(GetValue(bindingContext, searchPrefix, "ActivityGroupId"));
            model.CostCode = GetValue(bindingContext, searchPrefix, "CostCode");
            model.StartDate = GetDateTime(bindingContext, searchPrefix, "StartDate");
            model.EndDate = GetDateTime(bindingContext, searchPrefix, "EndDate");
            model.Dimension = dimension;
            model.Section = section;

            bindingContext.Model = model;

            return true;
        }

        private static DateTime GetDateTime(ModelBindingContext context, string prefix, string key)
        {
            var dateValue = GetValue(context, prefix, key);
            dateValue = dateValue.Replace("\"", string.Empty);
            dateValue = dateValue.Split('T')[0];
            return TryParser.DateTime(dateValue) ?? DateTime.Today;
        }

        private static string GetValue(ModelBindingContext context, string prefix, string key) 
        {
            var result = context.ValueProvider.GetValue(prefix + key);
            return result == null ? string.Empty : result.AttemptedValue;
        }
    }
}