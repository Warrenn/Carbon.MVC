using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;

namespace CarbonKnown.MVC.Models
{
    public class DataSelectionModel
    {
        private DataSelectionModel()
        {

        }

        public int StepNumber { get; set; }
        public string Label { get; set; }
        public bool CanEdit { get; set; }
        public int? MinInputLength { get; set; }
        public string JsonData { get; private set; }
        public string Name { get; private set; }
        public string InitialValue { get; private set; }
        public bool SourceIdDependant { get; set; }

        public static DataSelectionModel CreateFromEnum<TModel, TEnum>(
            TModel instance,
            Expression<Func<TModel, TEnum?>> expression,
            IDictionary<TEnum, string> enumValues,
            IDictionary<TEnum, object> enumExtension = null) 
            where TEnum : struct 
            where TModel: class 
        {
            var model = new DataSelectionModel();
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                var unaryExpression  = expression.Body as UnaryExpression;
                if (unaryExpression != null)
                {
                    memberExpression = unaryExpression.Operand as MemberExpression;
                }
            }
            if (memberExpression == null) throw new ArgumentException("Expression body must be a MemberExpression");
            model.Name = memberExpression.Member.Name;
            var memberFunction = expression.Compile();
            var initialValue = ((instance == null) ? null : memberFunction(instance)) ?? default(TEnum);
            var enumType = typeof (TEnum);
            Func<TEnum,string> valueFunction = v => (enumType.IsEnum)
                                     ? Enum.GetName(enumType, v)
                                     : v.ToString();
            model.InitialValue = valueFunction(initialValue);
            model.JsonData =
                JsonConvert.SerializeObject(
                    enumValues.Select(k => new
                        {
                            id = valueFunction(k.Key),
                            text = k.Value,
                            value = (enumExtension == null)
                                        ? null
                                        : enumExtension[k.Key]
                        }));
            return model;
        }
    }
}