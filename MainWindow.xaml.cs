using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfApp.Models;
using WpfApp.ViewModels;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int clickCount = 0;
        private ObservableCollection<MenuItemModel> menuItems;

        public MainWindow()
        {
            InitializeComponent();
            InitializeMenuData();
            InitializeAdvancedExamples();
        }

        private void InitializeMenuData()
        {
            // ListBox용 데이터 생성
            menuItems = new ObservableCollection<MenuItemModel>
            {
                new MenuItemModel("", "프로젝트", "프로젝트 관리", Colors.Purple),
                new MenuItemModel("", "팀", "팀원 및 협업", Colors.Green),
                new MenuItemModel("", "보고서", "통계 및 리포트", Colors.Blue),
                new MenuItemModel("", "설정", "시스템 설정", Colors.Gray)
            };

            TemplateListBox.ItemsSource = menuItems;
            CardListBox.ItemsSource = menuItems;
        }

        private void InitializeAdvancedExamples()
        {
            // MVVM 예제: DataContext 설정
            MvvmExamplePanel.DataContext = new CounterViewModel();

            // Progress Bar 이벤트 구독
            ProgressBar1.ValueChanged += (s, e) =>
            {
                AdvancedResultTextBlock.Text = $"Progress: {e.OldValue:F1} → {e.NewValue:F1}";
            };
        }

        // ===== 버튼 탭 이벤트 =====

        // UserControl 버튼 클릭 이벤트
        private void CustomButton_Click(object sender, RoutedEventArgs e)
        {
            clickCount++;
            var button = sender as Controls.UserControls.CustomButton;
            ResultTextBlock.Text = $"[UserControl] {button?.Text} 클릭됨!\n총 클릭 횟수: {clickCount}";
        }

        // OnRender Custom Control 버튼 클릭 이벤트
        private void CustomDrawButton_Click(object sender, RoutedEventArgs e)
        {
            clickCount++;
            var button = sender as Controls.CustomControls.CustomDrawButton;
            ResultTextBlock.Text = $"[OnRender] {button?.Text} 클릭됨!\n총 클릭 횟수: {clickCount}";
        }

        // Control Template 버튼 클릭 이벤트
        private void TemplateButton_Click(object sender, RoutedEventArgs e)
        {
            clickCount++;
            var button = sender as Button;
            ResultTextBlock.Text = $"[Template] {button?.Content} 클릭됨!\n총 클릭 횟수: {clickCount}";
        }

        // ===== 리스트/메뉴 탭 이벤트 =====

        // UserControl 메뉴 아이템 클릭
        private void MenuListItem_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as Controls.UserControls.MenuListItem;
            ListResultTextBlock.Text = $"[UserControl] {item?.Title} 선택됨!\n{item?.Description}";
        }

        // OnRender 메뉴 아이템 클릭
        private void DrawMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as Controls.CustomControls.CustomDrawMenuItem;
            ListResultTextBlock.Text = $"[OnRender] {item?.Title} 선택됨!\n{item?.Description}";
        }

        // Control Template ListBox 선택 변경
        private void TemplateListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TemplateListBox.SelectedItem is MenuItemModel item)
            {
                ListResultTextBlock.Text = $"[Template - 기본] {item.Title} 선택됨!\n{item.Description}";
            }
        }

        // Control Template CardListBox 선택 변경
        private void CardListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CardListBox.SelectedItem is MenuItemModel item)
            {
                ListResultTextBlock.Text = $"[Template - 카드] {item.Title} 선택됨!\n{item.Description}";
            }
        }

        // ===== 고급 예제 탭 이벤트 =====

        // Dependency Property - Progress Bar
        private void IncreaseProgress_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar1.Value += 10;
        }

        private void DecreaseProgress_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar1.Value -= 10;
        }

        private void ResetProgress_Click(object sender, RoutedEventArgs e)
        {
            ProgressBar1.Value = 0;
        }

        // 애니메이션 - 스피너 토글
        private void ToggleSpinner_Click(object sender, RoutedEventArgs e)
        {
            LoadingSpinner.Visibility = LoadingSpinner.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
    }
}
