using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml.Wordprocessing;
using Xunit;

namespace DocumentGeneration.Tests
{
    public class DocumentTableBuilderTests
    {
        [Fact]
        public void GivenSingleTableCell_AppendsATableWithASingleCell()
        {
            var body = new Body();
            var builder = new DocumentTableBuilder(body);
            builder.AddTableRow(new List<string> {"Meow"});
            builder.Build();

            var createdTableCells = body.Descendants<TableCell>().ToList();
            var cellContents = createdTableCells.Select(cell => cell.InnerText).ToList();

            Assert.Single(createdTableCells);
            Assert.Equal("Meow", cellContents[0]);
        }

        [Fact]
        public void GivenMultipleCellsInOneRow_AppendsATableWithAMultiCellRow()
        {
            var body = new Body();
            var builder = new DocumentTableBuilder(body);
            var cellData = new List<string> {"Meow", "Woof", "Quack", "Moo"};

            builder.AddTableRow(cellData);
            builder.Build();

            var createdTableCells = body.Descendants<TableCell>().ToList();
            var cellContents = createdTableCells.Select(cell => cell.InnerText).ToList();

            Assert.Equal(4, createdTableCells.Count);
            Assert.Equal(cellData, cellContents);
        }

        [Fact]
        public void GivenMultipleRows_AppendsASingleTableWithAMultipleRows()
        {
            var body = new Body();
            var builder = new DocumentTableBuilder(body);
            var rowOneData = new List<string> {"Meow", "Woof", "Quack", "Moo"};
            var rowTwoData = new List<string> {"Cluck", "Howl", "Purr", "Tweet"};

            builder.AddTableRow(rowOneData);
            builder.AddTableRow(rowTwoData);
            builder.Build();

            var createdTables = body.Descendants<Table>().ToList();
            var createdTableRows = createdTables.First().Descendants<TableRow>().ToList();
            var rowContents = createdTableRows
                .Select(row =>
                    row.Descendants<TableCell>().Select(cell => cell.InnerText).ToList()
                ).ToList();

            Assert.Single(createdTables);
            Assert.Equal(2, createdTableRows.Count);
            Assert.Equal(rowOneData, rowContents[0]);
            Assert.Equal(rowTwoData, rowContents[1]);
        }
    }
}