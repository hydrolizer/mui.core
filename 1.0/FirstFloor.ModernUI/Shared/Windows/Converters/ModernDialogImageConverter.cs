using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace FirstFloor.ModernUI.Windows.Converters
{
	/// <summary />
	public class ModernDialogImageConverter : IValueConverter
	{
		/// <summary />
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var image = value as MessageBoxImage? ?? MessageBoxImage.None;
			if (image == MessageBoxImage.None) return null;
			string resName;
			switch(image)
			{
				case MessageBoxImage.Error:
					resName = "Error";
					break;
				case MessageBoxImage.Warning:
					resName = "Warning";
					break;
				case MessageBoxImage.Question:
					resName = "Question";
					break;
				case MessageBoxImage.Information:
					resName = "Information";
					break;
				default:
					return null;
			}
			var rm = new ResourceManager(typeof(ModernDialog).FullName ?? string.Empty, typeof(ModernDialog).Assembly);
			using (MemoryStream memory = new MemoryStream())
			{
				((Bitmap)rm.GetObject(resName)).Save(memory, ImageFormat.Png);
				memory.Position = 0;
				var bi = new BitmapImage();
				bi.BeginInit();
				bi.CacheOption = BitmapCacheOption.OnLoad;
				bi.StreamSource = memory;
				bi.EndInit();
				return bi;
			}
		}

		/// <summary />
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
