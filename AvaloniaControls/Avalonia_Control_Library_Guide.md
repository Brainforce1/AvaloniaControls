
# Avalonia 12 – Custom Control omzetten naar NuGet Control Library

Deze gids beschrijft hoe je een bestaande **Avalonia app met een custom control** omzet naar een **herbruikbare NuGet control library** in **Avalonia 12**.

---

## ✅ Doel

- Een zuivere **class library** (geen app)
- Bevat enkel custom controls + styles
- Herbruikbaar in meerdere Avalonia apps
- Klaar om te publiceren naar NuGet

---

## 🧠 Belangrijk inzicht (Avalonia 12)

- ❌ Er bestaat **geen aparte “Avalonia Controls Library” template** meer
- ✅ Een control library is gewoon een **.NET class library**
- ✅ Styles en templates worden geladen via `StyleInclude`

---

## ✅ Stap 1 – App-specifieke bestanden verwijderen

In je **library project** verwijder je:

- `App.axaml`
- `App.axaml.cs`
- `MainWindow.axaml`
- `MainWindow.axaml.cs`
- `Program.cs`

👉 Een control library heeft **geen Application lifecycle**

---

## ✅ Stap 2 – Wat blijft er over

Structuur van je control library:

```
FancyProgressRing/
 ├── Controls/
 │    ├── ProgressRingControl.cs
 │    └── ProgressRingArc.cs
 ├── Themes/
 │    └── Generic.axaml
 └── FancyProgressRing.csproj
```

---

## ✅ Stap 3 – Generic.axaml correct instellen

```xml
<Styles
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="clr-namespace:FancyProgressRing.Controls">

  <Style Selector="local|ProgressRingControl">
    <!-- Control template -->
  </Style>

</Styles>
```

✅ `local|ProgressRingControl` is verplicht
✅ Namespace moet overeenkomen met C# namespace

---

## ✅ Stap 4 – csproj omzetten naar NuGet library

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>

    <!-- NuGet metadata -->
    <PackageId>FancyProgressRing</PackageId>
    <Version>1.0.0</Version>
    <Authors>JouwNaam</Authors>
    <Description>Animated progress ring control for Avalonia</Description>
    <PackageLicenseExpression>MIT</PackageLicenseExpression>
  </PropertyGroup>

  <ItemGroup>
    <!-- ✅ Essentieel -->
    <AvaloniaResource Include="Themes\Generic.axaml" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Avalonia" Version="12.*" />
  </ItemGroup>

</Project>
```

---

## ✅ Stap 5 – Gebruik in een Avalonia app

### App.axaml

```xml
<Application.Styles>
  <FluentTheme />

  <!-- ✅ Verplicht in Avalonia 12 -->
  <StyleInclude Source="avares://FancyProgressRing/Themes/Generic.axaml"/>
</Application.Styles>
```

### XAML gebruik

```xml
xmlns:ring="clr-namespace:FancyProgressRing.Controls;assembly=FancyProgressRing"

<ring:ProgressRingControl
    Progress="75"
    Subtitle="Loading..." />
```

---

## ✅ Stap 6 – NuGet package bouwen

```bash
dotnet pack -c Release
```

Output:

```
bin/Release/FancyProgressRing.1.0.0.nupkg
```

---

## ✅ Stap 7 – Publiceren naar NuGet.org

```bash
dotnet nuget push FancyProgressRing.1.0.0.nupkg   --api-key YOUR_API_KEY   --source https://api.nuget.org/v3/index.json
```

---

## ✅ Checklist

- ✅ Geen App.axaml in library
- ✅ Generic.axaml = AvaloniaResource
- ✅ StyleInclude in consumer app
- ✅ Correcte namespace + selector

---

## 🎉 Resultaat

Je hebt nu:

✅ een zuivere Avalonia control library
✅ klaar voor NuGet
✅ herbruikbaar in meerdere apps
✅ compatibel met Avalonia 12+

---

Veel succes met het publiceren van je control 🚀
