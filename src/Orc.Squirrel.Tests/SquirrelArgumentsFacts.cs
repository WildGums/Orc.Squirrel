namespace Orc.Squirrel.Tests
{
    using NUnit.Framework;

    public class SquirrelArgumentsFacts
    {
        [TestFixture]
        public class TheIsSquirrelArgumentMethod
        {
            [TestCase("--squirrel-install", true)]
            [TestCase("--squirrel-firstrun", true)]
            [TestCase("--squirrel-test", true)]
            [TestCase("\\squirrel\\path\\install", false)]
            public void ReturnsExpectedValueForParameter(string argument, bool expectedValue)
            {
                Assert.AreEqual(expectedValue, SquirrelArguments.IsSquirrelArgument(argument));
            }
        }
    }
}
