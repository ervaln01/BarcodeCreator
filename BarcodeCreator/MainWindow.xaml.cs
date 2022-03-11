namespace BarcodeCreator
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Windows;
	using System.Windows.Controls;

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			CodeTypeCB.DataContext = (Enum.GetValues(typeof(CodeTypes)) as IEnumerable<CodeTypes>).OrderBy(x => x.ToString()).ToDictionary(x => x, x => x);
			CodeTypeCB.SelectedValue = $"{CodeTypes.Code39}";
		}

		private void WidthSliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => barcode.BarWidth = e.NewValue;

		private void ShowCBClick(object sender, RoutedEventArgs e) => barcode.TextVisibility = ShowCB.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;

		private void CodeTBChanged(object sender, TextChangedEventArgs e) => barcode.Code = CodeTB.Text;

		private void CodeTypeCBChanged(object sender, SelectionChangedEventArgs e) => barcode.CodeType = CodeTypeCB.SelectedValue.ToString();

		private void Code39SliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => barcode.Code39WideRate = e.NewValue;
	}
}