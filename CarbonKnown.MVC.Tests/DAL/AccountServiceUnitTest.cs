using System.Data.Entity;
using System.Data.Entity.SqlServer;
using CarbonKnown.DAL;
using CarbonKnown.DAL.Models;
using CarbonKnown.MVC.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CarbonKnown.MVC.Tests.DAL
{
    [TestClass]
    public class AccountServiceUnitTest
    {
        public AccountServiceUnitTest()
        {
            var type = typeof(SqlProviderServices);
        }

        [TestMethod]
        public void MustFindMatchingUserViaUserName()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var userId = 3;
            var userProfileDbSet = new FakeDbSet<UserProfile>(new[]
                {
                    new UserProfile {UserId = 4, UserName = "notToBeReturned"},
                    new UserProfile {UserId = userId, UserName = "username"}
                });
            mockContext
                .Setup(context => context.UserProfiles)
                .Returns(userProfileDbSet);

            var sut = new AccountService(() => mockContext.Object);
            var upsertUserProfile = new UserProfile {UserName = "username"};
           
            //Act
            sut.UpsertUserProfile(upsertUserProfile);

            //Assert
            mockContext
                .Verify(context => context.SetState(
                    It.Is<UserProfile>(profile => profile.UserId == userId),
                    It.IsAny<EntityState>()));
        }

        [TestMethod]
        public void TheFoundUserProfileMustSetTheStateToModified()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var userProfileDbSet = new FakeDbSet<UserProfile>(new[]
                {
                    new UserProfile {UserName = "username"}
                });
            mockContext
                .Setup(context => context.UserProfiles)
                .Returns(userProfileDbSet);

            var sut = new AccountService(() => mockContext.Object);
            var upsertUserProfile = new UserProfile {UserName = "username"};

            //Act
            sut.UpsertUserProfile(upsertUserProfile);

            //Assert
            mockContext
                .Verify(context => context.SetState(
                    It.IsAny<UserProfile>(),
                    It.Is<EntityState>(state => state == EntityState.Modified)));
        }

        [TestMethod]
        public void UserProfileEmailMustBeSet()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var actualProfile = new UserProfile {UserName = "username"};
            var userProfileDbSet = new FakeDbSet<UserProfile>(new[]
                {
                    actualProfile
                });
            mockContext
                .Setup(context => context.UserProfiles)
                .Returns(userProfileDbSet);

            var sut = new AccountService(() => mockContext.Object);
            var upsertUserProfile = new UserProfile
                {
                    UserName = "username",
                    Email = "testemail"
                };

            //Act
            sut.UpsertUserProfile(upsertUserProfile);

            //Assert
            Assert.AreEqual("testemail", actualProfile.Email);
        }

        [TestMethod]
        public void UserProfileFirstNameMustBeSet()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var actualProfile = new UserProfile { UserName = "username" };
            var userProfileDbSet = new FakeDbSet<UserProfile>(new[]
                {
                    actualProfile
                });
            mockContext
                .Setup(context => context.UserProfiles)
                .Returns(userProfileDbSet);

            var sut = new AccountService(() => mockContext.Object);
            var upsertUserProfile = new UserProfile
            {
                UserName = "username",
                FirstName = "firstname"
            };

            //Act
            sut.UpsertUserProfile(upsertUserProfile);

            //Assert
            Assert.AreEqual("firstname", actualProfile.FirstName);
        }

        [TestMethod]
        public void UserProfileLastNameMustBeSet()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var actualProfile = new UserProfile { UserName = "username" };
            var userProfileDbSet = new FakeDbSet<UserProfile>(new[]
                {
                    actualProfile
                });
            mockContext
                .Setup(context => context.UserProfiles)
                .Returns(userProfileDbSet);

            var sut = new AccountService(() => mockContext.Object);
            var upsertUserProfile = new UserProfile
            {
                UserName = "username",
                LastName = "lastname"
            };

            //Act
            sut.UpsertUserProfile(upsertUserProfile);

            //Assert
            Assert.AreEqual("lastname", actualProfile.LastName);
        }

        [TestMethod]
        public void NewUserProfileUserNameMustBeSet()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var actualProfile = new UserProfile();
            var userProfileDbSet = new Mock<FakeDbSet<UserProfile>>();
            userProfileDbSet
                .Setup(helper => helper.Create())
                .Returns(actualProfile);
            mockContext
                .Setup(context => context.UserProfiles)
                .Returns(userProfileDbSet.Object);

            var sut = new AccountService(() => mockContext.Object);
            var upsertUserProfile = new UserProfile
            {
                UserName = "username",
            };

            //Act
            sut.UpsertUserProfile(upsertUserProfile);

            //Assert
            Assert.AreEqual("username", actualProfile.UserName);
        }

        [TestMethod]
        public void SaveChangesMustBeCalled()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var userProfileDbSet = new FakeDbSet<UserProfile>(new[]
                {
                    new UserProfile{UserName = "username"}
                });

            mockContext
                .Setup(context => context.UserProfiles)
                .Returns(userProfileDbSet);

            var sut = new AccountService(() => mockContext.Object);
            var upsertUserProfile = new UserProfile
            {
                UserName = "username",
            };

            //Act
            sut.UpsertUserProfile(upsertUserProfile);

            //Assert
            mockContext.Verify(context => context.SaveChanges());
        }

        [TestMethod]
        public void GetUserMustReturnMatchingUserName()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var userProfile = new UserProfile
                {
                    UserId = 3,
                    UserName = "username"
                };
            var userProfileDbSet = new FakeDbSet<UserProfile>(new[]
                {
                    userProfile,
                    new UserProfile {UserId = 4, UserName = "notToBeReturned"}
                });
            mockContext
                .Setup(context => context.UserProfiles)
                .Returns(userProfileDbSet);

            var sut = new AccountService(() => mockContext.Object);

            //Act
            var match = sut.GetUser("username");

            //Assert
            Assert.AreEqual(3, match.UserId);
        }

        [TestMethod]
        public void GetUserMustReturnNullThereIsntAMatch()
        {
            //Arrange
            var mockContext = new Mock<DataContext>();
            var userProfileDbSet = new FakeDbSet<UserProfile>();
            mockContext
                .Setup(context => context.UserProfiles)
                .Returns(userProfileDbSet);

            var sut = new AccountService(() => mockContext.Object);

            //Act
            var match = sut.GetUser("username");

            //Assert
            Assert.IsNull(match);
        }
    }
}
