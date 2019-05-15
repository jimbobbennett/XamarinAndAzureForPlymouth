using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace NumberTaker.UITests
{
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public class Tests
    {
        IApp app;
        Platform platform;

        public Tests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app = AppInitializer.StartApp(platform);
        }

        [Test]
        public void ErrorIfNoPhoto()
        {
            app.Repl();

            Func<AppQuery, AppQuery> button = c => c.Button("sendPhotoButton");

            app.Tap(button);

            Assert.IsTrue(app.Query((arg) => arg.Text("No photo")).Any());
        }
    }
}
