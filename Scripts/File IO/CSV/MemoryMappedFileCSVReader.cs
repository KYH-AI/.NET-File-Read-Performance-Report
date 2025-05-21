using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using UnityEngine;

public class MemoryMappedFileCSVReader : MonoBehaviour
{

    private TestData[] _testData;
    

    [ContextMenu("(CreateViewStream) 메모리 맵 CSV 파일 읽기 - Sequential Access")]
    public void CreateViewStreamReadCSVFile()
    {
        FileInfo fileInfo = new FileInfo(CSV_Reader_Processor.FilePath);
        // 파일의 크기 구하기
        long fileSize = fileInfo.Length;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        using (var mmf = MemoryMappedFile.CreateFromFile(CSV_Reader_Processor.FilePath, FileMode.Open ,null, 0, MemoryMappedFileAccess.Read))
        {
            // !!!! CreateViewStream & StreamReader !!!!
            
            using (var stream = mmf.CreateViewStream(0, fileSize, MemoryMappedFileAccess.Read))
            using (var reader = new StreamReader(stream, Encoding.UTF8, true, TestCaseFileInfo.BufferSize))
            {
                reader.ReadToEnd(); 
                
                // 데이터 소비(옵션)...
                
                stopwatch.Stop();
                Debug.Log($"MemoryMappedFile 읽기 완료! 시간: {stopwatch.ElapsedMilliseconds}ms");
                TestCaseFileInfo.TestTimer.Add(stopwatch.ElapsedMilliseconds);
            }

        }
        _testData = null;
    }
    
    [ContextMenu("(CreateViewAccessor) 메모리 맵 CSV 파일 읽기 - Sequential Access")]    
    public void CreateViewAccessorReadCSVFile()
    {
        
        // 파일의 크기 구하기
        FileInfo fileInfo = new FileInfo(CSV_Reader_Processor.FilePath);
        long fileSize = fileInfo.Length;
        
        // Buffer 이용
        int bufferSize = TestCaseFileInfo.BufferSize; // 버퍼 크기 설정
        byte[] buffer = new byte[bufferSize];
        long offset = 0;
        
        // StringBuilder 초기 용량 설정 (예: 파일 크기 가정의 절반 정도)
        StringBuilder stringBuilder = new StringBuilder((int)Math.Min(fileSize / 2, int.MaxValue)); 
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        using (var mmf = MemoryMappedFile.CreateFromFile(CSV_Reader_Processor.FilePath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read))
        {
            // !!!! CreateViewAccessor !!!!
            using (var accessor = mmf.CreateViewAccessor(0, fileSize, MemoryMappedFileAccess.Read))
            {
                // !!! Custom Buffer Size !!!
                while (offset < fileSize)
                {
                    // 이번에 읽을 바이트 수 (남은 데이터가 버퍼 크기보다 작을 수 있음)
                    int bytesToRead = (int)Math.Min(bufferSize, fileSize - offset);
                    
                    // 버퍼에 데이터를 읽기
                    accessor.ReadArray(offset, buffer, 0, bytesToRead);
                    
                    // 데이터 소비(옵션)...
                    
                    // 오프셋을 이동
                    offset += bytesToRead;
                }
            
                stopwatch.Stop();
                Debug.Log($"MemoryMappedFile 읽기 완료! 시간: {stopwatch.ElapsedMilliseconds}ms");
                TestCaseFileInfo.TestTimer.Add(stopwatch.ElapsedMilliseconds);
            }
        }
        _testData = null;
    }

    [ContextMenu("(CreateViewStream) 메모리 맵 CSV 파일 읽기 - Random Access")]
    public void CreateViewStreamReadCSVFile_RandomAccess()
    {
        // 파일의 크기 구하기
        FileInfo fileInfo = new FileInfo(CSV_Reader_Processor.FilePath);
        long fileSize = fileInfo.Length;
        // 시스템 페이지 크기(예: 4096 또는 65536  -> OS에서 설정됨)
        int allocationGranularity = Environment.SystemPageSize;
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // 임의 접근 해야하는 행 횟수 만큼 반복
        for (int i = 0; i < CSV_Reader_Processor.targetLineOffest.Length; i++)
        {
            // 대상 행의 실제 시작 오프셋 (rawOffset)
            long rawOffset = CSV_Reader_Processor.targetLineOffest[i];
            // 시스템 페이지 크기에 맞게 정렬된 오프셋(alignedOffset)
            long alignedOffset = rawOffset - (rawOffset % allocationGranularity);
            // 보정값(offsetAdjustment): 정렬된 오프셋과 실제 대상 행 시작 사이의 차이
            long offsetAdjustment = rawOffset - alignedOffset;
            long remainingBytes = fileSize - alignedOffset;
            int viewLength = (int)Math.Min(remainingBytes, Math.Max(TestCaseFileInfo.BufferSize, offsetAdjustment + TestCaseFileInfo.BufferSize));
            
            using (var mmf = MemoryMappedFile.CreateFromFile(CSV_Reader_Processor.FilePath, FileMode.Open ,null, 0, MemoryMappedFileAccess.Read))
            {
                // !!!! CreateViewStream & StreamReader !!!!
            
                using (var stream = mmf.CreateViewStream(alignedOffset, viewLength, MemoryMappedFileAccess.Read))
                using (var reader = new StreamReader(stream, Encoding.UTF8, true, TestCaseFileInfo.BufferSize))
                {
                    // 전체 매핑된 뷰를 문자열로 읽기
                    string viewData = reader.ReadToEnd();

                    // 뷰 내에서 대상 행의 데이터는 offsetAdjustment 위치에서 시작 (파일 포인터 역할)
                    int startIndex = (int)offsetAdjustment;

                    // 대상 행의 끝을 찾기 위해, startIndex 이후 첫 번째 줄바꿈 문자를 찾기
                    int newlineIndex = viewData.IndexOf('\n', startIndex);
                    int count = (newlineIndex >= 0) ? newlineIndex - startIndex : viewData.Length - startIndex;

                    // 대상 행 데이터를 문자열로 반환
                    string resultLine = viewData.Substring(startIndex, count).Trim();
                }
            }
        }
        stopwatch.Stop();
        Debug.Log($"MemoryMappedFile 읽기 완료! 시간: {stopwatch.ElapsedMilliseconds}ms");
        TestCaseFileInfo.TestTimer.Add(stopwatch.ElapsedMilliseconds);
        _testData = null;
    }

    [ContextMenu("(CreateViewAccessor) 메모리 맵 CSV 파일 읽기 - Random Access")]
    public void CreateViewAccessorReadCSVFile_RandomAccess()
    {
        int allocationGranularity = Environment.SystemPageSize;
        FileInfo fi = new FileInfo(CSV_Reader_Processor.FilePath);
        long fileSize = fi.Length;
        
        // 파일의 크기 구하기
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        for (int i = 0; i < CSV_Reader_Processor.targetLineOffest.Length; i++)
        {
            // 대상 행의 실제 시작 오프셋(바이트 단위)
            long rawOffset = CSV_Reader_Processor.targetLineOffest[i]; 
            // 페이지 크기에 맞게 정렬된 오프셋
            long alignedOffset = rawOffset - (rawOffset % allocationGranularity); 
            // 매핑된 뷰 내에서 실제 데이터 시작 위치 보정 값
            long offsetAdjustment = rawOffset - alignedOffset; 
            long remainingBytes = fileSize - alignedOffset;
            int viewLength = (int)Math.Min(remainingBytes, Math.Max(TestCaseFileInfo.BufferSize, offsetAdjustment + TestCaseFileInfo.BufferSize));
            
            using (var mmf = MemoryMappedFile.CreateFromFile(CSV_Reader_Processor.FilePath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read))
            using (var accessor = mmf.CreateViewAccessor(alignedOffset, viewLength, MemoryMappedFileAccess.Read))
            {
                byte[] buffer = new byte[viewLength];
                accessor.ReadArray(0, buffer, 0, viewLength);

                // 시작 인덱스는 보정 값(offsetAdjustment), 파일 포이넡 역할
                int startIndex = (int)offsetAdjustment;
                // buffer 내에서 startIndex 이후로 줄바꿈 문자('\n')의 위치를 찾음
                int newlineIndex = Array.IndexOf(buffer, (byte)'\n', startIndex);

                // 만약 줄바꿈 문자가 있으면 해당 위치까지, 없으면 viewLength 전체를 위치로 지정
                int count = newlineIndex >= 0 ? newlineIndex - startIndex : viewLength - startIndex;

                // 버퍼에 있는 바이트를 offset 값 만큼 문자열로 디코딩
                string resultLine = Encoding.UTF8.GetString(buffer, startIndex, count).Trim();
                
                // Debug.Log($"대상 오프셋 {rawOffset} (정렬: {alignedOffset}, 보정: {offsetAdjustment})의 행: {resultLine}");
            }
        }

        stopwatch.Stop();
        Debug.Log($"MemoryMappedFile 읽기 완료! 시간: {stopwatch.ElapsedMilliseconds}ms");
        TestCaseFileInfo.TestTimer.Add(stopwatch.ElapsedMilliseconds);
        _testData = null;
    }
}
