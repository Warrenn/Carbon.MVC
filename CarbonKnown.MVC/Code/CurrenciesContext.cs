using System.Collections.Generic;
using System.Globalization;

namespace CarbonKnown.MVC.Code
{
    public static class CurrenciesContext
    {
        public static IDictionary<string, CultureInfo> Cultures;
        public static IDictionary<string, RegionInfo> Regions;

        static CurrenciesContext()
        {
            Cultures = new SortedDictionary<string, CultureInfo>();
            Regions = new SortedDictionary<string, RegionInfo>();

            foreach (var culture in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                if (culture.IsNeutralCulture) continue;
                
                var region = new RegionInfo(culture.LCID);
                Cultures[region.ISOCurrencySymbol] = culture;
                Regions[region.ISOCurrencySymbol] = region;
            }
        }
    }
}