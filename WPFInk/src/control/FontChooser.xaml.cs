using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFInk.font;
using WPFInk.ink;

namespace WPFInk
{
	/// <summary>
	/// Interaction logic for FontChooser.xaml
	/// </summary>
	public partial class FontChooser : UserControl
	{
		private ICollection<FontFamily> _familyCollection;
		private InkFrame _inkFrame = null;

		private static readonly double[] CommonlyUsedFontSizes = new double[]
        {
            3.0,    4.0,   5.0,   6.0,   6.5,
            7.0,    7.5,   8.0,   8.5,   9.0,
            9.5,   10.0,  10.5,  11.0,  11.5,
            12.0,  12.5,  13.0,  13.5,  14.0,
            15.0,  16.0,  17.0,  18.0,  19.0,
            20.0,  22.0,  24.0,  26.0,  28.0,  30.0,  32.0,  34.0,  36.0,  38.0,
            40.0,  44.0,  48.0,  52.0,  56.0,  60.0,  64.0,  68.0,  72.0,  76.0,
            80.0,  88.0,  96.0, 104.0, 112.0, 120.0, 128.0, 136.0, 144.0
        };


		public FontChooser()
		{
			this.InitializeComponent();
			InitializeFontFamilyList();

			// Initialize the list of font sizes and select the nearest size.
			foreach (double value in CommonlyUsedFontSizes)
			{
                FontSizeCmb.Items.Add(new FontSizeListItem(value));
			}
			PresetFontColorCombo();
		}
		public void setInkFrame(InkFrame inkFrame)
		{
			this._inkFrame = inkFrame;
		}
		private void InitializeFontFamilyList()
		{
			ICollection<FontFamily> familyCollection = FontFamilyCollection;
			if (familyCollection != null)
			{
				FontFamilyListItem[] items = new FontFamilyListItem[familyCollection.Count];

				int i = 0;

				foreach (FontFamily family in familyCollection)
				{
					items[i++] = new FontFamilyListItem(family);
				}

				Array.Sort<FontFamilyListItem>(items);

				for (int j = items.Length - 1; j >= 0; j--)
				{
                    FontFamilyCmb.Items.Add(items[j]);
				}
			}
		}

		public ICollection<FontFamily> FontFamilyCollection
		{
			get
			{
				return (_familyCollection == null) ? Fonts.SystemFontFamilies : _familyCollection;
			}

			set
			{
				if (value != _familyCollection)
				{
					_familyCollection = value;
				}
			}
		}

		void PresetFontColorCombo()
		{
			int i;
			// Fill combobox with all known named colors

			for (i = 0; i < KnownColor.ColorNames.Length; i++)
			{
				FontColor.Items.Add(new ColorComboBoxItem(
					KnownColor.ColorNames[i],
					(SolidColorBrush)KnownColor.ColorTable[KnownColor.ColorNames[i]]
				));
			}
		}

		//字体
		private void FontFamily_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (this._inkFrame.InkCollector.SelectedRichTextBox != null)
			{
				TextRange textRange = new TextRange(this._inkFrame.InkCollector.SelectedRichTextBox.Document.ContentStart, this._inkFrame.InkCollector.SelectedRichTextBox.Document.ContentEnd);
                textRange.ApplyPropertyValue(RichTextBox.FontFamilyProperty, FontFamilyCmb.SelectedItem.ToString());
			}
            _inkFrame.TextFontFamily = FontFamilyCmb.SelectedItem.ToString();
		}

		//大小
		private void FontSize_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (this._inkFrame.InkCollector.SelectedRichTextBox != null)
			{
				TextRange textRange = new TextRange(this._inkFrame.InkCollector.SelectedRichTextBox.Document.ContentStart, this._inkFrame.InkCollector.SelectedRichTextBox.Document.ContentEnd);
                textRange.ApplyPropertyValue(RichTextBox.FontSizeProperty, FontSizeCmb.SelectedItem.ToString());
			}
            _inkFrame.TextFontSize = FontSizeCmb.SelectedItem.ToString();
		}

		//加粗
		int BoldTimes = 0;
		private void Bold_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (this._inkFrame.InkCollector.SelectedRichTextBox != null)
			{
				TextRange textRange = new TextRange(this._inkFrame.InkCollector.SelectedRichTextBox.Document.ContentStart, this._inkFrame.InkCollector.SelectedRichTextBox.Document.ContentEnd);
				if (BoldTimes % 2 == 0)
				{
					textRange.ApplyPropertyValue(RichTextBox.FontWeightProperty, "Bold");
					_inkFrame.TextFontWeight = "Bold";
					Bold.Background = Brushes.LemonChiffon;
				}
				else
				{
					textRange.ApplyPropertyValue(RichTextBox.FontWeightProperty, "Normal");
					_inkFrame.TextFontWeight = "Normal";
					Bold.Background = null;
				}
				BoldTimes++;
			}
		}

		//斜体
		int ItalicTimes = 0;
		private void Italics_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (this._inkFrame.InkCollector.SelectedRichTextBox != null)
			{
				TextRange textRange = new TextRange(this._inkFrame.InkCollector.SelectedRichTextBox.Document.ContentStart, this._inkFrame.InkCollector.SelectedRichTextBox.Document.ContentEnd);
				if (ItalicTimes % 2 == 0)
				{
					textRange.ApplyPropertyValue(RichTextBox.FontStyleProperty, "Italic");
					_inkFrame.TextFontStyle = "Italic";
					Italics.Background = Brushes.LemonChiffon;
				}
				else
				{
					textRange.ApplyPropertyValue(RichTextBox.FontStyleProperty, "Normal");
					_inkFrame.TextFontStyle = "Normal";
					Italics.Background = null;
				}
				ItalicTimes++;
			}
		}

		//颜色
		private void FontColor_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (this._inkFrame.InkCollector.SelectedRichTextBox != null)
			{
				TextRange textRange = new TextRange(this._inkFrame.InkCollector.SelectedRichTextBox.Document.ContentStart, this._inkFrame.InkCollector.SelectedRichTextBox.Document.ContentEnd);
				textRange.ApplyPropertyValue(RichTextBox.ForegroundProperty, ((ColorComboBoxItem)FontColor.SelectedItem).Brush);
			}
			_inkFrame.TextFontColor = ((ColorComboBoxItem)FontColor.SelectedItem).Brush;
		}

		

	}
}