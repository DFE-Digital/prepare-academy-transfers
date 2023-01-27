using Dfe.PrepareTransfers.Web.Pages.Projects;
using Data.Models;
using Data.Models.KeyStagePerformance;

namespace Dfe.PrepareTransfers.Web.Models
{
    public class PreviewPageAcademyModel
    {
        public Academy Academy { get; set; }
        public PupilNumbers PupilNumbersViewModel { get; set; }
        public Dfe.PrepareTransfers.Web.Pages.Projects.GeneralInformation.Index GeneralInformationViewModel { get; set; }
        public Dfe.PrepareTransfers.Web.Pages.Projects.LatestOfstedJudgement.Index LatestOfstedJudgementViewModel { get;  set; }
        public EducationPerformance EducationPerformance { get; set; }
    }
}