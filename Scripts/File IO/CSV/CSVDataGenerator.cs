using System;
using System.IO;
using System.Text;
using UnityEngine;
using Random = System.Random;


public class CSVDataGenerator : MonoBehaviour
{
    [Header("테스트 케이스 CSV 파일 이름")]
    [SerializeField] private string fileName = "Test_Case.csv";
    [Header("테스트 케이스 CSV 행 개수")]
    [SerializeField] private int recordCount = 10000;

    // 테스트 케이스 상대 경로
    public static string CSV_TEST_CASE_FILE_PATH = "Test Case/CSV/"; //   Test_Case.csv";

    

    [ContextMenu("테스트 케이스 CSV 파일 생성")]
    public void GenerateCharacterData()
    {
        //  저장 경로 사용
        string folderPath = Path.Combine(Application.persistentDataPath, "Test Case/CSV");
        
        
        // 폴더가 존재하지 않으면 생성
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
            Debug.Log($"폴더 생성 완료: {folderPath}");
        }

        // 최종 파일 경로
        string filePath = Path.Combine(folderPath, fileName);
        
        // CSV 파일 생성
        GenerateCharacterData(filePath, recordCount);

    }
    
    /// <summary>
    /// 테스트 케이스 CSV 파일 생성
    /// </summary>
    /// <param name="recordCount">행 개수</param>
    private void GenerateCharacterData(string filePath, int recordCount)
    {
        var random = new Random();
        var sb = new StringBuilder();

        // CSV 헤더
        sb.AppendLine("ID,Name,Level,Health,Experience,PositionX,PositionY,PositionZ");

        for (int i = 1; i <= recordCount; i++)
        {
            // 랜덤 데이터 생성
            string name = $"Character_{random.Next(1000, 9999)}";
            int level = random.Next(1, 51);
            int health = random.Next(50, 151);
            int experience = random.Next(0, 10001);
            float posX = (float)(random.NextDouble() * 200 - 100);
            float posY = (float)(random.NextDouble() * 200 - 100);
            float posZ = (float)(random.NextDouble() * 200 - 100);

            // CSV 데이터 행 추가
            sb.AppendLine($"{i},{name},{level},{health},{experience},{posX:F2},{posY:F2},{posZ:F2}");
        }

        // CSV 파일 저장
        File.WriteAllText(filePath, sb.ToString());
        
                
        // 파일 크기 확인 (GetCSVFileSize는 Application.dataPath를 기준으로 상대경로를 사용함)
        string fileSizeStr = TestCaseFileInfo.GetCSVFileSize(filePath);
        // 새 파일 이름 생성 (예: Test_Case_{용량}.csv)
        string directory = Path.GetDirectoryName(filePath);
        string newFileName = $"Test_Case_{fileSizeStr}.csv";
        string newFilePath = Path.Combine(directory, newFileName);
        
        // 파일 이름 변경
        File.Move(filePath, newFilePath);
        Console.WriteLine($"CSV 파일 생성 완료: {filePath}");
    }
    
    
    /// <summary>
    /// CSV 파싱
    /// </summary>
    /// <param name="csvContent">파싱 문자열</param>
    /// <param name="testArray">저장할 배열</param>
    public static void ParseCSV(string csvContent, TestData[] testArray)
    {
        // CSV 데이터 파싱
        string[] lines = csvContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        // 첫 번째 행(헤더)을 제외한 크기로 배열 초기화
        testArray = new TestData[lines.Length - 1];

        // 첫 번째 행(헤더)을 건너뛰고 두 번째 행부터 파싱 시작
        for (int i = 1; i < lines.Length; i++) 
        {
            // 각 행을 쉼표(,) 기준으로 분리
            string[] columns = lines[i].Split(',');

            // 각 열에 접근하여 TestData 객체를 생성
            testArray[i - 1] = new TestData()  // i - 1로 배열 인덱스를 맞춰줌
            {
                ID = int.Parse(columns[0]), // 첫 번째 열은 ID
                Name = columns[1],          // 두 번째 열은 Name
                Level = int.Parse(columns[2]), // 세 번째 열은 Level
                Health = int.Parse(columns[3]), // 네 번째 열은 Health
                Experience = int.Parse(columns[4]), // 다섯 번째 열은 Experience
                PositionX = float.Parse(columns[5]), // 여섯 번째 열은 PositionX
                PositionY = float.Parse(columns[6]), // 일곱 번째 열은 PositionY
                PositionZ = float.Parse(columns[7])  // 여덟 번째 열은 PositionZ
            };
        }
    }
}