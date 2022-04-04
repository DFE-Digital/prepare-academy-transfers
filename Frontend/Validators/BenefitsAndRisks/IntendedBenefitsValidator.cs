﻿using Data.Models.Projects;
using FluentValidation;
using Frontend.Models.Benefits;

namespace Frontend.Validators.BenefitsAndRisks
{
    public class IntendedBenefitsValidator : AbstractValidator<IntendedBenefitsViewModel>
    {
        public IntendedBenefitsValidator()
        {
            CascadeMode = CascadeMode.Stop;
            RuleFor(x => x.SelectedIntendedBenefits)
                .NotEmpty()
                .WithMessage("Select at least one intended benefit");

            RuleFor(x => x.OtherBenefit)
                .NotEmpty()
                .When(x => x.SelectedIntendedBenefits.Contains(TransferBenefits.IntendedBenefit.Other))
                .WithMessage("Enter the other benefit");
        }
    }
}