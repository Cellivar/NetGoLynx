using System;
using System.ComponentModel.DataAnnotations;

namespace NetGoLynx.Data
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class RegularExpressionNegateAttribute : RegularExpressionAttribute
    {
        /// <summary>
        /// Constructor that accepts the regular expression pattern
        /// </summary>
        /// <param name="pattern">The regular expression to use.  It cannot be null.</param>
        public RegularExpressionNegateAttribute(string pattern)
            : base(pattern)
        {
        }

        /// <summary>
        /// Override of <see cref="ValidationAttribute.IsValid(object)"/>
        /// </summary>
        /// <remarks>This override performs the specific regular expression matching of the given <paramref name="value"/></remarks>
        /// <param name="value">The value to test for validity.</param>
        /// <returns><c>true</c> if the given value matches the current regular expression pattern</returns>
        public override bool IsValid(object value)
        {
            var result = base.IsValid(value);

            return !result;
        }

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage;
        }
    }
}
