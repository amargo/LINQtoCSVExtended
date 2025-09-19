# Number Formatting Guide for LINQtoCSV Library

This documentation describes how to properly configure number formatting in the LINQtoCSV library, with special focus on Excel compatibility.

## Number Formatting Options

The LINQtoCSV library offers several methods for formatting numeric data during export, ensuring that numbers are properly displayed and can be processed in other applications, such as Excel.

### 1. `OutputFormat` Property in the `CsvColumn` Attribute

This is the simplest way to format numbers. By setting the `OutputFormat` property of the `CsvColumn` attribute, you can specify the exact formatting:

```csharp
[CsvColumn(FieldIndex = 1, OutputFormat = "0.00")]
public decimal Price { get; set; }
```

#### Common Formatting Patterns:

| Format | Description | Example |
|--------|-------------|---------|
| `"0"` | Integer without decimal places | 42 |
| `"0.00"` | Fixed two decimal places | 42.00 |
| `"0.####"` | Up to four decimal places, but only if needed | 42.1 or 42.1234 |
| `"F2"` | Fixed two decimal places | 42.00 |
| `"N0"` | Integer without thousands separator | 42 |
| `"C"` | Currency format | $42.00 |

### 2. Setting `FileCultureInfo`

In the `CsvFileDescription` object, you can set the culture that determines the default number and date formats:

```csharp
var csvFileDescription = new CsvFileDescription
{
    FileCultureInfo = new CultureInfo("en-US") // uses period as decimal separator
    // or
    // FileCultureInfo = new CultureInfo("hu-HU") // uses comma as decimal separator
};
```

### 3. Customizing `CultureInfo`

For special requirements, you can create a custom culture:

```csharp
var culture = new CultureInfo("en-US");
// Disable thousands separator
culture.NumberFormat.NumberGroupSeparator = "";
// Set decimal separator to period
culture.NumberFormat.NumberDecimalSeparator = ".";

var csvFileDescription = new CsvFileDescription
{
    FileCultureInfo = culture
};
```

## Excel-Compatible Export

For Excel compatibility, it's particularly important to:

1. **Use the appropriate decimal separator**: Use the decimal separator expected by Excel (typically a period in English versions)
2. **Omit the thousands separator**: Thousands separators can make it difficult for numbers to be processed
3. **Use clear number formats**: Use formats that Excel will definitely recognize as numbers

### Example of Excel-Compatible Configuration

```csharp
// Model definition
public class MyExcelData
{
    [CsvColumn(FieldIndex = 1, OutputFormat = "0.00")]  // Period as separator, two decimal places
    public decimal Amount { get; set; }
    
    [CsvColumn(FieldIndex = 2, OutputFormat = "0")]  // Integer
    public int Quantity { get; set; }
    
    [CsvColumn(FieldIndex = 3, OutputFormat = "0.####")] // Up to four decimal places if needed
    public double ExchangeRate { get; set; }
}

// When writing CSV
var csvContext = new CsvContext();
var csvFileDescription = new CsvFileDescription
{
    SeparatorChar = ',',
    FileCultureInfo = new CultureInfo("en-US") { // English format, Excel-compatible
        NumberFormat = {
            NumberGroupSeparator = "", // No thousands separator
            NumberDecimalSeparator = "." // Period as decimal separator
        }
    },
    // Important for Excel text recognition
    QuoteAllFields = false  // Only quote when necessary
};

csvContext.Write(data, "exported_data.csv", csvFileDescription);
```

## Tips and Tricks

- If Excel incorrectly recognizes numbers, check if you're using the appropriate culture and formatting settings
- The `OutputFormat` property overrides culture settings for a specific field
- The `UseOutputFormatForParsingCsvValue` setting also uses the `OutputFormat` for parsing when reading
- `CsvFileDescription.FileCultureInfo` globally affects the interpretation and formatting of numbers and dates

## New features and global settings (LINQtoCSVExtended)

- **SetExcelCompatible(bool useCommaDecimalSeparator = false)**
  - Apply Excel-compatible settings in one step: culture, field separator, and disabling thousands separators.
  - When `useCommaDecimalSeparator = false`: decimal point (.), field separator `,`.
  - When `useCommaDecimalSeparator = true`: decimal comma (,), field separator `;`.

- **CreateExcelCompatibleCulture(bool useCommaDecimalSeparator = false)**
  - Returns a `CultureInfo` configured for Excel-friendly number formatting (correct decimal separator, no thousands grouping).

- **UseThousandsSeparator property on CsvFileDescription**
  - Default: `true`. If set to `false` and NO explicit `FileCultureInfo`/`FileCultureName` is provided, the internal culture removes thousands grouping.
  - If an explicit `FileCultureInfo`/`FileCultureName` is provided, the library does not mutate that custom culture automatically.

- **Behavior of FileCultureInfo and FileCultureName**
  - When explicitly set, your custom culture is respected as-is. For Excel compatibility, either use `SetExcelCompatible` or adjust `NumberFormat` manually.

- **Default formatting by type (works even without attributes)**
  - Integers (`int`, `long`, `short`, `byte`): "D" (no decimal places).
  - Floating-point/currency (`double`, `float`, `decimal`): "F" (fixed-point with culture decimal digits).
  - Other types: "G".
  - If a field has an `OutputFormat`, it overrides the default.

Note: Parsing based on `OutputFormat` can be enabled via `UseOutputFormatForParsingCsvValue = true`.

## Summary

The LINQtoCSV library offers flexible options for number formatting, ensuring that exported CSV data is properly displayed and can be processed in other applications, especially Excel. By using appropriate formatting settings, you can avoid data conversion issues and ensure that Excel correctly recognizes numeric data.
