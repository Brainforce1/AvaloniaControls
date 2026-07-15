using Avalonia.Controls;
using System.Threading.Tasks;

namespace AvaloniaControlTesterApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            StartProgress();
        }

        private async void StartProgress()
        {
            int duration = 10000;
            int steps = 100;
            int delay = duration / steps;

            while (true)
            {
                for (int i = 0; i <= 100; i++)
                {
                    Ring.Progress = i;
                    Speedometer.Value = i + 30;
                    Thermometer.Value = i;
                    LedDisplay.Value = i;
                    await Task.Delay(delay);
                }

                // reset
                Ring.Progress = 0;
                Speedometer.Value = 30;
                Thermometer.Value = 0;
                LedDisplay.Value = 0;
                await Task.Delay(300);
            }
        }
    }
}