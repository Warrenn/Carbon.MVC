namespace CarbonKnown.DAL.Models.CarHire
{
    public enum CarGroupBill
    {
        /// <summary>
        /// A - Economy  &lt; 1.4 l 
        /// </summary>
        A,

        /// <summary>
        /// B - Compact 1.4 - 2 l (Petrol)
        /// </summary>
        B,

        /// <summary>
        /// C - Intermediate 1.6 l (Petrol) 
        /// </summary>
        C,

        /// <summary>
        /// D - Intermediate 1.6 l (Petrol)
        /// </summary>
        D,

        /// <summary>
        /// E - Standard 2.0 - 2.4 l (Petrol)
        /// </summary>
        E,

        /// <summary>
        /// F - Full Size 1.8 - 2 l (Petrol)
        /// </summary>
        F,

        /// <summary>
        /// G - Premium  1.8 - 2 l (Petrol)
        /// </summary>
        G,

        /// <summary>
        /// H - Hybrid HYB
        /// </summary>
        H,

        /// <summary>
        /// I - Compact  (Petrol)
        /// </summary>
        I,

        /// <summary>
        /// J - Luxury 2.3 - 2.5 l (Petrol)
        /// </summary>
        J,

        /// <summary>
        /// K - Speciality SUV 2.4 - 2.5 l (Petrol)
        /// </summary>
        K,

        /// <summary>
        /// L - Speciality Leisure 4X4  3 l 
        /// </summary>
        L,

        /// <summary>
        /// M - Economy 1.1 - 1.4 (Petrol)
        /// </summary>
        M,

        /// <summary>
        /// N - Speciality People Carrier 
        /// </summary>
        N,

        /// <summary>
        /// O - Full Size 1.8 - 2 l (Petrol)
        /// </summary>
        O,

        /// <summary>
        /// P - Full Size 1.4 l (Petrol)
        /// </summary>
        P,

        /// <summary>
        /// Average Petrol (If not known)
        /// </summary>
        AveragePetrol,

        /// <summary>
        /// Greater Than 2.0 l petrol
        /// </summary>
        GreaterThan2LPetrol,

        /// <summary>
        /// Less than 1.4 l petrol
        /// </summary>
        LessThan14LPetrol,

        /// <summary>
        /// 1.7  - 2.0 l diesel
        /// </summary>
        Diesel17To2L,

        /// <summary>
        /// Less than 1.7 l diesel 
        /// </summary>
        LessThan17Diesel,

        /// <summary>
        /// Greater than 2.0 l diesel 
        /// </summary>
        GreaterThan2LDiesel,

        /// <summary>
        /// Greater than 500cc
        /// </summary>
        GreaterThan500Cc,

        /// <summary>
        /// Average Diesel (capacity not known)
        /// </summary>
        AverageDiesel
    }
}
