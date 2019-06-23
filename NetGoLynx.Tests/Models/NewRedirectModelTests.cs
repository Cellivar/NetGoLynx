using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetGoLynx.Models.Home;

namespace NetGoLynx.Tests.Models
{
    [TestClass]
    public class NewRedirectModelTests
    {
        private (bool IsValid, List<ValidationResult> Results) TryValidate(object obj)
        {
            var context = new ValidationContext(obj, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(obj, context, results, true);

            return (isValid, results);
        }

        [DataTestMethod]
        [DataRow("ValidName")]
        [DataRow("Valid$Name")]
        [DataRow("Valid\uD83E\uDD8AName")]
        public void LinkNameIsValid(string name)
        {
            var model = new AddModel(name)
            {
                Target = "https://valid.target"
            };
            var result = TryValidate(model);

            Assert.IsTrue(result.IsValid);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow("{}[]")]
        [DataRow("Invalid Name")]
        [DataRow("12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890")]
        public void LinkNameIsNotValid(string name)
        {
            var model = new AddModel(name)
            {
                Target = "https://valid.target"
            };
            var result = TryValidate(model);

            Assert.IsFalse(result.IsValid);
        }

        [TestMethod]
        public void TestMethod1()
        {
            var model = new AddModel()
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
