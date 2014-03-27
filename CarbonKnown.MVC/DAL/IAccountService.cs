using CarbonKnown.DAL.Models;

namespace CarbonKnown.MVC.DAL
{
    public interface IAccountService
    {
        void UpsertUserProfile(UserProfile user);
        UserProfile GetUser(string name);
    }
}
