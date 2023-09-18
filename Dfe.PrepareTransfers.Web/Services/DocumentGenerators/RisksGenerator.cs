using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.DocumentGeneration;
using Dfe.PrepareTransfers.DocumentGeneration.Elements;
using Dfe.PrepareTransfers.Helpers;
using Dfe.PrepareTransfers.Web.Models.ProjectTemplate;
using System.Collections.Generic;

namespace Dfe.PrepareTransfers.Web.Services.DocumentGenerators
{
    public static class RisksGenerator
    {
        public static void AddRisks(DocumentBuilder builder, ProjectTemplateModel projectTemplateModel)
        {
            builder.ReplacePlaceholderWithContent("RisksInformation", build =>
            {
                build.AddTextHeading("Risks", HeadingLevel.One);

                if (projectTemplateModel.AnyIdentifiedRisks.Equals(true) )
                {
                      foreach (var item in projectTemplateModel.ListOfOtherFactors)
                    {

                        build.AddTable(new List<TextElement[]>
                        {
                         new[] { new TextElement { Value = EnumHelpers<TransferBenefits.OtherFactor>.GetDisplayValue(item.Key), Bold = true }, new TextElement { Value = item.Value } },
                        });
                    }
                }

                else
                {
                   

                        build.AddTable(new List<TextElement[]>
                        {
                         new[] { new TextElement { Value = "Risks" , Bold = true}, new TextElement { Value = "No risks Identified" } }
                        });
                 }

                  build.AddTable(new List<TextElement[]>
                        {
                          new[] { new TextElement { Value = "Equalities impact assessment considered", Bold = true }, new TextElement { Value = projectTemplateModel.EqualitiesImpactAssessmentConsidered } },
                        });

                
            });
        }
    }
}