using System;
using System.Globalization;
using System.Text;

namespace LINQtoCSV
{
    /// <summary>
    /// Summary description for CsvFileDescription
    /// </summary>
    public class CsvFileDescription
    {
        // Culture used to process the CSV values, specifically numbers and dates.
        private CultureInfo m_cultureInfo = CultureInfo.CurrentCulture;

        private int m_maximumNbrExceptions = 100;

        // Tracks whether the user explicitly set a culture via FileCultureInfo or FileCultureName
        private bool m_isCustomCultureSet = false;

        // --------------

        // Character used to separate fields in the file.
        // By default, this is comma (,).
        // For a tab delimited file, you would set this to
        // the tab character ('\t').
        public char SeparatorChar { get; set; }

        /// <summary>
        /// Gets or sets whether to use thousands separator in number formatting.
        /// Default is true. Set to false for better Excel compatibility.
        /// </summary>
        private bool m_useThousandsSeparator;

        public bool UseThousandsSeparator
        {
            get => m_useThousandsSeparator;
            set
            {
                m_useThousandsSeparator = value;
                // Apply policy only if culture was not explicitly set by the user
                ApplyNumberSeparatorPolicy();
            }
        }

        public bool NoSeparatorChar { get; set; }

        // Only used when writing a file
        //
        // If true, all fields are quoted whatever their content.
        // If false, only fields containing a FieldSeparator character,
        // a quote or a newline are quoted.
        //
        public bool QuoteAllFields { get; set; }

        // If true, then:
        // When writing a file, the column names are written in the
        // first line of the new file.
        // When reading a file, the column names are read from the first
        // line of the file.
        //
        public bool FirstLineHasColumnNames { get; set; }

        // If true, only public fields and properties with the
        // [CsvColumn] attribute are recognized.
        // If false, all public fields and properties are used.
        //
        public bool EnforceCsvColumnAttribute { get; set; }

        // FileCultureName and FileCultureInfo both get/set
        // the CultureInfo used for the file.
        // For example, if the file uses Dutch date and number formats
        // while the current culture is US English, set
        // FileCultureName to "nl-NL".
        //
        // To simply use the current culture, leave the culture as is.
        //
        public string FileCultureName
        {
            get { return m_cultureInfo.Name; }
            set
            {
                m_cultureInfo = new CultureInfo(value);
                m_isCustomCultureSet = true;
            }
        }

        public CultureInfo FileCultureInfo
        {
            get { return m_cultureInfo; }
            set
            {
                m_cultureInfo = value;
                // Mark that a custom culture was provided; do not mutate it automatically
                m_isCustomCultureSet = true;
            }
        }

        // When reading a file, exceptions thrown while the file is being read
        // are captured in an aggregate exception. That aggregate exception is then
        // thrown at the end - to make it easier to solve multiple problems with the
        // input file in one.
        //
        // However, after MaximumNbrExceptions, the aggregate exception is thrown
        // immediately.
        //
        // To not have a maximum at all, set this to -1.
        public int MaximumNbrExceptions
        {
            get { return m_maximumNbrExceptions; }
            set { m_maximumNbrExceptions = value; }
        }

        // Character encoding. Defaults should work in most cases.
        // However, when reading or writing non-English files, you may want to use
        // Unicode encoding.
        public Encoding TextEncoding { get; set; }

        public bool DetectEncodingFromByteOrderMarks { get; set; }

        public bool UseFieldIndexForReadingData { get; set; }
        public bool UseOutputFormatForParsingCsvValue { get; set; }
        public bool IgnoreTrailingSeparatorChar { get; set; }

        /// <summary>
        /// If set to true, wil read only the fields specified as attributes, and will discard other fields in the CSV file
        /// </summary>
        public bool IgnoreUnknownColumns { get; set; }

        // ---------------

        public CsvFileDescription()
        {
            m_cultureInfo = CultureInfo.CurrentCulture;
            FirstLineHasColumnNames = true;
            EnforceCsvColumnAttribute = false;
            QuoteAllFields = false;
            SeparatorChar = ',';
            TextEncoding = Encoding.UTF8;
            DetectEncodingFromByteOrderMarks = true;
            IgnoreTrailingSeparatorChar = false;
            NoSeparatorChar = false;
            UseFieldIndexForReadingData = false;
            UseOutputFormatForParsingCsvValue = false;
            IgnoreUnknownColumns = false;
            m_useThousandsSeparator = true; // default

            // Ensure initial culture respects the thousands separator policy if no custom culture is set yet
            ApplyNumberSeparatorPolicy();
        }

        /// <summary>
        /// Sets up Excel-compatible culture and format settings. This improves compatibility when
        /// exporting CSV files that will be opened in Excel.
        /// </summary>
        /// <param name="useCommaDecimalSeparator">If true, use comma as decimal separator and semicolon as field separator.
        /// If false, use period as decimal separator and comma as field separator.</param>
        public void SetExcelCompatible(bool useCommaDecimalSeparator = false)
        {
            // Beállítja a kultúrát a tizedes elválasztó alapján
            FileCultureInfo = CreateExcelCompatibleCulture(useCommaDecimalSeparator);

            // Excel számára jobb, ha csak szükség esetén idézőjelezzük a mezőket
            QuoteAllFields = false;

            // Ha vesszőt használunk tizedes elválasztóként, akkor pontosvesszőt használunk mezőelválasztóként
            SeparatorChar = useCommaDecimalSeparator ? ';' : ',';

            // Kikapcsoljuk az ezres elválasztót az Excel-kompatibilitás érdekében
            UseThousandsSeparator = false;
        }

        /// <summary>
        /// Creates an Excel-compatible culture for number formatting.
        /// </summary>
        /// <param name="useCommaDecimalSeparator">If true, use comma as decimal separator. If false, use period.</param>
        /// <returns>A CultureInfo object configured for Excel compatibility</returns>
        public static CultureInfo CreateExcelCompatibleCulture(bool useCommaDecimalSeparator = false)
        {
            // Angol vagy magyar alapú kultúra, a paramétertől függően
            CultureInfo ci = useCommaDecimalSeparator ?
                new CultureInfo("hu-HU") :
                new CultureInfo("en-US");

            // Tizedes elválasztó beállítása
            ci.NumberFormat.NumberDecimalSeparator = useCommaDecimalSeparator ? "," : ".";

            // Ezres elválasztó eltávolítása (üres string)
            ci.NumberFormat.NumberGroupSeparator = "";

            return ci;
        }

        /// <summary>
        /// Applies the thousands separator policy to the internal culture only if no custom culture was explicitly set.
        /// Keeps the decimal separator intact.
        /// </summary>
        private void ApplyNumberSeparatorPolicy()
        {
            if (m_isCustomCultureSet)
            {
                // Do not mutate user-provided cultures automatically
                return;
            }

            if (!m_useThousandsSeparator && m_cultureInfo != null)
            {
                // Work on a private copy: the default culture is CultureInfo.CurrentCulture,
                // which may be read-only or shared by the whole thread.
                m_cultureInfo = (CultureInfo)m_cultureInfo.Clone();
                string decimalSeparator = m_cultureInfo.NumberFormat.NumberDecimalSeparator;
                m_cultureInfo.NumberFormat.NumberGroupSeparator = "";
                m_cultureInfo.NumberFormat.NumberDecimalSeparator = decimalSeparator;
            }
        }
    }
}