using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;
namespace BrainForceOne.BrainforceControls;

public class LedDisplay : Control
{
    public static readonly StyledProperty<int> ValueProperty = AvaloniaProperty.Register<LedDisplay, int>(nameof(Value));
    public static readonly StyledProperty<bool> GlowProperty = AvaloniaProperty.Register<LedDisplay, bool>(nameof(Glow), true);
    public static readonly StyledProperty<bool> LeadingZerosProperty = AvaloniaProperty.Register<LedDisplay, bool>(nameof(LeadingZeros), true);
    public static readonly StyledProperty<Color> SegmentOnColorProperty = AvaloniaProperty.Register<LedDisplay, Color>(nameof(SegmentOnColor), Colors.Red);
    public static readonly StyledProperty<int> DigitsProperty = AvaloniaProperty.Register<LedDisplay, int>(nameof(Digits), 4);
    public static readonly StyledProperty<double> SegmentThicknessProperty = AvaloniaProperty.Register<LedDisplay, double>(nameof(SegmentThickness), 1.0);

    public int Digits
    {
        get => GetValue(DigitsProperty);
        set => SetValue(DigitsProperty, value);
    }


    public bool Glow
    {
        get => GetValue(GlowProperty);
        set
        {
            SetValue(GlowProperty, value);
            InvalidateVisual();
        }
    }
    public Color SegmentOnColor
    {
        get => GetValue(SegmentOnColorProperty);
        set => SetValue(SegmentOnColorProperty, value);
    }

    public double SegmentThickness
    {
        get => GetValue(SegmentThicknessProperty);
        set => SetValue(SegmentThicknessProperty, value);
    }

    public bool LeadingZeros
    {
        get => GetValue(LeadingZerosProperty);
        set
        {
            SetValue(LeadingZerosProperty, value);
            InvalidateVisual();
        }
    }

    public int Value
    {
        get => GetValue(ValueProperty);
        set
        {
            SetValue(ValueProperty, value);
            InvalidateVisual();
        }
    }
    public override void Render(DrawingContext context)
    {
        base.Render(context);

        string text = Value.ToString("D" + Digits);

        double spacing = Bounds.Width * 0.03;

        double digitWidth =
            (Bounds.Width - ((text.Length - 1) * spacing))
            / text.Length;

        bool nonNullEncountered = false;
        for (int i = 0; i < text.Length; i++)
        {
            bool enabled = false;
            if (text[i] == '0' && !LeadingZeros && !nonNullEncountered)
            {
                enabled = false;
            }
            else
            {
                nonNullEncountered = true;
                enabled = true;
            }
            DrawDigit(
                context,
                text[i],
                i * (digitWidth + spacing),
                0,
                digitWidth,
                Bounds.Height, enabled);
        }
    }

    private void DrawDigit(
        DrawingContext context,
        char digit,
        double x,
        double y,
        double width,
        double height,
        bool enabled)
    {
        if (!DigitMapper.DigitMap.TryGetValue(digit, out var segments))
            return;

        double t = Math.Min(width, height) * 0.15 * SegmentThickness;

        double hSegmentWidth = width - (2 * t);

        var top = new Rect(
            x + t,
            y,
            hSegmentWidth,
            t);

        var middle = new Rect(
            x + t,
            y + (height / 2) - (t / 2),
            hSegmentWidth,
            t);

        var bottom = new Rect(
            x + t,
            y + height - t,
            hSegmentWidth,
            t);

        var upperLeft = new Rect(
            x,
            y + t,
            t,
            (height / 2) - t);

        var upperRight = new Rect(
            x + width - t,
            y + t,
            t,
            (height / 2) - t);

        var lowerLeft = new Rect(
            x,
            y + (height / 2),
            t,
            (height / 2) - t);

        var lowerRight = new Rect(
            x + width - t,
            y + (height / 2),
            t,
            (height / 2) - t);

        DrawSegment(context, segments.HasFlag(Segments.A) && enabled, top, true);
        DrawSegment(context, segments.HasFlag(Segments.B) && enabled, upperRight, false);
        DrawSegment(context, segments.HasFlag(Segments.C) && enabled, lowerRight, false);
        DrawSegment(context, segments.HasFlag(Segments.D) && enabled, bottom, true);
        DrawSegment(context, segments.HasFlag(Segments.E) && enabled, lowerLeft, false);
        DrawSegment(context, segments.HasFlag(Segments.F) && enabled, upperLeft, false);
        DrawSegment(context, segments.HasFlag(Segments.G) && enabled, middle, true);
    }


    private void DrawSegment(
    DrawingContext context,
    bool active,
    Rect rect,
    bool horizontal)
    {
        var geometry = horizontal
            ? CreateHorizontalSegment(rect)
            : CreateVerticalSegment(rect);


        Color onColor = SegmentOnColor;

        Color offColor = Darken(onColor, 0.85);

        Color brightColor = Lighten(onColor, 0.65);
        Color mediumColor = onColor;
        Color darkColor = Darken(onColor, 0.35);

        if (active)
        {
            for (int i = 4; i >= 1; i--)
            {
                var glowBrush = new SolidColorBrush(
                    WithAlpha(onColor, (byte)(15 * i)));

                var glowRect = Glow
                    ? rect.Inflate(i * 2)
                    : rect;

                var glowGeometry = horizontal
                    ? CreateHorizontalSegment(glowRect)
                    : CreateVerticalSegment(glowRect);

                context.DrawGeometry(
                    glowBrush,
                    null,
                    glowGeometry);
            }
        }

        var bodyBrush = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),

            GradientStops =
        {
            new GradientStop(
                active ? brightColor : Lighten(offColor, 0.10),
                0),

            new GradientStop(
                active ? mediumColor : offColor,
                0.25),

            new GradientStop(
                active ? darkColor : Darken(offColor, 0.20),
                1)
        }
        };

        var borderColor = active
            ? Lighten(onColor, 0.40)
            : Darken(offColor, 0.30);

        context.DrawGeometry(
            bodyBrush,
            new Pen(
                new SolidColorBrush(borderColor),
                1),
            geometry);

        // glas reflectie
        var reflectionBrush = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),

            GradientStops =
        {
            new GradientStop(
                Color.FromArgb(100,255,255,255),
                0),

            new GradientStop(
                Color.FromArgb(30,255,255,255),
                0.35),

            new GradientStop(
                Color.FromArgb(0,255,255,255),
                1)
        }
        };

        context.DrawGeometry(
            reflectionBrush,
            null,
            geometry);
    }

    private StreamGeometry CreateHorizontalSegment(Rect rect)
    {
        double bevel = rect.Height * 0.45;

        var geo = new StreamGeometry();

        using (var ctx = geo.Open())
        {
            ctx.BeginFigure(
                new Point(rect.X + bevel, rect.Y),
                true);

            ctx.LineTo(
                new Point(rect.Right - bevel, rect.Y));

            ctx.LineTo(
                new Point(rect.Right, rect.Center.Y));

            ctx.LineTo(
                new Point(rect.Right - bevel, rect.Bottom));

            ctx.LineTo(
                new Point(rect.X + bevel, rect.Bottom));

            ctx.LineTo(
                new Point(rect.X, rect.Center.Y));

            ctx.EndFigure(true);
        }

        return geo;
    }

    private StreamGeometry CreateVerticalSegment(Rect rect)
    {
        double bevel = rect.Width * 0.45;

        var geo = new StreamGeometry();

        using (var ctx = geo.Open())
        {
            ctx.BeginFigure(
                new Point(rect.Center.X, rect.Y),
                true);

            ctx.LineTo(
                new Point(rect.Right, rect.Y + bevel));

            ctx.LineTo(
                new Point(rect.Right, rect.Bottom - bevel));

            ctx.LineTo(
                new Point(rect.Center.X, rect.Bottom));

            ctx.LineTo(
                new Point(rect.X, rect.Bottom - bevel));

            ctx.LineTo(
                new Point(rect.X, rect.Y + bevel));

            ctx.EndFigure(true);
        }

        return geo;
    }

    private static Color Lighten(Color color, double factor)
    {
        return Color.FromArgb(
            color.A,
            (byte)Math.Min(255, color.R + ((255 - color.R) * factor)),
            (byte)Math.Min(255, color.G + ((255 - color.G) * factor)),
            (byte)Math.Min(255, color.B + ((255 - color.B) * factor)));
    }

    private static Color Darken(Color color, double factor)
    {
        return Color.FromArgb(
            color.A,
            (byte)(color.R * (1.0 - factor)),
            (byte)(color.G * (1.0 - factor)),
            (byte)(color.B * (1.0 - factor)));
    }

    private static Color WithAlpha(Color color, byte alpha)
    {
        return Color.FromArgb(
            alpha,
            color.R,
            color.G,
            color.B);
    }
}

[Flags]
public enum Segments
{
    None = 0,
    A = 1,
    B = 2,
    C = 4,
    D = 8,
    E = 16,
    F = 32,
    G = 64
}

