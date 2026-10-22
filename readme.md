# LINQtoCSV

This project is actively developed at: https://github.com/amargo/LINQtoCSVExtended

## Documentation

- Number formatting: [NumberFormatting_EN.md](./docs/NumberFormatting_EN.md)

### Default number formatting when writing

When a field or property has no `OutputFormat` (including anonymous types and classes without
`[CsvColumn]`), the writer picks a format by type:

| Type | Format | Result |
|------|--------|--------|
| `int`, `long`, `short`, `byte` | `"D"` | `42` |
| `double`, `float`, `decimal` | `"F"` | decimals follow `FileCultureInfo.NumberFormat.NumberDecimalDigits` |
| anything else | `"G"` | |

So the number of decimals is controlled by the culture: with a `hu-HU` culture whose
`NumberDecimalDigits` is `0`, `1234.567m` is written as `1235`; with `2` it is `1234,57`.
This behaviour is relied on by consumers and is locked by a regression test. Set `OutputFormat`
on the column to override it.

## Changes in 1.7.0

- `UseThousandsSeparator = false` works on a private copy of the culture; it no longer modifies
  (or throws on a read-only) `CultureInfo.CurrentCulture`.
- Reading with `IgnoreUnknownColumns` + `EnforceCsvColumnAttribute` no longer throws
  `IndexOutOfRangeException` when unknown columns precede known ones.
- `AggregatedException` is rethrown with its original stack trace.
- Target frameworks: `netstandard2.0`, `net8.0`, `net48` (`net6.0` dropped: .NET 6/7 apps resolve
  `netstandard2.0`, .NET 8+ apps `net8.0`). Symbol package (`.snupkg`) and Source Link included.

This library makes it easy to use CSV files with LINQ queries. Its features include:

* Follows the most common rules for CSV files. Correctly handles data fields that contain commas and line breaks.
* In addition to comma, most delimiting characters can be used, including tab for tab-delimited fields.
* Can be used with an IEnumerable of an anonymous class - which is often returned by a LINQ query.
* Supports deferred reading.
* Supports processing files with international date and number formats.
* Supports different character encodings if you need them.
* Recognizes a wide variety of date and number formats when reading files.
* Provides fine control of date and number formats when writing files.
* Robust error handling, allowing you to quickly find and fix problems in large input files.

Full documentation is at  
http://www.codeproject.com/Articles/25133/LINQ-to-CSV-library
