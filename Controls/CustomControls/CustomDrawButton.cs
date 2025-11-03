using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfApp.Controls.CustomControls
{
    /// <summary>
    /// Custom Control with OnRender: MFC의 OnDraw와 가장 유사한 방식
    /// - OnRender()를 오버라이드하여 DrawingContext로 직접 그리기
    /// - 장점: 완전한 제어, 고성능, MFC 개발자에게 친숙
    /// - 단점: 코드가 많아짐, 모든 것을 직접 그려야 함
    /// </summary>
    public class CustomDrawButton : Control
    {
        // Dependency Properties
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(CustomDrawButton),
                new FrameworkPropertyMetadata("Draw Button", FrameworkPropertyMetadataOptions.AffectsRender));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // 상태 관리
        private bool isHovered = false;
        private bool isPressed = false;

        // Click 이벤트
        public event RoutedEventHandler Click;

        public CustomDrawButton()
        {
            // 기본 크기 설정
            this.Width = 200;
            this.Height = 50;

            // 마우스 이벤트 등록
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
        }

        /// <summary>
        /// MFC의 OnDraw()와 동일한 역할
        /// DrawingContext = MFC의 CDC* pDC
        /// </summary>
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // 크기 정보
            double width = this.ActualWidth > 0 ? this.ActualWidth : this.Width;
            double height = this.ActualHeight > 0 ? this.ActualHeight : this.Height;

            // 상태에 따른 색상 결정
            Color bgColor = Colors.LightGreen;
            Color borderColor = Colors.DarkGreen;
            Color textColor = Colors.DarkGreen;

            if (isPressed)
            {
                bgColor = Colors.DarkGreen;
                borderColor = Colors.Black;
                textColor = Colors.White;
            }
            else if (isHovered)
            {
                bgColor = Colors.LightSeaGreen;
                borderColor = Colors.DarkGreen;
            }

            // 1. 배경 그리기 (MFC: pDC->FillRect())
            Brush backgroundBrush = new SolidColorBrush(bgColor);
            Pen borderPen = new Pen(new SolidColorBrush(borderColor), 3);

            // 둥근 사각형 그리기
            dc.DrawRoundedRectangle(backgroundBrush, borderPen,
                new Rect(0, 0, width, height), 15, 15);

            // 2. 그라디언트 효과 추가 (MFC에서는 GradientFill 사용)
            if (!isPressed)
            {
                LinearGradientBrush gradientBrush = new LinearGradientBrush();
                gradientBrush.StartPoint = new Point(0, 0);
                gradientBrush.EndPoint = new Point(0, 1);
                gradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(80, 255, 255, 255), 0));
                gradientBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 255, 255, 255), 1));

                dc.DrawRoundedRectangle(gradientBrush, null,
                    new Rect(0, 0, width, height * 0.5), 15, 15);
            }

            // 3. 원 그리기 (MFC: pDC->Ellipse())
            double circleRadius = 12;
            Point circleCenter = new Point(25, height / 2);
            Brush circleBrush = new SolidColorBrush(borderColor);

            dc.DrawEllipse(circleBrush, null, circleCenter, circleRadius, circleRadius);

            // 4. 텍스트 그리기 (MFC: pDC->DrawText())
            FormattedText formattedText = new FormattedText(
                Text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Segoe UI"),
                16,
                new SolidColorBrush(textColor),
                VisualTreeHelper.GetDpi(this).PixelsPerDip);

            Point textPosition = new Point(
                (width - formattedText.Width) / 2,
                (height - formattedText.Height) / 2);

            dc.DrawText(formattedText, textPosition);

            // 5. 추가 장식: 테두리 하이라이트 (MFC: pDC->Draw3dRect() 유사)
            if (isHovered && !isPressed)
            {
                Pen highlightPen = new Pen(new SolidColorBrush(Color.FromArgb(100, 255, 255, 255)), 2);
                dc.DrawRoundedRectangle(null, highlightPen,
                    new Rect(2, 2, width - 4, height - 4), 13, 13);
            }
        }

        /// <summary>
        /// 크기 변경 시 다시 그리기 (MFC: OnSize())
        /// </summary>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            InvalidateVisual(); // MFC의 Invalidate()와 동일
        }
    }
}
