using Chris82111.GitManager.GitWrapper.Core.EnvironmentVariables;
using System.Diagnostics;

namespace Chris82111.GitManager.GitWrapper.Tests
{
    public abstract class EnvironmentVariableTestsBase
    {
        protected abstract IEnvironmentVariable CreateEnvironmentVariable();
        readonly List<string>  _examplePaths = new List<string>()
        {
            @"/path11\path12/path13",
            @"/path21/path22/path23",
            @"/path31\path32/path33\",
            @"/path41/path42/path43\",
            @"/path51\path52/path53/",
            @"/path61/path62/path63/"
        };

        [TestMethod]
        public void Test_01_IsExisting_Set_Get_Remove()
        {
            var env = CreateEnvironmentVariable();

            var variable = UniqueVariable(env, "Var");


            Assert.ThrowsExactly<ArgumentNullException>(() => env.IsExisting(""));
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => env.IsExisting(null));
#pragma warning restore CS8625

            Assert.ThrowsExactly<ArgumentNullException>(() => env.Set("", ""));
#pragma warning disable CS8625
            Assert.ThrowsExactly<ArgumentNullException>(() => env.Set(null, null));
#pragma warning restore CS8625

            Assert.ThrowsExactly<ArgumentNullException>(() => env.Get(""));
#pragma warning disable CS8625
            Assert.ThrowsExactly<ArgumentNullException>(() => env.Get(null));
#pragma warning restore CS8625

            Assert.ThrowsExactly<ArgumentNullException>(() => env.Remove(""));
#pragma warning disable CS8625
            Assert.ThrowsExactly<ArgumentNullException>(() => env.Remove(null));
#pragma warning restore CS8625


            Assert.IsNull(env.Get(variable));
            Assert.IsFalse(env.IsExisting(variable));
            env.Set(variable, "");
            Assert.IsTrue(env.IsExisting(variable));
            Assert.AreEqual("", env.Get(variable));
            env.Set(variable, "1");
            Assert.AreEqual("1", env.Get(variable));
            env.Set(variable, null);
            Assert.IsFalse(env.IsExisting(variable));
            env.Set(variable, "");
            Assert.IsTrue(env.IsExisting(variable));
            env.Remove(variable);
            Assert.IsFalse(env.IsExisting(variable));
        }
        
        [TestMethod]
        public void Test_02_GetArray_Count_Add()
        {
            var env = CreateEnvironmentVariable();

            var variable = UniqueVariable(env, "Var");


            Assert.ThrowsExactly<ArgumentNullException>(() => env.GetArray(""));
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Assert.ThrowsExactly<ArgumentNullException>(() => env.GetArray(null));
#pragma warning restore CS8625

            Assert.ThrowsExactly<ArgumentNullException>(() => env.Count(""));
#pragma warning disable CS8625
            Assert.ThrowsExactly<ArgumentNullException>(() => env.Count(null));
#pragma warning restore CS8625

            Assert.ThrowsExactly<ArgumentNullException>(() => env.Add("", @"/path11\path12/path13"));
#pragma warning disable CS8625
            Assert.ThrowsExactly<ArgumentNullException>(() => env.Add(null, @"/path11\path12/path13"));
#pragma warning restore CS8625
            Assert.ThrowsExactly<ArgumentNullException>(() => env.Add(variable, ""));
#pragma warning disable CS8625
            Assert.ThrowsExactly<ArgumentNullException>(() => env.Add(variable, null));
#pragma warning restore CS8625


            var examplePaths = new List<string>()
            {
                @"/path11\path12/path13",
                @"/path21\path22/path23",
                @"/path31\path32/path33"
            };

            Assert.IsNull(env.GetArray(variable));
            Assert.AreEqual(0, env.Count(variable));

            env.Set(variable, string.Join(Path.PathSeparator, examplePaths));
            var a1 = env.GetArray(variable);
            Assert.HasCount(3, a1!);
            Assert.IsTrue(a1!.SequenceEqual(examplePaths));
            Assert.AreEqual(3, env.Count(variable));

            env.Set(variable, null);
            Assert.AreEqual(0, env.Count(variable));
            env.Set(variable, "");
            Assert.AreEqual(0, env.Count(variable));
            env.Add(variable, "1");
            Assert.AreEqual(1, env.Count(variable));
            env.Add(variable, "2");
            Assert.AreEqual(2, env.Count(variable));
            env.Add(variable, "3");
            Assert.AreEqual(3, env.Count(variable));
        }

        [TestMethod]
        public void Test_03_Remove()
        {
            var env = CreateEnvironmentVariable();

            var variable = UniqueVariable(env, "Var");

            var resultPaths = new List<string>()
            {
                @"/path21/path22/path23",
                @"/path31\path32/path33\",
                @"/path41/path42/path43\",
                @"/path51\path52/path53/",
                @"/path61/path62/path63/"
            };


            Assert.ThrowsExactly<ArgumentNullException>(() => env.Remove("", @"/path11\path12/path13"));
#pragma warning disable CS8625
            Assert.ThrowsExactly<ArgumentNullException>(() => env.Remove(null, @"/path11\path12/path13"));
#pragma warning restore CS8625
            Assert.ThrowsExactly<ArgumentNullException>(() => env.Remove(variable, ""));
#pragma warning disable CS8625
            Assert.ThrowsExactly<ArgumentNullException>(() => env.Remove(variable, null));
#pragma warning restore CS8625


            env.Remove(variable);
            Assert.AreEqual(0, env.Remove(variable, @"/path71\path72/path73"));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            Assert.AreEqual(0, env.Remove(variable, @"/path71\path72/path73"));


            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path11\path12/path13");
            var readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path11/path12/path13");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path11\path12/path13\");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path11/path12/path13\");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path11\path12/path13/");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path11/path12/path13/");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));


            resultPaths = new List<string>()
            {
                @"/path11\path12/path13",
                @"/path31\path32/path33\",
                @"/path41/path42/path43\",
                @"/path51\path52/path53/",
                @"/path61/path62/path63/"
            };

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path21\path22/path23");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path21/path22/path23");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path21\path22/path23\");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path21/path22/path23\");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path21\path22/path23/");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path21/path22/path23/");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));


            resultPaths = new List<string>()
            {
                @"/path11\path12/path13",
                @"/path21/path22/path23",
                @"/path41/path42/path43\",
                @"/path51\path52/path53/",
                @"/path61/path62/path63/"
            };

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path31\path32/path33");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path31/path32/path33");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path31\path32/path33\");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path31/path32/path33\");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path31\path32/path33/");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path31/path32/path33/");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));


            resultPaths = new List<string>()
            {
                @"/path11\path12/path13",
                @"/path21/path22/path23",
                @"/path31\path32/path33\",
                @"/path51\path52/path53/",
                @"/path61/path62/path63/"
            };

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path41\path42/path43");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path41/path42/path43");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path41\path42/path43\");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path41/path42/path43\");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path41\path42/path43/");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path41/path42/path43/");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));


            resultPaths = new List<string>()
            {
                @"/path11\path12/path13",
                @"/path21/path22/path23",
                @"/path31\path32/path33\",
                @"/path41/path42/path43\",
                @"/path61/path62/path63/"
            };

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path51\path52/path53");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path51/path52/path53");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path51\path52/path53\");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path51/path52/path53\");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path51\path52/path53/");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path51/path52/path53/");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));


            resultPaths = new List<string>()
            {
                @"/path11\path12/path13",
                @"/path21/path22/path23",
                @"/path31\path32/path33\",
                @"/path41/path42/path43\",
                @"/path51\path52/path53/"
            };

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path61\path62/path63");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path61/path62/path63");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path61\path62/path63\");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path61/path62/path63\");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path61\path62/path63/");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            env.Remove(variable, @"/path61/path62/path63/");
            readPaths = env.GetArray(variable);
            Assert.IsTrue(readPaths!.SequenceEqual(resultPaths));

        }

        [TestMethod]
        public void Test_04_Count()
        {
            var env = CreateEnvironmentVariable();

            var variable = UniqueVariable(env, "Var");


            Assert.ThrowsExactly<ArgumentNullException>(() => env.Remove("", @"/path11\path12/path13"));
#pragma warning disable CS8625
            Assert.ThrowsExactly<ArgumentNullException>(() => env.Remove(null, @"/path11\path12/path13"));
#pragma warning restore CS8625
            Assert.ThrowsExactly<ArgumentNullException>(() => env.Remove(variable, ""));
#pragma warning disable CS8625
            Assert.ThrowsExactly<ArgumentNullException>(() => env.Remove(variable, null));
#pragma warning restore CS8625

            env.Remove(variable);
            Assert.AreEqual(0, env.Count(variable, @"/path71\path72/path73"));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            Assert.IsFalse(env.Contains(variable, @"/path71\path72/path73"));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            Assert.AreEqual(1, env.Count(variable, @"/path11\path12/path13"));
            Assert.AreEqual(1, env.Count(variable, @"/path21/path22/path23"));
            Assert.AreEqual(1, env.Count(variable, @"/path31\path32/path33\"));
            Assert.AreEqual(1, env.Count(variable, @"/path41/path42/path43\"));
            Assert.AreEqual(1, env.Count(variable, @"/path51\path52/path53/"));
            Assert.AreEqual(1, env.Count(variable, @"/path61/path62/path63/"));

            Assert.AreEqual(1, env.Count(variable, @"/path11/path12/path13"));
            Assert.AreEqual(1, env.Count(variable, @"/path11\path12/path13"));
            Assert.AreEqual(1, env.Count(variable, @"/path11/path12/path13\"));
            Assert.AreEqual(1, env.Count(variable, @"/path11\path12/path13\"));
            Assert.AreEqual(1, env.Count(variable, @"/path11/path12/path13/"));
            Assert.AreEqual(1, env.Count(variable, @"/path11\path12/path13/"));

            Assert.AreEqual(1, env.Count(variable, @"/path21/path22/path23"));
            Assert.AreEqual(1, env.Count(variable, @"/path21\path22/path23"));
            Assert.AreEqual(1, env.Count(variable, @"/path21/path22/path23\"));
            Assert.AreEqual(1, env.Count(variable, @"/path21\path22/path23\"));
            Assert.AreEqual(1, env.Count(variable, @"/path21/path22/path23/"));
            Assert.AreEqual(1, env.Count(variable, @"/path21\path22/path23/"));

            Assert.AreEqual(1, env.Count(variable, @"/path31/path32/path33"));
            Assert.AreEqual(1, env.Count(variable, @"/path31\path32/path33"));
            Assert.AreEqual(1, env.Count(variable, @"/path31/path32/path33\"));
            Assert.AreEqual(1, env.Count(variable, @"/path31\path32/path33\"));
            Assert.AreEqual(1, env.Count(variable, @"/path31/path32/path33/"));
            Assert.AreEqual(1, env.Count(variable, @"/path31\path32/path33/"));

            Assert.AreEqual(1, env.Count(variable, @"/path41/path42/path43"));
            Assert.AreEqual(1, env.Count(variable, @"/path41\path42/path43"));
            Assert.AreEqual(1, env.Count(variable, @"/path41/path42/path43\"));
            Assert.AreEqual(1, env.Count(variable, @"/path41\path42/path43\"));
            Assert.AreEqual(1, env.Count(variable, @"/path41/path42/path43/"));
            Assert.AreEqual(1, env.Count(variable, @"/path41\path42/path43/"));

            Assert.AreEqual(1, env.Count(variable, @"/path51/path52/path53"));
            Assert.AreEqual(1, env.Count(variable, @"/path51\path52/path53"));
            Assert.AreEqual(1, env.Count(variable, @"/path51/path52/path53\"));
            Assert.AreEqual(1, env.Count(variable, @"/path51\path52/path53\"));
            Assert.AreEqual(1, env.Count(variable, @"/path51/path52/path53/"));
            Assert.AreEqual(1, env.Count(variable, @"/path51\path52/path53/"));

            Assert.AreEqual(1, env.Count(variable, @"/path61/path62/path63"));
            Assert.AreEqual(1, env.Count(variable, @"/path61\path62/path63"));
            Assert.AreEqual(1, env.Count(variable, @"/path61/path62/path63\"));
            Assert.AreEqual(1, env.Count(variable, @"/path61\path62/path63\"));
            Assert.AreEqual(1, env.Count(variable, @"/path61/path62/path63/"));
            Assert.AreEqual(1, env.Count(variable, @"/path61\path62/path63/"));
        }

        [TestMethod]
        public void Test_05_Contains()
        {
            var env = CreateEnvironmentVariable();

            var variable = UniqueVariable(env, "Var");


            Assert.ThrowsExactly<ArgumentNullException>(() => env.Contains("", @"/path11\path12/path13"));
#pragma warning disable CS8625
            Assert.ThrowsExactly<ArgumentNullException>(() => env.Contains(null, @"/path11\path12/path13"));
#pragma warning restore CS8625
            Assert.ThrowsExactly<ArgumentNullException>(() => env.Contains(variable, ""));
#pragma warning disable CS8625
            Assert.ThrowsExactly<ArgumentNullException>(() => env.Contains(variable, null));
#pragma warning restore CS8625


            env.Remove(variable);
            Assert.IsFalse(env.Contains(variable, @"/path71\path72/path73"));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            Assert.IsFalse(env.Contains(variable, @"/path71\path72/path73"));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            Assert.IsTrue(env.Contains(variable, @"/path11\path12/path13"));
            Assert.IsTrue(env.Contains(variable, @"/path11/path12/path13"));
            Assert.IsTrue(env.Contains(variable, @"/path11\path12/path13\"));
            Assert.IsTrue(env.Contains(variable, @"/path11/path12/path13\"));
            Assert.IsTrue(env.Contains(variable, @"/path11\path12/path13/"));
            Assert.IsTrue(env.Contains(variable, @"/path11/path12/path13/"));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            Assert.IsTrue(env.Contains(variable, @"/path21\path22/path23"));
            Assert.IsTrue(env.Contains(variable, @"/path21/path22/path23"));
            Assert.IsTrue(env.Contains(variable, @"/path21\path22/path23\"));
            Assert.IsTrue(env.Contains(variable, @"/path21/path22/path23\"));
            Assert.IsTrue(env.Contains(variable, @"/path21\path22/path23/"));
            Assert.IsTrue(env.Contains(variable, @"/path21/path22/path23/"));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            Assert.IsTrue(env.Contains(variable, @"/path31\path32/path33"));
            Assert.IsTrue(env.Contains(variable, @"/path31/path32/path33"));
            Assert.IsTrue(env.Contains(variable, @"/path31\path32/path33\"));
            Assert.IsTrue(env.Contains(variable, @"/path31/path32/path33\"));
            Assert.IsTrue(env.Contains(variable, @"/path31\path32/path33/"));
            Assert.IsTrue(env.Contains(variable, @"/path31/path32/path33/"));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            Assert.IsTrue(env.Contains(variable, @"/path41\path42/path43"));
            Assert.IsTrue(env.Contains(variable, @"/path41/path42/path43"));
            Assert.IsTrue(env.Contains(variable, @"/path41\path42/path43\"));
            Assert.IsTrue(env.Contains(variable, @"/path41/path42/path43\"));
            Assert.IsTrue(env.Contains(variable, @"/path41\path42/path43/"));
            Assert.IsTrue(env.Contains(variable, @"/path41/path42/path43/"));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            Assert.IsTrue(env.Contains(variable, @"/path51\path52/path53"));
            Assert.IsTrue(env.Contains(variable, @"/path51/path52/path53"));
            Assert.IsTrue(env.Contains(variable, @"/path51\path52/path53\"));
            Assert.IsTrue(env.Contains(variable, @"/path51/path52/path53\"));
            Assert.IsTrue(env.Contains(variable, @"/path51\path52/path53/"));
            Assert.IsTrue(env.Contains(variable, @"/path51/path52/path53/"));

            env.Set(variable, string.Join(Path.PathSeparator, _examplePaths));
            Assert.IsTrue(env.Contains(variable, @"/path61\path62/path63"));
            Assert.IsTrue(env.Contains(variable, @"/path61/path62/path63"));
            Assert.IsTrue(env.Contains(variable, @"/path61\path62/path63\"));
            Assert.IsTrue(env.Contains(variable, @"/path61/path62/path63\"));
            Assert.IsTrue(env.Contains(variable, @"/path61\path62/path63/"));
            Assert.IsTrue(env.Contains(variable, @"/path61/path62/path63/"));

        }

        private static string UniqueVariable(IEnvironmentVariable environmentVariableBase, string name)
        {
            var uniqueVariable = name;
            for (int i = 0; environmentVariableBase.IsExisting(uniqueVariable); i++)
            {
                uniqueVariable = name += i.ToString("00");
            }
            return uniqueVariable;
        }
    }


    [TestClass]
    [DoNotParallelize]
    public class Test2_EnvironmentVariableTests : EnvironmentVariableTestsBase
    {
        protected override IEnvironmentVariable CreateEnvironmentVariable()
        {
            return EnvironmentVariable.Instance;
        }
    }

    [TestClass]
    [DoNotParallelize]
    public class Test2_ProcessEnvironmentVariableTests : EnvironmentVariableTestsBase
    {
        protected override IEnvironmentVariable CreateEnvironmentVariable()
        {
            var psi = new ProcessStartInfo();

            return new ProcessEnvironmentVariable(psi);
        }
    }
}
