using API.Mapping;
using API.Models.Downstream.D365;
using API.Models.Upstream.Enums;
using System;
using System.Collections.Generic;
using Xunit;

namespace API.Tests.MapperTests
{
    public class GetProjectsModelMapperTests
    {
        private readonly GetProjectsResponseMapper _mapper;

        public GetProjectsModelMapperTests()
        {
            _mapper = new GetProjectsResponseMapper(new GetProjectAcademiesResponseMapper());
        }

        [Fact]
        public void NullInputHandlingTest()
        {
            var model = (GetProjectsD365Model)null;

            var result = _mapper.Map(model);

            Assert.Null(result);
        }

        [Fact]
        public void ProjectId_MapTest()
        {
            var model = new GetProjectsD365Model
            {
                ProjectId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9")
            };

            var result = _mapper.Map(model);

            Assert.Equal(Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9"), result.ProjectId);
        }

        [Fact]
        public void ProjectName_MapTest()
        {
            var model = new GetProjectsD365Model
            {
                ProjectName = "Some name"
            };

            var result = _mapper.Map(model);

            Assert.Equal("Some name", result.ProjectName);
        }

        [Fact]
        public void ProjectStatus_MapTest()
        {
            var model = new GetProjectsD365Model
            {
                ProjectStatus = Models.D365.Enums.ProjectStatusEnum.Completed
            };

            var result = _mapper.Map(model);

            Assert.Equal(Models.Upstream.Enums.ProjectStatusEnum.Completed, result.ProjectStatus);
        }

        [Fact]
        public void ProjectInitiatorDetails_MapTest()
        {
            var model = new GetProjectsD365Model
            {
                ProjectInitiatorFullName = "Joe Bloggs",
                ProjectInitiatorUid = "joe.bloggs@email.com"
            };

            var result = _mapper.Map(model);

            Assert.Equal("Joe Bloggs", result.ProjectInitiatorFullName);
            Assert.Equal("joe.bloggs@email.com", result.ProjectInitiatorUid);
        }

        [Fact]
        public void ProjectAcademy_GuidFields_OneAcademy_MapTest()
        {
            var model = new GetProjectsD365Model
            {
                Academies = new List<AcademyTransfersProjectAcademy>
                {
                    new AcademyTransfersProjectAcademy
                    {
                        AcademyTransfersProjectAcademyId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9"),
                        AcademyId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fb1"),
                        ProjectId = Guid.Parse("a16e9020-9123-4420-8055-851d1b111eb1")
                    }
                }
            };

            var result = _mapper.Map(model);

            Assert.Equal(Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9"), result.ProjectAcademies[0].ProjectAcademyId);
            Assert.Equal(Guid.Parse("a16e9020-9123-4420-8055-851d1b672fb1"), result.ProjectAcademies[0].AcademyId);
            Assert.Equal(Guid.Parse("a16e9020-9123-4420-8055-851d1b111eb1"), result.ProjectAcademies[0].ProjectId);
        }

        [Fact]
        public void ProjectAcademy_GuidFields_ThreeAcademies_MapTest()
        {
            var model = new GetProjectsD365Model
            {
                Academies = new List<AcademyTransfersProjectAcademy>
                {
                    new AcademyTransfersProjectAcademy
                    {
                        AcademyTransfersProjectAcademyId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9"),
                        AcademyId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fb1"),
                        ProjectId = Guid.Parse("a16e9020-9123-4420-8055-851d1b111eb1")
                    },
                    new AcademyTransfersProjectAcademy
                    {
                        AcademyTransfersProjectAcademyId = Guid.Parse("b16e9020-9123-4420-8055-851d1b672fa9"),
                        AcademyId = Guid.Parse("b16e9020-9123-4420-8055-851d1b672fb1"),
                        ProjectId = Guid.Parse("b16e9020-9123-4420-8055-851d1b111eb1")
                    },
                    new AcademyTransfersProjectAcademy
                    {
                        AcademyTransfersProjectAcademyId = Guid.Parse("c16e9020-9123-4420-8055-851d1b672fa9"),
                        AcademyId = Guid.Parse("c16e9020-9123-4420-8055-851d1b672fb1"),
                        ProjectId = Guid.Parse("c16e9020-9123-4420-8055-851d1b111eb1")
                    }
                }
            };

            var result = _mapper.Map(model);

            Assert.Equal(Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9"), result.ProjectAcademies[0].ProjectAcademyId);
            Assert.Equal(Guid.Parse("a16e9020-9123-4420-8055-851d1b672fb1"), result.ProjectAcademies[0].AcademyId);
            Assert.Equal(Guid.Parse("a16e9020-9123-4420-8055-851d1b111eb1"), result.ProjectAcademies[0].ProjectId);

            Assert.Equal(Guid.Parse("b16e9020-9123-4420-8055-851d1b672fa9"), result.ProjectAcademies[1].ProjectAcademyId);
            Assert.Equal(Guid.Parse("b16e9020-9123-4420-8055-851d1b672fb1"), result.ProjectAcademies[1].AcademyId);
            Assert.Equal(Guid.Parse("b16e9020-9123-4420-8055-851d1b111eb1"), result.ProjectAcademies[1].ProjectId);

            Assert.Equal(Guid.Parse("c16e9020-9123-4420-8055-851d1b672fa9"), result.ProjectAcademies[2].ProjectAcademyId);
            Assert.Equal(Guid.Parse("c16e9020-9123-4420-8055-851d1b672fb1"), result.ProjectAcademies[2].AcademyId);
            Assert.Equal(Guid.Parse("c16e9020-9123-4420-8055-851d1b111eb1"), result.ProjectAcademies[2].ProjectId);
        }

        [Fact]
        public void ProjectAcademy_EsfaInterventionReasons_OneAcademy_NullValue_MapTest()
        {
            var model = new GetProjectsD365Model
            {
                Academies = new List<AcademyTransfersProjectAcademy>
                {
                    new AcademyTransfersProjectAcademy
                    {
                        EsfaInterventionReasons = null
                    }
                }
            };

            var result = _mapper.Map(model);

            Assert.Empty(result.ProjectAcademies[0].EsfaInterventionReasons);
        }

        [Fact]
        public void ProjectAcademy_EsfaInterventionReasons_OneAcademy_EmptyValue_MapTest()
        {
            var model = new GetProjectsD365Model
            {
                Academies = new List<AcademyTransfersProjectAcademy>
                {
                    new AcademyTransfersProjectAcademy
                    {
                        EsfaInterventionReasons = string.Empty
                    }
                }
            };

            var result = _mapper.Map(model);

            Assert.Empty(result.ProjectAcademies[0].EsfaInterventionReasons);
        }

        [Fact]
        public void ProjectAcademy_EsfaInterventionReasons_OneAcademy_OneValue_MapTest()
        {
            var model = new GetProjectsD365Model
            {
                Academies = new List<AcademyTransfersProjectAcademy>
                {
                    new AcademyTransfersProjectAcademy
                    {
                        EsfaInterventionReasons = "596500000"
                    }
                }
            };

            var result = _mapper.Map(model);

            Assert.Equal(Models.Upstream.Enums.EsfaInterventionReasonEnum.GovernanceConcerns, result.ProjectAcademies[0].EsfaInterventionReasons[0]);
            Assert.Single(result.ProjectAcademies[0].EsfaInterventionReasons);
        }

        [Fact]
        public void ProjectAcademy_EsfaInterventionReasons_OneAcademy_ThreeValues_MapTest()
        {
            var model = new GetProjectsD365Model
            {
                Academies = new List<AcademyTransfersProjectAcademy>
                {
                    new AcademyTransfersProjectAcademy
                    {
                        EsfaInterventionReasons = "596500000,596500001,596500002"
                    }
                }
            };

            var result = _mapper.Map(model);

            Assert.Equal(Models.Upstream.Enums.EsfaInterventionReasonEnum.GovernanceConcerns, result.ProjectAcademies[0].EsfaInterventionReasons[0]);
            Assert.Equal(Models.Upstream.Enums.EsfaInterventionReasonEnum.FinanceConcerns, result.ProjectAcademies[0].EsfaInterventionReasons[1]);
            Assert.Equal(Models.Upstream.Enums.EsfaInterventionReasonEnum.IrregularityConcerns, result.ProjectAcademies[0].EsfaInterventionReasons[2]);
            Assert.Single(result.ProjectAcademies);
            Assert.Equal(3, result.ProjectAcademies[0].EsfaInterventionReasons.Count);
        }

        [Fact]
        public void ProjectAcademy_EsfaInterventionReasons_ThreeAcademies_JaggedValues_MapTest()
        {
            var model = new GetProjectsD365Model
            {
                Academies = new List<AcademyTransfersProjectAcademy>
                {
                    new AcademyTransfersProjectAcademy
                    {
                        EsfaInterventionReasons = "596500000,596500001,596500002"
                    },
                    new AcademyTransfersProjectAcademy
                    {
                        EsfaInterventionReasons = "596500000"
                    },
                    new AcademyTransfersProjectAcademy
                    {
                        EsfaInterventionReasons = string.Empty
                    }
                }
            };

            var result = _mapper.Map(model);

            Assert.Equal(Models.Upstream.Enums.EsfaInterventionReasonEnum.GovernanceConcerns, result.ProjectAcademies[0].EsfaInterventionReasons[0]);
            Assert.Equal(Models.Upstream.Enums.EsfaInterventionReasonEnum.FinanceConcerns, result.ProjectAcademies[0].EsfaInterventionReasons[1]);
            Assert.Equal(Models.Upstream.Enums.EsfaInterventionReasonEnum.IrregularityConcerns, result.ProjectAcademies[0].EsfaInterventionReasons[2]);
            Assert.Equal(3, result.ProjectAcademies[0].EsfaInterventionReasons.Count);

            Assert.Equal(Models.Upstream.Enums.EsfaInterventionReasonEnum.GovernanceConcerns, result.ProjectAcademies[1].EsfaInterventionReasons[0]);
            Assert.Single(result.ProjectAcademies[1].EsfaInterventionReasons);

            Assert.Empty(result.ProjectAcademies[2].EsfaInterventionReasons);
        }

        [Fact]
        public void ProjectAcademy_RddOrRscInterventionReasons_OneAcademy_NullValue_MapTest()
        {
            var model = new GetProjectsD365Model
            {
                Academies = new List<AcademyTransfersProjectAcademy>
                {
                    new AcademyTransfersProjectAcademy
                    {
                        RddOrRscInterventionReasons = null
                    }
                }
            };

            var result = _mapper.Map(model);

            Assert.Empty(result.ProjectAcademies[0].RddOrRscInterventionReasons);
        }

        [Fact]
        public void ProjectAcademy_RddOrRscInterventionReasons_OneAcademy_EmptyValue_MapTest()
        {
            var model = new GetProjectsD365Model
            {
                Academies = new List<AcademyTransfersProjectAcademy>
                {
                    new AcademyTransfersProjectAcademy
                    {
                        RddOrRscInterventionReasons = string.Empty
                    }
                }
            };

            var result = _mapper.Map(model);

            Assert.Empty(result.ProjectAcademies[0].RddOrRscInterventionReasons);
        }

        [Fact]
        public void ProjectAcademy_RddOrRscInterventionReasons_OneAcademy_OneValue_MapTest()
        {
            var model = new GetProjectsD365Model
            {
                Academies = new List<AcademyTransfersProjectAcademy>
                {
                    new AcademyTransfersProjectAcademy
                    {
                        RddOrRscInterventionReasons = "596500000"
                    }
                }
            };

            var result = _mapper.Map(model);

            Assert.Equal(RddOrRscInterventionReasonEnum.TerminationWarningNotice, result.ProjectAcademies[0].RddOrRscInterventionReasons[0]);
            Assert.Single(result.ProjectAcademies[0].RddOrRscInterventionReasons);
        }

        [Fact]
        public void ProjectAcademy_RddOrRscInterventionReasons_OneAcademy_ThreeValues_MapTest()
        {
            var model = new GetProjectsD365Model
            {
                Academies = new List<AcademyTransfersProjectAcademy>
                {
                    new AcademyTransfersProjectAcademy
                    {
                        RddOrRscInterventionReasons = "596500000,596500001,596500002"
                    }
                }
            };

            var result = _mapper.Map(model);

            Assert.Equal(RddOrRscInterventionReasonEnum.TerminationWarningNotice, result.ProjectAcademies[0].RddOrRscInterventionReasons[0]);
            Assert.Equal(RddOrRscInterventionReasonEnum.RSCMindedToTerminateNotice, result.ProjectAcademies[0].RddOrRscInterventionReasons[1]);
            Assert.Equal(RddOrRscInterventionReasonEnum.OfstedInadequateRating, result.ProjectAcademies[0].RddOrRscInterventionReasons[2]);
            Assert.Single(result.ProjectAcademies);
            Assert.Equal(3, result.ProjectAcademies[0].RddOrRscInterventionReasons.Count);
        }

        [Fact]
        public void ProjectAcademy_RddOrRscInterventionReasons_ThreeAcademies_JaggedValues_MapTest()
        {
            var model = new GetProjectsD365Model
            {
                Academies = new List<AcademyTransfersProjectAcademy>
                {
                    new AcademyTransfersProjectAcademy
                    {
                        RddOrRscInterventionReasons = "596500000,596500001,596500002"
                    },
                    new AcademyTransfersProjectAcademy
                    {
                        RddOrRscInterventionReasons = "596500000"
                    },
                    new AcademyTransfersProjectAcademy
                    {
                        RddOrRscInterventionReasons = string.Empty
                    }
                }
            };

            var result = _mapper.Map(model);

            Assert.Equal(RddOrRscInterventionReasonEnum.TerminationWarningNotice, result.ProjectAcademies[0].RddOrRscInterventionReasons[0]);
            Assert.Equal(RddOrRscInterventionReasonEnum.RSCMindedToTerminateNotice, result.ProjectAcademies[0].RddOrRscInterventionReasons[1]);
            Assert.Equal(RddOrRscInterventionReasonEnum.OfstedInadequateRating, result.ProjectAcademies[0].RddOrRscInterventionReasons[2]);
            Assert.Equal(3, result.ProjectAcademies[0].RddOrRscInterventionReasons.Count);

            Assert.Equal(RddOrRscInterventionReasonEnum.TerminationWarningNotice, result.ProjectAcademies[1].RddOrRscInterventionReasons[0]);
            Assert.Single(result.ProjectAcademies[1].RddOrRscInterventionReasons);

            Assert.Empty(result.ProjectAcademies[2].RddOrRscInterventionReasons);
        }

        [Fact]
        public void ProjectAcademies_InterventionExplanationFields_JaggedValues_MapTest()
        {
            var model = new GetProjectsD365Model
            {
                Academies = new List<AcademyTransfersProjectAcademy>
                {
                    new AcademyTransfersProjectAcademy
                    {
                        RddOrRscInterventionReasonsExplained = "RDD Explanation",
                        EsfaInterventionReasonsExplained = "ESFA Explanation"
                    },
                    new AcademyTransfersProjectAcademy
                    {
                        RddOrRscInterventionReasonsExplained = "Another RDD Explanation",
                    },
                    new AcademyTransfersProjectAcademy
                    {
                        EsfaInterventionReasonsExplained = "Another ESFA Explanation"
                    },
                    new AcademyTransfersProjectAcademy
                    {

                    }
                }
            };

            var result = _mapper.Map(model);

            Assert.Equal("RDD Explanation", result.ProjectAcademies[0].RddOrRscInterventionReasonsExplained);
            Assert.Equal("ESFA Explanation", result.ProjectAcademies[0].EsfaInterventionReasonsExplained);

            Assert.Equal("Another RDD Explanation", result.ProjectAcademies[1].RddOrRscInterventionReasonsExplained);
            Assert.True(string.IsNullOrEmpty(result.ProjectAcademies[1].EsfaInterventionReasonsExplained));

            Assert.True(string.IsNullOrEmpty(result.ProjectAcademies[2].RddOrRscInterventionReasonsExplained));
            Assert.Equal("Another ESFA Explanation", result.ProjectAcademies[2].EsfaInterventionReasonsExplained);

            Assert.True(string.IsNullOrEmpty(result.ProjectAcademies[3].RddOrRscInterventionReasonsExplained));
            Assert.True(string.IsNullOrEmpty(result.ProjectAcademies[3].EsfaInterventionReasonsExplained));
        }

        [Fact]
        public void ProjectAcademies_NullList_MapTest()
        {
            var model = new GetProjectsD365Model()
            {
                Academies = null
            };

            var result = _mapper.Map(model);

            Assert.Empty(result.ProjectAcademies);
        }

        [Fact]
        public void ProjectAcademies_EmptyList_MapTest()
        {
            var model = new GetProjectsD365Model()
            {
                Academies = new List<AcademyTransfersProjectAcademy>()
            };

            var result = _mapper.Map(model);

            Assert.Empty(result.ProjectAcademies);
        }

        [Fact]
        public void ProjectTrusts_NullList_MapTest()
        {
            var model = new GetProjectsD365Model()
            {
                Trusts = null
            };

            var result = _mapper.Map(model);

            Assert.Empty(result.ProjectTrusts);
        }

        [Fact]
        public void ProjectTrusts_EmptyList_MapTest()
        {
            var model = new GetProjectsD365Model()
            {
                Trusts = new List<ProjectTrust>()
            };

            var result = _mapper.Map(model);

            Assert.Empty(result.ProjectTrusts);
        }

        [Fact]
        public void ProjectTrusts_OneTrust_MapTest()
        {
            var model = new GetProjectsD365Model()
            {
                Trusts = new List<ProjectTrust>
                {
                    new ProjectTrust
                    {
                        ProjectTrustId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9"),
                        TrustId = Guid.Parse("b16e9020-9123-4420-8055-851d1b672fa9"),
                        TrustName = "Some Trust"
                    }
                }
            };

            var result = _mapper.Map(model);

            Assert.Single(result.ProjectTrusts);
            Assert.Equal(Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9"), result.ProjectTrusts[0].ProjectTrustId);
            Assert.Equal(Guid.Parse("b16e9020-9123-4420-8055-851d1b672fa9"), result.ProjectTrusts[0].TrustId);
            Assert.Equal("Some Trust", result.ProjectTrusts[0].TrustName);
        }

        [Fact]
        public void ProjectTrusts_ThreeTrusts_MapTest()
        {
            var model = new GetProjectsD365Model()
            {
                Trusts = new List<ProjectTrust>
                {
                    new ProjectTrust
                    {
                        ProjectTrustId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9"),
                        TrustId = Guid.Parse("a16e0000-9123-4420-8055-851d1b672fa9"),
                        TrustName = "Some Trust"
                    },
                    new ProjectTrust
                    {
                        ProjectTrustId = Guid.Parse("b16e9020-9123-4420-8055-851d1b672fa9"),
                        TrustId = Guid.Parse("b16e0000-9123-4420-8055-851d1b672fa9"),
                        TrustName = "Another Trust"
                    },
                    new ProjectTrust
                    {
                        ProjectTrustId = Guid.Parse("c16e9020-9123-4420-8055-851d1b672fa9"),
                        TrustId = Guid.Parse("c16e0000-9123-4420-8055-851d1b672fa9"),
                        TrustName = "Yet Another Trust"
                    }
                }
            };

            var result = _mapper.Map(model);

            Assert.Equal(3, result.ProjectTrusts.Count);

            Assert.Equal(Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9"), result.ProjectTrusts[0].ProjectTrustId);
            Assert.Equal(Guid.Parse("a16e0000-9123-4420-8055-851d1b672fa9"), result.ProjectTrusts[0].TrustId);
            Assert.Equal("Some Trust", result.ProjectTrusts[0].TrustName);

            Assert.Equal(Guid.Parse("b16e9020-9123-4420-8055-851d1b672fa9"), result.ProjectTrusts[1].ProjectTrustId);
            Assert.Equal(Guid.Parse("b16e0000-9123-4420-8055-851d1b672fa9"), result.ProjectTrusts[1].TrustId);
            Assert.Equal("Another Trust", result.ProjectTrusts[1].TrustName);

            Assert.Equal(Guid.Parse("c16e9020-9123-4420-8055-851d1b672fa9"), result.ProjectTrusts[2].ProjectTrustId);
            Assert.Equal(Guid.Parse("c16e0000-9123-4420-8055-851d1b672fa9"), result.ProjectTrusts[2].TrustId);
            Assert.Equal("Yet Another Trust", result.ProjectTrusts[2].TrustName);
        }
    }
}
