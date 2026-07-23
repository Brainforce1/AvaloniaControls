using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Collections.Generic;

namespace BrainForceOne.BrainforceControls;

public class DialControl : Control
{
    public static readonly StyledProperty<bool> DisabledProperty =
    AvaloniaProperty.Register<DialControl, bool>(
        nameof(Disabled),
        false);

    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<DialControl, string>(
            nameof(Title),
            "Title");

    public static readonly StyledProperty<int> ValueProperty =
        AvaloniaProperty.Register<DialControl, int>(
            nameof(Value),
            0);

    public static readonly StyledProperty<double> MinimumProperty =
        AvaloniaProperty.Register<DialControl, double>(
            nameof(Minimum),
            0);

    public static readonly StyledProperty<double> MaximumProperty =
        AvaloniaProperty.Register<DialControl, double>(
            nameof(Maximum),
            100);

    public static readonly StyledProperty<double> RingThicknessProperty =
        AvaloniaProperty.Register<DialControl, double>(
            nameof(RingThickness),
            12);

    public static readonly StyledProperty<IBrush> TrackBrushProperty =
        AvaloniaProperty.Register<DialControl, IBrush>(
            nameof(TrackBrush),
            Brushes.LightGray);

    public static readonly StyledProperty<IBrush> ValueTextBrushProperty =
        AvaloniaProperty.Register<DialControl, IBrush>(
            nameof(ValueTextBrush),
            Brushes.LightGray);
    public static readonly StyledProperty<int> ValueTextSizeProperty =
    AvaloniaProperty.Register<DialControl, int>(
        nameof(ValueTextSize),
        46);

    public static readonly StyledProperty<int> TitleTextSizeProperty =
        AvaloniaProperty.Register<DialControl, int>(
            nameof(TitleTextSize),
            16);

    public static readonly StyledProperty<IBrush> TitleTextBrushProperty =
        AvaloniaProperty.Register<DialControl, IBrush>(
            nameof(TitleTextBrush),
            Brushes.LightGray);

    public static readonly StyledProperty<IBrush> ProgressBrushProperty =
        AvaloniaProperty.Register<DialControl, IBrush>(
            nameof(ProgressBrush),
            Brushes.DeepPink);

    public static readonly StyledProperty<IBrush> DisabledProgressBrushProperty =
        AvaloniaProperty.Register<DialControl, IBrush>(
            nameof(DisabledProgressBrush),
            Brushes.Gray);

    public static readonly StyledProperty<string> ValueFormatProperty =
        AvaloniaProperty.Register<DialControl, string>(
            nameof(ValueFormat),
            "%");

    public static readonly StyledProperty<IBrush?> BackgroundProperty =
        AvaloniaProperty.Register<DialControl, IBrush?>(
            nameof(Background),
            Brushes.Transparent);

    public static readonly StyledProperty<IBrush> GradientBrushProperty =
        AvaloniaProperty.Register<DialControl, IBrush>(
            nameof(GradientBrush),
            Brushes.White); // default second color for gradient

    public static readonly StyledProperty<bool> UseGradientProperty =
        AvaloniaProperty.Register<DialControl, bool>(
            nameof(UseGradient),
            false);

    public static readonly StyledProperty<FontFamily> FontFamilyProperty =
        AvaloniaProperty.Register<DialControl, FontFamily>(
            nameof(FontFamily), new FontFamily("Segoe UI"));

    public FontFamily FontFamily
    {
        get => GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }


    public bool UseGradient
    {
        get => GetValue(UseGradientProperty);
        set => SetValue(UseGradientProperty, value);
    }


    public IBrush GradientBrush
    {
        get => GetValue(GradientBrushProperty);
        set => SetValue(GradientBrushProperty, value);
    }


    public IBrush? Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    public event EventHandler<int>? ValueChanged;


    private bool _dragging;
    private double _dragStartValue;
    private Point _dragStartPoint;

    public DialControl()
    {
        Background = Brushes.Transparent;
    }

    private void OnValueChanged(AvaloniaPropertyChangedEventArgs args)
    {
        ValueChanged?.Invoke(this, Value);
    }


    static DialControl()
    {
        ValueProperty.Changed.AddClassHandler<DialControl>((dial, args) =>
        {
            dial.OnValueChanged(args);
        });

        AffectsRender<DialControl>(
            ValueProperty,
            MinimumProperty,
            MaximumProperty,
            RingThicknessProperty,
            TrackBrushProperty,
            ProgressBrushProperty,
            TitleProperty);
    }

    public IBrush DisabledProgressBrush
    {
        get => GetValue(DisabledProgressBrushProperty);
        set => SetValue(DisabledProgressBrushProperty, value);
    }

    public bool Disabled
    {
        get => GetValue(DisabledProperty);
        set => SetValue(DisabledProperty, value);
    }   

    public int ValueTextSize
    {
        get => GetValue(ValueTextSizeProperty);
        set => SetValue(ValueTextSizeProperty, value);
    }

    public int TitleTextSize
    {
        get => GetValue(TitleTextSizeProperty);
        set => SetValue(TitleTextSizeProperty, value);
    }

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public int Value
    {
        get => GetValue(ValueProperty);
        set
        {
            if(!Disabled)
                SetValue(ValueProperty, value);
        }
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

    public double RingThickness
    {
        get => GetValue(RingThicknessProperty);
        set => SetValue(RingThicknessProperty, value);
    }

    public IBrush ValueTextBrush
    {
        get => GetValue(ValueTextBrushProperty);
        set => SetValue(ValueTextBrushProperty, value);
    }

    public IBrush TitleTextBrush
    {
        get => GetValue(TitleTextBrushProperty);
        set => SetValue(TitleTextBrushProperty, value);
    }

    public IBrush TrackBrush
    {
        get => GetValue(TrackBrushProperty);
        set => SetValue(TrackBrushProperty, value);
    }

    public IBrush ProgressBrush
    {
        get => GetValue(ProgressBrushProperty);
        set => SetValue(ProgressBrushProperty, value);
    }

    public string ValueFormat
    {
        get => GetValue(ValueFormatProperty);
        set => SetValue(ValueFormatProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        context.DrawRectangle(
            Background,
            null,
            new Rect(Bounds.Size));

        double size = Math.Min(Bounds.Width, Bounds.Height);

        Rect dialRect = new(
            (Bounds.Width - size) / 2,
            (Bounds.Height - size) / 2,
            size,
            size);

        double radius = size / 2 - RingThickness;

        DrawTrack(context, dialRect, radius);

        DrawProgress(context, dialRect, radius);

        DrawText(context, dialRect);
    }

    private void DrawTrack(
        DrawingContext context,
        Rect rect,
        double radius)
    {
        DrawArc(
            context,
            rect.Center,
            radius,
            135,
            270,
            TrackBrush,
            useGradient: false,
            gradientEndBrush: null);
    }

    private void DrawProgress(
        DrawingContext context,
        Rect rect,
        double radius)
    {
        double percent =
            (Value - Minimum) /
            (Maximum - Minimum);

        double sweep = 270 * percent;

        DrawArc(
            context,
            rect.Center,
            radius,
            135,
            sweep,
            Disabled ? DisabledProgressBrush : ProgressBrush,
            useGradient: UseGradient,
            gradientEndBrush: GradientBrush);
    }


    private void DrawArc(
        DrawingContext context,
        Point center,
        double radius,
        double startAngle,
        double sweepAngle,
        IBrush brush,
        bool useGradient,
        IBrush gradientEndBrush)
    {
        sweepAngle = Math.Max(0, Math.Min(360, sweepAngle));
        if (sweepAngle <= 0) return;

        double halfThickness = RingThickness / 2.0;
        double outerR = radius + halfThickness;
        double innerR = Math.Max(0, radius - halfThickness);

        double startRad = startAngle * Math.PI / 180.0;
        double endRad = (startAngle + sweepAngle) * Math.PI / 180.0;

        int steps = Math.Max(2, (int)Math.Ceiling(Math.Abs(sweepAngle) / 2.0));

        var outerPoints = new List<Point>(steps + 1);
        for (int i = 0; i <= steps; i++)
        {
            double t = (double)i / steps;
            double ang = startRad + t * (endRad - startRad);
            outerPoints.Add(new Point(center.X + outerR * Math.Cos(ang),
                                      center.Y + outerR * Math.Sin(ang)));
        }

        var innerPoints = new List<Point>(steps + 1);
        for (int i = 0; i <= steps; i++)
        {
            double t = (double)i / steps;
            double ang = endRad - t * (endRad - startRad); 
            innerPoints.Add(new Point(center.X + innerR * Math.Cos(ang),
                                      center.Y + innerR * Math.Sin(ang)));
        }

        var poly = new StreamGeometry();
        using (var gc = poly.Open())
        {
            gc.BeginFigure(outerPoints[0], true); 
            for (int i = 1; i < outerPoints.Count; i++)
                gc.LineTo(outerPoints[i]);

            for (int i = 0; i < innerPoints.Count; i++)
                gc.LineTo(innerPoints[i]);

            gc.EndFigure(true);
        }

        IBrush finalBrush = brush;

        if (useGradient)
        {
            var baseColor = (brush as ISolidColorBrush)?.Color ?? Colors.Transparent;
            var gradColor = (gradientEndBrush as ISolidColorBrush)?.Color ?? Colors.Transparent;

            var bounds = poly.Bounds;
            double w = Math.Max(1, bounds.Width);
            double h = Math.Max(1, bounds.Height);

            var startAbs = outerPoints[0];
            var endAbs = outerPoints[outerPoints.Count - 1];

            var relStart = new RelativePoint(
                (startAbs.X - bounds.X) / w,
                (startAbs.Y - bounds.Y) / h,
                RelativeUnit.Relative);

            var relEnd = new RelativePoint(
                (endAbs.X - bounds.X) / w,
                (endAbs.Y - bounds.Y) / h,
                RelativeUnit.Relative);

            finalBrush = new LinearGradientBrush
            {
                StartPoint = relStart,
                EndPoint = relEnd,
                GradientStops =
            {
                new GradientStop(baseColor, 0.0),
                new GradientStop(gradColor, 1.0)
            }
            };
        }

        context.DrawGeometry(
            finalBrush,
            null, 
            poly);
    }

    private void DrawText(
        DrawingContext context,
        Rect rect)
    {
        Typeface typeface;

        try
        {
            var fam = this.FontFamily ?? Typeface.Default.FontFamily;
            typeface = new Typeface(fam, FontStyle.Normal, FontWeight.Normal);
        }
        catch
        {
            // Fallback als FontFamily niet geladen kan worden
            typeface = Typeface.Default;
        }


        var title = new FormattedText(
            Title,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            typeface,
            TitleTextSize,
            TitleTextBrush);

        var valueText = new FormattedText(
            $"{Value}{ValueFormat}",
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            typeface,
            ValueTextSize,
            ValueTextBrush);

        context.DrawText(
            title,
            new Point(
                rect.Center.X - title.Width / 2,
                rect.Center.Y - rect.Height / 9));

        context.DrawText(
            valueText,
            new Point(
                rect.Center.X - valueText.Width / 2,
                rect.Center.Y));
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if(Disabled)
            return;
        _dragging = true;
        _dragStartPoint = e.GetPosition(this);
        _dragStartValue = Value;
        e.Pointer.Capture(this);

        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if(Disabled) return;
        _dragging = false;

        if (e.Pointer.Captured == this)
        {
            e.Pointer.Capture(null);
        }

        e.Handled = true;
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        if (!_dragging || Disabled)
            return;

        double sensitivity = 0.4; 
        if ((e.KeyModifiers & KeyModifiers.Control) != 0) 
        {
            // Fine adjustment mode
            sensitivity = 0.02;   
        }

        var p = e.GetPosition(this);

        double dragDistance =
                (_dragStartPoint.Y - p.Y) * sensitivity;

        Value = Convert.ToInt32(Math.Clamp(
            _dragStartValue + dragDistance,
            Minimum,
            Maximum));
    }

    protected override void OnPointerWheelChanged(
        PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);
        
        if(Disabled) return;

        Value = Convert.ToInt32(Math.Clamp(
            Convert.ToDouble(Value) + e.Delta.Y,
            Minimum,
            Maximum));

        InvalidateVisual();
    }
}