using Dfe.PrepareTransfers.Web.Pages.Projects;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.Data.Models.KeyStagePerformance;

namespace Dfe.PrepareTransfers.Web.Models
{
    public class PreviewPageAcademyModel
    {
        public Academy Academy { get; set; }
        public PupilNumbers PupilNumbersViewModel { get; set; }
        public Pages.Projects.GeneralInformation.Index GeneralInformationViewModel { get; set; }
        public Pages.Projects.LatestOfstedJudgement.Index LatestOfstedJudgementViewModel { get;  set; }
        public EducationPerformance EducationPerformance { get; set; }
    }
}