namespace API.Models.Upstream.Enums
{
    /// <summary>
    /// Allowed values for the RDD or RSC intervention status codes:
    /// 1. Termination Warning Notice
    /// 2. RSC Minded to Terminate Notice
    /// 3. Ofsted Inadequate Rating
    /// </summary>
    public enum RddOrRscInterventionReasonEnum
    {
        TerminationWarningNotice = 1,
        RSCMindedToTerminateNotice = 2,
        OfstedInadequateRating = 3
    }
}