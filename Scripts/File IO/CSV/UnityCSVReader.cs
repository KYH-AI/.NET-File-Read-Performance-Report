using System;
using UnityEngine;


public class UnityCSVReader : MonoBehaviour
{
    /// <summary>
    /// Resources Load을 이용한 CSV 읽기 - Sequential Access
    /// </summary>
    [ContextMenu("(Resources.Load) CSV 파일 읽기 - Sequential Access")]
    public void UnityReaderCSVFile()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        TextAsset csvFile = Resources.Load<TextAsset>(CSV_Reader_Processor.ResourceFilePath);
        
        // 데이터 소비(옵션)...
        
        stopwatch.Stop();
        Debug.Log($"Resources.Load 읽기 완료! 시간: {stopwatch.ElapsedMilliseconds}ms");
        TestCaseFileInfo.TestTimer.Add(stopwatch.ElapsedMilliseconds);
        
        // 읽어온 텍스트 파일을 캐싱에서 제거 (*** 실험에 필수 ***)
        Resources.UnloadAsset(csvFile);
    }
    
    /// <summary>
    /// Resources Load을 이용한 CSV 읽기 - Random Access
    /// </summary>
    [ContextMenu("(Resources.Load) CSV 파일 읽기 - Random Access")]
    public void UnityReaderCSVFile_RandomAccess()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        TextAsset csvFile = Resources.Load<TextAsset>(CSV_Reader_Processor.ResourceFilePath);
        
        // 임의 접근 해야하는 행 횟수 만큼 반복
        for (int i = 0; i < CSV_Reader_Processor.targetLineIndex.Length; i++)
        {
            string csvData = csvFile.text;
            // 텍스트에서 가져온 문자열을 파싱
            string[] lines = csvData.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
            
            int targetIndex = CSV_Reader_Processor.targetLineIndex[i];
            if (targetIndex >= 0 && targetIndex < lines.Length)
            {
                // 대상 행의 데이터를 가져와 앞뒤 공백을 제거
                string resultLine = lines[targetIndex].Trim();
            }
            
            // 읽어온 텍스트 파일을 캐싱에서 제거 (*** 실험에 필수 ***)
            Resources.UnloadAsset(csvFile);
        }
        
        stopwatch.Stop();
        Debug.Log($"Resources.Load 읽기 완료! 시간: {stopwatch.ElapsedMilliseconds}ms");
        TestCaseFileInfo.TestTimer.Add(stopwatch.ElapsedMilliseconds);
    }
}
