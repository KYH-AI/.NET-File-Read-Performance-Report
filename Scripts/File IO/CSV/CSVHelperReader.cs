using System.IO;
using System.Globalization;
using UnityEngine;
using CsvHelper;


public class CSVHelperReader : MonoBehaviour
{
    [ContextMenu("(CSVHelper) CSV 파일 읽기 시작")]
    public void CSVHelperReadCSVFile()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        using (StreamReader streamReader = new StreamReader(CSV_Reader_Processor.FilePath))
        using (var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture))
        {
            string data = streamReader.ReadToEnd();
            stopwatch.Stop();
            Debug.Log($"CSVHelper 읽기 완료! 시간: {stopwatch.ElapsedMilliseconds}ms");
            TestCaseFileInfo.TestTimer.Add(stopwatch.ElapsedMilliseconds);
        }
    }
}
