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
        
        stopwatch.Stop();
        Debug.Log($"Resources.Load 읽기 완료! 시간: {stopwatch.ElapsedMilliseconds}ms");
        TestCaseFileInfo.TestTimer.Add(stopwatch.ElapsedMilliseconds);
        
        Resources.UnloadAsset(csvFile);
        
        //string csvData = csvFile.ToString();
    }
    
    /// <summary>
    /// Resources Load을 이용한 CSV 읽기 - Random Access
    /// </summary>
    [ContextMenu("(Resources.Load) CSV 파일 읽기 - Random Access")]
    public void UnityReaderCSVFile_RandomAccess()
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        TextAsset csvFile = Resources.Load<TextAsset>(CSV_Reader_Processor.ResourceFilePath);
        
        for (int i = 0; i < CSV_Reader_Processor.targetLineIndex.Length; i++)
        {
            string csvData = csvFile.text;
            string[] lines = csvData.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
            
            int targetIndex = CSV_Reader_Processor.targetLineIndex[i];
            if (targetIndex >= 0 && targetIndex < lines.Length)
            {
                // 대상 행의 데이터를 가져와 앞뒤 공백을 제거합니다.
                string resultLine = lines[targetIndex].Trim();
               // Debug.Log($"대상 행[{i}] (행 번호: {targetIndex}): {resultLine}");
            }
            
            Resources.UnloadAsset(csvFile);
        }
        
        stopwatch.Stop();
        Debug.Log($"Resources.Load 읽기 완료! 시간: {stopwatch.ElapsedMilliseconds}ms");
        TestCaseFileInfo.TestTimer.Add(stopwatch.ElapsedMilliseconds);
    }
}
