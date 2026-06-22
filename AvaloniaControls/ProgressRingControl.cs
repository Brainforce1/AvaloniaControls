using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using System;
using System.Threading.Tasks;

namespace BrainForceOne.AvaloniaControls;

public class ProgressRingControl : TemplatedControl
{

    private ProgressRingArc? _ring;
    private DropShadowEffect? _glow;


    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        _ring = e.NameScope.Find<ProgressRingArc>("PART_Ring");

        if (_ring?.Effect is DropShadowEffect effect)
        {
            _glow = effect;
            StartGlowAnimation();
        }
    }

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


    private async void StartGlowAnimation()
    {
        if (_glow == null)
            return;

        double minBlur = 8;
        double maxBlur = 20;

        double minOpacity = 0.4;
        double maxOpacity = 0.9;

        double t = 0;
        bool forward = true;

        while (true)
        {
            await Task.Delay(16); // ~60 FPS

            t += forward ? 0.02 : -0.02;

            if (t >= 1)
                forward = false;
            else if (t <= 0)
                forward = true;

            // Smooth sine easing
            double eased = (1 - Math.Cos(t * Math.PI)) / 2;

            _glow.BlurRadius = minBlur + (maxBlur - minBlur) * eased;
            _glow.Opacity = minOpacity + (maxOpacity - minOpacity) * eased;
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