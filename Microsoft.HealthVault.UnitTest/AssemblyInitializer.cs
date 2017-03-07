using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.HealthVault.UnitTest
{
    /// <summary>
    /// Code that runs once to initialize the test assembly.
    /// </summary>
    [TestClass]
    public static class AssemblyInitializer
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            // TODO: This is where we can set up assembly and IoC initialization.

        }
    }
}