# Camera.MAUI.Ex

This project is a video handling library for .NET MAUI applications, providing controls, services, and utilities for video playback, trimming, and camera integration.

---

## 📂 Project Structure

The project is organized into the following main directories:

### 1. **Controls**
Custom controls for video handling and trimming.

| File                        | Description                                      |
|-----------------------------|--------------------------------------------------|
| `CorrectedPanGestureRecognizer.cs` | Custom gesture recognizer with corrections for panning. |
| `ImageHighlightSlider.xaml` | UI component for highlighting image sections.    |
| `ImageSliderBase.xaml`      | Base UI for image sliders.                       |
| `TrimThumb.xaml`            | UI control for video trimming thumbs.           |
| `VideoTrimmingSlider.xaml`  | Slider control for video trimming operations.   |
| `VideoView.cs`              | Video playback control implementation.          |

---

### 2. **Interfaces**
Defines service contracts for abstraction.

| File                  | Description                                   |
|-----------------------|-----------------------------------------------|
| `ICameraService.cs`   | Interface for camera-related functionality.  |
| `IVideoService.cs`    | Interface for video playback/trimming services.|

---

### 3. **Models**
Data models used across the project.

| File          | Description                           |
|---------------|---------------------------------------|
| `Thumbnail.cs`| Represents thumbnail data for videos. |

---

### 4. **Services**
Service implementations for camera and video handling.

| File                | Description                                  |
|---------------------|----------------------------------------------|
| `CameraService.cs`  | Provides camera integration functionality.  |
| `VideoService.cs`   | Implements video playback and trimming logic.|

---

## 🚀 Features

1. **Video Trimming Control**:  
   The `VideoTrimmingSlider` allows users to trim videos efficiently by providing thumb sliders for setting start (`TrimFrom`) and end (`TrimTo`) positions.

2. **Camera Integration**:  
   Easily integrate camera functionality with `CameraService`.

3. **Video Playback**:  
   The `VideoView` supports video rendering and playback with LibVLC integration.

4. **Thumbnail Support**:  
   Generate and display video thumbnails using the `Thumbnail` model.

---

## 💻 Usage

### **XAML Example for VideoView**

```xml
<controls:VideoView 
    x:Name="VideoView"
    HeightRequest="480" 
    WidthRequest="640"
    HorizontalOptions="Fill"
    VerticalOptions="Fill">
    <controls:VideoView.GestureRecognizers>
        <TapGestureRecognizer Tapped="OnVideoTapped"/>
    </controls:VideoView.GestureRecognizers>
</controls:VideoView>
```


### **XAML Example for ImageHighlighter**

```xml
 <controls:ImageHighlightSlider x:Name="Slider"
                                FilePath="{Binding FilePath}"
                                MediaPlayer="{Binding MediaPlayer}">
   <controls:ImageHighlightSlider.Behaviors>
      <toolkit:EventToCommandBehavior
              EventName="SeekPlayer"
              Command="{Binding SeekPlayerCommand}"
              CommandParameter="{Binding SeekCommandParameter, Source={x:Reference Slider}}"
      />
   </controls:ImageHighlightSlider.Behaviors>
</controls:ImageHighlightSlider>
```

---
### **Video Trimming Slider**
   Add `VideoTrimmingSlider.xaml` to your page and bind `TrimFrom` and `TrimTo` for user interaction.

#### **XAML Example for Video Trimming Slider**

```xml
 <controls:VideoTrimmingSlider x:Name="Slider"
                               FilePath="{Binding FilePath}"
                               MediaPlayer="{Binding MediaPlayer}"
                               TrimFrom="{Binding TrimFrom, Mode=TwoWay}"
                               TrimTo="{Binding TrimTo, Mode=TwoWay}"
                               Margin="30,0,30,0">
   <controls:VideoTrimmingSlider.Behaviors>
      <toolkit:EventToCommandBehavior
              EventName="SeekPlayer"
              Command="{Binding SeekPlayerCommand}"
              CommandParameter="{Binding SeekCommandParameter, Source={x:Reference Slider}}"
      />
   </controls:VideoTrimmingSlider.Behaviors>
</controls:VideoTrimmingSlider>
```
---