using System;
using System.Data.Entity;
using System.Linq;
using CarbonKnown.DAL;
using CarbonKnown.DAL.Models;

namespace CarbonKnown.MVC.DAL
{
    public class AccountService : IAccountService
    {
        private readonly Func<DataContext> dataContextFactory;

        public AccountService()
            : this(() => new DataContext())
        {
        }

        public AccountService(Func<DataContext> dataContextFactory)
        {
            this.dataContextFactory = dataContextFactory;
        }

        public void UpsertUserProfile(UserProfile user)
        {
            using (var dbContext = dataContextFactory())
            {
                var existing = dbContext
                    .UserProfiles
                    .FirstOrDefault(userprofile => userprofile.UserName == user.UserName);
                if (existing == null)
                {
                    existing = dbContext.UserProfiles.Create();
                }
                else
                {
                    dbContext.SetState(existing, EntityState.Modified);
                }
                existing.Email = user.Email;
                existing.FirstName = user.FirstName;
                existing.LastName = user.LastName;
                existing.UserName = user.UserName;

                dbContext.SaveChanges();
            }
        }

        public UserProfile GetUser(string name)
        {
            using (var dbContext = dataContextFactory())
            {
                var existing = dbContext
                    .UserProfiles
                    .FirstOrDefault(userprofile => userprofile.UserName == name);
                return existing;
            }
        }
    }
}