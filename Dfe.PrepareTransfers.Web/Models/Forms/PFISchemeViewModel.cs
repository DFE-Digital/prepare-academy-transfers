namespace Dfe.PrepareTransfers.Web.Models.Forms
{
    public class PFISchemeViewModel
    {
        public string Urn { get; set; }
        public string PFIScheme { get; set; }
        public string PFISchemeDetails { get; set; }
        public bool AddOrEditPFIScheme { get; set; }
        public string HintText { get; set; }
        public bool ReturnToPreview { get; set; }
        public bool HideWarning { get; set; }
        public string TitleId { get => "pfi-scheme"; } // For jump links
    }
}
