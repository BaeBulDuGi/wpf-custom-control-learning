using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApp.Controls.CustomControls
{
    /// <summary>
    /// OnRender Custom Control: MFC의 OnDraw 방식으로 메뉴 아이템 직접 그리기
    /// 왼쪽에 동그란 아이콘, 오른쪽에 제목과 설명
    /// </summary>
    public class CustomDrawMenuItem : Control
    {
        // Dependency Properties
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(CustomDrawMenuItem),
                new FrameworkPropertyMetadata("Menu Item", FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(CustomDrawMenuItem),
                new FrameworkPropertyMetadata("Description", FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register("IconPath", typeof(string), typeof(CustomDrawMenuItem),
                new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender));

        public static readonly DependencyProperty IconColorProperty =
            DependencyProperty.Register("IconColor", typeof(Color), typeof(CustomDrawMenuItem),
                new FrameworkPropertyMetadata(Colors.OrangeRed, FrameworkPropertyMetadataOptions.AffectsRender));

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

        public Color IconColor
        {
            get { return (Color)GetValue(IconColorProperty); }
            set { SetValue(IconColorProperty, value); }
        }

        // 상태 관리
        private bool isHovered = false;
        private bool isPressed = false;
        private ImageBrush iconImageBrush = null;

        // Click 이벤트
        public event RoutedEventHandler Click;

        public CustomDrawMenuItem()
        {
            this.Width = 300;
            this.Height = 70;

            this.MouseEnter += (s, e) => { isHovered = true; InvalidateVisual(); };
            this.MouseLeave += (s, e) => { isHovered = false; isPressed = false; InvalidateVisual(); };
            this.MouseDown += (s, e) => { isPressed = true; InvalidateVisual(); };
            this.MouseUp += (s, e) =>
            {
                if (isPressed)
                {
                    isPressed = false;
                    InvalidateVisual();
                    Click?.Invoke(this, new RoutedEventArgs());
                }
            };

            this.Cursor = Cursors.Hand;

            // 아이콘 이미지 로드
            LoadIcon();
        }

        private void LoadIcon()
        {
            if (!string.IsNullOrEmpty(IconPath))
            {
                try
                {
                    iconImageBrush = new ImageBrush();
                    iconImageBrush.ImageSource = new BitmapImage(new Uri(IconPath, UriKind.RelativeOrAbsolute));
                    iconImageBrush.Stretch = Stretch.UniformToFill;
                }
                catch
                {
                    iconImageBrush = null;
                }
            }
        }

        /// <summary>
        /// MFC의 OnDraw()와 동일 - DrawingContext로 직접 그리기
        /// </summary>
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            double width = this.ActualWidth > 0 ? this.ActualWidth : this.Width;
            double height = this.ActualHeight > 0 ? this.ActualHeight : this.Height;

            // 1. 배경 그리기 (MFC: pDC->FillRect())
            Color bgColor = Color.FromRgb(60, 60, 60);
            if (isPressed)
                bgColor = Color.FromRgb(50, 50, 50);
            else if (isHovered)
                bgColor = Color.FromRgb(70, 70, 70);

            Brush backgroundBrush = new SolidColorBrush(bgColor);
            dc.DrawRectangle(backgroundBrush, null, new Rect(0, 0, width, height));

            // 2. 하단 구분선 그리기 (MFC: pDC->MoveTo(), LineTo())
            Pen separatorPen = new Pen(new SolidColorBrush(Color.FromRgb(85, 85, 85)), 1);
            dc.DrawLine(separatorPen, new Point(0, height - 1), new Point(width, height - 1));

            // 3. 왼쪽 아이콘 영역
            double iconSize = 50;
            double iconX = 15;
            double iconY = (height - iconSize) / 2;
            Point iconCenter = new Point(iconX + iconSize / 2, iconY + iconSize / 2);

            // 아이콘 배경 원 그리기 (MFC: pDC->Ellipse())
            Brush iconBgBrush = new SolidColorBrush(IconColor);
            dc.DrawEllipse(iconBgBrush, null, iconCenter, iconSize / 2, iconSize / 2);

            // 아이콘 이미지 또는 기본 심볼 그리기
            if (iconImageBrush != null && iconImageBrush.ImageSource != null)
            {
                // 이미지가 있으면 원형으로 클리핑하여 표시
                dc.PushClip(new EllipseGeometry(iconCenter, iconSize / 2, iconSize / 2));
                dc.DrawRectangle(iconImageBrush, null, new Rect(iconX, iconY, iconSize, iconSize));
                dc.Pop();
            }
            else
            {
                // 기본 심볼 (MFC: pDC->DrawText() 또는 커스텀 그리기)
                FormattedText iconText = new FormattedText(
                    "●",
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface("Segoe UI"),
                    24,
                    Brushes.White,
                    VisualTreeHelper.GetDpi(this).PixelsPerDip);

                Point iconTextPos = new Point(
                    iconCenter.X - iconText.Width / 2,
                    iconCenter.Y - iconText.Height / 2);

                dc.DrawText(iconText, iconTextPos);
            }

            // 4. 오른쪽 텍스트 영역
            double textX = iconX + iconSize + 10;
            double textAreaWidth = width - textX - 15;

            // 제목 그리기 (MFC: pDC->DrawText())
            FormattedText titleText = new FormattedText(
                Title,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.SemiBold, FontStretches.Normal),
                16,
                Brushes.White,
                VisualTreeHelper.GetDpi(this).PixelsPerDip);

            titleText.MaxTextWidth = textAreaWidth;
            titleText.Trimming = TextTrimming.CharacterEllipsis;

            double titleY = (height - titleText.Height - 16) / 2;
            dc.DrawText(titleText, new Point(textX, titleY));

            // 설명 그리기
            FormattedText descText = new FormattedText(
                Description,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI"),
                12,
                new SolidColorBrush(Colors.LightGray),
                VisualTreeHelper.GetDpi(this).PixelsPerDip);

            descText.MaxTextWidth = textAreaWidth;
            descText.Trimming = TextTrimming.CharacterEllipsis;

            double descY = titleY + titleText.Height + 2;
            dc.DrawText(descText, new Point(textX, descY));

            // 5. Hover 효과 (MFC: 추가 하이라이트)
            if (isHovered && !isPressed)
            {
                Pen highlightPen = new Pen(new SolidColorBrush(Color.FromArgb(50, 255, 255, 255)), 2);
                dc.DrawRectangle(null, highlightPen, new Rect(1, 1, width - 2, height - 2));
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            InvalidateVisual();
        }
    }
}
