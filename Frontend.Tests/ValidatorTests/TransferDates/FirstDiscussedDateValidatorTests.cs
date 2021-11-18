using FluentValidation.TestHelper;
using Frontend.Validators.TransferDates;
using Xunit;

namespace Frontend.Tests.ValidatorTests.TransferDates
{
    public class FirstDiscussedDateValidatorTests
    {
        [Fact]
        public void ShouldHaveChildValidator()
        {
            var validator = new FirstDiscussedDateValidator();

            validator.ShouldHaveChildValidator(a => a.FirstDiscussed, typeof(DateValidator));
        }
    }
}