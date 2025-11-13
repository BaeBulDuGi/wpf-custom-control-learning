# WPF Custom Control 학습 프로젝트

> MFC의 OnDraw()를 사용해본 개발자가 WPF Custom Control을 배우는 과정을 정리한 실습 프로젝트입니다.

![.NET](https://img.shields.io/badge/.NET-6.0-512BD4?style=flat&logo=dotnet)
![WPF](https://img.shields.io/badge/WPF-Windows-0078D6?style=flat&logo=windows)
![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=csharp)

## 📖 목차

- [프로젝트 소개](#-프로젝트-소개)
- [실행 방법](#-실행-방법)
- [학습 내용](#-학습-내용)
- [프로젝트 구조](#-프로젝트-구조)
- [주요 기능](#-주요-기능)
- [스크린샷](#-스크린샷)
- [배운 점](#-배운-점)

## 🎯 프로젝트 소개

이 프로젝트는 WPF Custom Control을 처음 배우는 개발자(특히 MFC 경험자)를 위한 실습 예제 모음입니다.

### 왜 이 프로젝트를 만들었나요?

- **MFC 경험 활용**: MFC의 `OnDraw()`와 유사한 WPF의 `OnRender()` 학습
- **3가지 방식 비교**: UserControl, OnRender, Control Template 직접 비교
- **실전 예제**: 버튼, 리스트, 프로그레스바 등 실용적인 컨트롤 구현
- **고급 주제**: Dependency Property, Animation, MVVM 패턴 실습

## 🚀 실행 방법

### 필요 환경

- Windows 10/11
- .NET 6.0 SDK 이상
- Visual Studio 2022 (또는 Visual Studio Code + C# Extension)

### 실행

```bash
# 저장소 클론
git clone https://github.com/BaeBulDuGi/wpf-custom-control-learning.git
cd wpf-custom-control-learning

# 실행
dotnet run --project WpfApp.csproj
```

또는 Visual Studio에서 `WpfApp.csproj`를 열고 F5를 누르세요.

## 📚 학습 내용

### 1. Custom Control 3가지 방식

#### 방식 1: UserControl (컴포지션)
- 기존 컨트롤을 조합해서 새로운 컨트롤 생성
- XAML로 디자인, C#로 이벤트 처리
- 가장 쉽고 빠른 방식

#### 방식 2: OnRender Custom Control (MFC 스타일)
- `OnRender(DrawingContext)` 로 직접 그리기
- MFC의 `OnDraw(CDC*)` 와 유사
- 픽셀 단위 정밀한 제어 가능

#### 방식 3: Control Template (XAML 스타일링)
- XAML만으로 컨트롤 모양 완전 변경
- 기존 동작 유지하면서 외관만 커스터마이징
- 애니메이션과 트리거 효과 적용

### 2. MFC vs WPF 비교

| 작업 | MFC | WPF |
|------|-----|-----|
| 직접 그리기 | `OnDraw(CDC* pDC)` | `OnRender(DrawingContext dc)` |
| 다시 그리기 | `Invalidate()` | `InvalidateVisual()` |
| 사각형 그리기 | `pDC->Rectangle()` | `dc.DrawRectangle()` |
| 원 그리기 | `pDC->Ellipse()` | `dc.DrawEllipse()` |
| 텍스트 그리기 | `pDC->TextOut()` | `dc.DrawText()` |
| 이벤트 처리 | Message Map | Event Handler / Command |
| 데이터 바인딩 | 없음 (수동) | `{Binding}` 자동 동기화 |
| UI 업데이트 | `UpdateData()` | INotifyPropertyChanged |

### 3. 고급 주제

- **Dependency Property**: 데이터 바인딩, 애니메이션, 값 검증
- **Storyboard & Triggers**: XAML로만 애니메이션 구현
- **MVVM 패턴**: Model-View-ViewModel 아키텍처
- **Data Binding**: 양방향 바인딩과 ObservableCollection

## 📁 프로젝트 구조

```
WpfApp/
├── App.xaml                    # 앱 진입점
├── App.xaml.cs
├── MainWindow.xaml             # 메인 창 (3개 탭)
├── MainWindow.xaml.cs
│
├── Controls/
│   ├── UserControls/           # UserControl 방식
│   │   ├── CustomButton.xaml
│   │   ├── CustomButton.xaml.cs
│   │   ├── MenuListItem.xaml
│   │   └── MenuListItem.xaml.cs
│   │
│   ├── CustomControls/         # OnRender 방식
│   │   ├── CustomDrawButton.cs
│   │   ├── CustomDrawMenuItem.cs
│   │   └── CustomProgressBar.cs
│   │
│   └── Templates/              # Control Template 방식
│       ├── ButtonTemplates.xaml
│       ├── MenuListTemplates.xaml
│       └── AnimatedTemplates.xaml
│
├── Models/
│   └── MenuItemModel.cs        # 데이터 모델
│
├── ViewModels/                 # MVVM 패턴
│   ├── BaseViewModel.cs
│   └── CounterViewModel.cs
│
└── README.md                   # 이 파일
```

## ✨ 주요 기능

### 탭 1: 버튼 컨트롤 예제

3가지 방식으로 구현한 커스텀 버튼들을 비교할 수 있습니다:
- **UserControl 버튼**: XAML 조합 방식
- **OnRender 버튼**: 직접 그리기 방식
- **Control Template 버튼**: 3가지 스타일 (Modern, Neon, 3D)

### 탭 2: 리스트/메뉴 컨트롤

메뉴 스타일의 리스트 아이템:
- 원형 아이콘 + 제목 + 설명
- Hover/Select 효과
- 3가지 방식으로 구현

### 탭 3: 고급 예제

#### Dependency Property 예제
- 커스텀 프로그레스 바
- 값 검증 (0-100 제한)
- 변경 이벤트 처리

#### 애니메이션 예제
- Scale 애니메이션 버튼 (Hover 시 1.1배 확대)
- 로딩 스피너 (무한 회전)
- Easing Functions (부드러운 효과)

#### MVVM 패턴 예제
- INotifyPropertyChanged 구현
- ICommand로 버튼 처리
- CanExecute로 버튼 활성화 제어
- 데이터 바인딩

## 📸 스크린샷

### 버튼 컨트롤 탭
```
┌─────────────────────────────────────┐
│  UserControl 버튼                    │
│  [●  확인]  [●  취소]  [●  닫기]     │
│                                      │
│  OnRender 버튼                       │
│  [●  저장]  [●  삭제]  [●  편집]     │
│                                      │
│  Control Template 버튼               │
│  Modern  |  Neon  |  3D Style       │
└─────────────────────────────────────┘
```

### 리스트/메뉴 컨트롤 탭
```
┌─────────────────────────────────────┐
│  (●) 프로젝트                         │
│      프로젝트 관리                    │
│  ───────────────────────────────    │
│  (●) 팀                              │
│      팀원 및 협업                     │
│  ───────────────────────────────    │
│  (●) 보고서                          │
│      통계 및 리포트                   │
└─────────────────────────────────────┘
```

### 고급 예제 탭
```
┌─────────────────────────────────────┐
│  Dependency Property                 │
│  [━━━━━━━━━━━━━━━━━━] 75.0%         │
│  [증가] [감소] [리셋]                 │
│                                      │
│  Animation                           │
│  [⟳ 로딩 스피너]  [애니메이션 버튼]   │
│                                      │
│  MVVM 패턴                           │
│  카운트: 42                          │
│  [증가] [감소] [리셋]                 │
└─────────────────────────────────────┘
```

## 🎓 배운 점

### 1. WPF 기본 구조
- XAML과 C# Code-Behind 분리
- ResourceDictionary로 스타일 관리
- Dependency Property 시스템

### 2. Custom Control 패턴
- 각 방식의 장단점과 사용 시기
- MFC 경험을 WPF에 적용하는 방법
- 상태 관리와 이벤트 처리

### 3. 고급 기능
- Dependency Property의 Coerce와 Callback
- Storyboard와 Trigger로 애니메이션
- MVVM 패턴의 실전 적용

### 4. 문제 해결 경험

#### 문제 1: StaticResource 순서 에러
```
System.Exception: 이름이 'EaseOut'인 리소스를 찾을 수 없습니다.
```
**해결**: XAML은 순차 로드되므로 리소스를 사용 전에 먼저 정의해야 함

#### 문제 2: Trigger 타겟팅 제약
```
Trigger 대상 'glow'을(를) 찾을 수 없습니다
```
**해결**: Effect 내부 요소는 직접 타겟팅 불가. 전체 Effect를 교체해야 함

## 🔗 참고 자료

- [Microsoft WPF Documentation](https://learn.microsoft.com/ko-kr/dotnet/desktop/wpf/)
- [WPF Tutorial](https://www.wpftutorial.net/)
- [WPF-Tutorial.com](https://wpf-tutorial.com/)

## 📝 라이선스

이 프로젝트는 학습 목적으로 만들어졌으며 자유롭게 사용하실 수 있습니다.

## 👤 작성자

**MFC 개발자에서 WPF 학습자로**

- 배경: MFC OnDraw() 경험
- 학습 목표: WPF Custom Control 마스터
- 학습 방법: 실습 중심, 3가지 방식 비교

---

**작성일**: 2025년
**개발 환경**: .NET 6.0, WPF, Visual Studio 2022

## 🚀 다음 단계

학습을 계속하고 싶다면:

- [ ] IValueConverter로 복잡한 데이터 변환
- [ ] Custom Panel로 레이아웃 직접 제어
- [ ] Behaviors로 코드 없이 동작 추가
- [ ] Prism/MVVM Toolkit 같은 프레임워크 학습
- [ ] Multi-Window 관리
- [ ] 국제화 (Localization)

---

⭐ 이 프로젝트가 도움이 되었다면 Star를 눌러주세요!
