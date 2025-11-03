using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApp.Controls.UserControls
{
    /// <summary>
    /// UserControl 방식: 기존 컨트롤들을 XAML로 조합
    /// - 장점: 쉽고 빠르게 제작, XAML 디자이너 사용 가능
    /// - 단점: 성능이 약간 낮음, 재사용성 제한적
    /// </summary>
    public partial class CustomButton : UserControl
    {
        // Dependency Property로 외부에서 Text 설정 가능
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(CustomButton),
                new PropertyMetadata("Custom Button", OnTextChanged));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CustomButton;
            if (control != null)
            {
                control.ButtonText.Text = e.NewValue.ToString();
            }
        }

        // Click 이벤트
        public event RoutedEventHandler Click;

        public CustomButton()
        {
            InitializeComponent();

            // 마우스 이벤트 처리 (MFC의 OnMouseMove, OnLButtonDown 등과 유사)
            this.MouseEnter += CustomButton_MouseEnter;
            this.MouseLeave += CustomButton_MouseLeave;
            this.MouseDown += CustomButton_MouseDown;
            this.MouseUp += CustomButton_MouseUp;
        }

        private void CustomButton_MouseEnter(object sender, MouseEventArgs e)
        {
            // Hover 효과
            MainBorder.Background = new SolidColorBrush(Colors.SkyBlue);
            MainBorder.BorderThickness = new Thickness(3);
        }

        private void CustomButton_MouseLeave(object sender, MouseEventArgs e)
        {
            // 원래대로
            MainBorder.Background = new SolidColorBrush(Colors.LightBlue);
            MainBorder.BorderThickness = new Thickness(2);
        }

        private void CustomButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // 눌림 효과
            MainBorder.Background = new SolidColorBrush(Colors.DarkBlue);
            ButtonText.Foreground = new SolidColorBrush(Colors.White);
        }

        private void CustomButton_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // 클릭 완료
            MainBorder.Background = new SolidColorBrush(Colors.SkyBlue);
            ButtonText.Foreground = new SolidColorBrush(Colors.DarkBlue);

            // Click 이벤트 발생
            Click?.Invoke(this, new RoutedEventArgs());
        }
    }
}
