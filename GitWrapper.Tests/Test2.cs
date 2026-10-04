using Chris82111.GitManager.GitWrapper.Core.Helpers;
using System.Runtime.InteropServices;

namespace Chris82111.GitManager.GitWrapper.Tests
{
    [TestClass]
    [DoNotParallelize]
    public sealed class Test2
    {
        public TestContext TestContext { get; set; } = null!;

        [TestMethod]
        public void Test_01_EnvironmentVariable()
        {
#if false
            var variable = "ShouldNeverExists";
            for(int i = 0; EnvironmentVariableHelper.IsExisting(variable); i++)
            {
                variable += i.ToString("00");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Assert.IsFalse(EnvironmentVariableHelper.IsExisting(variable));

                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path11\path12\path13"));
                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path11\path12\path13/"));
                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path11\path12\path13\"));

                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path21\path22\path23"));
                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path21\path22\path23/"));
                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path21\path22\path23\"));

                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path31\path32\path33"));
                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path31\path32\path33/"));
                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path31\path32\path33\"));

                EnvironmentVariableHelper.Set(variable, @"C:\path11\path12\path13");
                Assert.IsTrue(EnvironmentVariableHelper.IsExisting(variable));

                Assert.IsTrue(EnvironmentVariableHelper.Contains(variable, @"C:\path11\path12\path13"));
                Assert.IsTrue(EnvironmentVariableHelper.Contains(variable, @"C:\path11\path12\path13/"));
                Assert.IsTrue(EnvironmentVariableHelper.Contains(variable, @"C:\path11\path12\path13\"));

                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path21\path22\path23"));
                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path21\path22\path23/"));
                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path21\path22\path23\"));

                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path31\path32\path33"));
                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path31\path32\path33/"));
                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path31\path32\path33\"));

                EnvironmentVariableHelper.Add(variable, @"C:\path21\path22\path23/");
                Assert.IsTrue(EnvironmentVariableHelper.IsExisting(variable));

                Assert.IsTrue(EnvironmentVariableHelper.Contains(variable, @"C:\path11\path12\path13"));
                Assert.IsTrue(EnvironmentVariableHelper.Contains(variable, @"C:\path11\path12\path13/"));
                Assert.IsTrue(EnvironmentVariableHelper.Contains(variable, @"C:\path11\path12\path13\"));

                Assert.IsTrue(EnvironmentVariableHelper.Contains(variable, @"C:\path21\path22\path23"));
                Assert.IsTrue(EnvironmentVariableHelper.Contains(variable, @"C:\path21\path22\path23/"));
                Assert.IsTrue(EnvironmentVariableHelper.Contains(variable, @"C:\path21\path22\path23\"));

                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path31\path32\path33"));
                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path31\path32\path33/"));
                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path31\path32\path33\"));

                EnvironmentVariableHelper.Add(variable, @"C:\path31\path32\path33\");
                Assert.IsTrue(EnvironmentVariableHelper.IsExisting(variable));

                Assert.IsTrue(EnvironmentVariableHelper.Contains(variable, @"C:\path11\path12\path13"));
                Assert.IsTrue(EnvironmentVariableHelper.Contains(variable, @"C:\path11\path12\path13/"));
                Assert.IsTrue(EnvironmentVariableHelper.Contains(variable, @"C:\path11\path12\path13\"));

                Assert.IsTrue(EnvironmentVariableHelper.Contains(variable, @"C:\path21\path22\path23"));
                Assert.IsTrue(EnvironmentVariableHelper.Contains(variable, @"C:\path21\path22\path23/"));
                Assert.IsTrue(EnvironmentVariableHelper.Contains(variable, @"C:\path21\path22\path23\"));

                Assert.IsTrue(EnvironmentVariableHelper.Contains(variable, @"C:\path31\path32\path33"));
                Assert.IsTrue(EnvironmentVariableHelper.Contains(variable, @"C:\path31\path32\path33/"));
                Assert.IsTrue(EnvironmentVariableHelper.Contains(variable, @"C:\path31\path32\path33\"));



                EnvironmentVariableHelper.Remove(variable, @"C:\path11\path12\path13");

                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path11\path12\path13"));
                Assert.IsTrue(EnvironmentVariableHelper.Contains(variable, @"C:\path21\path22\path23"));
                Assert.IsTrue(EnvironmentVariableHelper.Contains(variable, @"C:\path31\path32\path33"));

                // Try to remove it a second time
                EnvironmentVariableHelper.Remove(variable, @"C:\path11\path12\path13");

                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path11\path12\path13"));
                Assert.IsTrue(EnvironmentVariableHelper.Contains(variable, @"C:\path21\path22\path23"));
                Assert.IsTrue(EnvironmentVariableHelper.Contains(variable, @"C:\path31\path32\path33"));

                EnvironmentVariableHelper.Remove(variable, @"C:\path21\path22\path23");

                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path11\path12\path13"));
                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path21\path22\path23"));
                Assert.IsTrue(EnvironmentVariableHelper.Contains(variable, @"C:\path31\path32\path33"));

                // Try to remove it a second time
                EnvironmentVariableHelper.Remove(variable, @"C:\path21\path22\path23");

                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path11\path12\path13"));
                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path21\path22\path23"));
                Assert.IsTrue(EnvironmentVariableHelper.Contains(variable, @"C:\path31\path32\path33"));

                EnvironmentVariableHelper.Remove(variable, @"C:\path31\path32\path33");

                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path11\path12\path13"));
                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path21\path22\path23"));
                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path31\path32\path33"));

                // Try to remove it a second time
                EnvironmentVariableHelper.Remove(variable, @"C:\path31\path32\path33");

                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path11\path12\path13"));
                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path21\path22\path23"));
                Assert.IsFalse(EnvironmentVariableHelper.Contains(variable, @"C:\path31\path32\path33"));
            }
#endif
        }
    }
}
