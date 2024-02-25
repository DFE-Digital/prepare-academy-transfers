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
   public static class TransferFeaturesGenerator
   {
      public static void AddFeaturesDetail(DocumentBuilder documentBuilder, ProjectTemplateModel projectTemplateModel)
      {
         documentBuilder.ReplacePlaceholderWithContent("FeaturesInformation", build =>
         {
            build.AddTextHeading("Features of the transfer",HeadingLevel.One);
            build.AddTable(new List<TextElement[]>
         { 
            new[] { new TextElement { Value = "Reason for this transfer", Bold = true }, new TextElement { Value = projectTemplateModel.ReasonForTheTransfer}},
            new[] { new TextElement { Value = "What are the specific reasons for this transfer?", Bold = true }, new TextElement { Value = projectTemplateModel.SpecificReasonsForTheTransfer}},
            new[] { new TextElement { Value = "What type of transfer is it?", Bold = true }, new TextElement { Value = projectTemplateModel.TypeOfTransfer}}
         });
         });

      }

   }
}
     