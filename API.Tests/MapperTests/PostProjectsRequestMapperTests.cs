using API.Mapping;
using API.Models.Request;
using API.Models.Upstream.Enums;
using System;
using System.Collections.Generic;
using Xunit;

namespace API.Tests.MapperTests
{
    public class PostProjectsRequestMapperTests
    {
        private readonly PostProjectsRequestMapper _mapper;

        public PostProjectsRequestMapperTests()
        {
            _mapper = new PostProjectsRequestMapper();
        }

        [Fact]
        public void AcademyId_OneAcademy_MapTest()
        {
            var request = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel
                    {
                        AcademyId = Guid.Parse("81014326-5d51-e911-a82e-000d3a385a17")
                    }
                }
            };

            var result = _mapper.Map(request);

            Assert.Equal("/accounts(81014326-5d51-e911-a82e-000d3a385a17)", result.Academies[0].AcademyId);
        }

        [Fact]
        public void ProjectInitiator_Fields_MapTest()
        {
            var request = new PostProjectsRequestModel
            {
                ProjectInitiatorFullName = "Joe Bloggs",
                ProjectInitiatorUid = "joebloggs@email.com"
            };

            var result = _mapper.Map(request);

            Assert.Equal("Joe Bloggs", result.ProjectInitiatorFullName);
            Assert.Equal("joebloggs@email.com", result.ProjectInitiatorUid);
        }

        [Fact]
        public void ProjectStatus_MapTest()
        {
            var request = new PostProjectsRequestModel
            {
                ProjectStatus = Models.Upstream.Enums.ProjectStatusEnum.Completed
            };

            var result = _mapper.Map(request);

            Assert.Equal(Models.D365.Enums.ProjectStatusEnum.Completed, result.ProjectStatus);
        }

        [Fact]
        public void AcademyId_ThreeAcademies_MapTest()
        {
            var request = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel
                    {
                        AcademyId = Guid.Parse("81014326-5d51-e911-a82e-000d3a385a17")
                    },
                    new PostProjectsAcademiesModel
                    {
                        AcademyId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fe1")
                    },
                    new PostProjectsAcademiesModel
                    {
                        AcademyId = Guid.Parse("9e644018-cbe7-4299-bd18-d30aa7bb13d6")
                    }
                }
            };

            var result = _mapper.Map(request);

            Assert.Equal("/accounts(81014326-5d51-e911-a82e-000d3a385a17)", result.Academies[0].AcademyId);
            Assert.Equal("/accounts(a16e9020-9123-4420-8055-851d1b672fe1)", result.Academies[1].AcademyId);
            Assert.Equal("/accounts(9e644018-cbe7-4299-bd18-d30aa7bb13d6)", result.Academies[2].AcademyId);
        }

        [Fact]
        public void EsfaInterventionReasons_OneAcademy_Empty_MapTest()
        {
            var request = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel
                    {
                        EsfaInterventionReasons = null
                    }
                }
            };

            var result = _mapper.Map(request);

            Assert.True(string.IsNullOrEmpty(result.Academies[0].EsfaInterventionReasons));
        }

        [Fact]
        public void EsfaInterventionReasons_OneAcademy_OneValue_MapTest()
        {
            var request = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel
                    {
                        EsfaInterventionReasons = new List<Models.Upstream.Enums.EsfaInterventionReasonEnum>
                        {
                            Models.Upstream.Enums.EsfaInterventionReasonEnum.FinanceConcerns
                        }
                    }
                }
            };

            var result = _mapper.Map(request);

            Assert.Equal("596500001", result.Academies[0].EsfaInterventionReasons);
        }

        [Fact]
        public void EsfaInterventionReasons_OneAcademy_ThreeValues_MapTest()
        {
            var request = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel
                    {
                        EsfaInterventionReasons = new List<Models.Upstream.Enums.EsfaInterventionReasonEnum>
                        {
                            Models.Upstream.Enums.EsfaInterventionReasonEnum.FinanceConcerns,
                            Models.Upstream.Enums.EsfaInterventionReasonEnum.IrregularityConcerns,
                            Models.Upstream.Enums.EsfaInterventionReasonEnum.SafeguardingConcerns
                        }
                    }
                }
            };

            var result = _mapper.Map(request);

            Assert.Equal("596500001,596500002,596500003", result.Academies[0].EsfaInterventionReasons);
        }

        [Fact]
        public void EsfaInterventionReasons_MultipleAcademies_MapTest()
        {
            var request = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel
                    {
                        EsfaInterventionReasons = null
                    },
                    new PostProjectsAcademiesModel
                    {
                        EsfaInterventionReasons = new List<Models.Upstream.Enums.EsfaInterventionReasonEnum>
                        {
                            Models.Upstream.Enums.EsfaInterventionReasonEnum.FinanceConcerns,
                        }
                    },
                    new PostProjectsAcademiesModel
                    {
                        EsfaInterventionReasons = new List<Models.Upstream.Enums.EsfaInterventionReasonEnum>
                        {
                            Models.Upstream.Enums.EsfaInterventionReasonEnum.FinanceConcerns,
                            Models.Upstream.Enums.EsfaInterventionReasonEnum.IrregularityConcerns,
                            Models.Upstream.Enums.EsfaInterventionReasonEnum.SafeguardingConcerns
                        }
                    }
                }
            };

            var result = _mapper.Map(request);

            Assert.Null(result.Academies[0].EsfaInterventionReasons);
            Assert.Equal("596500001", result.Academies[1].EsfaInterventionReasons);
            Assert.Equal("596500001,596500002,596500003", result.Academies[2].EsfaInterventionReasons);
        }

        [Fact]
        public void EsfaInterventionReasonsExplained_OneAcademy_Null_MapTest()
        {
            var request = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel
                    {
                        EsfaInterventionReasonsExplained = null
                    }
                }
            };

            var result = _mapper.Map(request);

            Assert.True(string.IsNullOrEmpty(result.Academies[0].EsfaInterventionReasonsExplained));
        }

        [Fact]
        public void EsfaInterventionReasonsExplained_OneAcademy_ValueSet_MapTest()
        {
            var request = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel
                    {
                        EsfaInterventionReasonsExplained = "Some explanation"
                    }
                }
            };

            var result = _mapper.Map(request);

            Assert.Equal("Some explanation", result.Academies[0].EsfaInterventionReasonsExplained);
        }

        [Fact]
        public void EsfaInterventionReasonsExplained_ThreeAcademies_MapTest()
        {
            var request = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel
                    {
                        EsfaInterventionReasonsExplained = "Some explanation"
                    },
                    new PostProjectsAcademiesModel
                    {
                        EsfaInterventionReasonsExplained = null
                    },
                    new PostProjectsAcademiesModel
                    {
                        EsfaInterventionReasonsExplained = "Another explanation"
                    }
                }
            };

            var result = _mapper.Map(request);

            Assert.Equal("Some explanation", result.Academies[0].EsfaInterventionReasonsExplained);
            Assert.True(string.IsNullOrEmpty(result.Academies[1].EsfaInterventionReasonsExplained));
            Assert.Equal("Another explanation", result.Academies[2].EsfaInterventionReasonsExplained);
        }

        [Fact]
        public void RddOrRscInterventionReasons_OneAcademy_Empty_MapTest()
        {
            var request = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel
                    {
                        RddOrRscInterventionReasons = null
                    }
                }
            };

            var result = _mapper.Map(request);

            Assert.True(string.IsNullOrEmpty(result.Academies[0].RddOrRscInterventionReasons));
        }

        [Fact]
        public void RddOrRscInterventionReasons_OneAcademy_OneValue_MapTest()
        {
            var request = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel
                    {
                        RddOrRscInterventionReasons = new List<RddOrRscInterventionReasonEnum>
                        {
                            RddOrRscInterventionReasonEnum.OfstedInadequateRating
                        }
                    }
                }
            };

            var result = _mapper.Map(request);

            Assert.Equal("596500002", result.Academies[0].RddOrRscInterventionReasons);
        }

        [Fact]
        public void RddOrRscInterventionReasons_OneAcademy_ThreeValues_MapTest()
        {
            var request = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel
                    {
                        RddOrRscInterventionReasons = new List<RddOrRscInterventionReasonEnum>
                        {
                            RddOrRscInterventionReasonEnum.OfstedInadequateRating,
                            RddOrRscInterventionReasonEnum.RSCMindedToTerminateNotice,
                            RddOrRscInterventionReasonEnum.TerminationWarningNotice
                        }
                    }
                }
            };

            var result = _mapper.Map(request);

            Assert.Equal("596500002,596500001,596500000", result.Academies[0].RddOrRscInterventionReasons);
        }

        [Fact]
        public void RddOrRscInterventionReasons_ThreeAcademies_MapTest()
        {
            var request = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel
                    {
                        RddOrRscInterventionReasons = new List<RddOrRscInterventionReasonEnum>
                        {
                            RddOrRscInterventionReasonEnum.OfstedInadequateRating,
                            RddOrRscInterventionReasonEnum.RSCMindedToTerminateNotice,
                            RddOrRscInterventionReasonEnum.TerminationWarningNotice
                        }
                    },
                    new PostProjectsAcademiesModel
                    {
                        RddOrRscInterventionReasons = new List<RddOrRscInterventionReasonEnum>
                        {
                            RddOrRscInterventionReasonEnum.OfstedInadequateRating,
                        }
                    },
                    new PostProjectsAcademiesModel
                    {
                        RddOrRscInterventionReasons = new List<RddOrRscInterventionReasonEnum>()
                    }
                }
            };

            var result = _mapper.Map(request);

            Assert.Equal("596500002,596500001,596500000", result.Academies[0].RddOrRscInterventionReasons);
            Assert.Equal("596500002", result.Academies[1].RddOrRscInterventionReasons);
            Assert.Null(result.Academies[2].RddOrRscInterventionReasons);
        }

        [Fact]
        public void AcademiesTrusts_OneAcademy_NoAcademyTrusts_MapTest()
        {
            var request = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel()
                }
            };

            var result = _mapper.Map(request);

            Assert.Empty(result.Academies[0].Trusts);
        }

        [Fact]
        public void AcademiesTrusts_OneAcademy_OneTrust_MapTest()
        {
            var request = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel()
                    {
                        Trusts = new List<PostProjectsAcademiesTrustsModel>
                        {
                            new PostProjectsAcademiesTrustsModel
                            {
                                TrustId = Guid.Parse("81014326-5d51-e911-a82e-000d3a385a17")
                            }
                        }
                    }
                }
            };

            var result = _mapper.Map(request);

            Assert.Equal("/accounts(81014326-5d51-e911-a82e-000d3a385a17)", result.Academies[0].Trusts[0].TrustId);
        }

        [Fact]
        public void AcademiesTrusts_OneAcademy_ThreeTrusts_MapTest()
        {
            var request = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel()
                    {
                        Trusts = new List<PostProjectsAcademiesTrustsModel>
                        {
                            new PostProjectsAcademiesTrustsModel
                            {
                                TrustId = Guid.Parse("81014326-5d51-e911-a82e-000d3a385a17")
                            },
                            new PostProjectsAcademiesTrustsModel
                            {
                                TrustId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fe1")
                            },
                            new PostProjectsAcademiesTrustsModel
                            {
                                TrustId = Guid.Parse("81014326-5e51-e911-a82e-000d3a385a17")
                            }
                        }
                    }
                }
            };

            var result = _mapper.Map(request);

            Assert.Equal("/accounts(81014326-5d51-e911-a82e-000d3a385a17)", result.Academies[0].Trusts[0].TrustId);
            Assert.Equal("/accounts(a16e9020-9123-4420-8055-851d1b672fe1)", result.Academies[0].Trusts[1].TrustId);
            Assert.Equal("/accounts(81014326-5e51-e911-a82e-000d3a385a17)", result.Academies[0].Trusts[2].TrustId);
        }

        [Fact]
        public void AcademiesTrusts_MultipleAcademies_JaggedTrusts_MapTest()
        {
            var request = new PostProjectsRequestModel
            {
                ProjectAcademies = new List<PostProjectsAcademiesModel>
                {
                    new PostProjectsAcademiesModel()
                    {
                        Trusts = new List<PostProjectsAcademiesTrustsModel>
                        {
                            new PostProjectsAcademiesTrustsModel
                            {
                                TrustId = Guid.Parse("81014326-5d51-e911-a82e-000d3a385a17")
                            }
                        }
                    },
                    new PostProjectsAcademiesModel
                    {
                        Trusts = new List<PostProjectsAcademiesTrustsModel>
                        {
                            new PostProjectsAcademiesTrustsModel
                            {
                                TrustId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fe1")
                            },
                            new PostProjectsAcademiesTrustsModel
                            {
                                TrustId = Guid.Parse("81014326-5e51-e911-a82e-000d3a385a17")
                            }
                        }
                    },
                    new PostProjectsAcademiesModel
                    {
                        Trusts = null
                    }
                }
            };

            var result = _mapper.Map(request);

            Assert.Equal("/accounts(81014326-5d51-e911-a82e-000d3a385a17)", result.Academies[0].Trusts[0].TrustId);
            Assert.Equal("/accounts(a16e9020-9123-4420-8055-851d1b672fe1)", result.Academies[1].Trusts[0].TrustId);
            Assert.Equal("/accounts(81014326-5e51-e911-a82e-000d3a385a17)", result.Academies[1].Trusts[1].TrustId);
            Assert.Empty(result.Academies[2].Trusts);
        }

        [Fact]
        public void ProjectTrusts_NoTrusts_MapTest()
        {
            var request = new PostProjectsRequestModel
            {
                ProjectTrusts = null
            };

            var result = _mapper.Map(request);

            Assert.Empty(result.Trusts);
        }

        [Fact]
        public void ProjectTrusts_OneTrust_MapTest()
        {
            var request = new PostProjectsRequestModel
            {
                ProjectTrusts = new List<PostProjectsTrustsModel>
                {
                    new PostProjectsTrustsModel
                    {
                        TrustId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fe1")
                    }
                }
            };

            var result = _mapper.Map(request);

            Assert.Equal("/accounts(a16e9020-9123-4420-8055-851d1b672fe1)", result.Trusts[0].TrustId);
        }

        [Fact]
        public void ProjectTrusts_ThreeTrusts_MapTest()
        {
            var request = new PostProjectsRequestModel
            {
                ProjectTrusts = new List<PostProjectsTrustsModel>
                {
                    new PostProjectsTrustsModel
                    {
                        TrustId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fe1")
                    },
                    new PostProjectsTrustsModel
                    {
                        TrustId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672ff2")
                    },
                    new PostProjectsTrustsModel
                    {
                        TrustId = Guid.Parse("a16e9020-9123-4420-8055-851d1b672fa9")
                    }
                }
            };

            var result = _mapper.Map(request);

            Assert.Equal("/accounts(a16e9020-9123-4420-8055-851d1b672fe1)", result.Trusts[0].TrustId);
            Assert.Equal("/accounts(a16e9020-9123-4420-8055-851d1b672ff2)", result.Trusts[1].TrustId);
            Assert.Equal("/accounts(a16e9020-9123-4420-8055-851d1b672fa9)", result.Trusts[2].TrustId);
        }
    }
}