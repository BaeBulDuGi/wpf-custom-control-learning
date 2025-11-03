using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp.Controls.CustomControls
{
    /// <summary>
    /// Dependency Property 학습용 커스텀 프로그레스 바
    /// - Dependency Property의 특징과 장점을 보여줍니다
    /// - 데이터 바인딩, 애니메이션, 스타일 지원
    /// </summary>
    public class CustomProgressBar : Control
    {
        // ===== Dependency Property 정의 =====
        // 일반 프로퍼티와 다르게 특별한 기능을 제공합니다

        /// <summary>
        /// Value: 현재 값 (0-100)
        /// - 데이터 바인딩 가능
        /// - 값 변경 시 자동으로 UI 업데이트
        /// - 애니메이션 적용 가능
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value",                          // 프로퍼티 이름
                typeof(double),                   // 타입
                typeof(CustomProgressBar),        // 소유자 클래스
                new FrameworkPropertyMetadata(
                    0.0,                          // 기본값
                    FrameworkPropertyMetadataOptions.AffectsRender,  // UI 다시 그리기
                    OnValueChanged,               // 값 변경 시 콜백
                    CoerceValue));                // 값 검증 콜백

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// Maximum: 최대값
        /// </summary>
        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register("Maximum", typeof(double), typeof(CustomProgressBar),
                new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        /// <summary>
        /// BarColor: 프로그레스 바 색상
        /// </summary>
        public static readonly DependencyProperty BarColorProperty =
            DependencyProperty.Register("BarColor", typeof(Brush), typeof(CustomProgressBar),
                new FrameworkPropertyMetadata(new SolidColorBrush(Colors.DodgerBlue),
                    FrameworkPropertyMetadataOptions.AffectsRender));

        public Brush BarColor
        {
            get { return (Brush)GetValue(BarColorProperty); }
            set { SetValue(BarColorProperty, value); }
        }

        /// <summary>
        /// ShowPercentage: 퍼센트 표시 여부
        /// </summary>
        public static readonly DependencyProperty ShowPercentageProperty =
            DependencyProperty.Register("ShowPercentage", typeof(bool), typeof(CustomProgressBar),
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));

        public bool ShowPercentage
        {
            get { return (bool)GetValue(ShowPercentageProperty); }
            set { SetValue(ShowPercentageProperty, value); }
        }

        // ===== Dependency Property 콜백 메서드 =====

        /// <summary>
        /// 값 변경 시 호출되는 콜백
        /// - 이전 값과 새 값에 접근 가능
        /// - 추가 로직 실행 가능
        /// </summary>
        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CustomProgressBar;
            if (control != null)
            {
                double oldValue = (double)e.OldValue;
                double newValue = (double)e.NewValue;

                // 값 변경 이벤트 발생
                control.ValueChanged?.Invoke(control, new ValueChangedEventArgs(oldValue, newValue));

                // 디버그 출력 (학습용)
                System.Diagnostics.Debug.WriteLine($"Progress 값 변경: {oldValue:F1} → {newValue:F1}");
            }
        }

        /// <summary>
        /// 값 검증 콜백 (Coerce)
        /// - 유효하지 않은 값을 자동으로 수정
        /// - 0-Maximum 범위로 제한
        /// </summary>
        private static object CoerceValue(DependencyObject d, object baseValue)
        {
            var control = d as CustomProgressBar;
            double value = (double)baseValue;

            if (control != null)
            {
                // 0 미만이면 0으로
                if (value < 0) return 0.0;
                // Maximum 초과하면 Maximum으로
                if (value > control.Maximum) return control.Maximum;
            }

            return value;
        }

        // ===== 커스텀 이벤트 =====
        public event EventHandler<ValueChangedEventArgs> ValueChanged;

        // ===== 생성자 =====
        public CustomProgressBar()
        {
            this.Width = 300;
            this.Height = 40;
        }

        // ===== OnRender - 실제 그리기 =====
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            double width = this.ActualWidth > 0 ? this.ActualWidth : this.Width;
            double height = this.ActualHeight > 0 ? this.ActualHeight : this.Height;

            // 1. 배경 (회색 테두리)
            Brush bgBrush = new SolidColorBrush(Color.FromRgb(50, 50, 50));
            Pen borderPen = new Pen(new SolidColorBrush(Color.FromRgb(100, 100, 100)), 2);
            dc.DrawRoundedRectangle(bgBrush, borderPen, new Rect(0, 0, width, height), 10, 10);

            // 2. 프로그레스 바 (진행률에 따라)
            double percentage = Maximum > 0 ? (Value / Maximum) : 0;
            double barWidth = (width - 8) * percentage;  // 여백 4px씩

            if (barWidth > 0)
            {
                // 클리핑 (둥근 모서리 처리)
                dc.PushClip(new RectangleGeometry(new Rect(4, 4, barWidth, height - 8), 8, 8));

                // 그라디언트 효과
                LinearGradientBrush gradientBrush = new LinearGradientBrush();
                gradientBrush.StartPoint = new Point(0, 0);
                gradientBrush.EndPoint = new Point(0, 1);

                Color barColor = (BarColor as SolidColorBrush)?.Color ?? Colors.DodgerBlue;
                gradientBrush.GradientStops.Add(new GradientStop(LightenColor(barColor, 0.3), 0));
                gradientBrush.GradientStops.Add(new GradientStop(barColor, 0.5));
                gradientBrush.GradientStops.Add(new GradientStop(DarkenColor(barColor, 0.2), 1));

                dc.DrawRoundedRectangle(gradientBrush, null,
                    new Rect(4, 4, barWidth, height - 8), 8, 8);

                dc.Pop();
            }

            // 3. 퍼센트 텍스트
            if (ShowPercentage)
            {
                string percentText = $"{percentage * 100:F1}%";
                FormattedText text = new FormattedText(
                    percentText,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal),
                    14,
                    Brushes.White,
                    VisualTreeHelper.GetDpi(this).PixelsPerDip);

                Point textPos = new Point(
                    (width - text.Width) / 2,
                    (height - text.Height) / 2);

                // 텍스트 그림자
                dc.DrawText(new FormattedText(percentText,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(new FontFamily("Segoe UI"), FontStyles.Normal, FontWeights.Bold, FontStretches.Normal),
                    14,
                    new SolidColorBrush(Color.FromArgb(150, 0, 0, 0)),
                    VisualTreeHelper.GetDpi(this).PixelsPerDip),
                    new Point(textPos.X + 1, textPos.Y + 1));

                dc.DrawText(text, textPos);
            }
        }

        // ===== 헬퍼 메서드 =====
        private Color LightenColor(Color color, double factor)
        {
            return Color.FromRgb(
                (byte)Math.Min(255, color.R + (255 - color.R) * factor),
                (byte)Math.Min(255, color.G + (255 - color.G) * factor),
                (byte)Math.Min(255, color.B + (255 - color.B) * factor));
        }

        private Color DarkenColor(Color color, double factor)
        {
            return Color.FromRgb(
                (byte)(color.R * (1 - factor)),
                (byte)(color.G * (1 - factor)),
                (byte)(color.B * (1 - factor)));
        }
    }

    /// <summary>
    /// 값 변경 이벤트 인자
    /// </summary>
    public class ValueChangedEventArgs : EventArgs
    {
        public double OldValue { get; }
        public double NewValue { get; }

        public ValueChangedEventArgs(double oldValue, double newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
