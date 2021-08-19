using System;

namespace AzureFunctions.Test.Helpers
{
    public class NullScope : IDisposable
    {
        private NullScope()
        {
        }

        public static NullScope Instance => new NullScope();

        public void Dispose()
        {
        }
    }
}
