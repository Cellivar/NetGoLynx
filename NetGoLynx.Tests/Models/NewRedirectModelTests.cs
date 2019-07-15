using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetGoLynx.Models.RedirectModels;

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
            var model = new RedirectMetadata(name)
            {
                Target = "https://valid.target"
            };
            var (IsValid, _) = TryValidate(model);

            Assert.IsTrue(IsValid);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow("{}[]")]
        [DataRow("Invalid Name")]
        [DataRow("12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890")]
        public void LinkNameIsNotValid(string name)
        {
            var model = new RedirectMetadata(name)
            {
                Target = "https://valid.target"
            };
            var (IsValid, _) = TryValidate(model);

            Assert.IsFalse(IsValid);
        }

        [TestMethod]
        public void RedirectFromMetadataRedirectWorks()
        {
            var model = new RedirectMetadata()
            {
                Name = "Name",
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
