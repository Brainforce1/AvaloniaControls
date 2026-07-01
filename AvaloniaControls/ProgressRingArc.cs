using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;

namespace BrainForceOne.AvaloniaControls;

public class ProgressRingArc : Control
{


    public static readonly StyledProperty<double> ProgressProperty =
        AvaloniaProperty.Register<ProgressRingArc, double>(nameof(Progress));

    public double Progress
    {
        get => GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    static ProgressRingArc()
    {
        ProgressProperty.Changed.AddClassHandler<ProgressRingArc>((x, e) => x.InvalidateVisual());
        StrokeThicknessProperty.Changed.AddClassHandler<ProgressRingArc>((x, e) => x.InvalidateVisual());
        StrokeProperty.Changed.AddClassHandler<ProgressRingArc>((x, e) => x.InvalidateVisual());
    }

    public static readonly StyledProperty<double> StrokeThicknessProperty =
        AvaloniaProperty.Register<ProgressRingArc, double>(nameof(StrokeThickness), 8);


    public static readonly StyledProperty<IBrush> StrokeProperty =
        AvaloniaProperty.Register<ProgressRingArc, IBrush>(nameof(Stroke), Brushes.Green);

    public double StrokeThickness
    {
        get => GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }

    public IBrush Stroke
    {
        get => GetValue(StrokeProperty);
        set => SetValue(StrokeProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        double thickness = StrokeThickness;

        if (thickness <= 0 || Bounds.Width <= 0 || Bounds.Height <= 0)
            return;

        var rect = Bounds;
        var center = rect.Center;

        double radius = (Math.Min(rect.Width, rect.Height) / 2) - (thickness / 2);

        var pen = new Pen(Stroke, thickness)
        {
            LineCap = PenLineCap.Round
        };

        if (Progress >= 100)
        {
            context.DrawEllipse(
                null,
                pen,
                center,
                radius,
                radius);

            return;
        }

        if (Progress <= 0)
            return;

        double startAngle = -90; // start on top
        double sweepAngle = 360 * (Progress / 100);

        if (sweepAngle < 0.5)
            sweepAngle = 0.5;

        var geometry = new StreamGeometry();

        using (var ctx = geometry.Open())
        {
            var start = PointOnCircle(center, radius, startAngle);
            var end = PointOnCircle(center, radius, startAngle + sweepAngle);

            ctx.BeginFigure(start, false);

            ctx.ArcTo(
                end,
                new Size(radius, radius),
                0,
                sweepAngle > 180,
                SweepDirection.Clockwise);
        }

        context.DrawGeometry(null, pen, geometry);
    }


    private Point PointOnCircle(Point center, double radius, double angleDegrees)
    {
        double angle = angleDegrees * Math.PI / 180.0;

        return new Point(
            center.X + radius * Math.Cos(angle),
            center.Y + radius * Math.Sin(angle));
    }
}
