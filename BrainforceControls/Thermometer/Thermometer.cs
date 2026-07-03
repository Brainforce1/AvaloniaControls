using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using System;

namespace BrainForceOne.BrainforceControls;


public class Thermometer : TemplatedControl
{
    #region Properties

    public static readonly StyledProperty<double> MinimumProperty =
        AvaloniaProperty.Register<Thermometer, double>(
            nameof(Minimum), 0);

    public static readonly StyledProperty<double> MaximumProperty =
        AvaloniaProperty.Register<Thermometer, double>(
            nameof(Maximum), 100);

    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<Thermometer, double>(
            nameof(Value), 50);

    public static readonly StyledProperty<Color> ValueTextColorProperty =
    AvaloniaProperty.Register<Thermometer, Color>(
        nameof(ValueTextColor), Colors.Black);

    public static readonly StyledProperty<Color> StartColorProperty =
        AvaloniaProperty.Register<Thermometer, Color>(
            nameof(StartColor),
            Colors.LimeGreen);

    public static readonly StyledProperty<Color> EndColorProperty =
        AvaloniaProperty.Register<Thermometer, Color>(
            nameof(EndColor),
            Colors.Red);

    public static readonly StyledProperty<Color> TickColorProperty =
        AvaloniaProperty.Register<Thermometer, Color>(
            nameof(TickColor),
            Colors.Black);

    public static readonly StyledProperty<bool> GlassEffectProperty =
        AvaloniaProperty.Register<Thermometer, bool>(
            nameof(GlassEffect),
            true);

    public static readonly StyledProperty<bool> ShowTicksProperty =
        AvaloniaProperty.Register<Thermometer, bool>(
            nameof(ShowTicks),
            true);

    public static readonly StyledProperty<double> TickIntervalProperty =
        AvaloniaProperty.Register<Thermometer, double>(
            nameof(TickInterval),
            10);

    public static readonly StyledProperty<TickPlacement> TickPlacementProperty =
        AvaloniaProperty.Register<Thermometer, TickPlacement>(
            nameof(TickPlacement),
            TickPlacement.Right);

    public static readonly StyledProperty<double> BulbSizeProperty =
        AvaloniaProperty.Register<Thermometer, double>(
            nameof(BulbSize),
            50);
    public static readonly StyledProperty<string> UnitProperty =
        AvaloniaProperty.Register<Thermometer, string>(
            nameof(Unit),
            "°C");

    #endregion

    static Thermometer()
    {
        AffectsRender<Thermometer>(
            MinimumProperty,
            MaximumProperty,
            ValueProperty,
            StartColorProperty,
            EndColorProperty,
            GlassEffectProperty,
            ShowTicksProperty,
            TickIntervalProperty,
            TickPlacementProperty,
            BulbSizeProperty);
    }

    #region CLR

    public Color ValueTextColor
    {
        get => GetValue(ValueTextColorProperty);
        set => SetValue(ValueTextColorProperty, value);
    }
    public string Unit
    {
        get => GetValue(UnitProperty);
        set => SetValue(UnitProperty, value);
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

    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public Color TickColor
    {
        get => GetValue(TickColorProperty);
        set => SetValue(TickColorProperty, value);
    }

    public Color StartColor
    {
        get => GetValue(StartColorProperty);
        set => SetValue(StartColorProperty, value);
    }

    public Color EndColor
    {
        get => GetValue(EndColorProperty);
        set => SetValue(EndColorProperty, value);
    }

    public bool GlassEffect
    {
        get => GetValue(GlassEffectProperty);
        set => SetValue(GlassEffectProperty, value);
    }

    public bool ShowTicks
    {
        get => GetValue(ShowTicksProperty);
        set => SetValue(ShowTicksProperty, value);
    }

    public double TickInterval
    {
        get => GetValue(TickIntervalProperty);
        set => SetValue(TickIntervalProperty, value);
    }

    public TickPlacement TickPlacement
    {
        get => GetValue(TickPlacementProperty);
        set => SetValue(TickPlacementProperty, value);
    }

    public double BulbSize
    {
        get => GetValue(BulbSizeProperty);
        set => SetValue(BulbSizeProperty, value);
    }

    #endregion

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (Bounds.Width <= 0 || Bounds.Height <= 0)
            return;

        double bulbDiameter = BulbSize;
        double bulbRadius = bulbDiameter / 2;

        double wallThickness = 5;

        double tubeWidth =
            Math.Max(
                16,
                bulbDiameter * 0.35);

        double centerX = Bounds.Width / 2;

        double tubeTop = 10;

        double tubeHeight =
            Bounds.Height -
            bulbDiameter -
            25;

        Rect outerTube = new(
            centerX - tubeWidth / 2,
            tubeTop,
            tubeWidth,
            tubeHeight);

        Rect outerBulb = new(
            centerX - bulbRadius,
            outerTube.Bottom - bulbRadius,
            bulbDiameter,
            bulbDiameter);

        Rect innerTube = new(
            outerTube.X + wallThickness,
            outerTube.Y + wallThickness,
            outerTube.Width - wallThickness * 2,
            outerTube.Height - wallThickness * 2);

        Rect innerBulb = new(
            outerBulb.X + wallThickness,
            outerBulb.Y + wallThickness,
            outerBulb.Width - wallThickness * 2,
            outerBulb.Height - wallThickness * 2);

        if (ShowTicks)
        {
            DrawTicks(
                context,
                outerTube);
        }

        DrawCurrentValue(
                context,
                outerBulb);

        DrawBody(
            context,
            outerTube,
            outerBulb);

        DrawInnerGlass(
            context,
            innerTube,
            innerBulb);

        DrawLiquid(
            context,
            innerTube,
            innerBulb);

        if (GlassEffect)
        {
            DrawReflection(
                context,
                innerTube,
                innerBulb);
        }


    }

    private void DrawCenteredText(
    DrawingContext context,
    string text,
    Point location,
    double fontSize = 12,
    IBrush? brush = null)
    {
        var typeface = new Typeface(FontFamily);
        var formattedText = new FormattedText(
            text,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            typeface,
            fontSize,
            brush ?? Brushes.Black);

        context.DrawText(
            formattedText,
            new Point(
                location.X - (formattedText.Width / 2),
                location.Y));
    }

    private void DrawBody(
        DrawingContext context,
        Rect tube,
        Rect bulb)
    {
        var bodyBrush =
            new LinearGradientBrush
            {
                StartPoint =
                    new RelativePoint(0, 0, RelativeUnit.Relative),

                EndPoint =
                    new RelativePoint(1, 1, RelativeUnit.Relative),

                GradientStops =
                {
                    new GradientStop(Color.Parse("#FFFFFF"),0),
                    new GradientStop(Color.Parse("#D8D8D8"),0.5),
                    new GradientStop(Color.Parse("#9D9D9D"),1)
                }
            };

        context.DrawRectangle(
            bodyBrush,
            new Pen(Brushes.Gray, 2),
            new RoundedRect(
                tube,
                tube.Width / 2));

        context.DrawEllipse(
            bodyBrush,
            new Pen(Brushes.Gray, 2),
            bulb.Center,
            bulb.Width / 2,
            bulb.Height / 2);
    }

    private void DrawInnerGlass(
        DrawingContext context,
        Rect tube,
        Rect bulb)
    {
        var glassBrush =
            new SolidColorBrush(
                Color.FromArgb(
                    80,
                    255,
                    255,
                    255));

        context.DrawRectangle(
            glassBrush,
            new Pen(
                Brushes.LightGray,
                1),
            new RoundedRect(
                tube,
                tube.Width / 2));

        context.DrawEllipse(
            glassBrush,
            new Pen(
                Brushes.LightGray,
                1),
            bulb.Center,
            bulb.Width / 2,
            bulb.Height / 2);
    }

    private void DrawLiquid(
        DrawingContext context,
        Rect tube,
        Rect bulb)
    {
        double range =
            Maximum - Minimum;

        if (range <= 0)
            return;

        double percentage =
            Math.Clamp(
                (Value - Minimum) / range,
                0,
                1);

        double fillHeight =
            tube.Height * percentage;

        var fillBrush =
            new LinearGradientBrush
            {
                StartPoint =
                    new RelativePoint(
                        0.5,
                        1,
                        RelativeUnit.Relative),

                EndPoint =
                    new RelativePoint(
                        0.5,
                        0,
                        RelativeUnit.Relative),

                GradientStops =
                {
                    new GradientStop(
                        StartColor,0),

                    new GradientStop(
                        EndColor,1)
                }
            };

        Rect fill = new(
            tube.Left,
            tube.Bottom - fillHeight,
            tube.Width,
            fillHeight);

        context.DrawRectangle(
            fillBrush,
            null,
            fill);

        context.DrawEllipse(
            fillBrush,
            null,
            bulb.Center,
            bulb.Width / 2,
            bulb.Height / 2);
    }

    private void DrawReflection(
        DrawingContext context,
        Rect tube,
        Rect bulb)
    {
        var reflectionBrush =
            new LinearGradientBrush
            {
                StartPoint =
                    new RelativePoint(
                        0,
                        0,
                        RelativeUnit.Relative),

                EndPoint =
                    new RelativePoint(
                        1,
                        0,
                        RelativeUnit.Relative),

                GradientStops =
                {
                    new GradientStop(
                        Color.FromArgb(
                            160,
                            255,
                            255,
                            255),
                        0),

                    new GradientStop(
                        Color.FromArgb(
                            0,
                            255,
                            255,
                            255),
                        1)
                }
            };

        Rect reflection = new(
            tube.Left + 2,
            tube.Top + 2,
            tube.Width * .25,
            tube.Height);

        context.DrawRectangle(
            reflectionBrush,
            null,
            reflection);

        context.DrawEllipse(
            new SolidColorBrush(
                Color.FromArgb(
                    120,
                    255,
                    255,
                    255)),
            null,
            new Point(
                bulb.Center.X - 5,
                bulb.Center.Y - 5),
            bulb.Width * .12,
            bulb.Height * .12);
    }

    private void DrawCurrentValue(
    DrawingContext context,
    Rect bulb)
    {
        string text =
            $"{Value:0}{Unit}";

        var typeface = new Typeface(FontFamily);

        var formattedText =
            new FormattedText(
                text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                14,
                new SolidColorBrush(ValueTextColor));

        double x =
            (Bounds.Width -
             formattedText.Width) / 2;

        double y =
            bulb.Bottom + 10;

        context.DrawText(
            formattedText,
            new Point(x, y));
    }

    private void DrawTicks(
    DrawingContext context,
    Rect tube)
    {
        if (TickInterval <= 0)
            return;

        double range =
            Maximum - Minimum;

        if (range <= 0)
            return;
        SolidColorBrush tickBrush =
            new SolidColorBrush(TickColor);

        var pen =
            new Pen(
                tickBrush,
                1);

        for (double tickValue = Minimum;
             tickValue <= Maximum;
             tickValue += TickInterval)
        {
            double p =
                (tickValue - Minimum) / range;

            double y =
                tube.Bottom -
                p * tube.Height;

            double x1;
            double x2;
            double x3;
            double x4;
            double textX;

            if (TickPlacement ==
                TickPlacement.Left)
            {
                x1 = tube.Left - 25;
                x2 = tube.Left - 8;
                x3 = tube.Right + 8;
                x4 = tube.Right + 25;
                textX =
                    x1 - 14;
            }
            else
            {
                x1 = tube.Right + 8;
                x2 = tube.Right + 25;
                x3 = tube.Left - 25;
                x4 = tube.Left - 8;
                textX =
                    x2 + 14;
            }

            context.DrawLine(
                pen,
                new Point(x1, y),
                new Point(x2, y));
            context.DrawLine(
                pen,
                new Point(x3, y),
                new Point(x4, y));

            DrawCenteredText(
                context,
                tickValue.ToString("0"),
                new Point(
                    textX,
                    y - 10),
                11,
                tickBrush);
        }
    }
}