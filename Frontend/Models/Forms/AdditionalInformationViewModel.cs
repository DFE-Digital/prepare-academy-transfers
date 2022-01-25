namespace Frontend.Models.Forms
{
    public class AdditionalInformationViewModel
    {
        public string Urn { get; set; }
        public bool AddOrEditAdditionalInformation { get; set; }
        public string AdditionalInformation { get; set; }
        public string HintText { get; set; }
        public bool ReturnToPreview { get; set; }
    }
}
