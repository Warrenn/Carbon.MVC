namespace CarbonKnown.MVC.Code
{
    public interface IEmailManager
    {
        void SendMail<T>(T model, string template, params string[] addresses);
    }
}
