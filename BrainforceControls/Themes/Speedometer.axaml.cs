using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;
using System;

namespace BrainForceOne.BrainforceControls;
    public partial class Speedometer : TemplatedControl
    {
        public static readonly StyledProperty<double> ValueProperty = AvaloniaProperty.Register<Speedometer, double>(nameof(Value), 0);
        public static readonly StyledProperty<bool> GlassEffectProperty = AvaloniaProperty.Register<Speedometer, bool>(nameof(GlassEffect), false);
        public static readonly StyledProperty<int> NeedleWidthProperty = AvaloniaProperty.Register<Speedometer, int>(nameof(NeedleWidth), 3);
        public static readonly StyledProperty<IImmutableSolidColorBrush> NeedleColorProperty = AvaloniaProperty.Register<Speedometer, IImmutableSolidColorBrush>(nameof(NeedleColor), Brushes.Red);
        public static readonly StyledProperty<GaugePosition> PositionProperty = AvaloniaProperty.Register<Speedometer, GaugePosition>(nameof(Position), GaugePosition.Top);
        public static readonly StyledProperty<double> SweepAngleProperty = AvaloniaProperty.Register<Speedometer, double>(nameof(SweepAngle), 180);
        public static readonly StyledProperty<double> MinimumProperty = AvaloniaProperty.Register<Speedometer, double>(nameof(Minimum), 0);
        public static readonly StyledProperty<double> MaximumProperty = AvaloniaProperty.Register<Speedometer, double>(nameof(Maximum), 100);
        public static readonly StyledProperty<int> TextSizeProperty = AvaloniaProperty.Register<Speedometer, int>(nameof(TextSize), 16);
        public static readonly StyledProperty<string?> UnitProperty = AvaloniaProperty.Register<Speedometer, string?>(nameof(UnitText), null);
        public static readonly StyledProperty<double> UnitTextSizeProperty = AvaloniaProperty.Register<Speedometer, double>(nameof(UnitTextSize), 12);
        public static readonly StyledProperty<bool> PreviewModeProperty = AvaloniaProperty.Register<ProgressRingControl, bool>(nameof(PreviewMode), false);

        public bool PreviewMode
        {
            get => GetValue(PreviewModeProperty);
            set => SetValue(PreviewModeProperty, value);
        }


    public double UnitTextSize
        {
            get => GetValue(UnitTextSizeProperty);
            set => SetValue(UnitTextSizeProperty, value);
        }

        public string? UnitText
        {
            get => GetValue(UnitProperty);
            set => SetValue(UnitProperty, value);
        }

        public Func<double, string>? ValueFormatter
        {
            get; set;
        }

        public int TextSize
        {
            get => GetValue(TextSizeProperty);
            set => SetValue(TextSizeProperty, value);
        }

        public double Minimum
        {
            get => GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public double Maximum
        {
            get => GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public double SweepAngle
        {
            get => GetValue(SweepAngleProperty);
            set => SetValue(SweepAngleProperty, value);
        }

        public GaugePosition Position
        {
            get => GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public bool GlassEffect
        {
            get => GetValue(GlassEffectProperty);
            set => SetValue(GlassEffectProperty, value);
        }

        public double Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public int NeedleWidth
        {
            get => GetValue(NeedleWidthProperty);
            set => SetValue(NeedleWidthProperty, value);
        }

        public IImmutableSolidColorBrush NeedleColor
        {
            get => GetValue(NeedleColorProperty);
            set => SetValue(NeedleColorProperty, value);
        }


        static Speedometer()
        {
            ValueProperty.Changed.AddClassHandler<Speedometer>((x, e) =>
                x.InvalidateVisual());

            GlassEffectProperty.Changed.AddClassHandler<Speedometer>((x, e) =>
                x.InvalidateVisual());

            NeedleWidthProperty.Changed.AddClassHandler<Speedometer>((x, e) =>
                x.InvalidateVisual());

            NeedleColorProperty.Changed.AddClassHandler<Speedometer>((x, e) =>
                x.InvalidateVisual());

            PositionProperty.Changed.AddClassHandler<Speedometer>((x, e) =>
            {
                x.InvalidateVisual();
            });

            SweepAngleProperty.Changed.AddClassHandler<Speedometer>((x, e) =>
            {
                x.InvalidateVisual();
            });

            MinimumProperty.Changed.AddClassHandler<Speedometer>((x, e) => x.InvalidateVisual());
            MaximumProperty.Changed.AddClassHandler<Speedometer>((x, e) => x.InvalidateVisual());
        }

        private Canvas? _canvas;

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _canvas = e.NameScope.Find<Canvas>("PART_SpeedometerCanvas");

            InvalidateVisual();
        }

    private double GetClampedSweep()
        {
            return Math.Clamp(SweepAngle, 45, 360);
        }

        private double GetStartAngle(double sweep)
        {
            return Position switch
            {
                GaugePosition.Top => -90 - sweep / 2,
                GaugePosition.Right => 0 - sweep / 2,
                GaugePosition.Left => 180 - sweep / 2,
                GaugePosition.Bottom => 90 - sweep / 2,
                _ => -90 - sweep / 2
            };
        }

        public enum GaugePosition
        {
            Top,
            Bottom,
            Left,
            Right
        }

        private double ValueToAngle(double value)
        {
            double min = Minimum;
            double max = Maximum;

            if (max <= min)
                return 0;

            double t = (value - min) / (max - min);
            t = Math.Clamp(t, 0, 1);

            double sweep = GetClampedSweep();
            double start = GetStartAngle(sweep);

            return start + t * sweep;
        }

        private void DrawValueText(DrawingContext ctx, Point center, double radius)
        {
            string valueText = ValueFormatter != null
                ? ValueFormatter(Value)
                : Value.ToString("0");

        var typeface = new Typeface(FontFamily);

        var valueFormatted = new FormattedText(
                valueText,
                System.Globalization.CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                typeface,
                TextSize,
                Brushes.White);

            // center
            var valuePos = new Point(
                center.X - valueFormatted.Width / 2,
                center.Y + radius * 0.25 - valueFormatted.Height / 2
            );

            ctx.DrawText(valueFormatted, valuePos);

            if (!string.IsNullOrEmpty(UnitText))
            {
                var unitFormatted = new FormattedText(
                    UnitText,
                    System.Globalization.CultureInfo.InvariantCulture,
                    FlowDirection.LeftToRight,
                    typeface,
                    UnitTextSize,
                    new SolidColorBrush(Color.FromRgb(180, 180, 180)));

                var unitPos = new Point(
                    center.X - unitFormatted.Width / 2,
                    valuePos.Y + valueFormatted.Height + 2
                );

                ctx.DrawText(unitFormatted, unitPos);
            }
        }


        private void DrawBackground(DrawingContext ctx, Point center, double radius)
        {
            // outer circle
            var outerBrush = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops = new GradientStops
        {
            new GradientStop(Color.Parse("#222"), 0),
            new GradientStop(Color.Parse("#111"), 1)
        }
            };

            ctx.DrawEllipse(outerBrush, null, center, radius + 20, radius + 20);

            // inner dial
            var innerBrush = new RadialGradientBrush
            {
                GradientStops = new GradientStops
        {
            new GradientStop(Color.Parse("#2c2c2c"), 0),
            new GradientStop(Color.Parse("#111"), 1)
        }
            };

            ctx.DrawEllipse(innerBrush, null, center, radius, radius);
        }

        private void DrawGlassEffect(DrawingContext ctx, Point center, double radius)
        {
            if (!GlassEffect) return;

            // Top highlight (gloss)
            var glossBrush = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
                GradientStops = new GradientStops
        {
            new GradientStop(Color.FromArgb(120, 255, 255, 255), 0),
            new GradientStop(Color.FromArgb(20, 255, 255, 255), 0.3),
            new GradientStop(Color.FromArgb(0, 255, 255, 255), 1)
        }
            };

            var rect = new Rect(
                center.X - radius,
                center.Y - radius,
                radius * 2,
                radius * 2
            );

            ctx.DrawEllipse(glossBrush, null, center, radius, radius);

            // subtle border
            var borderPen = new Pen(
                new SolidColorBrush(Color.FromArgb(50, 255, 255, 255)),
                1
            );

            ctx.DrawEllipse(null, borderPen, center, radius, radius);
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);

            var rect = Bounds;
            var center = new Point(rect.Width / 2, rect.Height / 2);
            double radius = Math.Min(rect.Width, rect.Height) * 0.4;

            DrawBackground(context, center, radius);

            DrawArc(context, center, radius);
            DrawTicks(context, center, radius);
            DrawValueText(context, center, radius);
            DrawNeedle(context, center, radius);        
            DrawGlassEffect(context, center, radius);
        }

        private void DrawArc(DrawingContext ctx, Point center, double radius)
        {
            double sweep = GetClampedSweep();
            double startAngle = GetStartAngle(sweep);
            double endAngle = startAngle + sweep;

            var geometry = new StreamGeometry();

            using (var g = geometry.Open())
            {
                var start = AngleToPoint(center, radius, startAngle);
                var end = AngleToPoint(center, radius, endAngle);

                g.BeginFigure(start, false);

                g.ArcTo(
                    end,
                    new Size(radius, radius),
                    0,
                    sweep > 180, 
                    SweepDirection.Clockwise);
            }

            ctx.DrawGeometry(null, new Pen(Brushes.DarkGray, 4), geometry);
        }

        private void DrawTicks(DrawingContext ctx, Point center, double radius)
        {
            int tickCount = 20;

            for (int i = 0; i <= tickCount; i++)
            {
                double t = i / (double)tickCount;
                double value = Minimum + t * (Maximum - Minimum);

                double angle = ValueToAngle(value);

                var p1 = AngleToPoint(center, radius - 10, angle);
                var p2 = AngleToPoint(center, radius, angle);

                // Glow logic
                double distance = Math.Abs(Value - value);
                double glowFactor = Math.Max(0, 1 - distance / 10);


                var color = InterpolateColor(Colors.Gray, Colors.Lime, glowFactor);

                // boost saturation
                color = Color.FromRgb(
                    (byte)Math.Min(255, color.R + glowFactor * 80),
                    (byte)Math.Min(255, color.G + glowFactor * 80),
                    (byte)Math.Min(255, color.B + glowFactor * 80)
                );


                var pen = new Pen(new SolidColorBrush(color), 2 + glowFactor * 3);

                ctx.DrawLine(pen, p1, p2);
            }
        }

        private void DrawNeedle(DrawingContext ctx, Point center, double radius)
        {
            double angle = ValueToAngle(Value);

            var end = AngleToPoint(center, radius - 20, angle);

            var pen = new Pen(NeedleColor, NeedleWidth);

            ctx.DrawLine(pen, center, end);

            // center circle
            ctx.DrawEllipse(Brushes.Black, null, center, 5, 5);
        }

        private Point AngleToPoint(Point center, double radius, double angleDeg)
        {
            double rad = angleDeg * Math.PI / 180;

            return new Point(
                center.X + radius * Math.Cos(rad),
                center.Y + radius * Math.Sin(rad)
            );
        }

        private Color InterpolateColor(Color a, Color b, double t)
        {
            t = Math.Clamp(t, 0, 1);

            return Color.FromRgb(
                (byte)(a.R + (b.R - a.R) * t),
                (byte)(a.G + (b.G - a.G) * t),
                (byte)(a.B + (b.B - a.B) * t)
            );
        }
    }