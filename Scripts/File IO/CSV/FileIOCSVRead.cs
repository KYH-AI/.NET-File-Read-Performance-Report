using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class FileIOCSVRead : MonoBehaviour
{
    private TestData[] _testData;

#region File (Only Sequential Access)

    [ContextMenu("(File ReadAllText) CSV 파일 읽기 - Sequential Access")]
    public void File_ReadAllTextCSVFile()
    {
        try
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            // CSV 파일의 전체 내용을 읽기
            // string csvData = File.ReadAllText(CSV_Reader_Processor.FilePath);
            File.ReadAllText(CSV_Reader_Processor.FilePath);
     
            stopwatch.Stop();
            Debug.Log($"File ReadAllText 읽기 완료! 시간: {stopwatch.ElapsedMilliseconds}ms");
            TestCaseFileInfo.TestTimer.Add(stopwatch.ElapsedMilliseconds);
            
            // CSV 파싱
            //CSVDataGenerator.ParseCSV(csvData, _testData);
            _testData = null;
        }
        catch (IOException ex)
        {
            Debug.LogError($"CSV 파일을 읽는 중 오류 발생: {ex.Message}");
        }
        

    }

    [ContextMenu("(File ReadAllBytes) CSV 파일 읽기 - Sequential Access")]
    public void File_ReadAllBytesCSVFile()
    {
        try
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            // CSV 파일 전체를 바이트 배열로 읽기
            // byte[] fileBytes = File.ReadAllBytes(CSV_Reader_Processor.FilePath);
            File.ReadAllBytes(CSV_Reader_Processor.FilePath);

            stopwatch.Stop();
            Debug.Log($"File ReadAllBytes 읽기 완료! 시간: {stopwatch.ElapsedMilliseconds}ms");
            TestCaseFileInfo.TestTimer.Add(stopwatch.ElapsedMilliseconds);
            
            // UTF-8 인코딩을 사용하여 문자열로 변환
          //  string csvData = Encoding.UTF8.GetString(fileBytes);
            
            // CSV 파싱
            //CSVDataGenerator.ParseCSV(csvData, _testData);
            _testData = null;
        }
        catch (IOException ex)
        {
            Debug.LogError($"CSV 파일을 읽는 중 오류 발생: {ex.Message}");
        }
    }

    [ContextMenu("(File ReadAllLines) CSV 파일 읽기 - Sequential Access")]
    public void File_ReadAllLinesCSVFile()
    {
        try
        { 
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            // 파일의 모든 줄을 읽어서 문자열 배열로 반환 (UTF8 인코딩 사용)
           // string[] lines = File.ReadAllLines(CSV_Reader_Processor.FilePath, Encoding.UTF8);
           File.ReadAllLines(CSV_Reader_Processor.FilePath);

            stopwatch.Stop();
            Debug.Log($"File ReadAllLines 읽기 완료! 시간: {stopwatch.ElapsedMilliseconds}ms");
            TestCaseFileInfo.TestTimer.Add(stopwatch.ElapsedMilliseconds);
            
            // 전체 파일 내용을 하나의 문자열로 합치기 (필요한 경우)  ----------------------- !!! Test Case에 예외 !!!
         //   string csvData = string.Join("\n", lines);
            
            // CSV 파싱
            //CSVDataGenerator.ParseCSV(csvData, _testData);
            _testData = null;
        }
        catch (IOException ex)
        {
            Debug.LogError($"CSV 파일을 읽는 중 오류 발생: {ex.Message}");
        }
    }

    [ContextMenu("(File ReadLines) CSV 파일 읽기 - Sequential Access")]
    public void File_ReadLinesCSVFile()
    {
        try
        {
           
            StringBuilder stringBuilder = new StringBuilder(); // !!!!!!!!!!!!!!!!!!!!!!!!
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            // File.ReadLines는 파일의 각 줄을 순차적으로 열거합니다.
            foreach (string line in File.ReadLines(CSV_Reader_Processor.FilePath, Encoding.UTF8))
            {
                //stringBuilder.AppendLine(line);
            }
            
            stopwatch.Stop();
            Debug.Log($"File ReadLines 읽기 완료! 시간: {stopwatch.ElapsedMilliseconds}ms");
            TestCaseFileInfo.TestTimer.Add(stopwatch.ElapsedMilliseconds);
            
            // 최종적으로 모든 줄을 합쳐 하나의 문자열로 생성
          //  string csvData = stringBuilder.ToString();
            
            // CSV 파싱
            //CSVDataGenerator.ParseCSV(csvData, _testData);
            _testData = null;
        }
        catch (IOException ex)
        {
            Debug.LogError($"CSV 파일을 읽는 중 오류 발생: {ex.Message}");
        }
    }
    
#endregion


#region File (Only Random Access)

    [ContextMenu("(File ReadAllText) CSV 파일 읽기 - Random Access")]
    public void File_ReadAllTextCSVFile_RandomAccess()
    {
        try
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            // 매번 특정 행을 찾을 경우 파일을 다시 읽는 기준으로 진행
            for (int i = 0; i < CSV_Reader_Processor.targetLineOffest.Length; i++)
            {
                // CSV 파일의 전체 내용을 읽기
                string csvData = File.ReadAllText(CSV_Reader_Processor.FilePath, Encoding.UTF8);
                int startOffset = (int)CSV_Reader_Processor.targetLineOffest[i];
                int endOffset = csvData.IndexOf('\n', startOffset);
                if (endOffset < 0)
                    endOffset = csvData.Length;
                string resultLine = csvData.Substring(startOffset, endOffset - startOffset).Trim();
            }
            
            stopwatch.Stop();
            Debug.Log($"File ReadAllText - Random Access 읽기 완료! 시간: {stopwatch.ElapsedMilliseconds}ms");
            TestCaseFileInfo.TestTimer.Add(stopwatch.ElapsedMilliseconds);
            _testData = null;
        }
        catch (IOException ex)
        {
            Debug.LogError($"CSV 파일을 읽는 중 오류 발생: {ex.Message}");
        }
    }
    
    [ContextMenu("(File ReadAllByte) CSV 파일 읽기 - Random Access")]
    public void File_ReadAllBytesCSVFile_RandomAccess()
    {
        try
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            // CSV 파일 전체를 바이트 배열로 읽어옵니다.
            byte[] fileBytes = File.ReadAllBytes(CSV_Reader_Processor.FilePath);
            
            for (int i = 0; i < CSV_Reader_Processor.targetLineOffest.Length; i++)
            {
                List<int> lineStartIndexes = new List<int>();
                lineStartIndexes.Add(0); // 첫 행은 항상 0
                for (int j = 0; j < fileBytes.Length; j++)
                {
                    if (fileBytes[j] == (byte)'\n')
                    {
                        // 줄바꿈 문자 다음이 새 행 시작 (단, i+1이 배열 범위 내에 있을 때)
                        if (j + 1 < fileBytes.Length)
                            lineStartIndexes.Add(j + 1);
                    }
                }
                
                int targetOffset = (int)CSV_Reader_Processor.targetLineOffest[i];
                // 종료 오프셋: 전체 인덱스에서 targetOffset보다 큰 첫 번째 값
                int endOffset = fileBytes.Length; // 기본: 파일 끝
                foreach (int index in lineStartIndexes)
                {
                    if (index > targetOffset)
                    {
                        endOffset = index - 1; // 줄바꿈 문자는 제외
                        break;
                    }
                }
                string resultLine = Encoding.UTF8.GetString(fileBytes, targetOffset, endOffset - targetOffset).Trim();
            }

            stopwatch.Stop();
            Debug.Log($"File.ReadAllBytes - Random Access 읽기 완료! 시간: {stopwatch.ElapsedMilliseconds}ms");
            TestCaseFileInfo.TestTimer.Add(stopwatch.ElapsedMilliseconds);
            
            _testData = null;
        }
        catch (IOException ex)
        {
            Debug.LogError($"CSV 파일을 읽는 중 오류 발생: {ex.Message}");
        }
    }
    
    [ContextMenu("(File ReadAllLines) CSV 파일 읽기 - Random Access")]
    public void File_ReadAllLinesCSVFile_RandomAccess()
    {
        try
        { 
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < CSV_Reader_Processor.targetLineIndex.Length; i++)
            {
                // 파일의 모든 줄을 읽어서 문자열 배열로 반환 (UTF8 인코딩 사용)
                string[] lines = File.ReadAllLines(CSV_Reader_Processor.FilePath, Encoding.UTF8);
                string resultLine = lines[CSV_Reader_Processor.targetLineIndex[i]];
            }
            
            stopwatch.Stop();
            Debug.Log($"File ReadAllLines 읽기 완료! 시간: {stopwatch.ElapsedMilliseconds}ms");
            TestCaseFileInfo.TestTimer.Add(stopwatch.ElapsedMilliseconds);
            _testData = null;
        }
        catch (IOException ex)
        {
            Debug.LogError($"CSV 파일을 읽는 중 오류 발생: {ex.Message}");
        }
    }

    [ContextMenu("(File ReadAllLines) CSV 파일 읽기 - Random Access")]
    public void File_ReadLinesCSVFile_RandomAccess()
    {
        try
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < CSV_Reader_Processor.targetLineIndex.Length; i++)
            {
                string[] lines = File.ReadLines(CSV_Reader_Processor.FilePath, Encoding.UTF8).ToArray();
                string resultLine = lines[CSV_Reader_Processor.targetLineIndex[i]];
            }
            
            stopwatch.Stop();
            Debug.Log($"File ReadLines 읽기 완료! 시간: {stopwatch.ElapsedMilliseconds}ms");
            TestCaseFileInfo.TestTimer.Add(stopwatch.ElapsedMilliseconds);
            _testData = null;
        }
        catch (IOException ex)
        {
            Debug.LogError($"CSV 파일을 읽는 중 오류 발생: {ex.Message}");
        }
    }
    
#endregion


#region Stream (Only Sequential Access)

    [ContextMenu("(StreamReader_ReadToEnd) CSV 파일 읽기 - Sequential Access")]
    public void StreamReaderCSVFile_ReadToEnd()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        using (StreamReader reader = new StreamReader(CSV_Reader_Processor.FilePath, Encoding.UTF8, true, TestCaseFileInfo.BufferSize))
        {
            reader.ReadToEnd();
        }
        stopwatch.Stop();
        TestCaseFileInfo.TestTimer.Add(stopwatch.ElapsedMilliseconds);
        Debug.Log($"StreamReader_ReadToEnd로 파일 읽기 완료! 시간: {stopwatch.ElapsedMilliseconds}ms");
    }

    [ContextMenu("(FileStream) CSV 파일 읽기 - Sequential Access")]
    public void FileStreamReaderCSVFile()
    {
        // 고정 버퍼 크기 
        int bufferSize = TestCaseFileInfo.BufferSize;
        byte[] buffer = new byte[bufferSize];
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        // FileStream을 이용해 파일을 열고, 일정 크기 단위로 읽기
        using (FileStream fs = new FileStream(CSV_Reader_Processor.FilePath, FileMode.Open, FileAccess.Read))
        {
            while (fs.Read(buffer, 0, bufferSize) > 0)
            {
    
            }
        }
    
        stopwatch.Stop();
        TestCaseFileInfo.TestTimer.Add(stopwatch.ElapsedMilliseconds);
        Debug.Log($"FileStream로 파일 읽기 완료! 시간: {stopwatch.ElapsedMilliseconds}ms");
        
        _testData = null;
    }

    [ContextMenu("(BinaryReader + FileStream) CSV 파일 읽기 - Sequential Access")]
    public void BinaryReaderWithFileStreamCSVFile()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        // !!! FileStream 이용 !!!
        using (BinaryReader reader = new BinaryReader(File.Open(CSV_Reader_Processor.FilePath, FileMode.Open)))
        {
            // byte[] fileBytes = reader.ReadBytes((int)reader.BaseStream.Length);
            reader.ReadBytes((int)reader.BaseStream.Length);

            stopwatch.Stop();
            TestCaseFileInfo.TestTimer.Add(stopwatch.ElapsedMilliseconds);
            Debug.Log($"BinaryReader로 파일 읽기 완료! 시간: {stopwatch.ElapsedMilliseconds}ms");
            
         //   string csvData = Encoding.UTF8.GetString(fileBytes);
        }
    }
    
#endregion File (Only Sequential Access)
  

#region Stream (Only Random Access)

    [ContextMenu("(StreamReader) CSV 파일 읽기 - Random Access")]
    public void StreamReaderCSVFile_RandomAccess()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        for (int i = 0; i < CSV_Reader_Processor.targetLineOffest.Length; i++)
        {
            using (StreamReader reader = new StreamReader(CSV_Reader_Processor.FilePath, Encoding.UTF8, true, TestCaseFileInfo.BufferSize))
            {
                reader.BaseStream.Seek(CSV_Reader_Processor.targetLineOffest[i], SeekOrigin.Begin);
                reader.DiscardBufferedData();
                string resultLine = reader.ReadLine();
            }
        }
        
        stopwatch.Stop();
        TestCaseFileInfo.TestTimer.Add(stopwatch.ElapsedMilliseconds);
        Debug.Log($"StreamReader로 파일 읽기 완료! 시간: {stopwatch.ElapsedMilliseconds}ms");
    }

    [ContextMenu("(FileStream) CSV 파일 읽기 - Random Access")]
    public void FileStreamReaderCSVFile_RandomAccess()
    {
        // 고정 버퍼 크기 (예: 1024바이트)를 사용해 데이터를 읽습니다.
        int bufferSize = 1024;
        byte[] buffer = new byte[bufferSize];
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < CSV_Reader_Processor.targetLineOffest.Length; i++)
        {
            // FileStream을 이용해 파일을 읽습니다.
            using (FileStream fs = new FileStream(CSV_Reader_Processor.FilePath, FileMode.Open, FileAccess.Read))
            {
                // FileStream의 Seek를 사용해 해당 바이트 오프셋으로 이동합니다.
                fs.Seek(CSV_Reader_Processor.targetLineOffest[i], SeekOrigin.Begin);
                int bytesRead = fs.Read(buffer, 0, bufferSize);
                // 읽은 바이트 배열을 UTF8 문자열로 변환합니다.
                string resultLine = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                
                // 해당 행의 끝(줄바꿈 문자)를 찾습니다.
                int newlineIndex = resultLine.IndexOf('\n');
                if (newlineIndex >= 0)
                    resultLine = resultLine.Substring(0, newlineIndex);

                // 결과 문자열의 앞뒤 공백 제거
                resultLine = resultLine.Trim();
            }
        }
        
        stopwatch.Stop();
        TestCaseFileInfo.TestTimer.Add(stopwatch.ElapsedMilliseconds);
        Debug.Log($"FileStream로 파일 읽기 완료! 시간: {stopwatch.ElapsedMilliseconds}ms");
        _testData = null;
    }

    [ContextMenu("(BinaryReader + FileStream) CSV 파일 읽기 - Random Access")]
    public void BinaryReaderWithFileStreamCSVFile_RandomAccess()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        for (int i = 0; i < CSV_Reader_Processor.targetLineOffest.Length; i++)
        {
            // !!! FileStream 이용 !!!
            using (BinaryReader reader = new BinaryReader(File.Open(CSV_Reader_Processor.FilePath, FileMode.Open)))
            {
                reader.BaseStream.Seek(CSV_Reader_Processor.targetLineOffest[i], SeekOrigin.Begin);
                // 대상 행의 데이터를 읽기 위해 한 줄(줄바꿈 문자 전까지)의 바이트들을 누적합니다.
                List<byte> lineBytes = new List<byte>();
                while (true)
                {
                    try
                    {
                        byte currentByte = reader.ReadByte();
                        // 줄바꿈 문자를 만나면 해당 행의 읽기를 종료합니다.
                        if (currentByte == (byte)'\n') break;
                        lineBytes.Add(currentByte);
                    }
                    catch (EndOfStreamException)
                    {
                        // 파일 끝에 도달한 경우 종료
                        break;
                    }
                }
                
                // 누적된 바이트를 UTF8 인코딩으로 문자열로 변환합니다.
                string resultLine = Encoding.UTF8.GetString(lineBytes.ToArray()).Trim();
            }
        }
        
        stopwatch.Stop();
        TestCaseFileInfo.TestTimer.Add(stopwatch.ElapsedMilliseconds);
        Debug.Log($"BinaryReader로 파일 읽기 완료! 시간: {stopwatch.ElapsedMilliseconds}ms");
    }

#endregion

}
