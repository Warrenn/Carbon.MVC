using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using CarbonKnown.DAL.Models;

namespace CarbonKnown.Calculation
{
    public class CalculationAttribute : Attribute
    {
        public CalculationAttribute(
            string name,
            string calculationId)
        {
            Name = name;
            CalculationId = Guid.Parse(calculationId);
            ActivityIdGuids = new Collection<Guid>();
            FactorIdGuids = new Collection<Guid>();
        }

        public string Activity
        {
            get { return string.Empty; }
            set
            {
                Guid id;
                if (Guid.TryParse(value, out id))
                {
                    ActivityIdGuids.Add(id);
                }
            }
        }

        public Type Activities
        {
            get { return typeof (object); }
            set
            {
                ActivityIdGuids =
                    (from activityField in value.GetFields(BindingFlags.Public | BindingFlags.Static)
                     where
                         (activityField.FieldType == typeof(Guid)) &&
                         (activityField.IsPublic) &&
                         (activityField.IsStatic)
                     select (Guid)activityField.GetValue(null))
                        .ToList();

            }
        }

        public string Factor
        {
            get { return string.Empty; }
            set
            {
                Guid id;
                if (Guid.TryParse(value, out id))
                {
                    FactorIdGuids.Add(id);
                }
            }
        }

        public Type Factors
        {
            get { return typeof (object); }
            set
            {
                FactorIdGuids =
                    (from field in value.GetFields(BindingFlags.Public | BindingFlags.Static)
                     where
                         (field.FieldType == typeof(Guid)) &&
                         (field.IsPublic) &&
                         (field.IsStatic)
                     select (Guid)field.GetValue(null))
                        .ToList();
            }
        }

        public string Name { get; private set; }
        public Guid CalculationId { get; private set; }
        public ICollection<Guid> FactorIdGuids { get; private set; }
        public ICollection<Guid> ActivityIdGuids { get; private set; }
        public ConsumptionType ConsumptionType { get; set; }
    }
}
