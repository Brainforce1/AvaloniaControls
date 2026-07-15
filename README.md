# BrainforceControls
**Modern UI controls for Avalonia®**

A small, focused library of high-quality, render-based controls for Avalonia®.


## Table of contents
- Installation  
- Activate library styles  
- ProgressRingControl  
  - Features  
  - Properties  
  - XAML example  
  - C# animation example  
- Speedometer  
  - Features  
  - Properties  
  - XAML example  
  - C# animation example  
- LEDDisplay
   - Features
   - Properties
   - XAML example
- Layout example  
- Custom fonts  
- Designer vs runtime note  
- Troubleshooting  
- License  

---

## Installation

Install from NuGet:

    dotnet add package BrainforceControls

Or via Package Manager:

    Install-Package BrainforceControls

The assembly name is **BrainForceOne.BrainforceControls**.

---

## Activate library styles

To ensure the controls render with their default styles (including bundled fonts and templates), include the library `Generic.axaml` in your application `App.axaml`:

    <Application xmlns="https://github.com/avaloniaui"
                 xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
      <Application.Styles>
        <FluentTheme Mode="Dark"/>
        <!-- Include the control library styles so defaults (fonts, templates) are applied -->
        <StyleInclude Source="avares://BrainForceOne.BrainforceControls/Themes/Generic.axaml"/>
      </Application.Styles>
    </Application>

If you do not include the library styles, the controls will still work but defaults (for example the bundled font or default visual settings) may not be applied.

---

## ProgressRingControl

A modern, animated ring indicator with optional glow, titles and configurable stroke.
---

<img src="https://raw.githubusercontent.com/Brainforce1/AvaloniaControls/refs/heads/master/BrainforceControls/docs/images/ProgressRing.gif" width="300" />




---

### ProgressRingControl — Features

- Smooth, configurable progress animation  
- Supertitle and Subtitle text areas  
- Glow effect (color, blur radius, opacity)  
- Configurable stroke thickness and size  
- Template-based control — fully stylable  
- Custom font support via `FontFamily`  
- Designer preview friendly  

### ProgressRingControl — Properties

| Property        | Type        | Description                    |
|----------------|------------:|--------------------------------|
| `Progress`     | `double`    | Current progress (0–100)       |
| `StrokeThickness` | `double` | Ring thickness                 |
| `Supertitle`   | `string`    | Upper small title              |
| `Subtitle`     | `string`    | Lower subtitle                 |
| `GlowEnabled`  | `bool`      | Enable glow effect             |
| `GlowColor`    | `Color`     | Glow color                     |
| `GlowBlurRadius` | `double`  | Glow blur radius               |
| `GlowOpacity`  | `double`    | Glow opacity (0–1)             |
| `FontFamily`   | `FontFamily`| Custom font for text           |

### ProgressRingControl — XAML example

    <controls:ProgressRingControl
        x:Name="Ring"
        Progress="0"
        Supertitle="STREAMING FROM"
        Subtitle="Server@Home"
        HorizontalAlignment="Center"
        StrokeThickness="20"
        VerticalAlignment="Center"
        Width="240"
        Height="240"/>

### ProgressRingControl — C# animation example

    private async void StartProgress()
    {
        while (true)
        {
            for (int i = 0; i <= 100; i++)
            {
                Ring.Progress = i;
                await Task.Delay(30);
            }

            Ring.Progress = 0;
            await Task.Delay(200);
        }
    }

Recommendation: `Task.Delay` or `DispatcherTimer` are fine for demos. For smooth, jitter-free animations use render-based updates (update in `Render()` using elapsed time and call `InvalidateVisual()`), or Avalonia® composition animations.

---

## Speedometer

A fully drawn gauge control with a needle, configurable sweep, glass effect and unit display.
---

<img src="https://raw.githubusercontent.com/Brainforce1/AvaloniaControls/refs/heads/master/BrainforceControls/docs/images/Speedometer.gif" width="300" />

---

### Speedometer — Features

- Render-based visuals for crisp drawing  
- Smooth needle animation with easing (when animated via render/composition)  
- Configurable sweep angle (for example 180 or 270 degrees)  
- Minimum / Maximum value range  
- Unit text and value text with custom font support  
- Optional glass effect and styling via template  
- Designer preview friendly  

### Speedometer — Properties

| Property      | Type          | Description                     |
|--------------|--------------:|---------------------------------|
| `Value`      | `double`      | Current gauge value             |
| `Minimum`    | `double`      | Minimum value                   |
| `Maximum`    | `double`      | Maximum value                   |
| `SweepAngle` | `double`      | Degrees covered by the gauge    |
| `Position`   | `GaugePosition` | Needle position (Top/Bottom)  |
| `GlassEffect`| `bool`        | Enable glass highlight          |
| `TextSize`   | `int`         | Value text size                 |
| `UnitText`   | `string`      | Unit label (for example km/h)   |
| `UnitTextSize` | `int`       | Unit text size                  |
| `FontFamily` | `FontFamily`  | Custom font for labels          |

### Speedometer — XAML example

    <controls:Speedometer
        x:Name="Speedometer"
        Width="240"
        Height="240"
        Position="Top"
        GlassEffect="True"
        SweepAngle="270"
        Minimum="30.0"
        Maximum="200.0"
        TextSize="30"
        UnitText="cm/h"
        UnitTextSize="12"
        Value="65"/>

### Speedometer — C# animation example

    private async void AnimateSpeed()
    {
        while (true)
        {
            for (int i = 30; i <= 200; i++)
            {
                Speedometer.Value = i;
                await Task.Delay(20);
            }

            for (int i = 200; i >= 30; i--)
            {
                Speedometer.Value = i;
                await Task.Delay(20);
            }
        }
    }

Recommendation: Avoid relying on `Task.Delay` for frame-perfect motion. Prefer updating the control from a render loop (use `InvalidateVisual()` and compute delta time with `Stopwatch`) or use Avalonia composition animations where possible.

---

## LEDDisplay

A render-based seven-segment display control for showing numeric values with configurable digits, color, glow and segment thickness.
---

<img src="https://raw.githubusercontent.com/Brainforce1/AvaloniaControls/refs/heads/master/BrainforceControls/docs/images/LedDisplay.gif" width="300" />

---

### LEDDisplay — Features

- Render-based seven-segment digits for crisp visuals
- Configurable number of digits
- Optional leading-zero display
- Configurable segment color and thickness
- Optional glow effect for active segments
- Designer preview friendly

### LEDDisplay — Properties

| Property           | Type      | Default | Description                              |
|-------------------|-----------|---------|------------------------------------------|
| `Value`           | `int`     | `0`     | Numeric value shown on the display       |
| `Digits`          | `int`     | `4`     | Number of digits                         |
| `LeadingZeros`    | `bool`    | `true`  | Show zeros before the value              |
| `Glow`            | `bool`    | `true`  | Enable glow around active segments       |
| `SegmentOnColor`  | `Color`   | `Red`   | Color of active segments                |
| `SegmentThickness`| `double`  | `1.0`   | Relative segment thickness               |

### LEDDisplay — XAML example

    <controls:LedDisplay
        x:Name="LedDisplay"
        Width="240"
        Height="80"
        Digits="4"
        Value="1234"
        LeadingZeros="True"
        Glow="True"
        SegmentOnColor="Red"
        SegmentThickness="1.0"/>

---

## Layout example

Below is the exact XAML sample used in the test app:

    <Window
        xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:local="clr-namespace:AvaloniaControlTesterApp"
        xmlns:controls="clr-namespace:BrainForceOne.BrainforceControls;assembly=BrainForceOne.BrainforceControls"
        x:Class="AvaloniaControlTesterApp.MainWindow"
        Width="500" Height="500"
        Background="#0B1A2A">

        <Grid ColumnDefinitions="*,*">

            <controls:ProgressRingControl
                x:Name="Ring"
                Progress="0"
                Supertitle="STREAMING FROM"
                Subtitle="Server@Home"
                HorizontalAlignment="Center"
                StrokeThickness="20"
                VerticalAlignment="Center"
                Width="240"
                Height="240"
                Grid.Column="0"/>

            <controls:Speedometer
                x:Name="Speedometer"
                Width="240"
                Height="240"
                Position="Top"
                GlassEffect="True"
                SweepAngle="270"
                Minimum="30.0"
                Maximum="200.0"
                TextSize="30"
                UnitText="cm/h"
                UnitTextSize="12"
                Value="65"
                Grid.Column="1"/>
        </Grid>

    </Window>

---
## Thermometer Control

The `Thermometer` is an Avalonia® control that displays a vertical temperature indicator with a gradient between `StartColor` and `EndColor`. The control supports ticks, glass effect, unit text, and a configurable bulb size.

<img src="https://raw.githubusercontent.com/Brainforce1/AvaloniaControls/refs/heads/master/BrainforceControls/docs/images/Thermometer.gif" width="300" />

### Properties

| Property | Type | Default | Description |
|-----------|------|---------|--------------|
| `Minimum` | `double` | `0` | Minimum value of the scale |
| `Maximum` | `double` | `100` | Maximum value of the scale |
| `Value` | `double` | `50` | Current value |
| `StartColor` | `Color` | `LimeGreen` | Color at minimum value |
| `EndColor` | `Color` | `Red` | Color at maximum value |
| `TickColor` | `Color` | `Black` | Color of the scale ticks |
| `GlassEffect` | `bool` | `true` | Enables glass effect |
| `ShowTicks` | `bool` | `true` | Shows scale ticks |
| `TickInterval` | `double` | `10` | Distance between ticks |
| `TickPlacement` | `TickPlacement` | `Right` | Placement of ticks |
| `BulbSize` | `double` | `50` | Size of the bulb at the bottom |
| `Unit` | `string` | `°C` | Unit text |

### XAML Example

```xml
<thermo:Thermometer
    Width="150"
    Height="400"
    Minimum="0"
    Maximum="120"
    Value="90"
    BulbSize="40"
    StartColor="LimeGreen"
    EndColor="Red"
    ShowTicks="True"
    Unit="°C"
    TickInterval="10"
    TickColor="Red"
    TickPlacement="Left"
    GlassEffect="True"/>
```
---

## Custom fonts

The controls support `FontFamily` (same pattern as Avalonia® controls). You can set a font from your app or use the library default.

Example: use an embedded font from your app:

1. Add font file to your app project (for example `Assets/Fonts/MyFont.ttf`) and include it as an Avalonia® resource in the app project file:

       <ItemGroup>
         <AvaloniaResource Include="Assets/Fonts/MyFont.ttf" />
       </ItemGroup>

2. Register a `FontFamily` resource in your app `App.axaml` or a resource dictionary:

       <FontFamily x:Key="MyAppFont">
         avares://MyApp/Assets/Fonts/MyFont.ttf#My Font
       </FontFamily>

3. Use it on the control:

       <controls:Speedometer FontFamily="{StaticResource MyAppFont}" />

Library default font: The library ships a default font and sets it in `Generic.axaml`. To ensure the default is applied, include the library styles in your `App.axaml` (see “Activate library styles”).

---

## Designer vs runtime note

- Designer preview often loads library styles and simulates a stable render loop.  
- Runtime requires you to include `avares://BrainForceOne.BrainforceControls/Themes/Generic.axaml` in your `Application.Styles` to apply the library defaults (fonts, templates, resources).  
- If fonts or styles appear in preview but not at runtime, check that your app includes the `StyleInclude` and that the assembly name in `avares://` URIs matches exactly.  

---

## Troubleshooting

**Fonts not showing in runtime**

- Ensure `Generic.axaml` is included in your app `App.axaml`.  
- Verify the `avares://` URI uses the correct assembly name and internal font family name (the font's internal name, not the file name).  
- Confirm the font file is included as `AvaloniaResource` in the project that hosts the font.  

**Preview shows font but runtime does not**

- Designer may load library resources automatically. Your app must explicitly include the library styles to get the same defaults at runtime.  

**Jittery or jumpy animations**

- Avoid `DispatcherTimer` or `Task.Delay` for frame-sensitive animations.  
- Use render-based updates (update animation state in `Render()` using elapsed time and call `InvalidateVisual()`), or use Avalonia® composition animations.  
- If you must use timers, compute motion based on elapsed time (delta) rather than fixed increments.  

**Needle or ring renders under text**

- Drawing order matters: draw background elements first, then text, then needle (or draw needle last) so it appears above text.  

---

## License

MIT License — see LICENSE file in the package for full text.
