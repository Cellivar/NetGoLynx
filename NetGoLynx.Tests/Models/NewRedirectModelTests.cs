using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetGoLynx.Tests.Models
{
    [TestClass]
    public class NewRedirectModelTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var model = new NetGoLynx.Models.NewRedirectModel()
            {
                LinkName = "Name",
                Description = "Description",
                Target = "https://target.com"
            };

            var redirect = model.ToRedirect();

            Assert.AreEqual("Name", redirect.Name);
            Assert.AreEqual("Description", redirect.Description);
            Assert.AreEqual("https://target.com", redirect.Target);
        }

    }
}
