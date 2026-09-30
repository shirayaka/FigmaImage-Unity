using NUnit.Framework;

namespace ProjectArea.UI.Tests
{
    public class FigmaImagePackageTests
    {
        [Test]
        public void FigmaImageAcceptanceTestsPass()
        {
            string result = FigmaImageTests.RunAllTests();

            Assert.That(result, Does.Contain("61/61 Tests Passed"), result);
        }

        [Test]
        public void RoundedImageAcceptanceTestsPass()
        {
            string result = RoundedImageTests.RunAllTests();

            Assert.That(result, Does.Contain("10/10 Tests Passed"), result);
        }
    }
}
