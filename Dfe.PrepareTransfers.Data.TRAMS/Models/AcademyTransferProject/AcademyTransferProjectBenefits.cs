using System.Collections.Generic;

namespace Dfe.PrepareTransfers.Data.TRAMS.Models.AcademyTransferProject
{
    public class AcademyTransferProjectBenefits
    {
        public IntendedTransferBenefits IntendedTransferBenefits { get; set; }
        public OtherFactorsToConsider OtherFactorsToConsider { get; set; }
        public bool? AnyRisks { get; set; }
        public bool? EqualitiesImpactAssessmentConsidered { get; set; }
        public bool? IsCompleted { get; set; }
    }

    public class OtherFactorsToConsider
    {
        public OtherFactor HighProfile { get; set; }
        public OtherFactor ComplexLandAndBuilding { get; set; }
        public OtherFactor FinanceAndDebt { get; set; }
        public OtherFactor OtherRisks { get; set; }
    }

    public class OtherFactor
    {
        public bool? ShouldBeConsidered { get; set; }
        public string FurtherSpecification { get; set; }
    }

    public class IntendedTransferBenefits
    {
        public List<string> SelectedBenefits { get; set; }
        public string OtherBenefitValue { get; set; }
    }
}