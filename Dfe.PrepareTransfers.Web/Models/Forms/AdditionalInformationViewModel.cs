﻿namespace Dfe.PrepareTransfers.Web.Models.Forms
{
    public class AdditionalInformationViewModel
    {
        public string Urn { get; set; }
        public bool AddOrEditAdditionalInformation { get; set; }
        public string AdditionalInformation { get; set; }
        public string HintText { get; set; }
        public bool ReturnToPreview { get; set; }
        public bool HideWarning { get; set; }
        public string TitleId { get => "additional-information"; } // For jump links
    }
}
