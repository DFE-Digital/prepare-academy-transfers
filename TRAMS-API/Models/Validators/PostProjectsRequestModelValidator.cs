using API.Models.Request;
using FluentValidation;
using System;
using System.Linq;

namespace API.Models.Validators
{
    public class PostProjectsRequestModelValidator : AbstractValidator<PostProjectsRequestModel>
    {
        public PostProjectsRequestModelValidator()
        {
            RuleFor(p => p.ProjectInitiatorFullName).NotEmpty().WithMessage("Must not be empty");
            RuleFor(p => p.ProjectInitiatorUid).NotEmpty().WithMessage("Must not be empty");
            RuleFor(p => p.ProjectStatus).Must(s => MustBeAllowedProjectStatus(s)).WithMessage("Must be 1 for In Progress or 2 for Completed");

            RuleForEach(p => p.ProjectAcademies).SetValidator(new PostProjectsAcademiesModelValidator());
            RuleForEach(p => p.ProjectTrusts).SetValidator(new PostProjectsTrustsModelValidator());
        }

        private static bool MustBeAllowedProjectStatus(int s)
        { 
            return Mapping.MappingDictionaries.ProjectStatusMap.Keys.Any(k => k == s);
        }

        internal class PostProjectsAcademiesModelValidator : AbstractValidator<PostProjectsAcademiesModel>
        {
            public PostProjectsAcademiesModelValidator()
            {
                RuleFor(p => p.AcademyId).NotEmpty().WithMessage("Must not be empty");

                RuleFor(p => p.EsfaInterventionReasons).Must(s => s == null || s.Distinct().Count() == s.Count).WithMessage("Duplicate code detected");
                RuleFor(p => p.RddOrRscInterventionReasons).Must(s => s == null || s.Distinct().Count() == s.Count).WithMessage("Duplicate code detected");

                RuleForEach(p => p.EsfaInterventionReasons).Must(s => MustBeAllowedEsfaInterventionReason(s)).WithMessage("Invalid code");
                RuleForEach(p => p.RddOrRscInterventionReasons).Must(s => MustBeAllowedRddOrRscInterventionReason(s)).WithMessage("Invalid code");

                RuleFor(p => p.EsfaInterventionReasonsExplained).Must(s => WordCount(s) < 2000).WithMessage("Must be shorter than 2000 words");
                RuleFor(p => p.RddOrRscInterventionReasonsExplained).Must(s => WordCount(s) < 2000).WithMessage("Must be shorter than 2000 words");

                RuleForEach(p => p.Trusts).SetValidator(new PostProjectsAcademiesTrustsModelValidator());
            }

            private int WordCount(string text)
            {
                int wordCount = 0, index = 0;

                // skip whitespace until first word
                while (index < text.Length && char.IsWhiteSpace(text[index]))
                {
                    index++;
                }
                    
                while (index < text.Length)
                {
                    // check if current char is part of a word
                    while (index < text.Length && !char.IsWhiteSpace(text[index]))
                        index++;

                    wordCount++;

                    // skip whitespace until next word
                    while (index < text.Length && char.IsWhiteSpace(text[index]))
                        index++;
                }

                return wordCount;
            }

            private bool MustBeAllowedRddOrRscInterventionReason(int s)
            {
                return Mapping.MappingDictionaries.RddOrRscInterventionReasonMap.Keys.Any(k => k == s);
            }

            private bool MustBeAllowedEsfaInterventionReason(int s)
            {
                return Mapping.MappingDictionaries.EsfaInterventionReasonMap.Keys.Any(k => k == s);
            }

            internal class PostProjectsAcademiesTrustsModelValidator : AbstractValidator<PostProjectsAcademiesTrustsModel>
            {
                public PostProjectsAcademiesTrustsModelValidator()
                {
                    RuleFor(p => p.TrustId).NotEmpty().WithMessage("Must not be empty");
                }
            }
        }

        internal class PostProjectsTrustsModelValidator : AbstractValidator<PostProjectsTrustsModel>
        {
            public PostProjectsTrustsModelValidator()
            {
                RuleFor(p => p.TrustId).NotEmpty().WithMessage("Must not be empty");
            }
        }
    }
}
