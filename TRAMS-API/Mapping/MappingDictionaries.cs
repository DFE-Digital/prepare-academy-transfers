 using System.Collections.Generic;

namespace API.Mapping
{
    public class MappingDictionaries
    {
        public static Dictionary<Models.Upstream.Enums.ProjectStatusEnum, Models.D365.Enums.ProjectStatusEnum> ProjecStatusEnumMap = new Dictionary<Models.Upstream.Enums.ProjectStatusEnum, Models.D365.Enums.ProjectStatusEnum>
        {
            {
                Models.Upstream.Enums.ProjectStatusEnum.InProgress, 
                Models.D365.Enums.ProjectStatusEnum.InProgress
            },
            {
                Models.Upstream.Enums.ProjectStatusEnum.Completed, 
                Models.D365.Enums.ProjectStatusEnum.Completed 
            }
        };

        public static Dictionary<Models.Upstream.Enums.EsfaInterventionReasonEnum, 
                                 Models.D365.Enums.EsfaInterventionReasonEnum> EsfaInterventionReasonEnumMap = new Dictionary<Models.Upstream.Enums.EsfaInterventionReasonEnum, 
                                                                                                                              Models.D365.Enums.EsfaInterventionReasonEnum>
        {
            {
                Models.Upstream.Enums.EsfaInterventionReasonEnum.FinanceConcerns, 
                Models.D365.Enums.EsfaInterventionReasonEnum.FinanceConcerns
            },
            {
                Models.Upstream.Enums.EsfaInterventionReasonEnum.GovernanceConcerns, 
                Models.D365.Enums.EsfaInterventionReasonEnum.GovernanceConcerns
            },
            {
                Models.Upstream.Enums.EsfaInterventionReasonEnum.IrregularityConcerns, 
                Models.D365.Enums.EsfaInterventionReasonEnum.IrregularityConcerns
            },
            {
                Models.Upstream.Enums.EsfaInterventionReasonEnum.SafeguardingConcerns, 
                Models.D365.Enums.EsfaInterventionReasonEnum.SafeguardingConcerns
            }
        };

        public static Dictionary<Models.Upstream.RddOrRscInterventionReasonEnum, Models.D365.Enums.RddOrRscInterventionReasonEnum> RddOrRscInterventionReasonEnumMap = new Dictionary<Models.Upstream.RddOrRscInterventionReasonEnum, Models.D365.Enums.RddOrRscInterventionReasonEnum>
        {
            {
                Models.Upstream.RddOrRscInterventionReasonEnum.TerminationWarningNotice,
                Models.D365.Enums.RddOrRscInterventionReasonEnum.TerminationWarningNotice 
            },
            {
                Models.Upstream.RddOrRscInterventionReasonEnum.RSCMindedToTerminateNotice,
                Models.D365.Enums.RddOrRscInterventionReasonEnum.RSCMindedToTerminateNotice 
            },
            {
                Models.Upstream.RddOrRscInterventionReasonEnum.OfstedInadequateRating,
                Models.D365.Enums.RddOrRscInterventionReasonEnum.OfstedInadequateRating 
            }
        };
    }
}
