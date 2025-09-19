# LINQtoCSV

This project is actively developed at: https://github.com/amargo/LINQtoCSVExtended

## Documentation

- Number formatting: [NumberFormatting_EN.md](./docs/NumberFormatting_EN.md)

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
