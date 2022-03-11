namespace BarcodeCreator_Framework
{
	using System;
	using System.Collections.ObjectModel;
	using System.ComponentModel;
	using System.Windows;
	using System.Windows.Controls;
	using System.Windows.Media;

	public partial class Barcode : UserControl, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public string Code
		{
			get => (string)GetValue(CodeProp);
			set => SetValue(CodeProp, value);
		}

		public string CodeType
		{
			get => (string)GetValue(TypeProp);
			set => SetValue(TypeProp, value);
		}

		public double Code39WideRate
		{
			get => (double)GetValue(WideRateProp);
			set => SetValue(WideRateProp, value);
		}

		public Visibility TextVisibility
		{
			get => (Visibility)GetValue(VisibilityProp);
			set => SetValue(VisibilityProp, value);
		}

		public double BarWidth
		{
			get => (double)GetValue(WidthProp);
			set => SetValue(WidthProp, value);
		}

		private static readonly DependencyProperty CodeProp = DependencyProperty.Register(nameof(Code), typeof(string), typeof(Barcode), new PropertyMetadata(CodeMetadata));
		private static readonly DependencyProperty TypeProp = DependencyProperty.Register(nameof(CodeType), typeof(string), typeof(Barcode), new PropertyMetadata($"{CodeTypes.Code39}", CodeTypeMetadata));
		private static readonly DependencyProperty WideRateProp = DependencyProperty.Register(nameof(Code39WideRate), typeof(double), typeof(Barcode), new PropertyMetadata(1d, WideRateMetadata));
		private static readonly DependencyProperty VisibilityProp = DependencyProperty.Register(nameof(TextVisibility), typeof(Visibility), typeof(Barcode), new PropertyMetadata(Visibility.Visible, TextVisibilityMetadata));
		private static readonly DependencyProperty WidthProp = DependencyProperty.Register(nameof(BarWidth), typeof(double), typeof(Barcode), new PropertyMetadata(1.5, BarWidthMetadata));

		private readonly BarcodeEngine barcodeEngine;
		private readonly ObservableCollection<BarcodeItem> barCodeItems = new ObservableCollection<BarcodeItem>()
		{
			new BarcodeItem() { Color = Brushes.Black, Width = 5 },
			new BarcodeItem() { Color = Brushes.White, Width = 5 },
			new BarcodeItem() { Color = Brushes.Black, Width = 5 },
		};

		public Barcode()
		{
			InitializeComponent();
			barcodeEngine = new BarcodeEngine();
			DataContext = barCodeItems;
		}

		public void RaisePropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		private static void CodeMetadata(object sender, DependencyPropertyChangedEventArgs e)
		{
			var bc = sender as Barcode;
			bc.codeTextBlock.Text = e.NewValue.ToString();
			bc.barCodeItems.Clear();
			bc.barcodeEngine.Generate(e.NewValue.ToString()).ForEach(item => bc.barCodeItems.Add(item));
			bc.RaisePropertyChanged(nameof(Code));
		}

		private static void CodeTypeMetadata(object sender, DependencyPropertyChangedEventArgs e)
		{
			_ = Enum.TryParse<CodeTypes>((string)e.NewValue, out var result);
			var bc = sender as Barcode;
			bc.CodeType = (string)e.NewValue;
			bc.barcodeEngine.CodeType = result;
			bc.barCodeItems.Clear();
			bc.barcodeEngine.Generate(bc.Code).ForEach(item => bc.barCodeItems.Add(item));
			bc.RaisePropertyChanged(nameof(CodeType));
			bc.RaisePropertyChanged(nameof(Code));
		}

		private static void WideRateMetadata(object sender, DependencyPropertyChangedEventArgs e)
		{
			var bc = sender as Barcode;
			bc.Code39WideRate = (double)e.NewValue;
			bc.barcodeEngine.Code39WideRate = bc.Code39WideRate;
			bc.barCodeItems.Clear();
			bc.barcodeEngine.Generate(bc.Code).ForEach(item => bc.barCodeItems.Add(item));
			bc.RaisePropertyChanged(nameof(Code39WideRate));
		}

		private static void TextVisibilityMetadata(object sender, DependencyPropertyChangedEventArgs e)
		{
			var bc = sender as Barcode;
			bc.TextVisibility = (Visibility)e.NewValue;
			bc.RaisePropertyChanged(nameof(TextVisibility));
		}

		private static void BarWidthMetadata(object sender, DependencyPropertyChangedEventArgs e)
		{
			var bc = sender as Barcode;
			bc.BarWidth = (double)e.NewValue;
			bc.barCodeItems.Clear();
			bc.barcodeEngine.BarWidth = (double)e.NewValue;
			bc.barcodeEngine.Generate(bc.Code).ForEach(item => bc.barCodeItems.Add(item));
			bc.RaisePropertyChanged(nameof(BarWidth));
		}
	}
}