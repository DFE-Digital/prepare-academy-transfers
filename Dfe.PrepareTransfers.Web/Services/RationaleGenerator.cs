using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.DocumentGeneration;
using Dfe.PrepareTransfers.DocumentGeneration.Elements;
using Dfe.PrepareTransfers.DocumentGeneration.Interfaces;
using Dfe.PrepareTransfers.Helpers;
using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.ProjectTemplate;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;

namespace Dfe.PrepareTransfers.Web.Services
{
    public static class RationaleGenerator
    {
        public static void AddRationale(DocumentBuilder documentBuilder, ProjectTemplateModel projectTemplateModel)
        {

            documentBuilder.ReplacePlaceholderWithContent("RationaleInformation", build =>
            {
                build.AddTextHeading("Rationale", HeadingLevel.One);
                build.AddTextHeading("Rationale for project", HeadingLevel.Two);
                build.AddTable(new List<TextElement[]>
            {
            new[] { new TextElement { Value = projectTemplateModel.RationaleForProject, Bold = true } },
            });

                build.AddTextHeading("Rational for trust or sponsor ", HeadingLevel.Two);
                build.AddTable(new List<TextElement[]>
            {
            new[] { new TextElement { Value = projectTemplateModel.RationaleForTrust, Bold = true } },
            });
            }


            );

        }
    }
}