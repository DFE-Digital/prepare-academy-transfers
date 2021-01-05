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
            RuleFor(p => p.ProjectStatus).NotEmpty().WithMessage("Must not be empty");

            RuleFor(p => p.ProjectStatus).Must(s => MustBeAllowedProjectStatus(s)).WithMessage("Invalid status code");

            RuleForEach(p => p.ProjectAcademies).SetValidator(new PostProjectsAcademiesModelValidator());
            RuleForEach(p => p.ProjectTrusts).SetValidator(new PostProjectsTrustsModelValidator());
        }

        private static bool MustBeAllowedProjectStatus(int statusCode)
        {
            return Mapping.MappingDictionaries.ProjectStatusMap.Keys.Any(k => k == statusCode);
        }

        internal class PostProjectsAcademiesModelValidator : AbstractValidator<PostProjectsAcademiesModel>
        {
            public PostProjectsAcademiesModelValidator()
            {
                RuleFor(p => p.AcademyId).NotEmpty().WithMessage("Must not be empty");

                RuleFor(p => p.EsfaInterventionReasons).Must(s => s == null || s.Distinct().Count() == s.Count).WithMessage("Duplicate status code detected");
                RuleFor(p => p.RddOrRscInterventionReasons).Must(s => s == null || s.Distinct().Count() == s.Count).WithMessage("Duplicate status code detected");

                RuleForEach(p => p.EsfaInterventionReasons).Must(s => MustBeAllowedEsfaInterventionReason(s)).WithMessage("Invalid status code");
                RuleForEach(p => p.RddOrRscInterventionReasons).Must(s => MustBeAllowedRddOrRscInterventionReason(s)).WithMessage("Invalid status code");

                RuleFor(p => p.EsfaInterventionReasonsExplained).Must(s => WordCount(s) < 2000).WithMessage("Must be shorter than 2000 words");
                RuleFor(p => p.RddOrRscInterventionReasonsExplained).Must(s => WordCount(s) < 2000).WithMessage("Must be shorter than 2000 words");

                RuleForEach(p => p.Trusts).SetValidator(new PostProjectsAcademiesTrustsModelValidator());
            }

            private int WordCount(string text)
            {
                if(string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
                {
                    return 0;
                }

                var wordCount = 0;
                var index = 0;

                //trim to first non-space character
                while (index < text.Length && char.IsWhiteSpace(text[index]))
                {
                    index++;
                }
                    
                while (index < text.Length)
                {
                    // skip if part of current word
                    while (index < text.Length && !char.IsWhiteSpace(text[index]))
                    {
                        index++;
                    }
                        
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
