using FluentValidation.TestHelper;
using Frontend.Validators.TransferDates;
using Xunit;

namespace Frontend.Tests.ValidatorTests.TransferDates
{
    public class HtbDateValidatorTests
    {
        [Fact]
        public void ShouldHaveChildValidator()
        {
            var validator = new HtbDateValidator();

            validator.ShouldHaveChildValidator(a => a.HtbDate, typeof(DateValidator));
        }
    }
}