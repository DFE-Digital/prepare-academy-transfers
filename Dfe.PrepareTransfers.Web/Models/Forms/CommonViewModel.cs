namespace Dfe.PrepareTransfers.Web.Models.Forms
{
    public abstract class CommonViewModel
    {
        public string Urn { get; set; }
        public bool ReturnToPreview { get; set; }
        public string OutgoingAcademyName { get; set; }
    }
}