using System.Collections.Generic;
using Dfe.PrepareTransfers.Data.Models.Projects;
using Dfe.PrepareTransfers.DocumentGeneration;
using Dfe.PrepareTransfers.Web.Services;
using static Dfe.PrepareTransfers.Web.Services.DocumentGenerators.ProjectOverviewGenerator;
using static Dfe.PrepareTransfers.Web.Services.DocumentGenerators.BenefitsGenerator;
using static Dfe.PrepareTransfers.Web.Services.DocumentGenerators.RisksGenerator;
using static Dfe.PrepareTransfers.Web.Services.DocumentGenerators.RationaleGenerator;
using static Dfe.PrepareTransfers.Web.Services.DocumentGenerators.LegalRequirementsGenerator;
using Moq;
using Xunit;
using Dfe.PrepareTransfers.Web.Services.Interfaces;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using System.IO;
using System.Linq;
using Dfe.PrepareTransfers.Helpers;
using System;
using Dfe.PrepareTransfers.Web.Services.Responses;
using Dfe.Academisation.ExtensionMethods;

namespace Dfe.PrepareTransfers.Web.Tests.ServicesTests
{
    public class DocumentGeneratorTests
    {
        private readonly GetProjectTemplateModel _subject;

        private readonly GetInformationForProjectResponse _getTestInformationForProject;
        private readonly Mock<IGetInformationForProject> _getInformationForProject;

        private readonly string _projectUrn = "projectId";

          public DocumentGeneratorTests()
        {
            _getInformationForProject = new Mock<IGetInformationForProject>();

            _subject = new GetProjectTemplateModel(_getInformationForProject.Object);
            
            _getTestInformationForProject =
                    TestFixtures.GetInformationForProject.GetTestInformationForProject(_projectUrn);
            
            _getInformationForProject.Setup(s => s.Execute(_projectUrn)).ReturnsAsync(
                    _getTestInformationForProject);
        }
       
            private static DocumentBuilder AddPlaceholderToDocument(string placeholder)
            {
                
                DocumentBuilder documentBuilder = new DocumentBuilder();

                documentBuilder.AddParagraph(placeholder);
                
                var template = documentBuilder.Build();
                
                var memoryStream = new MemoryStream();
                memoryStream.Write(template);
                
                var builderFromTemplate = DocumentBuilder.CreateFromTemplate(memoryStream, new object());

                return builderFromTemplate;
            }

            private static  List<Text> ListOfExpectedElementData(Byte[] document)
            {
                 var createdDocument = WordprocessingDocument.Open(new MemoryStream(document), false);
                 var createdText = createdDocument.MainDocumentPart.Document.Body
                    .Descendants<Text>()
                    .ToList();
                
                return createdText;
            }

 
            [Fact]
            public async void ProjectOverviewGenerator_generates_correct_data()
            {
                var result = await _subject.Execute(_projectUrn);
                
                var placeholderDocument = AddPlaceholderToDocument("[ProjectOverview]");

                AddProjectOverviewDetail(placeholderDocument,result.ProjectTemplateModel);
            
                var createdText = ListOfExpectedElementData(placeholderDocument.Build());

                var project = _getTestInformationForProject.Project;

                Assert.Equal("Project Overview", createdText[0].InnerText); 
                Assert.Equal("Recommendation", createdText[1].InnerText);        
                Assert.Equal(project.AcademyAndTrustInformation.Recommendation.ToString(), createdText[2].InnerText);   
                Assert.Equal("Author", createdText[3].InnerText);  
                Assert.Equal(project.AcademyAndTrustInformation.Author, createdText[4].InnerText);
                Assert.Equal("Project name", createdText[5].InnerText);
                Assert.Equal(project.IncomingTrustName, createdText[6].InnerText);    
                Assert.Equal("Date of advisory board", createdText[7].InnerText); 
                Assert.Equal(DateTime.Parse(project.Dates.Htb), DateTime.Parse(createdText[8].InnerText));
                Assert.Equal("Proposed academy transfer date", createdText[9].InnerText);
                Assert.Equal(DateTime.Parse(project.Dates.Target),DateTime.Parse(createdText[10].InnerText));
                Assert.Equal("Reason for this transfer", createdText[11].InnerText);
                Assert.Equal(EnumHelpers<TransferFeatures.ReasonForTheTransferTypes>.GetDisplayValue(project.Features.ReasonForTheTransfer), createdText[12].InnerText);
                Assert.Equal("What type of transfer is it?", createdText[13].InnerText);
                Assert.Equal( EnumHelpers<TransferFeatures.TransferTypes>.GetDisplayValue(project.Features.TypeOfTransfer), createdText[14].InnerText);
            }

            [Fact]   

            public async void BenefitsGenerator_generates_correct_data()
            {
                var result = await _subject.Execute(_projectUrn);
                
                var placeholderDocument = AddPlaceholderToDocument("[BenefitsInformation]");

                AddBenefits(placeholderDocument,result.ProjectTemplateModel);
            
                var createdText = ListOfExpectedElementData(placeholderDocument.Build());

                var benefits = _getTestInformationForProject.Project.Benefits;

                Assert.Equal("Benefits", createdText[0].InnerText); 
                Assert.Equal(EnumHelpers<TransferBenefits.IntendedBenefit>.GetDisplayValue(benefits.IntendedBenefits[0]), createdText[1].InnerText);        
                Assert.Equal(EnumHelpers<TransferBenefits.IntendedBenefit>.GetDisplayValue(benefits.IntendedBenefits[1]), createdText[2].InnerText);   
                
            }

            [Fact]   

            public async void RisksGenerator_generates_correct_data()
            {
                var result = await _subject.Execute(_projectUrn);
                
                var placeholderDocument = AddPlaceholderToDocument("[RisksInformation]");

                AddRisks(placeholderDocument,result.ProjectTemplateModel);
            
                var createdText = ListOfExpectedElementData(placeholderDocument.Build());

                var benefits = _getTestInformationForProject.Project.Benefits;
               
                Assert.Equal("Risks", createdText[0].InnerText); 
                Assert.Equal(EnumHelpers<TransferBenefits.OtherFactor>.GetDisplayValue(TransferBenefits.OtherFactor.HighProfile), createdText[1].InnerText);   
                Assert.Equal(benefits.OtherFactors[TransferBenefits.OtherFactor.HighProfile], createdText[2].InnerText); 
                Assert.Equal(EnumHelpers<TransferBenefits.OtherFactor>.GetDisplayValue(TransferBenefits.OtherFactor.FinanceAndDebtConcerns), createdText[3].InnerText);   
                Assert.Equal(benefits.OtherFactors[TransferBenefits.OtherFactor.FinanceAndDebtConcerns], createdText[4].InnerText); 
                Assert.Equal("Equalities impact assessment considered", createdText[5].InnerText);   
                Assert.Equal("Not Considered", createdText[6].InnerText); 
                
            }

            [Fact]   
            public async void RationaleGenerator_generates_correct_data()
            {
                var result = await _subject.Execute(_projectUrn);
                
                var placeholderDocument = AddPlaceholderToDocument("[RationaleInformation]");

                AddRationale(placeholderDocument,result.ProjectTemplateModel);
            
                var createdText = ListOfExpectedElementData(placeholderDocument.Build());

                var rationale = _getTestInformationForProject.Project.Rationale;
               
                Assert.Equal("Rationale", createdText[0].InnerText); 
                Assert.Equal("Rationale for project", createdText[1].InnerText); 
                Assert.Equal(rationale.Project, createdText[2].InnerText); 
                Assert.Equal("Rational for trust or sponsor ", createdText[3].InnerText); 
                Assert.Equal(rationale.Trust, createdText[4].InnerText);                
            }

            [Fact]   
            public async void LegalRequirementsGenerator_generates_correct_data()
            {
                var result = await _subject.Execute(_projectUrn);
                
                var placeholderDocument = AddPlaceholderToDocument("[LegalInformation]");

                AddLegalRequirementsDetail(placeholderDocument,result.ProjectTemplateModel);
            
                var createdText = ListOfExpectedElementData(placeholderDocument.Build());

                var legalRequirements = _getTestInformationForProject.Project.LegalRequirements;
               
                Assert.Equal("Legal Requirements", createdText[0].InnerText); 
                Assert.Equal("Outgoing trust resolution", createdText[1].InnerText); 
                Assert.Equal(EnumExtensions.ToDescription(legalRequirements.OutgoingTrustConsent), createdText[2].InnerText); 
                Assert.Equal("Incoming trust agreement", createdText[3].InnerText); 
                Assert.Equal(legalRequirements.IncomingTrustAgreement.ToString(), createdText[4].InnerText); 
                Assert.Equal("Diocesan consent", createdText[5].InnerText); 
                Assert.Equal(legalRequirements.DiocesanConsent.ToString(), createdText[6].InnerText);               
            }
    }
}

