using System;

namespace CarbonKnown.MVC.Models
{
    // ReSharper disable InconsistentNaming
    public class CrumbNode
    {
        public CrumbNode()
        {

        }

        public CrumbNode(string contents, Guid? activityGroupId = null, string costCode = null)
        {
            this.contents = contents;
            data = new CrumbData
                {
                    activityGroupId = activityGroupId,
                    costCode = costCode
                };
        }

        public string contents { get; set; }
        public CrumbData data { get; set; }
    }

    // ReSharper restore InconsistentNaming
}