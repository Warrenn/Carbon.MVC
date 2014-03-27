using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CarbonKnown.MVC.Models;

namespace CarbonKnown.MVC.Code
{
    public class DataTableParamModelBinderAttribute : Attribute, IModelBinder
    {
        private static int GetInt(ModelBindingContext context, string prefix, string key)
        {
            int value;
            var stringValue = GetValue(context, prefix, key);
            int.TryParse(stringValue, out value);
            return value;
        }

        private static string GetValue(ModelBindingContext context, string prefix, string key) 
        {
            var result = context.ValueProvider.GetValue(prefix + key);
            return result == null ? string.Empty : result.AttemptedValue;
        }

        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = (DataTableParamModel)bindingContext.Model ?? new DataTableParamModel();

            var hasPrefix = bindingContext
                .ValueProvider
                .ContainsPrefix(bindingContext.ModelName);

            var searchPrefix = (hasPrefix) ? bindingContext.ModelName + "." : string.Empty;
            model.sEcho = GetValue(bindingContext, searchPrefix, "sEcho");
            model.sSearch = GetValue(bindingContext, searchPrefix, "sSearch");
            model.sColumns = GetValue(bindingContext, searchPrefix, "sColumns");
            model.iDisplayLength = GetInt(bindingContext, searchPrefix, "iDisplayLength");
            model.iDisplayStart = GetInt(bindingContext, searchPrefix, "iDisplayStart");
            model.iColumns = GetInt(bindingContext, searchPrefix, "iColumns");
            model.iSortingCols = GetInt(bindingContext, searchPrefix, "iSortingCols");
            var sortColumns = new List<DataTableSortModel>();
            for (var index = 0; index < model.iSortingCols; index++)
            {
                var colIndex = GetInt(bindingContext, searchPrefix, "iSortCol_" + index);
                var ascending = string.Equals(GetValue(bindingContext, searchPrefix, "sSortDir_" + index), "asc",
                                              StringComparison.InvariantCultureIgnoreCase);
                sortColumns.Add(new DataTableSortModel { Ascending = ascending, ColumnId = colIndex });
            }
            model.SortCol = sortColumns;

           return model;
        }
    }
}