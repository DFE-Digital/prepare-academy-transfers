using System.Collections.Generic;

namespace Dfe.PrepareTransfers.Data.TRAMS.Models.AcademyTransferProject
{
    public class AcademyTransferProjectFeatures
    {
        public string OtherTransferTypeDescription { get; set; }
        public List<string> SpecificReasonsForTransfer { get; set; }
        public bool? RddOrEsfaIntervention { get; set; }
        public string RddOrEsfaInterventionDetail { get; set; }
        public string TypeOfTransfer { get; set; }
        public string WhoInitiatedTheTransfer { get; set; }
        public bool? IsCompleted { get; set; }
    }
}