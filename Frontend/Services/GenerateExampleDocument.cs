using System.Collections.Generic;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Frontend.Services
{
    public class GenerateExampleDocument
    {
        private MemoryStream _memoryStream;
        private readonly DocumentBuilder _builder;

        private class BuilderTableCell
        {
            public string Text = "";
            public BuilderTableCellOptions Options;
        }

        private class BuilderTableCellOptions
        {
            public bool Bold;
            public bool MergeAbove;
        }

        private class DocumentBuilder
        {
            private readonly WordprocessingDocument _document;
            private readonly MainDocumentPart _mainPart;
            public readonly Body Body;

            public DocumentBuilder(MemoryStream memoryStream)
            {
                _document = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document);
                _mainPart = _document.AddMainDocumentPart();
                _mainPart.Document = new Document(new Body());
                Body = _mainPart.Document.Body;
            }

            public void AddParagraphToDocument(string text)
            {
                var run = new Run(new Text(text));
                var runProperties = new RunProperties();
                runProperties.AppendChild(new Bold());
                run.RunProperties = runProperties;

                var para = new Paragraph(run);
                Body.AppendChild(para);
            }

            public void AddTable(List<List<BuilderTableCell>> data)
            {
                var table = new Table();

                var tableProperties = new TableProperties
                {
                    TableBorders = new TableBorders
                    {
                        TopBorder = new TopBorder
                            {Color = "000000", Val = BorderValues.Single, Size = 8, Space = 0},
                        RightBorder = new RightBorder
                            {Color = "000000", Val = BorderValues.Single, Size = 8, Space = 0},
                        BottomBorder = new BottomBorder
                            {Color = "000000", Val = BorderValues.Single, Size = 8, Space = 0},
                        LeftBorder = new LeftBorder
                            {Color = "000000", Val = BorderValues.Single, Size = 8, Space = 0},
                        InsideVerticalBorder = new InsideVerticalBorder
                            {Color = "000000", Val = BorderValues.Single, Size = 8, Space = 0},
                        InsideHorizontalBorder = new InsideHorizontalBorder
                            {Color = "000000", Val = BorderValues.Single, Size = 8, Space = 0},
                    }
                };

                table.AppendChild(tableProperties);

                data.ForEach(dataRow =>
                {
                    var tableRow = new TableRow();
                    dataRow.ForEach(dataCell =>
                    {
                        var tableCell = new TableCell();
                        var tableCellProperties = new TableCellProperties
                        {
                            VerticalMerge = new VerticalMerge
                            {
                                Val = MergedCellValues.Restart
                            }
                        };

                        if (dataCell.Options != null && dataCell.Options.MergeAbove)
                        {
                            tableCellProperties.VerticalMerge.Val = MergedCellValues.Continue;
                        }

                        tableCell.TableCellProperties = tableCellProperties;

                        var paragraph = new Paragraph();
                        var run = new Run(new Text(dataCell.Text));

                        if (dataCell.Options != null && dataCell.Options.Bold)
                        {
                            run.RunProperties = new RunProperties(new Bold());
                        }

                        paragraph.AppendChild(run);
                        tableCell.AppendChild(paragraph);
                        tableRow.AppendChild(tableCell);
                    });
                    table.AppendChild(tableRow);
                });

                Body.AppendChild(table);
            }

            public void Build()
            {
                _document.Save();
                _document.Close();
            }

            public void AddLineBreak()
            {
                AddParagraphToDocument("");
            }
        }

        public GenerateExampleDocument(MemoryStream memoryStream)
        {
            _memoryStream = memoryStream;
            _builder = new DocumentBuilder(_memoryStream);
        }

        public void Execute()
        {
            var rows = new List<List<BuilderTableCell>>
            {
                new List<BuilderTableCell>
                {
                    new BuilderTableCell {Text = "Recommendation", Options = new BuilderTableCellOptions {Bold = true}},
                    new BuilderTableCell {Text = "A bunch of example text etc etc with some more to pad the text out"},
                    new BuilderTableCell {Text = "Date:", Options = new BuilderTableCellOptions {Bold = true}},
                    new BuilderTableCell {Text = "01/09/2020"}
                },
                new List<BuilderTableCell>
                {
                    new BuilderTableCell {Options = new BuilderTableCellOptions {MergeAbove = true}},
                    new BuilderTableCell {Options = new BuilderTableCellOptions {MergeAbove = true}},
                    new BuilderTableCell {Text = "Author:", Options = new BuilderTableCellOptions {Bold = true}},
                    new BuilderTableCell {Text = "Meow Meowington"}
                },
                new List<BuilderTableCell>
                {
                    new BuilderTableCell
                        {Text = "Is AO Required?", Options = new BuilderTableCellOptions {Bold = true}},
                    new BuilderTableCell {Text = "No"},
                    new BuilderTableCell {Text = "Cleared by:", Options = new BuilderTableCellOptions {Bold = true}},
                    new BuilderTableCell {Text = "Barks Barkington"}
                }
            };

            _builder.AddTable(rows);
            _builder.AddLineBreak();
            _builder.AddTable(rows);
            _builder.Build();
        }
    }
}