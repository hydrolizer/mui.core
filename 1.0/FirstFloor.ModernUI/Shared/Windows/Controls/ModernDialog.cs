using FirstFloor.ModernUI.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FirstFloor.ModernUI.Themes;

namespace FirstFloor.ModernUI.Windows.Controls
{
	/// <summary>
	/// Represents a Modern UI styled dialog window.
	/// </summary>
	public class ModernDialog
			: DpiAwareWindow
	{
		/// <summary>
		/// Identifies the BackgroundContent dependency property.
		/// </summary>
		public static readonly DependencyProperty BackgroundContentProperty = DependencyProperty.Register("BackgroundContent", typeof(object), typeof(ModernDialog));
		/// <summary>
		/// Identifies the Buttons dependency property.
		/// </summary>
		public static readonly DependencyProperty ButtonsProperty = DependencyProperty.Register("Buttons", typeof(IEnumerable<Button>), typeof(ModernDialog));

		/// <summary/>
		public static readonly DependencyProperty ImageProperty = DependencyProperty.Register(
			nameof(Image), typeof(MessageBoxImage), typeof(ModernDialog),
			new FrameworkPropertyMetadata(MessageBoxImage.None,
			FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure)
		);

		/// <summary/>
		public static readonly DependencyProperty OptionInfoProperty = DependencyProperty.Register(
			nameof(OptionInfo), typeof(DialogOptionInfo), typeof(ModernDialog),
			new FrameworkPropertyMetadata(
				null,
				FrameworkPropertyMetadataOptions.AffectsArrange |
				FrameworkPropertyMetadataOptions.AffectsMeasure
			)
		);

		/// <summary/>
		public DialogOptionInfo OptionInfo
		{
			get => (DialogOptionInfo)GetValue(OptionInfoProperty);
			set => SetValue(OptionInfoProperty, value);
		}

		/// <summary/>
		public MessageBoxImage Image
		{
			get => (MessageBoxImage) (GetValue(ImageProperty) ?? MessageBoxImage.None);
			set => SetValue(ImageProperty, value);
		}

		private ICommand closeCommand;

		private Button okButton;
		private Button cancelButton;
		private Button yesButton;
		private Button noButton;
		private Button closeButton;

		private MessageBoxResult messageBoxResult = MessageBoxResult.None;

		/// <summary>
		/// Initializes a new instance of the <see cref="ModernDialog"/> class.
		/// </summary>
		public ModernDialog()
		{
			this.DefaultStyleKey = typeof(ModernDialog);
			this.WindowStartupLocation = WindowStartupLocation.CenterOwner;

			this.closeCommand = new RelayCommand(o =>
			{
				var result = o as MessageBoxResult?;
				if (result.HasValue)
				{
					this.messageBoxResult = result.Value;

					// sets the Window.DialogResult as well
					if (result.Value == MessageBoxResult.OK || result.Value == MessageBoxResult.Yes)
					{
						this.DialogResult = true;
					}
					else if (result.Value == MessageBoxResult.Cancel || result.Value == MessageBoxResult.No)
					{
						this.DialogResult = false;
					}
					else
					{
						this.DialogResult = null;
					}
				}
				Close();
			});

			this.Buttons = new Button[] { this.CloseButton };

			// set the default owner to the app main window (if possible)
			if (Application.Current != null && Application.Current.MainWindow != this)
			{
				this.Owner = Application.Current.MainWindow;
			}

			Loaded += (s, e) =>
			{
				var buttons = Buttons.ToList();
				var maxWidth = buttons.Select(b => b.ActualWidth).Max();
				foreach (var b in buttons)
					b.Width = maxWidth;
				if (buttons.Count == 1)
					buttons[0].IsCancel = true;
			};
		}

		private Button CreateCloseDialogButton(string content, bool isDefault, bool isCancel, MessageBoxResult result)
		{
			return new Button
			{
				Content = content,
				Command = this.CloseCommand,
				CommandParameter = result,
				IsDefault = isDefault,
				IsCancel = isCancel,
				MinHeight = 21,
				MinWidth = 65,
				Margin = new Thickness(4, 0, 0, 0)
			};
		}

		/// <summary>
		/// Gets the close window command.
		/// </summary>
		public ICommand CloseCommand
		{
			get { return this.closeCommand; }
		}

		/// <summary>
		/// Gets the Ok button.
		/// </summary>
		public Button OkButton
		{
			get
			{
				if (this.okButton == null)
				{
					this.okButton = CreateCloseDialogButton(FirstFloor.ModernUI.Resources.Ok, true, false, MessageBoxResult.OK);
				}
				return this.okButton;
			}
		}

		/// <summary>
		/// Gets the Cancel button.
		/// </summary>
		public Button CancelButton
		{
			get
			{
				if (this.cancelButton == null)
				{
					this.cancelButton = CreateCloseDialogButton(FirstFloor.ModernUI.Resources.Cancel, false, true, MessageBoxResult.Cancel);
				}
				return this.cancelButton;
			}
		}

		/// <summary>
		/// Gets the Yes button.
		/// </summary>
		public Button YesButton
		{
			get
			{
				if (this.yesButton == null)
				{
					this.yesButton = CreateCloseDialogButton(FirstFloor.ModernUI.Resources.Yes, true, false, MessageBoxResult.Yes);
				}
				return this.yesButton;
			}
		}

		/// <summary>
		/// Gets the No button.
		/// </summary>
		public Button NoButton
		{
			get
			{
				if (this.noButton == null)
				{
					this.noButton = CreateCloseDialogButton(FirstFloor.ModernUI.Resources.No, false, true, MessageBoxResult.No);
				}
				return this.noButton;
			}
		}

		/// <summary>
		/// Gets the Close button.
		/// </summary>
		public Button CloseButton
		{
			get
			{
				if (this.closeButton == null)
				{
					this.closeButton = CreateCloseDialogButton(FirstFloor.ModernUI.Resources.Close, true, false, MessageBoxResult.None);
					closeButton.IsCancel = true;
				}
				return this.closeButton;
			}
		}

		/// <summary>
		/// Gets or sets the background content of this window instance.
		/// </summary>
		public object BackgroundContent
		{
			get { return GetValue(BackgroundContentProperty); }
			set { SetValue(BackgroundContentProperty, value); }
		}

		/// <summary>
		/// Gets or sets the dialog buttons.
		/// </summary>
		public IEnumerable<Button> Buttons
		{
			get { return (IEnumerable<Button>)GetValue(ButtonsProperty); }
			set { SetValue(ButtonsProperty, value); }
		}

		/// <summary>
		/// Gets the message box result.
		/// </summary>
		/// <value>
		/// The message box result.
		/// </value>
		public MessageBoxResult MessageBoxResult
		{
			get { return this.messageBoxResult; }
		}

		/// <summary>
		/// Displays a messagebox.
		/// </summary>
		public static MessageBoxResult ShowMessage(
			string text,
			string title,
			MessageBoxButton button,
			MessageBoxImage image,
			SupportedThemes? theme,
			DialogOptionInfo optionInfo,
			Window owner,
			out bool? optionValue
		)
		{
			var dlg = new ModernDialog
			{
				Title = title,
				Content = new BBCodeBlock { BBCode = text, Margin = new (0, 0, 0, 8) },
				MinHeight = 0,
				MinWidth = 0,
				MaxHeight = 480,
				MaxWidth = 640,
				Image = image,
				OptionInfo = optionInfo
			};
			if(theme.HasValue)
				foreach(var s in new[]
				{
					"ModernWindowStyles.xaml",
					"ModernUI.xaml",
					theme==SupportedThemes.Light
						? "ModernUI.Light.xaml"
						: "ModernUI.Dark.xaml"
				})
					dlg.Resources.MergedDictionaries.Add(
						new ResourceDictionary
						{
							Source = new Uri($"/FirstFloor.ModernUI;component/Assets/{s}", UriKind.RelativeOrAbsolute)
						});
			if (owner != null)
			{
				dlg.Owner = owner;
			}

			dlg.Buttons = GetButtons(dlg, button);
			dlg.ShowDialog();
			optionValue = dlg.OptionInfo?.DefaultValue;
			return dlg.messageBoxResult;
		}

		/// <summary>
		/// Displays a messagebox.
		/// </summary>
		public static MessageBoxResult ShowMessage(
			string text,
			string title,
			MessageBoxButton button,
			MessageBoxImage image,
			SupportedThemes? theme,
			Window owner
		)
			=> ShowMessage(text, title, button, image, theme, null, owner, out _);

		/// <summary>
		/// Displays a messagebox.
		/// </summary>
		public static MessageBoxResult ShowMessage(
			string text,
			string title,
			MessageBoxButton button,
			MessageBoxImage image = MessageBoxImage.None,
			Window owner=null)
		{
			return ShowMessage(text, title, button, image, null, null, owner, out _);
		}

		private static IEnumerable<Button> GetButtons(ModernDialog owner, MessageBoxButton button)
		{
			if (button == MessageBoxButton.OK)
			{
				yield return owner.OkButton;
			}
			else if (button == MessageBoxButton.OKCancel)
			{
				yield return owner.OkButton;
				yield return owner.CancelButton;
			}
			else if (button == MessageBoxButton.YesNo)
			{
				yield return owner.YesButton;
				yield return owner.NoButton;
			}
			else if (button == MessageBoxButton.YesNoCancel)
			{
				yield return owner.YesButton;
				yield return owner.NoButton;
				yield return owner.CancelButton;
			}
		}
	}
}
