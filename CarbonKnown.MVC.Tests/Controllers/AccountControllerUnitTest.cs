using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CarbonKnown.MVC.Tests.Controllers
{
    [TestClass]
    public class AccountControllerUnitTest
    {
        [TestMethod]
        public void LoginGetSetsTheBiewBagReturnUrl()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void LoginGetGoesToTheLoginView()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void LoginPostCallsControllerLogin()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void LoginPostRedirectsIfUrlIsLocal()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void LoginPostRedirectsToActionIfUrlIsNotLocal()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void LoginPostAddsModelErrorIfLoginFails()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void LoginPostGoesToLoginViewIfLoginFails()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void LoginPostFailsIfModelstateIsInvalid()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void LogOffCallsControllerLogOff()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void LogOffGoesToLoginAction()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void RegisterGoesToRegisterView()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void RegisterPostCallsCreateUserAndAccount()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void RegisterPostCallsLogin()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void RegisterPostCallsUpsertUserProfile()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void RegisterPostSuccessGoesToIndexDashboard()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void RegisterPostExceptionFindsResourceFromStatusCode()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void RegisterPostExceptionShowsModelError()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void RegisterPostWithModelErrorDoesNotCallCreateUserLoginAndUpsert()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ManageGetsUserFromUserName()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ManageSetsModelFromAccountServiceGetUser()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ManageShowsManageView()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ManagePostSetsTheReturnUrlToManageAction()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ManagePostCallsServiceUpsertWithModelData()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ManagePostCallsLoginBeforeCallingChangePassword()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ManagePostCallsChangePasswordOnlyIfLoginSucceeds()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ManagePostAddsModelErrorIfLoginFails()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ManagePostAddsModelErrorIfWebsecurityThrowsAnException()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ManagePostSetsStatusMessageToFailureIfThereIsAnError()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ManagePostSetsStatusMessageToSuccessIfItSucceeds()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void ManagePostShowsManageView()
        {
            Assert.Inconclusive();
        }
    }
}
