# SDD Word Document Generation

A library for aiding the generation of word documents in C# applications within SDD.


## Live examples

The [SDD Academy Transfers service](https://github.com/dfe-digital/academy-transfers-api) uses this for word document generation.

## Technical documentation

This is a .NET library which uses the [Open-XML SDK](https://github.com/OfficeDev/Open-XML-SDK) provided by microsoft in order to generate word documents. Its aim is to
hide away some of the finer details of document creation to speed up the creation of word documents throughout SDD.

### Example

```c#
class GenerateDocument {
    public byte[] Execute(){
        MemoryStream ms;

        await using (ms = new MemoryStream())
        {
            var builder = new DocumentBuilder(ms);
            builder.AddParagraph(pBuilder => {
                pBuilder.AddText("This is some text");
            }
            builder.Build();
        }

        return ms.ToArray()
    }
}

```

### Usage

#### `DocumentBuilder`

The main class to interact with is the `DocumentBuilder`.

`DocumentBuilder(Stream stream)`

The `DocumentBuilder` takes a stream to write the created document to, once this has been created you can call
the methods available to build up your document. 

Once you have added your content, you call the `Build()` method and your document will be written to the stream.

#### `TextElement`s

Text elements allow you to create text with formatting applied. At the moment, the following formatting is supported:

- Bold - true/false
- Underline - true/false
- Italic - true/false
- FontSize - In half points
- Colour - Colour hex code

The content of the text element is stored inside the `Value` field, and can be passed in the contructor.

Examples:

```
var boldText = new TextElement("Bold text") { Bold = true };
var multipleFormatting = new TextElement("Multiple") { Bold = true, Italic = true, Color = "FF69B4" }
```

#### Paragraphs

`DocumentBuilder.AddParagraph(Action<IParagraphBuilder> action)`

To create a paragraph of text you must pass in a function that uses the `IParagraphBuilder` interface to
build up a paragraph from the given text.

**Adding content**

The `ParagraphBuilder` lets you build paragraphs in the following ways:

- `AddText(string text)`
  - Adds text to the paragraph, this is the standard approach when no formatting is needed.
- `AddText(TextElement textElement)`
  - Adds a `TextElement` to the paragraph, this is the standard approach when adding an item of text that needs formatting.
- `AddText(TextElement[] text)`
  - Adds multiple `TextElements`s to the paragraph, this is useful when you want to add multiple pieces of text to the paragraph with mixed formatting between them.
- `AddNewLine()`
  - Adds a new line to the paragraph

**Formatting**

The `ParagraphBuilder` currently lets you format the paragraph in the following ways:

- `Justify(ParagraphJustification)`
  - Sets the justification of the paragraph to Left, center, or right.

#### Headings

`DocumentBuilder.AddHeading(Action<IHeadingBuilder> action)`

To create a heading, you must pass in a function that uses the `IHeadingBuilder` interface to build up a heading.

**Adding content**

You can add a single element of text in the following ways:

- `AddText(string text)`
  - Used for adding an item of text with no formatting
- `AddText(TextElement text)`
  - Used for adding an item of text with formatting


#### Lists

`DocumentBuilder.AddNumberedList(Action<IListBuilder> action)`

`DocumentBuilder.AddBulletedList(Action<IListBuilder> action)`

To create a list of either type, you must pass in a function that uses the `IListBuilder` interface to build up
a list.

**Types of lists**

- Numbered list
- Bulleted list

**Adding content**

The `ListBuilder` lets you add list items in the following ways:

- `AddItem(string item)`
  - Adds a string as a list item, the standard approach when no formatting is needed.
- `AddItem(TextElement item)`
  - Adds a `TextElement` as a list item for when formatting is needed
- `AddItem(TextElement[] elements)`
  - Adds multiple `TextElement`s as a list item for when a mix of formatting is needed
- `AddItem(Action<IParagraphBuilder> action)`
  - Adds a paragraph built by a paragraph builder as a list item. This is useful if you need to create a list item that needs some features of the paragraph builder (e.g. Adding a new line, or any future formatting functionality)

#### Tables

**Adding a new table**

Tables can be added to the document in the following ways:

- `DocumentBuilder.AddTable(Action<ITableBuilder> action)`
  - Lets you add a table via the `ITableBuilder` (see below)
- `DocumentBuilder.AddTable(IEnumberable<TextElement[]> rows)`
  - Lets you add a table quickly via a 2D array of `TextElement`s. This can be used when you want to quickly create a table made up of one `TextElement` each with no mixed formatting. This creates a table using the default styling (a black border)

**Using the `ITableBuilder`**

If you want more fine-grained control over creating your tables, you can use the `ITableBuilder` to build them.

**Formatting**

You can set the formatting of the table in the following ways:

- `SetBorderStyle(TableBorderStyle style)`
  - Sets the border style, currently two are available: `Solid` and `None`

**Adding a row**

`AddRow(Action<ITableRowBuilder> action)`

Adding a row to the table uses the `ITableRowBuilder`, letting you have fine-grained control over how cells get added to the rows.

The row builder allows you to add cells in the following ways:

- `AddCell(string text)`
  - Adds a single cell with the given text
- `AddCell(TextElement textElement)`
  - Adds a single cell with the given `TextElement`
- `AddCells(string[] text)`
  - Adds multiple cells with the given strings, useful for creating a whole row at once
- `AddCells(TextElement[] text)`
  - Adds multiple cells with the given `TextElements`s, useful for creating a whole row at once with differently formatted cells.

#### Headers/Footers

`DocumentBuilder.AddHeader(Action<IHeaderBuilder> action)`

`DocumentBuilder.AddFooter(Action<IFooterBuilder> action)`

Adds a header and footer using the given builders, these both expose the same interface.

**Adding content**

Headers and footers can both hold paragraphs and tables, as such using the builder you can add content using the methods given above under Paragraphs and Tables

### Converting HTML content

You can make use of the `HtmlToDocument.Convert` helper to convert HTML content into word document content with a given builder.

```c#

class GenerateDocument {
    public byte[] Execute(){
        MemoryStream ms;

        await using (ms = new MemoryStream())
        {
            var builder = new DocumentBuilder(ms);
            HtmlToDocument.Convert(builder, "<p>Some html content</p>");
            builder.Build();
        }

        return ms.ToArray()
    }
}
```