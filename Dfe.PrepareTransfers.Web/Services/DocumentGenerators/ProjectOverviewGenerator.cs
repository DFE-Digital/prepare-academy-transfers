using Dfe.Academisation.ExtensionMethods;
using Dfe.PrepareTransfers.Data.Models;
using Dfe.PrepareTransfers.DocumentGeneration;
using Dfe.PrepareTransfers.DocumentGeneration.Elements;
using Dfe.PrepareTransfers.DocumentGeneration.Interfaces;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.ProjectTemplate;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using System.Collections.Generic;

namespace Dfe.PrepareTransfers.Web.Services.DocumentGenerators
{
   public static class ProjectOverviewGenerator
   {
      public static void AddProjectOverviewDetail(DocumentBuilder documentBuilder, ProjectTemplateModel projectTemplateModel)
      {
         documentBuilder.ReplacePlaceholderWithContent("ProjectOverview", build =>
         {
            build.AddTextHeading("Project Overview",HeadingLevel.One);
            build.AddTable(new List<TextElement[]>
         {
            new[] { new TextElement { Value = "Recommendation", Bold = true }, new TextElement { Value = projectTemplateModel.Recommendation } },
            new[] { new TextElement { Value = "Author", Bold = true }, new TextElement { Value = projectTemplateModel.Author }},
            new[] { new TextElement { Value = "Project name", Bold = true }, new TextElement { Value = projectTemplateModel.ProjectName }},
            new[] { new TextElement { Value = "Date of advisory board", Bold = true }, new TextElement { Value = projectTemplateModel.DateOfHtb }},
            new[] { new TextElement { Value = "Proposed academy transfer date", Bold = true }, new TextElement { Value = projectTemplateModel.DateOfProposedTransfer}},
            new[] { new TextElement { Value = "Reason for this transfer", Bold = true }, new TextElement { Value = projectTemplateModel.ReasonForTheTransfer}},
            new[] { new TextElement { Value = "What type of transfer is it?", Bold = true }, new TextElement { Value = projectTemplateModel.TypeOfTransfer}}
         });
         });

      }

   }
}
     