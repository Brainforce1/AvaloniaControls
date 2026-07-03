using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BrainForceOne.BrainforceControls;

public class ProgressRingControl : TemplatedControl
{
    private static readonly Color DefaultGlowColor = Color.FromRgb(0x22, 0xC5, 0x5E);
    private static readonly Color DefaultBackgroundRingColor = Color.FromRgb(0x2F, 0x5F, 0x5F);
    private static readonly (Color Color, double Offset)[] ReferenceGradientStops =
    [
        (Color.FromRgb(0x34, 0xD3, 0x99), 0.0),
        (Color.FromRgb(0x22, 0xC5, 0x5E), 0.3),
        (Color.FromRgb(0x4A, 0xDE, 0x80), 0.6),
        (Color.FromRgb(0x86, 0xEF, 0xAC), 1.0)
    ];

    private ProgressRingArc? _ring;
    private DropShadowEffect? _glow;
    private CancellationTokenSource? _glowAnimationCts;

    public static readonly StyledProperty<bool> PreviewModeProperty = AvaloniaProperty.Register<ProgressRingControl, bool>(nameof(PreviewMode), false);

    public bool PreviewMode
    {
        get => GetValue(PreviewModeProperty);
        set => SetValue(PreviewModeProperty, value);
    }

    static ProgressRingControl()
    {
        GlowColorProperty.Changed.AddClassHandler<ProgressRingControl>((x, e) => x.UpdateCalculatedBrush());
        GlowColorProperty.Changed.AddClassHandler<ProgressRingControl>((x, e) => x.UpdateGlowState());
        GlowEnabledProperty.Changed.AddClassHandler<ProgressRingControl>((x, e) => x.UpdateGlowState());
        GlowBlurRadiusProperty.Changed.AddClassHandler<ProgressRingControl>((x, e) => x.UpdateGlowState());
        GlowOpacityProperty.Changed.AddClassHandler<ProgressRingControl>((x, e) => x.UpdateGlowState());
    }

    public ProgressRingControl()
    {
        SetCurrentValue(RingBrushProperty, CreateGlowBrush(DefaultGlowColor));
        SetCurrentValue(BackgroundRingBrushProperty, new SolidColorBrush(DefaultBackgroundRingColor));

    }


    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _ring = e.NameScope.Find<ProgressRingArc>("PART_Ring");

        UpdateCalculatedBrush();
        UpdateGlowState();
    }

    public static readonly StyledProperty<bool> GlowEnabledProperty =
        AvaloniaProperty.Register<ProgressRingControl, bool>(nameof(GlowEnabled), true);

    public static readonly StyledProperty<Color> GlowColorProperty =
        AvaloniaProperty.Register<ProgressRingControl, Color>(nameof(GlowColor), DefaultGlowColor);

    public static readonly StyledProperty<double> GlowBlurRadiusProperty =
        AvaloniaProperty.Register<ProgressRingControl, double>(nameof(GlowBlurRadius), 20);

    public static readonly StyledProperty<double> GlowOpacityProperty =
        AvaloniaProperty.Register<ProgressRingControl, double>(nameof(GlowOpacity), 0.9);

    public static readonly StyledProperty<double> ProgressProperty =
        AvaloniaProperty.Register<ProgressRingControl, double>(nameof(Progress), 0);

    public static readonly StyledProperty<string> ProgressTextProperty =
        AvaloniaProperty.Register<ProgressRingControl, string>(nameof(ProgressText), "0%");

    public static readonly StyledProperty<string> SubtitleProperty =
        AvaloniaProperty.Register<ProgressRingControl, string>(nameof(Subtitle), "SubTitle");
    public static readonly StyledProperty<string> SupertitleProperty =
        AvaloniaProperty.Register<ProgressRingControl, string>(nameof(Supertitle), "SuperTitle");

    public static readonly StyledProperty<IBrush> RingBrushProperty =
        AvaloniaProperty.Register<ProgressRingControl, IBrush>(nameof(RingBrush), Brushes.MediumAquamarine);

    public static readonly StyledProperty<IBrush> BackgroundRingBrushProperty =
        AvaloniaProperty.Register<ProgressRingControl, IBrush>(nameof(BackgroundRingBrush), Brushes.Gray);

    public static readonly StyledProperty<double> StrokeThicknessProperty =
        AvaloniaProperty.Register<ProgressRingControl, double>(nameof(StrokeThickness), 12);


    public bool GlowEnabled
    {
        get => GetValue(GlowEnabledProperty);
        set => SetValue(GlowEnabledProperty, value);
    }

    public Color GlowColor
    {
        get => GetValue(GlowColorProperty);
        set => SetValue(GlowColorProperty, value);
    }

    public double GlowBlurRadius
    {
        get => GetValue(GlowBlurRadiusProperty);
        set => SetValue(GlowBlurRadiusProperty, value);
    }

    public double GlowOpacity
    {
        get => GetValue(GlowOpacityProperty);
        set => SetValue(GlowOpacityProperty, value);
    }

    private void UpdateCalculatedBrush()
    {
        SetCurrentValue(RingBrushProperty, CreateGlowBrush(GlowColor));

        if (_ring is not null)
            _ring.Stroke = RingBrush;
    }

    private void UpdateGlowState()
    {
        if (_ring is null)
            return;

        if (!GlowEnabled)
        {
            StopGlowAnimation();
            _ring.Effect = null;
            return;
        }

        _glow ??= new DropShadowEffect();
        _glow.Color = GlowColor;
        _ring.Effect = _glow;
        StartGlowAnimation();
    }

    private static IBrush CreateGlowBrush(Color baseColor)
    {
        var (baseHue, baseSaturation, baseLightness) = ToHsl(baseColor);
        var (referenceBaseHue, referenceBaseSaturation, referenceBaseLightness) = ToHsl(DefaultGlowColor);
        var gradientStops = new GradientStops();

        foreach (var (referenceColor, offset) in ReferenceGradientStops)
        {
            var (referenceHue, referenceSaturation, referenceLightness) = ToHsl(referenceColor);

            var adjustedHue = NormalizeHue(baseHue + NormalizeHueDelta(referenceHue - referenceBaseHue));
            var adjustedSaturation = Clamp(baseSaturation + (referenceSaturation - referenceBaseSaturation), 0, 1);
            var adjustedLightness = Clamp(baseLightness + (referenceLightness - referenceBaseLightness), 0, 1);

            gradientStops.Add(new GradientStop(FromHsl(adjustedHue, adjustedSaturation, adjustedLightness, baseColor.A), offset));
        }

        return new ConicGradientBrush
        {
            Center = new RelativePoint(0.5, 0.5, RelativeUnit.Relative),
            GradientStops = gradientStops
        };
    }

    private static (double H, double S, double L) ToHsl(Color color)
    {
        double r = color.R / 255d;
        double g = color.G / 255d;
        double b = color.B / 255d;

        double max = Math.Max(r, Math.Max(g, b));
        double min = Math.Min(r, Math.Min(g, b));
        double h = 0;
        double s = 0;
        double l = (max + min) / 2;

        double delta = max - min;

        if (delta > 0)
        {
            s = l > 0.5 ? delta / (2 - max - min) : delta / (max + min);

            if (max == r)
                h = (g - b) / delta + (g < b ? 6 : 0);
            else if (max == g)
                h = (b - r) / delta + 2;
            else
                h = (r - g) / delta + 4;

            h *= 60;
        }

        return (h, s, l);
    }

    private static Color FromHsl(double hue, double saturation, double lightness, byte alpha)
    {
        double c = (1 - Math.Abs(2 * lightness - 1)) * saturation;
        double x = c * (1 - Math.Abs((hue / 60d) % 2 - 1));
        double m = lightness - c / 2;

        double r1 = 0;
        double g1 = 0;
        double b1 = 0;

        if (hue < 60)
        {
            r1 = c;
            g1 = x;
        }
        else if (hue < 120)
        {
            r1 = x;
            g1 = c;
        }
        else if (hue < 180)
        {
            g1 = c;
            b1 = x;
        }
        else if (hue < 240)
        {
            g1 = x;
            b1 = c;
        }
        else if (hue < 300)
        {
            r1 = x;
            b1 = c;
        }
        else
        {
            r1 = c;
            b1 = x;
        }

        byte r = (byte)Math.Round((r1 + m) * 255);
        byte g = (byte)Math.Round((g1 + m) * 255);
        byte b = (byte)Math.Round((b1 + m) * 255);

        return Color.FromArgb(alpha, r, g, b);
    }

    private static double NormalizeHue(double hue)
    {
        hue %= 360;
        return hue < 0 ? hue + 360 : hue;
    }

    private static double NormalizeHueDelta(double delta)
    {
        delta %= 360;

        if (delta > 180)
            delta -= 360;
        else if (delta < -180)
            delta += 360;

        return delta;
    }

    private static double Clamp(double value, double min, double max) => Math.Max(min, Math.Min(max, value));

    private void StopGlowAnimation()
    {
        _glowAnimationCts?.Cancel();
        _glowAnimationCts?.Dispose();
        _glowAnimationCts = null;
    }

    private void StartGlowAnimation()
    {
        if (_glow == null || !GlowEnabled)
            return;

        StopGlowAnimation();

        _glowAnimationCts = new CancellationTokenSource();
        _ = AnimateGlowAsync(_glow, _glowAnimationCts.Token);
    }

    private async Task AnimateGlowAsync(DropShadowEffect glow, CancellationToken cancellationToken)
    {
        double maxBlur = Math.Max(0, GlowBlurRadius);
        double minBlur = maxBlur * 0.4;

        double maxOpacity = Clamp(GlowOpacity, 0, 1);
        double minOpacity = maxOpacity * 0.45;

        double t = 0;
        bool forward = true;

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(16, cancellationToken).ConfigureAwait(true); // ~60 FPS
            }
            catch (TaskCanceledException)
            {
                break;
            }

            if (cancellationToken.IsCancellationRequested)
                break;

            t += forward ? 0.02 : -0.02;

            if (t >= 1)
                forward = false;
            else if (t <= 0)
                forward = true;

            // Smooth sine easing
            double eased = (1 - Math.Cos(t * Math.PI)) / 2;

            glow.BlurRadius = minBlur + (maxBlur - minBlur) * eased;
            glow.Opacity = minOpacity + (maxOpacity - minOpacity) * eased;
        }
    }

    public double Progress
    {
        get => GetValue(ProgressProperty);
        set
        {
            SetValue(ProgressProperty, value);
            SetValue(ProgressTextProperty, $"{value}%");
        }
    }

    public string ProgressText
    {
        get => GetValue(ProgressTextProperty);
        set => SetValue(ProgressTextProperty, value);
    }

    public string Supertitle
    {
        get => GetValue(SupertitleProperty);
        set => SetValue(SupertitleProperty, value);
    }

    public string Subtitle
    {
        get => GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public IBrush RingBrush
    {
        get => GetValue(RingBrushProperty);
        set => SetValue(RingBrushProperty, value);
    }

    public IBrush BackgroundRingBrush
    {
        get => GetValue(BackgroundRingBrushProperty);
        set => SetValue(BackgroundRingBrushProperty, value);
    }

    public double StrokeThickness
    {
        get => GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }
}