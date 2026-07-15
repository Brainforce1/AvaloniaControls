using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
using System;
namespace BrainForceOne.BrainforceControls;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using SkiaSharp;
using System;

public class Dial : Control
{
    // Properties
    public static readonly StyledProperty<double> MinimumProperty =
        AvaloniaProperty.Register<Dial, double>(nameof(Minimum), 0);

    public static readonly StyledProperty<double> MaximumProperty =
        AvaloniaProperty.Register<Dial, double>(nameof(Maximum), 100);

    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<Dial, double>(nameof(Value), 0);

    public static readonly StyledProperty<Color> IndicatorColorProperty =
        AvaloniaProperty.Register<Dial, Color>(nameof(IndicatorColor), Colors.White);

    public static readonly StyledProperty<double> IndicatorLengthProperty =
        AvaloniaProperty.Register<Dial, double>(nameof(IndicatorLength), 14);

    public static readonly StyledProperty<double> IndicatorThicknessProperty =
        AvaloniaProperty.Register<Dial, double>(nameof(IndicatorThickness), 3);

    public static readonly StyledProperty<Bitmap?> SkinProperty =
        AvaloniaProperty.Register<Dial, Bitmap?>(nameof(Skin));

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

    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public Color IndicatorColor
    {
        get => GetValue(IndicatorColorProperty);
        set => SetValue(IndicatorColorProperty, value);
    }

    public double IndicatorLength
    {
        get => GetValue(IndicatorLengthProperty);
        set => SetValue(IndicatorLengthProperty, value);
    }

    public double IndicatorThickness
    {
        get => GetValue(IndicatorThicknessProperty);
        set => SetValue(IndicatorThicknessProperty, value);
    }

    public Bitmap? Skin
    {
        get => GetValue(SkinProperty);
        set => SetValue(SkinProperty, value);
    }

    // Drag state
    private bool _isDragging;
    private double _dragStartAngle;
    private double _dragStartValue;

    // Cached visuals
    private IBrush? _shadowBrush;
    private IBrush? _metalBrush;
    private IBrush? _rimBrush;
    private Pen? _bevelPen;
    private Pen? _glossPen;
    private Pen? _tickPen;

    static Dial()
    {
        ValueProperty.Changed.AddClassHandler<Dial>((d, _) => d.InvalidateVisual());
    }

    public Dial()
    {
        // Hit‑test altijd aan
        IsHitTestVisible = true;
        Focusable = false;
    }

    // ---------- INPUT ----------

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        var props = e.GetCurrentPoint(this).Properties;
        if (!props.IsLeftButtonPressed)
            return;

        var p = e.GetPosition(this);
        double angle = GetAngleFromPoint(p);

        // Click‑to‑set
        double normalized = (angle + 135) / 270.0;
        double newValue = Minimum + normalized * (Maximum - Minimum);
        Value = Math.Clamp(newValue, Minimum, Maximum);

        _isDragging = true;
        _dragStartAngle = angle;
        _dragStartValue = Value;

        e.Pointer.Capture(this);
        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (!_isDragging)
            return;

        var p = e.GetPosition(this);
        double currentAngle = GetAngleFromPoint(p);

        double delta = currentAngle - _dragStartAngle;

        // wrap‑around smoothing
        if (delta > 180) delta -= 360;
        if (delta < -180) delta += 360;

        double newValue = _dragStartValue + (delta / 270.0) * (Maximum - Minimum);
        Value = Math.Clamp(newValue, Minimum, Maximum);

        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        _isDragging = false;
        e.Pointer.Capture(null);
        e.Handled = true;
    }

    private double GetAngleFromPoint(Point p)
    {
        double dx = p.X - Bounds.Width / 2;
        double dy = p.Y - Bounds.Height / 2;

        double angle = Math.Atan2(dy, dx) * 180 / Math.PI;
        if (angle < 0)
            angle += 360;

        return angle;
    }

    // ---------- RENDER ----------

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        double size = Math.Min(Bounds.Width, Bounds.Height);
        if (size <= 0)
            return;

        EnsureCachedVisuals();

        double radius = size * 0.45;
        Point center = new(size / 2, size / 2);

        // Shadow
        if (_shadowBrush != null)
            context.DrawEllipse(_shadowBrush, null, center, radius * 1.1, radius * 1.1);

        // Clip to round dial
        var clip = new EllipseGeometry { Center = center, RadiusX = radius, RadiusY = radius };
        using (context.PushGeometryClip(clip))
        {
            // Skin or metal base
            if (Skin != null)
            {
                var rect = new Rect(center.X - radius, center.Y - radius, radius * 2, radius * 2);
                context.DrawImage(Skin, rect);
            }
            else if (_metalBrush != null)
            {
                context.DrawEllipse(_metalBrush, null, center, radius, radius);
            }

            // Rim
            if (_rimBrush != null)
                context.DrawEllipse(_rimBrush, null, center, radius, radius);

            // Bevel
            if (_bevelPen != null)
                context.DrawEllipse(null, _bevelPen, center, radius - 2, radius - 2);

            // Gloss
            if (_glossPen != null)
                context.DrawEllipse(null, _glossPen, center, radius - 4, radius - 4);

            // Indicator
            DrawIndicator(context, center, radius);
        }

        // Ticks buiten de knob
        DrawTicks(context, center, radius);
    }

    private void EnsureCachedVisuals()
    {
        if (_shadowBrush == null)
        {
            _shadowBrush = new RadialGradientBrush
            {
                GradientOrigin = new RelativePoint(0.5, 0.8, RelativeUnit.Relative),
                Center = new RelativePoint(0.5, 0.8, RelativeUnit.Relative),
                RadiusX = new RelativeScalar(1.2, RelativeUnit.Relative),
                RadiusY = new RelativeScalar(1.2, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.FromArgb(80, 0, 0, 0), 0.5),
                    new GradientStop(Color.FromArgb(0, 0, 0, 0), 1.0)
                }
            };
        }

        if (_metalBrush == null)
        {
            _metalBrush = new RadialGradientBrush
            {
                GradientOrigin = new RelativePoint(0.3, 0.3, RelativeUnit.Relative),
                Center = new RelativePoint(0.5, 0.5, RelativeUnit.Relative),
                RadiusX = new RelativeScalar(0.5, RelativeUnit.Relative),
                RadiusY = new RelativeScalar(0.5, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.FromRgb(220, 220, 220), 0.0),
                    new GradientStop(Color.FromRgb(120, 120, 120), 0.5),
                    new GradientStop(Color.FromRgb(40, 40, 40), 1.0)
                }
            };
        }

        if (_rimBrush == null)
        {
            _rimBrush = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.FromArgb(180, 255, 255, 255), 0.0),
                    new GradientStop(Color.FromArgb(120, 80, 80, 80), 1.0)
                }
            };
        }

        if (_bevelPen == null)
        {
            _bevelPen = new Pen(new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.FromRgb(240, 240, 240), 0.0),
                    new GradientStop(Color.FromRgb(80, 80, 80), 1.0)
                }
            }, 4);
        }

        if (_glossPen == null)
        {
            _glossPen = new Pen(new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.FromArgb(200, 255, 255, 255), 0.0),
                    new GradientStop(Color.FromArgb(0, 255, 255, 255), 0.5),
                    new GradientStop(Color.FromArgb(120, 200, 200, 200), 1.0)
                }
            }, 2);
        }

        if (_tickPen == null)
        {
            _tickPen = new Pen(new SolidColorBrush(Color.FromRgb(100, 100, 100)), 1);
        }
    }

    private void DrawIndicator(DrawingContext context, Point center, double radius)
    {
        double normalized = (Value - Minimum) / (Maximum - Minimum);
        double angle = normalized * 270 - 135;
        double rad = Math.PI * angle / 180;

        Point start = new(
            center.X + Math.Cos(rad) * (radius - IndicatorLength - 4),
            center.Y + Math.Sin(rad) * (radius - IndicatorLength - 4));

        Point end = new(
            center.X + Math.Cos(rad) * (radius - 4),
            center.Y + Math.Sin(rad) * (radius - 4));

        var pen = new Pen(new SolidColorBrush(IndicatorColor), IndicatorThickness);
        context.DrawLine(pen, start, end);
    }

    private void DrawTicks(DrawingContext context, Point center, double radius)
    {
        if (_tickPen == null)
            return;

        double tickRadius = radius + 6;

        for (int i = 0; i < 60; i++)
        {
            double a = i * 360.0 / 60;
            double r = Math.PI * a / 180;

            Point ts = new(
                center.X + Math.Cos(r) * tickRadius,
                center.Y + Math.Sin(r) * tickRadius);

            Point te = new(
                center.X + Math.Cos(r) * (tickRadius + 4),
                center.Y + Math.Sin(r) * (tickRadius + 4));

            context.DrawLine(_tickPen, ts, te);
        }
    }
}


