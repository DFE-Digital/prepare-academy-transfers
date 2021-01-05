using API.Models.Request;
using API.Models.Validators;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace API.Tests
{
    public class PostAchievementsModelValidatorTests
    {
        private readonly PostProjectsRequestModelValidator _validator;

        public PostAchievementsModelValidatorTests()
        {
            _validator = new PostProjectsRequestModelValidator();
        }

        [Fact]
        public void ProjectInitiatorFullName_Is_Required()
        {
            //Assert error raised when Project Initiator Full Name is empty

            var model = new PostProjectsRequestModel();

            var result = _validator.Validate(model);

            Assert.Contains(result.Errors, t => t.PropertyName == "ProjectInitiatorFullName" && t.ErrorMessage == "Must not be empty");

            //Assert error not raised when Project Initiator Full Name is not empty

            model = new PostProjectsRequestModel
            {
                ProjectInitiatorFullName = "Some Name"
            };

            result = _validator.Validate(model);

            Assert.DoesNotContain(result.Errors, e => e.PropertyName == "ProjectInitiatorFullName" && e.ErrorMessage == "Must not be empty");
        }

        [Fact]
        public void ProjectInitiatorUid_Is_Required()
        {
            //Assert error raised when Project Initiator Uid is empty

            var model = new PostProjectsRequestModel();

            var result = _validator.Validate(model);

            Assert.Contains(result.Errors, t => t.PropertyName == "ProjectInitiatorUid" && t.ErrorMessage == "Must not be empty");

            //Assert error not raised when Project Initiator Uid is not empty

            model = new PostProjectsRequestModel
            {
                ProjectInitiatorUid = "some@email.com"
            };

            result = _validator.Validate(model);

            Assert.DoesNotContain(result.Errors, e => e.PropertyName == "ProjectInitiatorUid" && e.ErrorMessage == "Must not be empty");
        }

        [Fact]
        public void ProjectStatus_Is_Required()
        {
            //Assert error raised when Project Status is empty

            var model = new PostProjectsRequestModel();

            var result = _validator.Validate(model);

            Assert.Contains(result.Errors, t => t.PropertyName == "ProjectStatus" && t.ErrorMessage == "Must not be empty");

            //Assert error not raised when Project Status is not empty

            model = new PostProjectsRequestModel
            {
                ProjectStatus = 8
            };

            result = _validator.Validate(model);

            Assert.DoesNotContain(result.Errors, e => e.PropertyName == "ProjectStatus" && e.ErrorMessage == "Must not be empty");
        }

        [Fact]
        public void ProjectStatus_Should_Be_Valid()
        { 
            //Assert error raised when Project Status is provided but invalid

            var model = new PostProjectsRequestModel
            {
                ProjectStatus = 8
            };

            var result = _validator.Validate(model);

            Assert.Contains(result.Errors, t => t.PropertyName == "ProjectStatus" && t.ErrorMessage == "Invalid status code");

            //Assert error not raised when Project Status is provided and valid 

            model = new PostProjectsRequestModel
            {
                ProjectStatus = 1
            };

            result = _validator.Validate(model);

            Assert.DoesNotContain(result.Errors, e => e.PropertyName == "ProjectStatus" && e.ErrorMessage == "Invalid status code");
        }

        [Fact]
        public void ProjectAcademy_Id_IsRequired()
        {
            //Set up two project academies, one with a null ID one with a set ID

            var model = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                   new PostProjectsAcademiesModel
                   {
                       
                   },
                   new PostProjectsAcademiesModel
                   {
                       AcademyId = Guid.NewGuid()
                   }
                }
            };

            var result = _validator.Validate(model);

            Assert.Contains(result.Errors, e => e.PropertyName == "ProjectAcademies[0].AcademyId" && e.ErrorMessage == "Must not be empty");
            Assert.DoesNotContain(result.Errors, e => e.PropertyName == "ProjectAcademies[1].AcademyId");
        }

        [Fact]
        public void ProjectAcademy_EsfaInterventionReasons_Is_Not_Required()
        {
            var model = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel()
                }
            };

            var result = _validator.Validate(model);

            Assert.DoesNotContain(result.Errors, e => e.PropertyName == "ProjectAcademies[0].EsfaInterventionReasons");
        }

        [Fact]
        public void ProjectAcademy_EsfaInterventionReasons_Must_Be_Unique()
        {
            var model = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel
                    {
                        EsfaInterventionReasons = new List<int> { 1, 2, 1}
                    }
                }
            };

            var result = _validator.Validate(model);

            Assert.Contains(result.Errors, e => e.PropertyName == "ProjectAcademies[0].EsfaInterventionReasons" && e.ErrorMessage == "Duplicate status code detected");
        }

        [Fact]
        public void ProjectAcademy_EsfaInterventionReasons_MustBe_Valid()
        {
            //Invalid code raises error
            var model = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel
                    {
                        EsfaInterventionReasons = new List<int> { 42 }
                    }
                }
            };

            var result = _validator.Validate(model);

            Assert.Contains(result.Errors, e => e.PropertyName == "ProjectAcademies[0].EsfaInterventionReasons[0]" && e.ErrorMessage == "Invalid status code");

            //Valid code doesn't raise error

            model = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel
                    {
                        EsfaInterventionReasons = new List<int> { 1 }
                    }
                }
            };

            result = _validator.Validate(model);

            Assert.DoesNotContain(result.Errors, e => e.PropertyName == "ProjectAcademies[0].EsfaInterventionReasons[0]" && e.ErrorMessage == "Invalid status code");
        }

        [Fact]
        public void ProjectAcademy_RddOrRscInterventionReasons_Is_Not_Required()
        {
            var model = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel()
                }
            };

            var result = _validator.Validate(model);

            Assert.DoesNotContain(result.Errors, e => e.PropertyName == "ProjectAcademies[0].RddOrRscInterventionReasons");
        }

        [Fact]
        public void ProjectAcademy_RddOrRscInterventionReasons_Must_Be_Unique()
        {
            var model = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel
                    {
                        RddOrRscInterventionReasons = new List<int> { 1, 2, 1}
                    }
                }
            };

            var result = _validator.Validate(model);

            Assert.Contains(result.Errors, e => e.PropertyName == "ProjectAcademies[0].RddOrRscInterventionReasons" && e.ErrorMessage == "Duplicate status code detected");
        }

        [Fact]
        public void ProjectAcademy_RddOrRscInterventionReasons_MustBe_Valid()
        {
            //Invalid code raises error
            var model = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel
                    {
                        RddOrRscInterventionReasons = new List<int> { 42 }
                    }
                }
            };

            var result = _validator.Validate(model);

            Assert.Contains(result.Errors, e => e.PropertyName == "ProjectAcademies[0].RddOrRscInterventionReasons[0]" && e.ErrorMessage == "Invalid status code");

            //Valid code doesn't raise error

            model = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel
                    {
                        RddOrRscInterventionReasons = new List<int> { 1 }
                    }
                }
            };

            result = _validator.Validate(model);

            Assert.DoesNotContain(result.Errors, e => e.PropertyName == "ProjectAcademies[0].RddOrRscInterventionReasons[0]");
        }

        [Fact]
        public void EsfaInterventionReasonsExplained_Is_Not_Required()
        {
            var model = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel()
                }
            };

            var result = _validator.Validate(model);

            Assert.DoesNotContain(result.Errors, e => e.PropertyName == "ProjectAcademies[0].EsfaInterventionReasonsExplained");
        }

        [Fact]
        public void EsfaInterventionReasonExplained_Must_Be_Shorter_Than_2000_Words()
        {
            //Explanation of 1999 words doesn't raise error
            var underBoundaryText = new StringBuilder();

            for(var i = 1; i < 2000; i++)
            {
                underBoundaryText.Append("word ");
            }

            var model = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel
                    {
                        EsfaInterventionReasonsExplained = underBoundaryText.ToString()
                    }
                }
            };

            var result = _validator.Validate(model);

            Assert.DoesNotContain(result.Errors, e => e.PropertyName == "ProjectAcademies[0].EsfaInterventionReasonsExplained");

            //Esplanation of 2000 words raises error
            var overBoundaryText = new StringBuilder();

            for (var i = 1; i <= 2000; i++)
            {
                overBoundaryText.Append("word ");
            }

            model = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel
                    {
                        EsfaInterventionReasonsExplained = overBoundaryText.ToString()
                    }
                }
            };

            result = _validator.Validate(model);

            Assert.Contains(result.Errors, e => e.PropertyName == "ProjectAcademies[0].EsfaInterventionReasonsExplained" && e.ErrorMessage == "Must be shorter than 2000 words");
        }

        [Fact]
        public void RddOrRscInterventionReasonsExplained_Is_Not_Required()
        {
            var model = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel()
                }
            };

            var result = _validator.Validate(model);

            Assert.DoesNotContain(result.Errors, e => e.PropertyName == "ProjectAcademies[0].RddOrRscInterventionReasonsExplained");
        }

        [Fact]
        public void RddOrRscInterventionReasonExplained_Must_Be_Shorter_Than_2000_Words()
        {
            //Explanation of 1999 words doesn't raise error
            var underBoundaryText = new StringBuilder();

            for (var i = 1; i < 2000; i++)
            {
                underBoundaryText.Append("word ");
            }

            var model = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel
                    {
                        RddOrRscInterventionReasonsExplained = underBoundaryText.ToString()
                    }
                }
            };

            var result = _validator.Validate(model);

            Assert.DoesNotContain(result.Errors, e => e.PropertyName == "ProjectAcademies[0].RddOrRscInterventionReasonsExplained");

            //Esplanation of 2000 words raises error
            var overBoundaryText = new StringBuilder();

            for (var i = 1; i <= 2000; i++)
            {
                overBoundaryText.Append("word ");
            }

            model = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel
                    {
                        RddOrRscInterventionReasonsExplained = overBoundaryText.ToString()
                    }
                }
            };

            result = _validator.Validate(model);

            Assert.Contains(result.Errors, e => e.PropertyName == "ProjectAcademies[0].RddOrRscInterventionReasonsExplained" && e.ErrorMessage == "Must be shorter than 2000 words");
        }

        [Fact]
        public void AcademyTrust_Id_Required()
        {
            //No TrustId set raises error
            var model = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel
                    {
                        Trusts = new List<PostProjectsAcademiesTrustsModel>
                        {
                            new PostProjectsAcademiesTrustsModel()
                        }
                    }
                }
            };

            var result = _validator.Validate(model);

            Assert.Contains(result.Errors, e => e.PropertyName == "ProjectAcademies[0].Trusts[0].TrustId" && e.ErrorMessage == "Must not be empty");

            //TrustId set doesn't raise error
            model = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel
                    {
                        Trusts = new List<PostProjectsAcademiesTrustsModel>
                        {
                            new PostProjectsAcademiesTrustsModel
                            {
                                TrustId = Guid.NewGuid()
                            }
                        }
                    }
                }
            };

            result = _validator.Validate(model);

            Assert.DoesNotContain(result.Errors, e => e.PropertyName == "ProjectAcademies[0].Trusts[0].TrustId");
        }

        [Fact]
        public void ProjectTrust_Id_Required()
        {
            //No TrustId set raises error
            var model = new PostProjectsRequestModel
            {
                ProjectTrusts = new List<PostProjectsTrustsModel>
                {
                    new PostProjectsTrustsModel()
                }
            };

            var result = _validator.Validate(model);

            Assert.Contains(result.Errors, e => e.PropertyName == "ProjectTrusts[0].TrustId" && e.ErrorMessage == "Must not be empty");

            //TrustId set doesn't raise error
            model = new PostProjectsRequestModel
            {
                ProjectTrusts = new List<PostProjectsTrustsModel>
                {
                    new PostProjectsTrustsModel { TrustId = Guid.NewGuid() }
                }
            };

            result = _validator.Validate(model);

            Assert.DoesNotContain(result.Errors, e => e.PropertyName == "ProjectTrusts[0].TrustId");
        }
    }
}
