using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestCaseFileInfo
{
    // 테스트 메서드 개수
    public static int TestCaseMethodCount = 10;
    // 테스트 타이머
    public static List<long> TestTimer;
    // 행 오프셋 계산 타이머
    public static long ReadIndexTimer;
    // 읽을 버퍼 사이즈
    public static int BufferSize { get; private set; } = 1024;
    
    public static string GetCSVFileSize(string filePath)
    {
        // 전체 경로 확인
        string fullPath = Path.Combine(Application.dataPath, filePath);
    
        // FileInfo를 사용하여 파일 크기 확인
        FileInfo fileInfo = new FileInfo(fullPath);
    
        if (!fileInfo.Exists)
        {
            Debug.LogError($"CSV 파일을 찾을 수 없습니다: {fullPath}");
            return default;
        }

        long bytes = fileInfo.Length;
        
        if (bytes < 1024)
        {
            return $"{bytes} bytes";
        }
        else if (bytes < 1024 * 1024)
        {
            return $"{bytes / 1024.0:F2} KB";
        }
        else if (bytes < 1024 * 1024 * 1024)
        {
            return $"{bytes / (1024.0 * 1024):F2} MB";
        }
        else
        {
            return $"{bytes / (1024.0 * 1024 * 1024):F2} GB";
        }
    }
}
