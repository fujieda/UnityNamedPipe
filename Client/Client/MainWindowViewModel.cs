using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Client
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly string[] _colors =
        {
            "#ffffff", // white
            "#ff4b00", // CUD red
            "#fff100", // CUD yellow
            "#03af7c", // CUD green
            "#0050ff", // CUD blue
        };

        private readonly IPC _ipc = new IPC {ReceiveTimeout = 10};
        private bool _enabled = true;
        private string _status = "";

        public MainWindowViewModel()
        {
            Colors = _colors.Select(name => new ColorRecord(name)).ToList();
        }

        public string[] Scenes => new[] {"Bounce", "Rotate"};

        public bool IsEnabled
        {
            get => _enabled;
            set
            {
                _enabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        public IList<ColorRecord> Colors { get; }

        public string SelectedScene
        {
            set
            {
                if (value == null)
                    return;
                Task.Run(async () =>
                {
                    IsEnabled = false;
                    try
                    {
                        Status = await _ipc.Send("Scene " + value);
                    }
                    catch (Exception e)
                    {
                        Status = e.Message;
                    }
                    IsEnabled = true;
                });
            }
        }

        public ColorRecord SelectedColor
        {
            set
            {
                if (value == null)
                    return;
                Task.Run(async () =>
                {
                    IsEnabled = false;
                    try
                    {
                        Status = await _ipc.Send("Color " + value.Name);
                    }
                    catch (Exception e)
                    {
                        Status = e.Message;
                    }
                    IsEnabled = true;
                });
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                OnPropertyChanged(nameof(Status));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public class ColorRecord
        {
            public ColorRecord(string name)
            {
                Name = name;
                Color = new SolidColorBrush((Color)(ColorConverter.ConvertFromString(name) ??
                                                    System.Windows.Media.Colors.White));
            }

            public string Name { get; }

            public SolidColorBrush Color { get; }
        }
    }
}