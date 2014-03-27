using System;
using CarbonKnown.DAL.Models;

namespace CarbonKnown.MVC.Models
{
    public class InputHistoryDataModel
    {
            public string Name { get; set; }
            public DateTime EditDate { get; set; }
            public string UserName { get; set; }
            public string Type { get; set; }
            public SourceStatus Status { get; set; }
            public Guid Id { get; set; }
    }
}