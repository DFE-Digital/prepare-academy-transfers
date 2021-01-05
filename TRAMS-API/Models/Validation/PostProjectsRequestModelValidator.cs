using API.Models.Request;
using FluentValidation;
using System;
using System.Linq;

namespace API.Models.Validation
{
    public class PostProjectsRequestModelValidator : AbstractValidator<PostProjectsRequestModel>
    {
        public PostProjectsRequestModelValidator()
        {
            RuleFor(p => p.ProjectInitiatorFullName).NotEmpty().WithMessage(ValidationMessages.MustNotBeEmpty);
            RuleFor(p => p.ProjectInitiatorUid).NotEmpty().WithMessage(ValidationMessages.MustNotBeEmpty);
            RuleFor(p => p.ProjectStatus).NotEmpty().WithMessage(ValidationMessages.MustNotBeEmpty);

            RuleFor(p => p.ProjectStatus).Must(s => MustBeAllowedProjectStatus(s)).WithMessage(ValidationMessages.InvalidStatusCode);

            RuleFor(p => p.ProjectInitiatorFullName).Length(1, 100).WithMessage(string.Format(ValidationMessages.CharLengthExceeded, "100"));
            RuleFor(p => p.ProjectInitiatorUid).Length(1, 100).WithMessage(string.Format(ValidationMessages.CharLengthExceeded, "100"));

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
                RuleFor(p => p.AcademyId).NotEmpty().WithMessage(ValidationMessages.MustNotBeEmpty);

                RuleFor(p => p.EsfaInterventionReasons).Must(s => s == null || s.Distinct().Count() == s.Count).WithMessage(ValidationMessages.DuplicateStatusCode);
                RuleFor(p => p.RddOrRscInterventionReasons).Must(s => s == null || s.Distinct().Count() == s.Count).WithMessage(ValidationMessages.DuplicateStatusCode);

                RuleForEach(p => p.EsfaInterventionReasons).Must(s => MustBeAllowedEsfaInterventionReason(s)).WithMessage(ValidationMessages.InvalidStatusCode);
                RuleForEach(p => p.RddOrRscInterventionReasons).Must(s => MustBeAllowedRddOrRscInterventionReason(s)).WithMessage(ValidationMessages.InvalidStatusCode);

                RuleFor(p => p.EsfaInterventionReasonsExplained).Must(s => WordCount(s) < 2000).WithMessage(string.Format(ValidationMessages.WordLengthExceeded, "2000"));
                RuleFor(p => p.RddOrRscInterventionReasonsExplained).Must(s => WordCount(s) < 2000).WithMessage(string.Format(ValidationMessages.WordLengthExceeded, "2000"));

                RuleForEach(p => p.Trusts).SetValidator(new PostProjectsAcademiesTrustsModelValidator());
            }

            private int WordCount(string text)
            {
                if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text))
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
                    RuleFor(p => p.TrustId).NotEmpty().WithMessage(ValidationMessages.MustNotBeEmpty);
                }
            }
        }

        internal class PostProjectsTrustsModelValidator : AbstractValidator<PostProjectsTrustsModel>
        {
            public PostProjectsTrustsModelValidator()
            {
                RuleFor(p => p.TrustId).NotEmpty().WithMessage(ValidationMessages.MustNotBeEmpty);
            }
        }
    }
}
