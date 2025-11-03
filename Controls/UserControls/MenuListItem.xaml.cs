using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApp.Controls.UserControls
{
    /// <summary>
    /// UserControl 방식: 메뉴바 스타일의 리스트 아이템
    /// 왼쪽에 동그란 아이콘, 오른쪽에 제목과 설명
    /// </summary>
    public partial class MenuListItem : UserControl
    {
        // Dependency Properties
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(MenuListItem),
                new PropertyMetadata("Menu Item", OnTitleChanged));

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(MenuListItem),
                new PropertyMetadata("Description", OnDescriptionChanged));

        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register("IconPath", typeof(string), typeof(MenuListItem),
                new PropertyMetadata(string.Empty, OnIconPathChanged));

        public static readonly DependencyProperty IconColorProperty =
            DependencyProperty.Register("IconColor", typeof(Brush), typeof(MenuListItem),
                new PropertyMetadata(new SolidColorBrush(Colors.DodgerBlue), OnIconColorChanged));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        public string IconPath
        {
            get { return (string)GetValue(IconPathProperty); }
            set { SetValue(IconPathProperty, value); }
        }

        public Brush IconColor
        {
            get { return (Brush)GetValue(IconColorProperty); }
            set { SetValue(IconColorProperty, value); }
        }

        // Click 이벤트
        public event RoutedEventHandler Click;

        public MenuListItem()
        {
            InitializeComponent();

            // 마우스 이벤트
            this.MouseEnter += MenuListItem_MouseEnter;
            this.MouseLeave += MenuListItem_MouseLeave;
            this.MouseDown += MenuListItem_MouseDown;
            this.MouseUp += MenuListItem_MouseUp;
        }

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MenuListItem;
            if (control != null)
            {
                control.TitleText.Text = e.NewValue.ToString();
            }
        }

        private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MenuListItem;
            if (control != null)
            {
                control.DescriptionText.Text = e.NewValue.ToString();
            }
        }

        private static void OnIconPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MenuListItem;
            if (control != null && !string.IsNullOrEmpty(e.NewValue.ToString()))
            {
                try
                {
                    control.IconImageBrush.ImageSource = new BitmapImage(new Uri(e.NewValue.ToString(), UriKind.RelativeOrAbsolute));
                    control.IconText.Visibility = Visibility.Collapsed;
                }
                catch
                {
                    // 이미지 로드 실패 시 기본 아이콘 표시
                    control.IconText.Visibility = Visibility.Visible;
                }
            }
        }

        private static void OnIconColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MenuListItem;
            if (control != null)
            {
                control.IconBackground.Fill = (Brush)e.NewValue;
            }
        }

        private void MenuListItem_MouseEnter(object sender, MouseEventArgs e)
        {
            MainBorder.Background = new SolidColorBrush(Color.FromRgb(70, 70, 70));
        }

        private void MenuListItem_MouseLeave(object sender, MouseEventArgs e)
        {
            MainBorder.Background = new SolidColorBrush(Color.FromRgb(60, 60, 60));
        }

        private void MenuListItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainBorder.Background = new SolidColorBrush(Color.FromRgb(50, 50, 50));
        }

        private void MenuListItem_MouseUp(object sender, MouseButtonEventArgs e)
        {
            MainBorder.Background = new SolidColorBrush(Color.FromRgb(70, 70, 70));
            Click?.Invoke(this, new RoutedEventArgs());
        }
    }
}
