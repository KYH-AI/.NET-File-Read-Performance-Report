using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class CSV_Reader_Processor : MonoBehaviour
{
    
    private readonly CSVLogger _csvLogger = new CSVLogger();
    
    [Header("테스트 파일 용량 타입")]
    [SerializeField] public FileSizeType fileSizeType;
    [Header("테스트 반복 횟수")]
    [SerializeField] private int testCount = 0;
    
    // CSV의 첫 행, 중간 행, 마지막 행 저장
    public static long[] targetLineOffest = new long[3];
    public static int[] targetLineIndex = new int[3];
    
    
    [Header("메모리 맵 파일 IO")]
    [SerializeField] private MemoryMappedFileCSVReader mmfCsv;
    [Header(".Net C# IO")]
    [SerializeField] private FileIOCSVRead fIoCsv;
    [Header("Unity File IO")]
    [SerializeField] private UnityCSVReader unityCsvReader;
    
    // 파일 경로
    public static string FilePath = string.Empty;
    public static string ResourceFilePath = string.Empty;

    // 파일 경로 
    private string[] csvFilePaths;
    // Resources 폴더 파일 경로
    private string[] resourceCSVFilePaths;
    
    
    
    [ContextMenu("파일 경로 테스트")]
    private void ReadAllCSVFilePath()
    {
        // 예: "Assets/Test Case/CSV/KB" 폴더 내 모든 파일
        // 실제 파일이 "Test_Case_1 KB" 형태라면, 패턴을 "Test_Case_* KB" 등으로 조정 가능
        string directory = Path.Combine(Application.dataPath, CSVDataGenerator.CSV_TEST_CASE_FILE_PATH + fileSizeType.ToString());

        // 파일 검색 패턴 설정 (확장자가 없거나 특정 확장자가 있는 경우 패턴을 변경)
        // 예: "Test_Case_* KB" 로 하면 "Test_Case_1 KB", "Test_Case_2 KB" 같은 파일만 검색
        // 만약 ".csv" 확장자가 있다면 "Test_Case_*.csv" 로 수정
        string[] foundFiles = Directory.GetFiles(directory, "Test_Case_*", SearchOption.TopDirectoryOnly);

        // .meta 확장자 제외
        csvFilePaths = foundFiles.Where(path => Path.GetExtension(path) != ".meta").ToArray();

        /*
        Debug.Log($"파일 개수: {csvFilePaths.Length}");
        foreach (var path in csvFilePaths)
        {
            Debug.Log($"발견된 파일: {path}");
        }
        */
        
        directory = Path.Combine(Application.dataPath, "Resources", CSVDataGenerator.CSV_TEST_CASE_FILE_PATH + fileSizeType.ToString());
        foundFiles = Directory.GetFiles(directory, "Test_Case_*", SearchOption.TopDirectoryOnly);

        // .meta 확장자 제외
        resourceCSVFilePaths = foundFiles.Where(path => Path.GetExtension(path) != ".meta").ToArray();

        Debug.Log($"파일 개수: {resourceCSVFilePaths.Length}");
        foreach (var path in resourceCSVFilePaths)
        {
            Debug.Log($"발견된 파일: {path}");
        }
    }
    
    [ContextMenu("[Sequential Access] CSV 읽기 시작")]
    public void SequentialAccessReadCsvFile()
    {
        // 테스트 타이머 리스트 생성
        TestCaseFileInfo.TestTimer = new List<long>(testCount * TestCaseFileInfo.TestCaseMethodCount);
        
        // 테스트할 파일 개수만큼 진행
        for (int j = 0; j < csvFilePaths.Length; j++)
        {
            FilePath = csvFilePaths[j];
            ResourceFilePath =  "Test Case/" + "CSV/" + fileSizeType.ToString() + "/" + Path.GetFileNameWithoutExtension(resourceCSVFilePaths[j]);

            if (!CSVRead()) break;
            
          // CSV 파일 생성
          _csvLogger.CreatePerformanceFile("Sequential Access",
                                        testCount.ToString(),
                                        "CSV",
                                        TestCaseFileInfo.GetCSVFileSize(FilePath),
                                        "Unity.ResourceLoad<TextAsset>",
                                        $"MemoryMappedFile + StreamRead (Buffer Size = { TestCaseFileInfo.BufferSize })", 
                                        $"MemoryMappedFile + Accessor (Buffer Size = { TestCaseFileInfo.BufferSize })",
                                        "System.IO.File.ReadAllText",
                                        "System.IO.File.ReadAllBytes",
                                        "System.IO.File.ReadAllLines",
                                        "System.IO.File.ReadLines",
                                        $"System.IO.StreamReader (Buffet Size = { TestCaseFileInfo.BufferSize })",
                                        $"System.IO.FileStream (Buffet Size = { TestCaseFileInfo.BufferSize })",
                                        $"System.IO.BinaryReaderWithFileStream");

                // ----------- 실험 횟수 만큼 반복 -----------
              for (int i = 0; i < testCount; i++)
              {
                           
                  /* Unity */
                  unityCsvReader.UnityReaderCSVFile();
                  
                  /* MMF */
                  mmfCsv.CreateViewStreamReadCSVFile();
                  mmfCsv.CreateViewAccessorReadCSVFile();
                  
                  /* File */
                 fIoCsv.File_ReadAllTextCSVFile();
                 fIoCsv.File_ReadAllBytesCSVFile();
                 fIoCsv.File_ReadAllLinesCSVFile();
                 fIoCsv.File_ReadLinesCSVFile();
                  
                  /* Stream */
                 fIoCsv.StreamReaderCSVFile_ReadToEnd();
                 fIoCsv.FileStreamReaderCSVFile();
                 fIoCsv.BinaryReaderWithFileStreamCSVFile();
         
              }

              // 타이머 형 변환
              long[] timerList = TestCaseFileInfo.TestTimer.ToArray();
              string[] timer = timerList.Select(t => t.ToString()).ToArray();
          
                // 타이머 CSV에 기록
                _csvLogger.PerformanceLog(timer);
                TestCaseFileInfo.TestTimer.Clear();
        }
    }
    
    [ContextMenu("[Random Access] CSV 읽기 시작")]
    public void RandomAccessReadCsvFile()
    {
        // 테스트 타이머 리스트 생성
        TestCaseFileInfo.TestTimer = new List<long>(testCount * TestCaseFileInfo.TestCaseMethodCount);
        
        // 테스트할 파일 개수만큼 진행
        for (int j = 0; j < csvFilePaths.Length; j++)
        {
            FilePath = csvFilePaths[j];
            ResourceFilePath = "Test Case/" + "CSV/" + fileSizeType.ToString() + "/" +
                               Path.GetFileNameWithoutExtension(resourceCSVFilePaths[j]);

            if (!CSVRead()) break;

            
            // CSV 파일 생성
            _csvLogger.CreatePerformanceFile("Random Access",
                testCount.ToString(),
                "CSV",
                TestCaseFileInfo.GetCSVFileSize(FilePath),
                "Unity.ResourceLoad<TextAsset>",
                $"MemoryMappedFile + StreamRead (Buffer Size = {TestCaseFileInfo.BufferSize})",
                $"MemoryMappedFile + Accessor (Buffer Size = {TestCaseFileInfo.BufferSize})",
                "System.IO.File.ReadAllText",
                "System.IO.File.ReadAllBytes",
                "System.IO.File.ReadAllLines",
                "System.IO.File.ReadLines",
                $"System.IO.StreamReader (Buffet Size = {TestCaseFileInfo.BufferSize})",
                $"System.IO.FileStream (Buffet Size = {TestCaseFileInfo.BufferSize})",
                $"System.IO.BinaryReaderWithFileStream");
             

            BuildLineIndex();
            
            // ----------- 실험 횟수 만큼 반복 -----------
            for (int i = 0; i < testCount; i++)
            {
                           
                /* Unity */
                unityCsvReader.UnityReaderCSVFile_RandomAccess();
                  
                /* MMF */
                 mmfCsv.CreateViewStreamReadCSVFile_RandomAccess();
                 mmfCsv.CreateViewAccessorReadCSVFile_RandomAccess();
                  
                /* File */
                fIoCsv.File_ReadAllTextCSVFile_RandomAccess();
                fIoCsv.File_ReadAllBytesCSVFile_RandomAccess();
                fIoCsv.File_ReadAllLinesCSVFile_RandomAccess();
                fIoCsv.File_ReadLinesCSVFile_RandomAccess();
                  
                /* Stream */
                fIoCsv.StreamReaderCSVFile_RandomAccess();
                fIoCsv.FileStreamReaderCSVFile_RandomAccess();
                fIoCsv.BinaryReaderWithFileStreamCSVFile_RandomAccess();
            }

            // 타이머 형 변환
            long[] timerList = TestCaseFileInfo.TestTimer.ToArray();
            // 바이트 인덱스 매핑 시간 추가
            for (int i = 0; i < timerList.Length; i++) timerList[i] += TestCaseFileInfo.ReadIndexTimer;
            string[] timer = timerList.Select(t => t.ToString()).ToArray();
          
            // 타이머 CSV에 기록
            _csvLogger.PerformanceLog(timer);
            TestCaseFileInfo.TestTimer.Clear();
        }
    }

    /// <summary>
    /// CSV 파일의 각 첫 행, 중간 행, 마지막 행의 시작 오프셋(바이트 단위)를 인덱싱
    /// </summary>
    private void BuildLineIndex()
    {
        if (!CSVRead()) return;
        
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        // 파일 전체를 바이트 배열로 읽기
        byte[] fileBytes = File.ReadAllBytes(FilePath);

        List<long> lineOffsets = new List<long>();
        // 첫 행의 시작은 0
        lineOffsets.Add(0);

        // 파일 전체를 순회하면서 줄바꿈('\n') 문자를 찾기
        for (int i = 0; i < fileBytes.Length; i++)
        {
            if (fileBytes[i] == (byte)'\n')
            {
                // 줄바꿈 문자 다음이 새 행의 시작이므로 i+1을 추가
                if (i + 1 < fileBytes.Length)
                    lineOffsets.Add(i + 1);
            }
        }

        // 행 바이트 오프셋 준비
        targetLineOffest[0] = lineOffsets[1];
        targetLineOffest[1] = lineOffsets[lineOffsets.Count / 2];
        targetLineOffest[2] = lineOffsets[lineOffsets.Count - 1];

        // 행 인덱스 준비
        targetLineIndex[0] = 1;
        targetLineIndex[1] = lineOffsets.Count / 2;
        targetLineIndex[2] = lineOffsets.Count - 1;
        
        stopwatch.Stop();
        TestCaseFileInfo.ReadIndexTimer = stopwatch.ElapsedMilliseconds;
    }


    /// <summary>
    /// CSV 파일 읽기
    /// </summary>
    /// <returns>읽기 성공 확인</returns>
    public static bool CSVRead()
    {
        // 파일 권한 확인
        if (!File.Exists(FilePath))
        {
            Debug.LogError("파일 경로가 잘못되었거나 파일이 존재하지 않습니다: " + FilePath);

            return false;
        }
        return true;
    }
    
    

}

public enum FileSizeType
{
    KB,
    MB,
    GB
}
