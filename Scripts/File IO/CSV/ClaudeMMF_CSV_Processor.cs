using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ClaudeMMF_CSV_Processor : MonoBehaviour
{

    // Configuration
    [SerializeField] private string csvFilePath = CSVDataGenerator.CSV_TEST_CASE_FILE_PATH;
    [SerializeField] private int bufferSize = 4096; // Chunk size for processing

    [ContextMenu("Process CSV Using Memory Mapped File")]
    public void ProcessCSV()
    {
        string filePath = Path.Combine(Application.dataPath, csvFilePath);
        
        if (!File.Exists(filePath))
        {
            Debug.LogError($"CSV file not found at path: {filePath}");
            return;
        }

        FileInfo fileInfo = new FileInfo(filePath);
        long fileSize = fileInfo.Length;

        var stopwatch = Stopwatch.StartNew();
        List<Dictionary<string, string>> parsedData = new List<Dictionary<string, string>>();
        
        try
        {
            using (var mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open))
            {
                // Process the file using line-based approach
                ProcessCSVLines(mmf, fileSize, parsedData);
            }
            
            stopwatch.Stop();
            Debug.Log($"CSV processed successfully using Memory Mapped File!");
            Debug.Log($"Time taken: {stopwatch.ElapsedMilliseconds}ms");
            Debug.Log($"Records processed: {parsedData.Count}");
            
            // Optionally display some sample data
            if (parsedData.Count > 0)
            {
                Debug.Log("Sample data (first record):");
                foreach (var kvp in parsedData[0])
                {
                    Debug.Log($"  {kvp.Key}: {kvp.Value}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error processing CSV: {ex.Message}");
            Debug.LogException(ex);
        }
    }

    private void ProcessCSVLines(MemoryMappedFile mmf, long fileSize, List<Dictionary<string, string>> results)
    {
        string[] headers = null;
        StringBuilder lineBuilder = new StringBuilder();
        bool inQuotes = false;
        long position = 0;
        int bufferOffset = 0;
        byte[] buffer = new byte[bufferSize];

        using (var accessor = mmf.CreateViewAccessor(0, fileSize, MemoryMappedFileAccess.Read))
        {
            while (position < fileSize)
            {
                // Calculate how many bytes to read in this chunk
                int bytesToRead = (int)Math.Min(bufferSize, fileSize - position);
                
                // Read a chunk of data
                accessor.ReadArray(position, buffer, 0, bytesToRead);
                position += bytesToRead;
                
                // Process each byte in the buffer
                for (int i = 0; i < bytesToRead; i++)
                {
                    char c = (char)buffer[i];
                    
                    // Handle quoted fields (ignore line breaks and commas within quotes)
                    if (c == '"')
                    {
                        inQuotes = !inQuotes;
                        lineBuilder.Append(c);
                    }
                    // When we reach a line break outside of quotes, process the line
                    else if ((c == '\n' || (c == '\r' && i + 1 < bytesToRead && buffer[i + 1] == '\n')) && !inQuotes)
                    {
                        if (c == '\r') // Skip the following \n for \r\n line endings
                        {
                            i++; // Skip the next character (\n)
                        }
                        
                        string line = lineBuilder.ToString().Trim();
                        if (!string.IsNullOrEmpty(line))
                        {
                            if (headers == null)
                            {
                                // First line contains headers
                                headers = ParseCSVLine(line);
                            }
                            else
                            {
                                // Parse data line and add to results
                                string[] values = ParseCSVLine(line);
                                if (values.Length > 0)
                                {
                                    var dataRow = new Dictionary<string, string>();
                                    for (int j = 0; j < Math.Min(headers.Length, values.Length); j++)
                                    {
                                        dataRow[headers[j]] = values[j];
                                    }
                                    results.Add(dataRow);
                                }
                            }
                        }
                        
                        lineBuilder.Clear();
                    }
                    else
                    {
                        lineBuilder.Append(c);
                    }
                }
            }
            
            // Process any remaining content
            string remainingLine = lineBuilder.ToString().Trim();
            if (!string.IsNullOrEmpty(remainingLine) && headers != null)
            {
                string[] values = ParseCSVLine(remainingLine);
                if (values.Length > 0)
                {
                    var dataRow = new Dictionary<string, string>();
                    for (int j = 0; j < Math.Min(headers.Length, values.Length); j++)
                    {
                        dataRow[headers[j]] = values[j];
                    }
                    results.Add(dataRow);
                }
            }
        }
    }
    
    private string[] ParseCSVLine(string line)
    {
        List<string> result = new List<string>();
        StringBuilder field = new StringBuilder();
        bool inQuotes = false;
        
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            
            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    // Escaped quote (double quote)
                    field.Append('"');
                    i++; // Skip the next quote
                }
                else
                {
                    // Toggle quote mode
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                // End of field
                result.Add(field.ToString());
                field.Clear();
            }
            else
            {
                field.Append(c);
            }
        }
        
        // Add the last field
        result.Add(field.ToString());
        
        return result.ToArray();
    }
    
    [ContextMenu("Compare With Standard File IO")]
    public void CompareWithFileIO()
    {
        string filePath = Path.Combine(Application.dataPath, csvFilePath);
        
        if (!File.Exists(filePath))
        {
            Debug.LogError($"CSV file not found at path: {filePath}");
            return;
        }
        
        // Test memory mapped file approach
        var mmfStopwatch = Stopwatch.StartNew();
        int mmfRecordCount = 0;
        
        using (var mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open))
        {
            List<Dictionary<string, string>> parsedData = new List<Dictionary<string, string>>();
            ProcessCSVLines(mmf, new FileInfo(filePath).Length, parsedData);
            mmfRecordCount = parsedData.Count;
        }
        
        mmfStopwatch.Stop();
        long mmfTime = mmfStopwatch.ElapsedMilliseconds;
        
        // Test standard file IO approach
        var fileIOStopwatch = Stopwatch.StartNew();
        int fileIORecordCount = 0;
        
        using (var reader = new StreamReader(filePath))
        {
            string headerLine = reader.ReadLine();
            if (headerLine != null)
            {
                string[] headers = ParseCSVLine(headerLine);
                
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        fileIORecordCount++;
                    }
                }
            }
        }
        
        fileIOStopwatch.Stop();
        long fileIOTime = fileIOStopwatch.ElapsedMilliseconds;
        
        // Report results
        Debug.Log("=== Performance Comparison ===");
        Debug.Log($"Memory Mapped File: {mmfTime}ms, {mmfRecordCount} records");
        Debug.Log($"Standard File IO: {fileIOTime}ms, {fileIORecordCount} records");
        Debug.Log($"Difference: {Math.Abs(mmfTime - fileIOTime)}ms");
        
        if (mmfTime < fileIOTime)
        {
            Debug.Log($"Memory Mapped File was faster by {(double)fileIOTime / mmfTime:F2}x");
        }
        else if (fileIOTime < mmfTime)
        {
            Debug.Log($"Standard File IO was faster by {(double)mmfTime / fileIOTime:F2}x");
        }
        else
        {
            Debug.Log("Both methods took the same amount of time");
        }
    }
}
