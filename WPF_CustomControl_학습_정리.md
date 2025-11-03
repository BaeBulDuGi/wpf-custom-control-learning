# WPF Custom Control 완벽 가이드 - MFC 개발자를 위한 실전 학습

> MFC의 OnDraw()를 사용해본 개발자가 WPF Custom Control을 배우는 과정을 정리한 글입니다.

## 목차
1. [들어가며](#들어가며)
2. [WPF 기본 구조 이해](#wpf-기본-구조-이해)
3. [Custom Control 3가지 방식](#custom-control-3가지-방식)
4. [실습 1: 커스텀 버튼 만들기](#실습-1-커스텀-버튼-만들기)
5. [실습 2: 메뉴 스타일 리스트 컨트롤](#실습-2-메뉴-스타일-리스트-컨트롤)
6. [고급 주제](#고급-주제)
7. [발생한 문제와 해결](#발생한-문제와-해결)
8. [마치며](#마치며)

---

## 들어가며

MFC에서는 `OnDraw()` 함수 안에서 CDC를 사용해 직접 그리기 작업을 했습니다. WPF는 어떻게 다를까요?

**MFC vs WPF 핵심 차이점:**
- **MFC**: `OnDraw(CDC* pDC)` → 픽셀 단위 직접 그리기
- **WPF**: XAML + C# 분리, 벡터 그래픽, 데이터 바인딩

WPF에서도 MFC처럼 직접 그릴 수 있고(`OnRender`), XAML로만 디자인할 수도 있습니다. 이 글에서는 **세 가지 방식을 모두** 다룹니다.

---

## WPF 기본 구조 이해

### 1. XAML이란?
```xml
<!-- XAML: UI를 선언적으로 정의 -->
<Button Content="클릭" Width="100" Height="30"/>
```
- HTML처럼 태그로 UI 작성
- 디자이너와 개발자가 분리 작업 가능
- Visual Studio에서 실시간 미리보기 지원

### 2. Code-Behind (C#)
```csharp
// XAML과 연결된 C# 코드
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent(); // XAML 로드
    }
}
```

### 3. 프로젝트 구조
```
WpfApp/
├── App.xaml              # 앱 진입점
├── MainWindow.xaml       # 메인 창 (UI)
├── MainWindow.xaml.cs    # 메인 창 (로직)
├── Controls/             # 커스텀 컨트롤
│   ├── UserControls/     # UserControl 방식
│   ├── CustomControls/   # OnRender 방식
│   └── Templates/        # Control Template 방식
├── Models/               # 데이터 모델
└── ViewModels/           # MVVM 패턴
```

---

## Custom Control 3가지 방식

WPF에서 커스텀 컨트롤을 만드는 방법은 크게 3가지입니다.

### 방식 1: UserControl (컴포지션)
**특징:** 기존 컨트롤을 조합해서 새로운 컨트롤 생성
- 가장 쉽고 빠름
- XAML로 디자인, C#로 이벤트 처리
- 재사용 가능한 UI 블록 만들기 좋음

**언제 사용?**
- 여러 컨트롤을 묶어서 하나로 만들 때
- 디자이너와 협업할 때
- 빠른 프로토타이핑

### 방식 2: OnRender Custom Control (MFC 스타일)
**특징:** `OnRender(DrawingContext dc)`로 직접 그리기
- **MFC의 `OnDraw()`와 가장 유사**
- DrawingContext로 도형, 텍스트, 이미지 직접 그리기
- 완전한 커스텀 렌더링 가능

**언제 사용?**
- MFC 경험을 살리고 싶을 때
- 픽셀 단위 정밀한 제어가 필요할 때
- 복잡한 그래픽 작업 (차트, 게이지 등)

**MFC 개발자를 위한 비교:**
| MFC | WPF |
|-----|-----|
| `OnDraw(CDC* pDC)` | `OnRender(DrawingContext dc)` |
| `pDC->Rectangle()` | `dc.DrawRectangle()` |
| `pDC->Ellipse()` | `dc.DrawEllipse()` |
| `pDC->TextOut()` | `dc.DrawText()` |
| `Invalidate()` | `InvalidateVisual()` |

### 방식 3: Control Template (XAML 스타일링)
**특징:** XAML만으로 컨트롤의 모양 완전히 변경
- 코드 없이 순수 XAML로만 디자인
- 기존 컨트롤의 동작은 유지하면서 외관만 변경
- 테마, 스킨 적용 가능

**언제 사용?**
- 기존 컨트롤의 동작은 그대로 두고 디자인만 바꿀 때
- 일관된 디자인 시스템 구축
- 애니메이션과 트리거 효과

---

## 실습 1: 커스텀 버튼 만들기

각 방식으로 동일한 기능의 버튼을 만들어보겠습니다.

### 1-1. UserControl 방식

**CustomButton.xaml** (UI 정의)
```xml
<UserControl x:Class="WpfApp.Controls.UserControls.CustomButton"
             xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
             xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Border x:Name="MainBorder"
            Background="#FF2196F3"
            CornerRadius="15"
            Padding="20,10"
            Cursor="Hand">
        <StackPanel Orientation="Horizontal" HorizontalAlignment="Center">
            <!-- 아이콘 (동그라미) -->
            <Ellipse x:Name="IconCircle"
                     Width="24" Height="24"
                     Fill="White"
                     Margin="0,0,10,0"/>

            <!-- 텍스트 -->
            <TextBlock x:Name="TextBlock"
                       Text="버튼"
                       Foreground="White"
                       FontSize="16"
                       VerticalAlignment="Center"/>
        </StackPanel>
    </Border>
</UserControl>
```

**CustomButton.xaml.cs** (이벤트 처리)
```csharp
public partial class CustomButton : UserControl
{
    // Dependency Property로 Text 바인딩 가능하게
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string),
            typeof(CustomButton), new PropertyMetadata("버튼"));

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    // Click 이벤트
    public event RoutedEventHandler Click;

    public CustomButton()
    {
        InitializeComponent();

        // 마우스 이벤트로 Hover 효과
        MainBorder.MouseEnter += (s, e) =>
        {
            MainBorder.Background = new SolidColorBrush(Color.FromRgb(33, 150, 243));
        };

        MainBorder.MouseLeave += (s, e) =>
        {
            MainBorder.Background = new SolidColorBrush(Color.FromRgb(33, 150, 243));
        };

        MainBorder.MouseLeftButtonDown += (s, e) =>
        {
            Click?.Invoke(this, new RoutedEventArgs());
        };
    }
}
```

**사용 방법:**
```xml
<local:CustomButton Text="확인" Click="CustomButton_Click"/>
```

---

### 1-2. OnRender Custom Control 방식 (MFC 스타일)

**CustomDrawButton.cs**
```csharp
public class CustomDrawButton : Control
{
    // Dependency Properties
    public static readonly DependencyProperty TextProperty =
        DependencyProperty.Register("Text", typeof(string),
            typeof(CustomDrawButton),
            new FrameworkPropertyMetadata("버튼",
                FrameworkPropertyMetadataOptions.AffectsRender)); // 변경 시 다시 그리기

    public string Text
    {
        get { return (string)GetValue(TextProperty); }
        set { SetValue(TextProperty, value); }
    }

    // 상태 관리
    private bool isHovered = false;
    private bool isPressed = false;

    public event RoutedEventHandler Click;

    public CustomDrawButton()
    {
        this.Width = 120;
        this.Height = 40;
        this.Cursor = Cursors.Hand;

        // 마우스 이벤트
        this.MouseEnter += (s, e) => { isHovered = true; InvalidateVisual(); };
        this.MouseLeave += (s, e) => { isHovered = false; isPressed = false; InvalidateVisual(); };
        this.MouseLeftButtonDown += (s, e) => { isPressed = true; InvalidateVisual(); };
        this.MouseLeftButtonUp += (s, e) =>
        {
            if (isPressed) Click?.Invoke(this, new RoutedEventArgs());
            isPressed = false;
            InvalidateVisual();
        };
    }

    // MFC의 OnDraw()와 동일한 역할!
    protected override void OnRender(DrawingContext dc)
    {
        base.OnRender(dc);

        double width = this.ActualWidth > 0 ? this.ActualWidth : this.Width;
        double height = this.ActualHeight > 0 ? this.ActualHeight : this.Height;

        // 1. 배경색 결정 (상태에 따라)
        Color bgColor = isPressed ? Color.FromRgb(25, 118, 210) :
                        isHovered ? Color.FromRgb(66, 165, 245) :
                        Color.FromRgb(33, 150, 243);

        Brush backgroundBrush = new SolidColorBrush(bgColor);
        Pen borderPen = new Pen(new SolidColorBrush(Colors.White), 0);

        // 2. 둥근 사각형 배경 그리기
        Rect rect = new Rect(0, 0, width, height);
        dc.DrawRoundedRectangle(backgroundBrush, borderPen, rect, 15, 15);

        // 3. 원 아이콘 그리기
        Point center = new Point(30, height / 2);
        double radius = 12;
        dc.DrawEllipse(Brushes.White, null, center, radius, radius);

        // 4. 텍스트 그리기
        FormattedText formattedText = new FormattedText(
            Text,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.SemiBold, FontStretches.Normal),
            16,
            Brushes.White,
            VisualTreeHelper.GetDpi(this).PixelsPerDip);

        Point textPos = new Point(
            50,
            (height - formattedText.Height) / 2);

        dc.DrawText(formattedText, textPos);
    }
}
```

**핵심 포인트:**
- `OnRender(DrawingContext dc)`: MFC의 `OnDraw(CDC* pDC)`와 동일
- `InvalidateVisual()`: MFC의 `Invalidate()`와 동일 (다시 그리기 요청)
- `DrawingContext`: CDC처럼 그리기 도구
- `FrameworkPropertyMetadataOptions.AffectsRender`: 속성 변경 시 자동으로 다시 그리기

---

### 1-3. Control Template 방식

**ButtonTemplates.xaml**
```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <!-- Modern Button Style -->
    <Style x:Key="ModernButtonStyle" TargetType="Button">
        <Setter Property="Background" Value="#FF2196F3"/>
        <Setter Property="Foreground" Value="White"/>
        <Setter Property="FontSize" Value="16"/>
        <Setter Property="FontWeight" Value="SemiBold"/>
        <Setter Property="Padding" Value="20,10"/>
        <Setter Property="Cursor" Value="Hand"/>

        <!-- Template으로 완전히 새로 디자인 -->
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="Button">
                    <Border x:Name="border"
                            Background="{TemplateBinding Background}"
                            CornerRadius="15"
                            Padding="{TemplateBinding Padding}">
                        <StackPanel Orientation="Horizontal" HorizontalAlignment="Center">
                            <!-- 아이콘 -->
                            <Ellipse Width="24" Height="24" Fill="White" Margin="0,0,10,0"/>
                            <!-- 텍스트 -->
                            <ContentPresenter VerticalAlignment="Center"/>
                        </StackPanel>
                    </Border>

                    <!-- Trigger로 상태별 스타일 변경 -->
                    <ControlTemplate.Triggers>
                        <!-- Hover 효과 -->
                        <Trigger Property="IsMouseOver" Value="True">
                            <Setter TargetName="border" Property="Background" Value="#FF42A5F5"/>
                        </Trigger>
                        <!-- Press 효과 -->
                        <Trigger Property="IsPressed" Value="True">
                            <Setter TargetName="border" Property="Background" Value="#FF1976D2"/>
                        </Trigger>
                    </ControlTemplate.Triggers>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>
</ResourceDictionary>
```

**사용 방법:**
```xml
<!-- App.xaml에 리소스 등록 -->
<Application.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="Controls/Templates/ButtonTemplates.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Application.Resources>

<!-- MainWindow.xaml에서 사용 -->
<Button Content="확인" Style="{StaticResource ModernButtonStyle}" Click="Button_Click"/>
```

---

## 실습 2: 메뉴 스타일 리스트 컨트롤

좌측에 원형 아이콘, 우측에 제목과 설명이 있는 메뉴 항목을 만들어보겠습니다.

### 데이터 모델

**MenuItemModel.cs**
```csharp
public class MenuItemModel
{
    public string IconPath { get; set; }      // 아이콘 이미지 경로
    public string Title { get; set; }         // 제목
    public string Description { get; set; }   // 설명
    public Color IconColor { get; set; }      // 아이콘 배경색

    public MenuItemModel(string iconPath, string title, string description, Color iconColor)
    {
        IconPath = iconPath;
        Title = title;
        Description = description;
        IconColor = iconColor;
    }
}
```

### Control Template 방식으로 ListBox 커스터마이징

**MenuListTemplates.xaml**
```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <!-- ListBox 스타일 -->
    <Style x:Key="MenuListBoxStyle" TargetType="ListBox">
        <Setter Property="Background" Value="#FF2C2C2C"/>
        <Setter Property="BorderThickness" Value="0"/>
        <Setter Property="Padding" Value="0"/>

        <!-- ListBoxItem 스타일 -->
        <Setter Property="ItemContainerStyle">
            <Setter.Value>
                <Style TargetType="ListBoxItem">
                    <Setter Property="Padding" Value="0"/>
                    <Setter Property="Margin" Value="0"/>
                    <Setter Property="Template">
                        <Setter.Value>
                            <ControlTemplate TargetType="ListBoxItem">
                                <Border x:Name="border"
                                        Background="Transparent"
                                        Padding="15,10"
                                        BorderThickness="0,0,0,1"
                                        BorderBrush="#FF404040">
                                    <ContentPresenter/>
                                </Border>

                                <ControlTemplate.Triggers>
                                    <Trigger Property="IsMouseOver" Value="True">
                                        <Setter TargetName="border" Property="Background" Value="#FF3D3D3D"/>
                                    </Trigger>
                                    <Trigger Property="IsSelected" Value="True">
                                        <Setter TargetName="border" Property="Background" Value="#FF0D47A1"/>
                                    </Trigger>
                                </ControlTemplate.Triggers>
                            </ControlTemplate>
                        </Setter.Value>
                    </Setter>
                </Style>
            </Setter.Value>
        </Setter>
    </Style>

    <!-- DataTemplate: 각 항목의 내용 -->
    <DataTemplate x:Key="MenuItemTemplate">
        <Grid Height="60">
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="60"/>
                <ColumnDefinition Width="*"/>
            </Grid.ColumnDefinitions>

            <!-- 원형 아이콘 -->
            <Ellipse Grid.Column="0"
                     Width="40" Height="40"
                     HorizontalAlignment="Center">
                <Ellipse.Fill>
                    <SolidColorBrush Color="{Binding IconColor}"/>
                </Ellipse.Fill>
            </Ellipse>

            <!-- 텍스트 영역 -->
            <StackPanel Grid.Column="1" VerticalAlignment="Center">
                <TextBlock Text="{Binding Title}"
                           FontSize="16"
                           FontWeight="SemiBold"
                           Foreground="White"/>
                <TextBlock Text="{Binding Description}"
                           FontSize="12"
                           Foreground="#FFB0B0B0"
                           Margin="0,3,0,0"/>
            </StackPanel>
        </Grid>
    </DataTemplate>
</ResourceDictionary>
```

### 사용 방법

**MainWindow.xaml.cs**
```csharp
private ObservableCollection<MenuItemModel> menuItems;

private void InitializeMenuData()
{
    menuItems = new ObservableCollection<MenuItemModel>
    {
        new MenuItemModel("", "프로젝트", "프로젝트 관리", Colors.Purple),
        new MenuItemModel("", "팀", "팀원 및 협업", Colors.Green),
        new MenuItemModel("", "보고서", "통계 및 리포트", Colors.Blue),
        new MenuItemModel("", "설정", "시스템 설정", Colors.Gray)
    };

    MenuListBox.ItemsSource = menuItems;
}
```

**MainWindow.xaml**
```xml
<ListBox x:Name="MenuListBox"
         Style="{StaticResource MenuListBoxStyle}"
         ItemTemplate="{StaticResource MenuItemTemplate}"
         SelectionChanged="MenuListBox_SelectionChanged"/>
```

---

## 고급 주제

### 1. Dependency Property (의존성 속성)

일반 C# 속성과 다르게 WPF의 특별한 속성 시스템입니다.

**장점:**
- 데이터 바인딩 지원
- 애니메이션 적용 가능
- 값 검증 (Coerce)
- 변경 알림 (Callback)

**예제: CustomProgressBar**
```csharp
public class CustomProgressBar : Control
{
    // Dependency Property 정의
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            "Value",                          // 프로퍼티 이름
            typeof(double),                   // 타입
            typeof(CustomProgressBar),        // 소유자
            new FrameworkPropertyMetadata(
                0.0,                          // 기본값
                FrameworkPropertyMetadataOptions.AffectsRender,  // 변경 시 다시 그리기
                OnValueChanged,               // 값 변경 콜백
                CoerceValue));                // 값 검증

    // 일반 속성처럼 사용
    public double Value
    {
        get { return (double)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }

    // 값 변경 시 호출
    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = d as CustomProgressBar;
        double oldValue = (double)e.OldValue;
        double newValue = (double)e.NewValue;

        // 이벤트 발생
        control.ValueChanged?.Invoke(control, new ValueChangedEventArgs(oldValue, newValue));
    }

    // 값 검증 (0-100 범위로 제한)
    private static object CoerceValue(DependencyObject d, object baseValue)
    {
        var control = d as CustomProgressBar;
        double value = (double)baseValue;

        if (value < 0) return 0.0;
        if (value > control.Maximum) return control.Maximum;

        return value;
    }

    public event EventHandler<ValueChangedEventArgs> ValueChanged;

    protected override void OnRender(DrawingContext dc)
    {
        // 진행률에 따라 바 그리기
        double percentage = Maximum > 0 ? (Value / Maximum) : 0;
        double barWidth = ActualWidth * percentage;

        // 배경
        dc.DrawRectangle(Brushes.Gray, null, new Rect(0, 0, ActualWidth, ActualHeight));
        // 진행 바
        dc.DrawRectangle(Brushes.DodgerBlue, null, new Rect(0, 0, barWidth, ActualHeight));
    }
}
```

---

### 2. 애니메이션 (Storyboard & Triggers)

코드 없이 XAML만으로 부드러운 애니메이션을 만들 수 있습니다.

**AnimatedTemplates.xaml**
```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <!-- Easing Functions 먼저 정의 -->
    <CubicEase x:Key="EaseOut" EasingMode="EaseOut"/>

    <!-- 애니메이션 버튼 스타일 -->
    <Style x:Key="AnimatedScaleButtonStyle" TargetType="Button">
        <Setter Property="Background" Value="#FF2196F3"/>
        <Setter Property="RenderTransformOrigin" Value="0.5,0.5"/>
        <Setter Property="RenderTransform">
            <Setter.Value>
                <ScaleTransform ScaleX="1" ScaleY="1"/>
            </Setter.Value>
        </Setter>

        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="Button">
                    <Border Background="{TemplateBinding Background}" CornerRadius="10">
                        <ContentPresenter HorizontalAlignment="Center" VerticalAlignment="Center"/>
                    </Border>

                    <ControlTemplate.Triggers>
                        <!-- Hover 시 1.1배 확대 애니메이션 -->
                        <Trigger Property="IsMouseOver" Value="True">
                            <Trigger.EnterActions>
                                <BeginStoryboard>
                                    <Storyboard>
                                        <DoubleAnimation
                                            Storyboard.TargetProperty="(UIElement.RenderTransform).(ScaleTransform.ScaleX)"
                                            To="1.1"
                                            Duration="0:0:0.2"
                                            EasingFunction="{StaticResource EaseOut}"/>
                                        <DoubleAnimation
                                            Storyboard.TargetProperty="(UIElement.RenderTransform).(ScaleTransform.ScaleY)"
                                            To="1.1"
                                            Duration="0:0:0.2"
                                            EasingFunction="{StaticResource EaseOut}"/>
                                    </Storyboard>
                                </BeginStoryboard>
                            </Trigger.EnterActions>
                            <Trigger.ExitActions>
                                <BeginStoryboard>
                                    <Storyboard>
                                        <DoubleAnimation
                                            Storyboard.TargetProperty="(UIElement.RenderTransform).(ScaleTransform.ScaleX)"
                                            To="1.0"
                                            Duration="0:0:0.2"/>
                                        <DoubleAnimation
                                            Storyboard.TargetProperty="(UIElement.RenderTransform).(ScaleTransform.ScaleY)"
                                            To="1.0"
                                            Duration="0:0:0.2"/>
                                    </Storyboard>
                                </BeginStoryboard>
                            </Trigger.ExitActions>
                        </Trigger>
                    </ControlTemplate.Triggers>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>

    <!-- 로딩 스피너 (무한 회전) -->
    <Style x:Key="LoadingSpinnerStyle" TargetType="Control">
        <Setter Property="Template">
            <Setter.Value>
                <ControlTemplate TargetType="Control">
                    <Grid RenderTransformOrigin="0.5,0.5">
                        <Grid.RenderTransform>
                            <RotateTransform x:Name="SpinnerRotate" Angle="0"/>
                        </Grid.RenderTransform>

                        <!-- 8개의 점 -->
                        <Ellipse Width="8" Height="8" Fill="DodgerBlue" Opacity="1.0"
                                 HorizontalAlignment="Center" VerticalAlignment="Top"/>
                        <Ellipse Width="8" Height="8" Fill="DodgerBlue" Opacity="0.75"
                                 HorizontalAlignment="Right" VerticalAlignment="Center"/>
                        <!-- ... 나머지 점들 -->
                    </Grid>

                    <ControlTemplate.Triggers>
                        <Trigger Property="IsVisible" Value="True">
                            <Trigger.EnterActions>
                                <BeginStoryboard>
                                    <Storyboard>
                                        <!-- 무한 회전 -->
                                        <DoubleAnimation
                                            Storyboard.TargetName="SpinnerRotate"
                                            Storyboard.TargetProperty="Angle"
                                            From="0" To="360"
                                            Duration="0:0:1"
                                            RepeatBehavior="Forever"/>
                                    </Storyboard>
                                </BeginStoryboard>
                            </Trigger.EnterActions>
                        </Trigger>
                    </ControlTemplate.Triggers>
                </ControlTemplate>
            </Setter.Value>
        </Setter>
    </Style>
</ResourceDictionary>
```

---

### 3. MVVM 패턴 (Model-View-ViewModel)

WPF의 대표적인 아키텍처 패턴입니다.

**구조:**
- **Model**: 데이터와 비즈니스 로직
- **View**: XAML UI
- **ViewModel**: View와 Model 연결, INotifyPropertyChanged 구현

**BaseViewModel.cs**
```csharp
public class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // 헬퍼 메서드: 값이 변경되었을 때만 알림
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value))
            return false;

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
```

**CounterViewModel.cs**
```csharp
public class CounterViewModel : BaseViewModel
{
    private int _count;
    private string _message;

    // 데이터 바인딩 프로퍼티
    public int Count
    {
        get => _count;
        set
        {
            if (SetProperty(ref _count, value))
            {
                UpdateMessage(); // Count 변경 시 메시지도 업데이트
            }
        }
    }

    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }

    // Commands
    public ICommand IncrementCommand { get; }
    public ICommand DecrementCommand { get; }
    public ICommand ResetCommand { get; }

    public CounterViewModel()
    {
        _count = 0;
        _message = "버튼을 클릭해보세요!";

        // Command 초기화
        IncrementCommand = new RelayCommand(Increment, CanIncrement);
        DecrementCommand = new RelayCommand(Decrement, CanDecrement);
        ResetCommand = new RelayCommand(Reset);
    }

    private void Increment(object? parameter)
    {
        Count++;
    }

    private bool CanIncrement(object? parameter)
    {
        return Count < 100; // 100 미만일 때만 실행 가능
    }

    private void Decrement(object? parameter)
    {
        Count--;
    }

    private bool CanDecrement(object? parameter)
    {
        return Count > 0;
    }

    private void Reset(object? parameter)
    {
        Count = 0;
    }

    private void UpdateMessage()
    {
        if (Count == 0)
            Message = "0입니다. 증가 버튼을 눌러보세요!";
        else if (Count >= 100)
            Message = "최대값 100에 도달했습니다!";
        else if (Count % 10 == 0)
            Message = $"좋아요! 현재 {Count}입니다.";
        else
            Message = $"현재 카운트: {Count}";
    }
}

// RelayCommand 헬퍼 클래스
public class RelayCommand : ICommand
{
    private readonly Action<object?> _execute;
    private readonly Predicate<object?>? _canExecute;

    public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }

    public event EventHandler? CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecute == null || _canExecute(parameter);
    }

    public void Execute(object? parameter)
    {
        _execute(parameter);
    }
}
```

**XAML에서 사용:**
```xml
<StackPanel DataContext="{Binding CounterViewModel}">
    <!-- Count 바인딩 -->
    <TextBlock Text="{Binding Count}" FontSize="48"/>

    <!-- Message 바인딩 -->
    <TextBlock Text="{Binding Message}"/>

    <!-- Command 바인딩 -->
    <Button Content="증가" Command="{Binding IncrementCommand}"/>
    <Button Content="감소" Command="{Binding DecrementCommand}"/>
    <Button Content="리셋" Command="{Binding ResetCommand}"/>
</StackPanel>
```

**Code-Behind:**
```csharp
public MainWindow()
{
    InitializeComponent();

    // DataContext 설정
    MvvmPanel.DataContext = new CounterViewModel();
}
```

---

## 발생한 문제와 해결

### 문제 1: StaticResource 'EaseOut' 찾을 수 없음

**에러 메시지:**
```
System.Exception: 이름이 'EaseOut'인 리소스를 찾을 수 없습니다.
```

**원인:**
AnimatedTemplates.xaml에서 `EaseOut` 리소스를 파일 하단에 정의했는데, 위쪽 스타일에서 먼저 참조해서 발생한 에러입니다.

XAML은 **순차적으로 로드**되므로, 리소스를 사용하기 전에 먼저 정의되어야 합니다.

**해결:**
```xml
<!-- ❌ 잘못된 순서 -->
<Style x:Key="AnimatedButton">
    <Setter Property="..." EasingFunction="{StaticResource EaseOut}"/>
</Style>
<CubicEase x:Key="EaseOut" EasingMode="EaseOut"/>

<!-- ✅ 올바른 순서 -->
<CubicEase x:Key="EaseOut" EasingMode="EaseOut"/>
<Style x:Key="AnimatedButton">
    <Setter Property="..." EasingFunction="{StaticResource EaseOut}"/>
</Style>
```

**교훈:** XAML 리소스는 **선언 순서가 중요**합니다. 공통 리소스는 파일 상단에 정의하세요.

---

### 문제 2: Trigger에서 Element 타겟팅 불가

**에러 메시지:**
```
Trigger 대상 'glow'을(를) 찾을 수 없습니다
```

**원인:**
Control Template의 Trigger에서 `x:Name`으로 정의한 요소를 직접 타겟팅하려고 했습니다.

**잘못된 코드:**
```xml
<Border x:Name="glowBorder">
    <Border.Effect>
        <DropShadowEffect x:Name="glow" BlurRadius="0"/>
    </Border.Effect>
</Border>

<Trigger Property="IsMouseOver" Value="True">
    <!-- ❌ Effect 내부 요소는 타겟팅 불가 -->
    <Setter TargetName="glow" Property="BlurRadius" Value="30"/>
</Trigger>
```

**해결:**
```xml
<Trigger Property="IsMouseOver" Value="True">
    <!-- ✅ 전체 Effect를 교체 -->
    <Setter TargetName="glowBorder" Property="Effect">
        <Setter.Value>
            <DropShadowEffect Color="#FF00FFFF" BlurRadius="30"/>
        </Setter.Value>
    </Setter>
</Trigger>
```

**교훈:** Trigger에서는 **직접 자식 요소만** TargetName으로 지정 가능합니다.

---

### 문제 3: Nullable 경고

**경고 메시지:**
```
CS8618: null을 허용하지 않는 이벤트 'Click'은(는) 생성자를 종료할 때 null이 아닌 값을 포함해야 합니다.
```

**원인:**
.NET 6.0 이상에서는 Nullable Reference Types가 기본으로 활성화되어 있습니다.

**해결 방법 1: Nullable로 선언**
```csharp
public event RoutedEventHandler? Click; // ? 추가
```

**해결 방법 2: 프로젝트 설정에서 비활성화**
```xml
<!-- WpfApp.csproj -->
<PropertyGroup>
    <Nullable>disable</Nullable>
</PropertyGroup>
```

---

## 마치며

### 배운 내용 정리

1. **WPF 기본 구조**
   - XAML과 C# Code-Behind 분리
   - ResourceDictionary로 스타일 관리
   - Dependency Property 시스템

2. **Custom Control 3가지 방식**
   - **UserControl**: 빠르고 쉬운 컴포지션
   - **OnRender**: MFC 스타일 직접 그리기
   - **Control Template**: XAML로만 스타일링

3. **고급 주제**
   - Dependency Property로 바인딩과 검증
   - Storyboard와 Trigger로 애니메이션
   - MVVM 패턴으로 UI와 로직 분리

### MFC 개발자를 위한 팁

| 하고 싶은 것 | MFC | WPF |
|------------|-----|-----|
| 직접 그리기 | `OnDraw(CDC*)` | `OnRender(DrawingContext)` |
| 다시 그리기 | `Invalidate()` | `InvalidateVisual()` |
| 이벤트 처리 | Message Map | Event Handler / Command |
| 데이터 바인딩 | 없음 (수동) | `{Binding}` 자동 동기화 |
| UI 업데이트 | `UpdateData()` | INotifyPropertyChanged |

### 다음 단계

- **Data Binding 심화**: IValueConverter, MultiBinding
- **Commands 심화**: CommandParameter, EventToCommand
- **Styles & Resources**: 테마 시스템 구축
- **Custom Panel**: 레이아웃 직접 제어
- **Behaviors**: 코드 없이 동작 추가

---

## 전체 프로젝트 구조

```
WpfApp/
├── App.xaml
├── App.xaml.cs
├── MainWindow.xaml
├── MainWindow.xaml.cs
│
├── Controls/
│   ├── UserControls/
│   │   ├── CustomButton.xaml
│   │   ├── CustomButton.xaml.cs
│   │   ├── MenuListItem.xaml
│   │   └── MenuListItem.xaml.cs
│   │
│   ├── CustomControls/
│   │   ├── CustomDrawButton.cs
│   │   ├── CustomDrawMenuItem.cs
│   │   └── CustomProgressBar.cs
│   │
│   └── Templates/
│       ├── ButtonTemplates.xaml
│       ├── MenuListTemplates.xaml
│       └── AnimatedTemplates.xaml
│
├── Models/
│   └── MenuItemModel.cs
│
└── ViewModels/
    ├── BaseViewModel.cs
    └── CounterViewModel.cs
```

---

**작성일**: 2025년
**개발 환경**: .NET 6.0, WPF
**참고**: 이 글은 실제 학습 과정을 바탕으로 작성되었습니다.

