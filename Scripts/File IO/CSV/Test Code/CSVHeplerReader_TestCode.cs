using System.Globalization;
using System.IO;
using System.Text;
using CsvHelper;

public class CSVHeplerReader_TestCode
{

    public static void CSVHeplerReadCSVFile()
    {
      //  if (!CSV_Reader_Processor.CSVRead(CSVDataGenerator.CSV_TEST_CASE_FILE_PATH)) return;
        
        StringBuilder stringBuilder = new StringBuilder();
        using (StreamReader streamReader = new StreamReader(CSV_Reader_Processor.FilePath))
        using (var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture))
        {
            while (csv.Read())
            {
                stringBuilder.Append(csv.Parser.Record);
            }
        }
    }
}
