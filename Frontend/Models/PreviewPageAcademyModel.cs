using Frontend.Pages.Projects;
using Data.Models;
using Data.Models.KeyStagePerformance;

namespace Frontend.Models
{
    public class PreviewPageAcademyModel
    {
        public Academy Academy { get; set; }
        public PupilNumbers PupilNumbersViewModel { get; set; }
        public Frontend.Pages.Projects.GeneralInformation.Index GeneralInformationViewModel { get; set; }
        public Frontend.Pages.Projects.LatestOfstedJudgement.Index LatestOfstedJudgementViewModel { get;  set; }
        public EducationPerformance EducationPerformance { get; set; }
    }
}