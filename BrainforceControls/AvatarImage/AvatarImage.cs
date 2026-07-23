using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
namespace BrainForceOne.BrainforceControls;
public class AvatarImage : Control
{
    public static readonly StyledProperty<string?> ImageUrlProperty =
        AvaloniaProperty.Register<AvatarImage, string?>(nameof(ImageUrl));

    public static readonly StyledProperty<IBrush> RingBrushProperty =
        AvaloniaProperty.Register<AvatarImage, IBrush>(
            nameof(RingBrush),
            Brushes.DeepSkyBlue);

    public static readonly StyledProperty<double> RingThicknessProperty =
        AvaloniaProperty.Register<AvatarImage, double>(
            nameof(RingThickness),
            4);

    public static readonly StyledProperty<IBrush> PlaceholderBrushProperty =
        AvaloniaProperty.Register<AvatarImage, IBrush>(
            nameof(PlaceholderBrush),
            Brushes.LightGray);

    public static readonly StyledProperty<bool> GlassEffectProperty =
        AvaloniaProperty.Register<AvatarImage, bool>(
            nameof(GlassEffect),
            false);

    public static readonly StyledProperty<bool> HideLoadingRingProperty =
        AvaloniaProperty.Register<AvatarImage, bool>(
            nameof(HideLoadingRing),
            false);

    /// <summary>
    /// Gets or sets the delay in milliseconds for simulating image loading during testing.
    /// </summary>
    public static readonly StyledProperty<int> DelayForLoadTestProperty =
        AvaloniaProperty.Register<AvatarImage, int>(
            nameof(DelayForLoadTest),
            0);

    public static readonly StyledProperty<IBrush> GlassOverlayBrushProperty =
        AvaloniaProperty.Register<AvatarImage, IBrush>(
            nameof(GlassOverlayBrush),
            new SolidColorBrush(Color.FromArgb(40, 255, 255, 255)));

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ImageUrlProperty)
        {
            _ = OnImageUrlChanged(change.GetNewValue<string?>());
        }
    }

    public int DelayForLoadTest
    {
        get => GetValue(DelayForLoadTestProperty);
        set => SetValue(DelayForLoadTestProperty, value);
    }

    public bool HideLoadingRing
    {
        get => GetValue(HideLoadingRingProperty);
        set => SetValue(HideLoadingRingProperty, value);
    }

    public bool GlassEffect
    {
        get => GetValue(GlassEffectProperty);
        set => SetValue(GlassEffectProperty, value);
    }

    public IBrush GlassOverlayBrush
    {
        get => GetValue(GlassOverlayBrushProperty);
        set => SetValue(GlassOverlayBrushProperty, value);
    }

    public string? ImageUrl
    {
        get => GetValue(ImageUrlProperty);
        set => SetValue(ImageUrlProperty, value);
    }

    public IBrush RingBrush
    {
        get => GetValue(RingBrushProperty);
        set => SetValue(RingBrushProperty, value);
    }

    public double RingThickness
    {
        get => GetValue(RingThicknessProperty);
        set => SetValue(RingThicknessProperty, value);
    }

    public IBrush PlaceholderBrush
    {
        get => GetValue(PlaceholderBrushProperty);
        set => SetValue(PlaceholderBrushProperty, value);
    }

    private static readonly HttpClient HttpClient = new();

    private static readonly ConcurrentDictionary<string, Bitmap> Cache = new();

    private readonly DispatcherTimer _animationTimer;

    private Bitmap? _bitmap;
    private bool _isLoading;
    private double _rotationAngle;
    private CancellationTokenSource? _loadCts;

    static AvatarImage()
    {
        AffectsRender<AvatarImage>(
            ImageUrlProperty,
            RingBrushProperty,
            RingThicknessProperty,
            PlaceholderBrushProperty,
            GlassEffectProperty,
            GlassOverlayBrushProperty);
    }

    public AvatarImage()
    {
        _animationTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16)
        };

        _animationTimer.Tick += (_, _) =>
        {
            if (!_isLoading)
                return;

            _rotationAngle += 5;

            if (_rotationAngle > 360)
                _rotationAngle -= 360;

            InvalidateVisual();
        };
    }

    private async Task OnImageUrlChanged(string? url)
    {
        if(_loadCts != null)
            await _loadCts?.CancelAsync();
        _loadCts = new CancellationTokenSource();

        _bitmap = null;

        if (string.IsNullOrWhiteSpace(url))
        {
            _isLoading = false;
            _animationTimer.Stop();
            InvalidateVisual();
            return;
        }

        if (Cache.TryGetValue(url, out var cached))
        {
            _bitmap = cached;
            _isLoading = false;
            _animationTimer.Stop();
            InvalidateVisual();
            return;
        }

        _isLoading = true;
        _animationTimer.Start();
        InvalidateVisual();

        try
        {
#if DEBUG
            if(DelayForLoadTest > 0)
                await Task.Delay(DelayForLoadTest);
#endif
            Bitmap bitmap;

            if (url.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                || url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                var bytes = await HttpClient.GetByteArrayAsync(
                    url,
                    _loadCts.Token);

                await using var ms = new MemoryStream(bytes);

                bitmap = new Bitmap(ms);
            }
            else
            {
                await using var fs = File.OpenRead(url);

                bitmap = new Bitmap(fs);
            }

            if (_loadCts.IsCancellationRequested)
                return;

            Cache[url] = bitmap;

            _bitmap = bitmap;
        }
        catch
        {
            _bitmap = null;
        }
        finally
        {
            _isLoading = false;
            _animationTimer.Stop();

            Dispatcher.UIThread.Post(InvalidateVisual);
        }
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        var bounds = Bounds;

        if (bounds.Width <= 0 || bounds.Height <= 0)
            return;

        var diameter = Math.Min(bounds.Width, bounds.Height);

        var circleBounds = new Rect(
            (bounds.Width - diameter) / 2,
            (bounds.Height - diameter) / 2,
            diameter,
            diameter);

        var imageBounds = circleBounds.Deflate(RingThickness);

        DrawPlaceholder(context, imageBounds);

        if (_bitmap != null)
        {
            DrawCircularImage(
                context,
                _bitmap,
                imageBounds);

            if (GlassEffect)
            {
                DrawGlassEffect(
                    context,
                    imageBounds);
            }
        }

        if (_isLoading && !HideLoadingRing)
        {
            DrawLoadingRing(
                context,
                circleBounds);
        }
    }

    private void DrawPlaceholder(
        DrawingContext context,
        Rect rect)
    {
        context.DrawEllipse(
            PlaceholderBrush,
            null,
            rect.Center,
            rect.Width / 2,
            rect.Height / 2);
    }

    private void DrawCircularImage(
        DrawingContext context,
        Bitmap bitmap,
        Rect destination)
    {

            var sourceRect = CalculateCropRect(bitmap);
            var radius = destination.Width / 2;

            using (context.PushClip(
                new RoundedRect(destination, radius)))
            {
                context.DrawImage(
                    bitmap,
                    sourceRect,
                    destination);
            }
    }
    private void DrawGlassEffect(
DrawingContext context,
Rect destination)
    {
        var radius = destination.Width / 2;

        using (context.PushClip(
            new RoundedRect(destination, radius)))
        {
            // Stronger glass tint
            context.DrawRectangle(
                new SolidColorBrush(
                    Color.FromArgb(35, 255, 255, 255)),
                null,
                destination);

            // More visible outer highlight ring
            context.DrawEllipse(
                null,
                new Pen(
                    new SolidColorBrush(
                        Color.FromArgb(140, 255, 255, 255)),
                    1.5),
                destination.Center,
                destination.Width / 2 - 0.75,
                destination.Height / 2 - 0.75);

            // Main reflection
            var highlightRect = new Rect(
                destination.X + destination.Width * 0.08,
                destination.Y + destination.Height * 0.06,
                destination.Width * 0.65,
                destination.Height * 0.28);

            var highlightBrush = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
                GradientStops =
                [
                    new GradientStop(
                Color.FromArgb(120, 255, 255, 255), 0),

            new GradientStop(
                Color.FromArgb(40, 255, 255, 255), 0.5),

            new GradientStop(
                Color.FromArgb(0, 255, 255, 255), 1)
                ]
            };

            context.DrawEllipse(
                highlightBrush,
                null,
                highlightRect.Center,
                highlightRect.Width / 2,
                highlightRect.Height / 2);

            // Secondary small sparkle
            var sparkleRect = new Rect(
                destination.X + destination.Width * 0.18,
                destination.Y + destination.Height * 0.12,
                destination.Width * 0.12,
                destination.Height * 0.06);

            context.DrawEllipse(
                new SolidColorBrush(
                    Color.FromArgb(110, 255, 255, 255)),
                null,
                sparkleRect.Center,
                sparkleRect.Width / 2,
                sparkleRect.Height / 2);
        }
    }

    private static Rect CalculateCropRect(Bitmap bitmap)
    {
        double imgW = bitmap.PixelSize.Width;
        double imgH = bitmap.PixelSize.Height;

        if (imgW == imgH)
            return new Rect(0, 0, imgW, imgH);

        if (imgW > imgH)
        {
            var x = (imgW - imgH) / 2;
            return new Rect(x, 0, imgH, imgH);
        }

        var y = (imgH - imgW) / 2;
        return new Rect(0, y, imgW, imgW);
    }

    private void DrawLoadingRing(
        DrawingContext context,
        Rect bounds)
    {
        var radius = bounds.Width / 2 - RingThickness / 2;

        var geometry = CreateArcGeometry(
            bounds.Center,
            radius,
            _rotationAngle,
            110);

        context.DrawGeometry(
            null,
            new Pen(RingBrush, RingThickness),
            geometry);
    }

    private static Geometry CreateArcGeometry(
        Point center,
        double radius,
        double startAngle,
        double sweepAngle)
    {
        var geometry = new StreamGeometry();

        using var ctx = geometry.Open();

        var startRadians = DegreesToRadians(startAngle);
        var endRadians = DegreesToRadians(startAngle + sweepAngle);

        var startPoint = new Point(
            center.X + radius * Math.Cos(startRadians),
            center.Y + radius * Math.Sin(startRadians));

        var endPoint = new Point(
            center.X + radius * Math.Cos(endRadians),
            center.Y + radius * Math.Sin(endRadians));

        ctx.BeginFigure(startPoint, false);

        ctx.ArcTo(
            endPoint,
            new Size(radius, radius),
            0,
            sweepAngle > 180,
            SweepDirection.Clockwise);

        return geometry;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (double.IsInfinity(availableSize.Width)
            && double.IsInfinity(availableSize.Height))
        {
            return new Size(64, 64);
        }

        if (double.IsInfinity(availableSize.Width))
            return new Size(availableSize.Height, availableSize.Height);

        if (double.IsInfinity(availableSize.Height))
            return new Size(availableSize.Width, availableSize.Width);

        var size = Math.Min(
            availableSize.Width,
            availableSize.Height);

        return new Size(size, size);
    }
}
