using System;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace CarbonKnown.FileReaders
{
    public static class DisposableExtension
    {
        public static void Dispose(this IDisposable disposable, string exceptionPolicy)
        {
            if (disposable == null) return;
            try
            {
                disposable.Dispose();
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, exceptionPolicy);
            }
        }
    }
}
