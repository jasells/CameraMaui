using System;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Colors = Camera.MAUI.Ex.Resources.Colors;

namespace Camera.MAUI.Ex.Controls;

public partial class TrimThumb : Grid
{
	public static readonly BindableProperty ThumbColorProperty =
		BindableProperty.Create(
			nameof(ThumbColor),
			typeof(Color),
			typeof(TrimThumb),
			Colors.Leaf
		);
	
	public static readonly BindableProperty ThumbCornerRadiusProperty =
		BindableProperty.Create(
			nameof(ThumbCornerRadius),
			typeof(Thickness),
			typeof(TrimThumb),
			new Thickness(5,0,5,0)
		);
	
	public static readonly BindableProperty ThumbInnerLineColorProperty =
		BindableProperty.Create(
			nameof(ThumbInnerLineColor),
			typeof(Color),
			typeof(TrimThumb),
			Colors.Snow
		);
	
	public static readonly BindableProperty ThumbInnerLineCornerRadiusProperty =
		BindableProperty.Create(
			nameof(ThumbCornerRadius),
			typeof(Thickness),
			typeof(TrimThumb),
			new Thickness(50,50,50,50)
		);
	
	public static readonly BindableProperty ThumbInnerLineHeightProperty =
		BindableProperty.Create(
			nameof(ThumbInnerLineHeight),
			typeof(double),
			typeof(TrimThumb),
			11d
		);
	
	public static readonly BindableProperty ThumbInnerLineWidthProperty =
		BindableProperty.Create(
			nameof(ThumbInnerLineWidth),
			typeof(double),
			typeof(TrimThumb),
			1d
		);
	
	public Color ThumbColor
	{
		get => (Color)GetValue(ThumbColorProperty);
		set => SetValue(ThumbColorProperty, value);
	}
	
	public Thickness ThumbCornerRadius
	{
		get => (Thickness)GetValue(ThumbCornerRadiusProperty);
		set => SetValue(ThumbCornerRadiusProperty, value);
	}
	
	public Color ThumbInnerLineColor
	{
		get => (Color)GetValue(ThumbInnerLineColorProperty);
		set => SetValue(ThumbInnerLineColorProperty, value);
	}
	
	public Thickness ThumbInnerLineCornerRadius
	{
		get => (Thickness)GetValue(ThumbInnerLineCornerRadiusProperty);
		set => SetValue(ThumbInnerLineCornerRadiusProperty, value);
	}
	
	public double ThumbInnerLineHeight
	{
		get => (double)GetValue(ThumbInnerLineHeightProperty);
		set => SetValue(ThumbInnerLineHeightProperty, value);
	}
	
	public double ThumbInnerLineWidth
	{
		get => (double)GetValue(ThumbInnerLineWidthProperty);
		set => SetValue(ThumbInnerLineWidthProperty, value);
	}
	
	public TrimThumb()
	{
		InitializeComponent();
	}

	public bool IsLeft
	{
		set => Bookend.CornerRadius = value ? new CornerRadius(5, 0, 5, 0) : new CornerRadius(0, 5, 0, 5);
    }

    public event EventHandler<double>? XChanged;

    public double Xmin {get; set;}
	public double Xmax {get; set;}

	private double _xVal;
	public double Xval 
	{ 
		get => _xVal;
		set 
		{ 
			if (_xVal != value)
			{
				_xVal = value;
				XChanged?.Invoke(this, _xVal);
			}
		}
	}
}