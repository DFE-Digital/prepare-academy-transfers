using API.Mapping.Request;
using API.Models.Upstream.Enums;
using API.Models.Upstream.Request;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace API.Tests.MapperTests
{
    public class PutProjectAcademyMapperTests
    {
        private readonly PutProjectAcademiesRequestDynamicsMapper _dynamicsMapper; 

        public PutProjectAcademyMapperTests()
        {
            _dynamicsMapper = new PutProjectAcademiesRequestDynamicsMapper();
        }

        [Fact]
        public void AcademyId_MapTest()
        {
            var request = new PutProjectAcademiesRequestModel
            {
                AcademyId = Guid.Parse("81014326-5d51-e911-a82e-000d3a385a17")
            };
                
            var result = _dynamicsMapper.Map(request);

            Assert.Equal("/accounts(81014326-5d51-e911-a82e-000d3a385a17)", result.AcademyId);
        }

        [Fact]
        public void EsfaInterventionReasons_Empty_MapTest()
        {
            var request = new PutProjectAcademiesRequestModel
            {
                EsfaInterventionReasons = null
            };
                
            var result = _dynamicsMapper.Map(request);

            Assert.True(string.IsNullOrEmpty(result.EsfaInterventionReasons));
        }

        [Fact]
        public void EsfaInterventionReasons_OneValue_MapTest()
        {
            var request = new PutProjectAcademiesRequestModel
            {
                EsfaInterventionReasons = new List<Models.Upstream.Enums.EsfaInterventionReasonEnum>
                {
                    Models.Upstream.Enums.EsfaInterventionReasonEnum.FinanceConcerns
                }
            };

            var result = _dynamicsMapper.Map(request);

            Assert.Equal("596500001", result.EsfaInterventionReasons);
        }

        [Fact]
        public void EsfaInterventionReasons_ThreeValues_MapTest()
        {
            var request = new PutProjectAcademiesRequestModel
            {
                EsfaInterventionReasons = new List<Models.Upstream.Enums.EsfaInterventionReasonEnum>
                {
                    Models.Upstream.Enums.EsfaInterventionReasonEnum.FinanceConcerns,
                    Models.Upstream.Enums.EsfaInterventionReasonEnum.IrregularityConcerns,
                    Models.Upstream.Enums.EsfaInterventionReasonEnum.SafeguardingConcerns
                }
            };

            var result = _dynamicsMapper.Map(request);

            Assert.Equal("596500001,596500002,596500003", result.EsfaInterventionReasons);
        }


        [Fact]
        public void EsfaInterventionReasonsExplained_Null_MapTest()
        {
            var request = new PutProjectAcademiesRequestModel
            {
                EsfaInterventionReasonsExplained = null
            };

            var result = _dynamicsMapper.Map(request);

            Assert.True(string.IsNullOrEmpty(result.EsfaInterventionReasonsExplained));
        }

        [Fact]
        public void EsfaInterventionReasonsExplained_OneAcademy_ValueSet_MapTest()
        {
            var request = new PutProjectAcademiesRequestModel
            {
                EsfaInterventionReasonsExplained = "Some explanation"
            };

            var result = _dynamicsMapper.Map(request);

            Assert.Equal("Some explanation", result.EsfaInterventionReasonsExplained);
        }

        [Fact]
        public void RddOrRscInterventionReasons_Empty_MapTest()
        {
            var request = new PutProjectAcademiesRequestModel
            {
                RddOrRscInterventionReasons = null
            };

            var result = _dynamicsMapper.Map(request);

            Assert.True(string.IsNullOrEmpty(result.RddOrRscInterventionReasons));
        }

        [Fact]
        public void RddOrRscInterventionReasons_OneAcademy_OneValue_MapTest()
        {
            var request = new PutProjectAcademiesRequestModel
            {
                RddOrRscInterventionReasons = new List<RddOrRscInterventionReasonEnum>
                {
                    RddOrRscInterventionReasonEnum.OfstedInadequateRating
                }
            };

            var result = _dynamicsMapper.Map(request);

            Assert.Equal("596500002", result.RddOrRscInterventionReasons);
        }

        [Fact]
        public void RddOrRscInterventionReasons_OneAcademy_ThreeValues_MapTest()
        {
            var request = new PutProjectAcademiesRequestModel
            {
                RddOrRscInterventionReasons = new List<RddOrRscInterventionReasonEnum>
                {
                    RddOrRscInterventionReasonEnum.OfstedInadequateRating,
                    RddOrRscInterventionReasonEnum.RSCMindedToTerminateNotice,
                    RddOrRscInterventionReasonEnum.TerminationWarningNotice
                }
            };

            var result = _dynamicsMapper.Map(request);

            Assert.Equal("596500002,596500001,596500000", result.RddOrRscInterventionReasons);
        }
    }
}
