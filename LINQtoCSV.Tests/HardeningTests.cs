using System;
using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LINQtoCSV.Tests
{
    [TestClass]
    public class HardeningTests : Test
    {
        private static CultureInfo HuCulture(int decimalDigits)
        {
            var culture = new CultureInfo("hu-HU");
            culture.NumberFormat.NumberDecimalSeparator = ",";
            culture.NumberFormat.NumberGroupSeparator = string.Empty;
            culture.NumberFormat.NumberDecimalDigits = decimalDigits;
            return culture;
        }

        private static void WithCurrentCulture(CultureInfo culture, Action action)
        {
            CultureInfo original = Thread.CurrentThread.CurrentCulture;
            try
            {
                Thread.CurrentThread.CurrentCulture = culture;
                action();
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = original;
            }
        }

        // Locks the a836c37 behaviour: attribute-less double/decimal fields are written with "F",
        // so the number of decimals follows the culture's NumberDecimalDigits.
        [DataTestMethod]
        [DataRow(0, "a;1235;3;42")]
        [DataRow(2, "a;1234,57;3,14;42")]
        public void Write_AttributeLessNumbers_UseCultureNumberDecimalDigits(int decimalDigits, string expectedRow)
        {
            var rows = new[] { new { Name = "a", Amount = 1234.567m, Ratio = 3.14159d, Count = 42 } };
            var description = new CsvFileDescription
            {
                SeparatorChar = ';',
                FirstLineHasColumnNames = true,
                FileCultureInfo = HuCulture(decimalDigits)
            };

            WithCurrentCulture(CultureInfo.InvariantCulture, () =>
                AssertWrite(rows, description, "Name;Amount;Ratio;Count\n" + expectedRow + "\n"));
        }
    }
}
