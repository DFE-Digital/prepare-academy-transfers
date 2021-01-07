using API.Models.D365;
using System.Collections.Generic;

namespace API.Mapping
{
    public class MappingDictionaries
    {
        public static Dictionary<int, ProjectStatusEnum> ProjectStatusMap = new Dictionary<int, ProjectStatusEnum>
        {
            {1, ProjectStatusEnum.InProgress },
            {2, ProjectStatusEnum.Completed }
        };

        public static Dictionary<int, EsfaInterventionReasonEnum> EsfaInterventionReasonMap = new Dictionary<int, EsfaInterventionReasonEnum>
        {
            {1, EsfaInterventionReasonEnum.GovernanceConcerns },
            {2, EsfaInterventionReasonEnum.FinanceConcerns },
            {3, EsfaInterventionReasonEnum.IrregularityConcerns },
            {4, EsfaInterventionReasonEnum.SafeguardingConcerns }
        };

        public static Dictionary<int, RddOrRscInterventionReasonEnum> RddOrRscInterventionReasonMap = new Dictionary<int, RddOrRscInterventionReasonEnum>
        {
            {1, RddOrRscInterventionReasonEnum.TerminationWarningNotice },
            {2, RddOrRscInterventionReasonEnum.RSCMindedToTerminateNotice },
            {3, RddOrRscInterventionReasonEnum.OfstedInadequateRating }
        };
    }
}
