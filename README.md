# 📊 C# .NET File Read Performance Report

이 프로젝트는 Unity6 환경과 C# .NET에서 다양한 **파일 읽기 API**들의 성능을 비교 실험한 결과를 정리한 리포트입니다. CSV 데이터를 기반으로 **순차 접근**과 **임의 접근**에 대해 API 별 처리 속도를 비교하고, 파일 크기 변화에 따른 성능 추이를 분석했습니다.

> 보다 상세한 실험 결과는 아래 링크 <br>
> https://docs.google.com/document/d/13_8LHIu2aBhEKYX0J89omnZ5-SOsNDJtxMSXQyJWB70/edit?tab=t.beismw1qlcy1

## 🔍 실험 목적

- **CSV 데이터**를 다양한 방식으로 읽는 성능을 비교
- **파일 크기(1KB ~ 1GB)**와 **접근 방식(순차/임의)**에 따른 I/O 시간 측정
- 실제 **Unity 게임 개발**에서 발생하는 파일 I/O 병목 현상을 수치화하고 최적화 전략 제시

## ⚙️ 실험 환경

| 항목 | 사양 |
|------|------|
| CPU | AMD Ryzen 7 5700X3D 8-Core |
| Memory | DDR4 64GB |
| OS | Windows 10 Pro |
| Engine | Unity6 (6000.0.40f1) |

## 📂 테스트 대상 API

### .NET File I/O API
- `File.ReadAllText`
- `File.ReadAllBytes`
- `File.ReadAllLines`
- `File.ReadLines`
- `StreamReader.ReadToEnd()`
- `FileStream.Read()`
- `BinaryReader.ReadBytes()`
- `MemoryMappedFile.CreateViewStream()`
- `MemoryMappedFile.CreateViewAccessor()`

### Unity API
- `Resources.Load<TextAsset>`

## 🧪 실험 방식

- 실험 데이터: 1KB ~ 1GB 크기의 CSV 파일
- 접근 방식:
  - **순차 접근**: 파일 전체를 처음부터 끝까지 읽음
  - **임의 접근**: 파일 내 특정 인덱스의 행만 읽음 (초반, 중간, 끝)
- 각 API별 32회 반복 측정, 평균 실행 시간(ms) 기록
- 디코딩/파싱 시간은 임의 접근 실험에만 포함

## 📈 주요 결과 요약

### 순차 접근 (Sequential Access)

| API | 평가 | 비고 |
|-----|------|------|
| `BinaryReader.ReadBytes` | 🟢 매우 빠름 | 바이트 반환, 디코딩 필요 |
| `File.ReadAllBytes` | 🟢 빠름 | 전체 바이트 반환 |
| `Resources.Load` | 🟢 빠름 | Unity 전용, 프로젝트 내 리소스 |
| `FileStream.Read` | 🟢 빠름 | 직접 읽기 스트림 |
| `StreamReader.ReadToEnd` | 🟡 보통 | 간편하지만 대용량에선 비효율 |
| `ReadLines`, `ReadAllLines` | 🔴 느림 | 라인 단위 분할 비용 큼 |

### 임의 접근 (Random Access)

| API | 평가 | 비고 |
|-----|------|------|
| `MemoryMappedFile` (ViewStream/Accessor) | 🟢 매우 빠름 | Seek 기반 빠른 접근 |
| `BinaryReader`, `FileStream` | 🟢 매우 빠름 | 오프셋 접근 최적 |
| `StreamReader` | 🟢 빠름 | Seek 후 읽기 |
| `ReadAllText`, `ReadAllBytes` | 🟡 보통 | 전체를 읽고 특정 행 파싱 |
| `ReadLines`, `ReadAllLines` | 🔴 느림 | 반복 호출 비용 높음 |
| `Resources.Load` | 🔴 매우 느림 | 대용량에선 부적합 |

## 📎 소스 코드 링크

- [`MemoryMappedFileCSVReader.cs`](https://github.com/KYH-AI/.NET-File-Read-Performance-Report/blob/main/Scripts/File%20IO/CSV/MemoryMappedFileCSVReader.cs)
- [`UnityCSVReader.cs`](https://github.com/KYH-AI/.NET-File-Read-Performance-Report/blob/main/Scripts/File%20IO/CSV/UnityCSVReader.cs)
- [`FileIOCSVRead.cs`](https://github.com/KYH-AI/.NET-File-Read-Performance-Report/blob/main/Scripts/File%20IO/CSV/FileIOCSVRead.cs)
- [`CSV_Reader_Processor.cs`](https://github.com/KYH-AI/.NET-File-Read-Performance-Report/blob/main/Scripts/File%20IO/CSV/CSV_Reader_Processor.cs)

## 🧠 결론 및 추천

- 대용량 파일은 `BinaryReader` 혹은 `FileStream`과 같이 **바이트 단위 API**를 사용
- 소규모 파일은 `ReadAllText` 또는 Unity의 `Resources.Load`가 간편하고 빠름
- 특정 위치만 자주 읽는 경우는 `MemoryMappedFile` 방식이 가장 효율적

---

> 이 실험은 단순 성능 테스트가 아닌, **실제 게임 개발에서 파일 I/O 최적화 전략**을 제시하는 분석 리포트입니다.
