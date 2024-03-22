using System.ComponentModel;

namespace FirstFloor.ModernUI.Windows;

/// <summary />
public sealed class DialogOptionInfo : INotifyPropertyChanged
{
	/// <summary />
	public string Content { get; set; }

	bool? _defaultValue;
	/// <summary />
	public bool? DefaultValue
	{
		get => _defaultValue;
		set
		{
			if (_defaultValue == value) return;
			_defaultValue = value;
			PropertyChanged?.Invoke(this, new (nameof(DefaultValue)));
		}
	}

	/// <summary />
	public bool AllowNull { get; set; }

	/// <summary />
	public event PropertyChangedEventHandler PropertyChanged;
}