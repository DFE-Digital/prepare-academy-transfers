namespace API.Models.Upstream.Enums
{
    /// <summary>
    /// The enum of accepted values for ESFA Intervention Reasons
    /// 1. Governance Concerns
    /// 2. Finance Concerns
    /// 3. Irregularity Concerns
    /// 4. Safeguarding Concerns
    /// </summary>
    public enum EsfaInterventionReasonEnum
    {
        GovernanceConcerns = 1,
        FinanceConcerns = 2,
        IrregularityConcerns = 3,
        SafeguardingConcerns = 4
    }
}