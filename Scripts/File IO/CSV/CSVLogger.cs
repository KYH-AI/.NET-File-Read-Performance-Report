using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

/// <summary>
/// ANO CSV 파일 로고 생성
/// </summary>
public class CSVLogger 
{
    private string filePath;

    // 파일 이름 중복 체크 및 새로운 파일 이름 생성
    private string GetUniqueFilePath(string directoryPath, string baseFileName)
    {
        int count = 1;
        string filePath = Path.Combine(directoryPath, $"{baseFileName} {count}.csv");

        // 파일이 존재할 경우, 중복 방지를 위해 파일명 뒤에 숫자를 증가시킴
        while (File.Exists(filePath))
        {
            count++;
            filePath = Path.Combine(directoryPath, $"{baseFileName} {count}.csv");
        }

        return filePath;
    }

    /// <summary>
    /// 퍼포먼스 기록 CSV 파일 생성
    /// </summary>
    /// <param name="fileType">Test Case File Type</param>
    /// <param name="fileSize">Test Case File Size</param>
    public void CreatePerformanceFile(string accessType, string testCylce, string fileType, string fileSize, 
                                            string TestReader1, 
                                            string TestReader2, 
                                            string TestReader3, 
                                            string TestReader4,
                                            string TestReader5,
                                            string TestReader6,
                                            string TestReader7,
                                            string TestReader8,
                                            string TestReader9,
                                            string TestReader10)
    {
        // 사용자 바탕화면 경로 가져오기
        string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        string directoryPath = Path.Combine(desktopPath, "Memory Map File Performance");

        // 폴더가 존재하지 않으면 생성
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // 파일 이름이 중복되는 경우 새로운 이름으로 파일 생성
        filePath = GetUniqueFilePath(directoryPath, $"File Read Performance [{fileSize}]");

        // CSV 파일에 헤더 추가
        using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            writer.WriteLine($"[{accessType}] Test Case File ({fileType}) Size : {fileSize} / Test Cycle : {testCylce} / Buffer Size : {TestCaseFileInfo.BufferSize}," +
                             $"{TestReader1}, " +
                             $"{TestReader2}, " +
                             $"{TestReader3}, " +
                             $"{TestReader4}, " +
                             $"{TestReader5}, " +
                             $"{TestReader6}, " +
                             $"{TestReader7}, " +
                             $"{TestReader8}, " +
                             $"{TestReader9}, " +
                             $"{TestReader10}"); // CSV 헤더
        }
        
        Debug.Log(filePath + " File Read Performance 파일 생성");
    }

    /// <summary>
    /// 퍼포먼스 수치 기록
    /// </summary>
    /// <param name="data">퍼포먼스 데이터</param>
    public void PerformanceLog(string[] data)
    {
        // CSV 파일에 데이터 추가
        using (StreamWriter writer = new StreamWriter(filePath, true, Encoding.UTF8))
        {
            for (int i = 0; i < data.Length - 1; i+=TestCaseFileInfo.TestCaseMethodCount )
            {
                // data 배열의 요소를 쉼표(,)로 구분하여 한 줄로 작성
                string logEntry = ","
                                  + data[i] + "ms" + ","
                                  + data[i + 1] + "ms" + ","
                                  + data[i + 2] + "ms" + ","
                                  + data[i + 3] + "ms" + ","
                                  + data[i + 4] + "ms" + ","
                                  + data[i + 5] + "ms" + ","
                                  + data[i + 6] + "ms" + ","
                                  + data[i + 7] + "ms" + ","
                                  + data[i + 8] + "ms" + ","
                                  + data[i + 9] + "ms";
                writer.WriteLine(logEntry);  // 퍼포먼스 결과 CSV 파일에 기록
            }
        }
        
    }
}