using System;

namespace CarbonKnown.DAL.Models
{
    [Flags]
    public enum ConsumptionType 
    {
        Water = 0x0001,
        Electricity = 0x0002,
        Paper = 0x0004,
        CarHire = 0x0008,
        HotelNights = 0x0010,
        AirTravel = 0x0020,
        Waste = 0x0040,
        Courier = 0x0080,
        Other = 0x0100
    }
}
