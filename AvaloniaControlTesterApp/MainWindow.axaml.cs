using Avalonia.Controls;
using System.Threading.Tasks;
using BrainForceOne.AvaloniaControls;

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
            int duration = 10000; // 10 seconden
            int steps = 100;
            int delay = duration / steps;

            while (true) // 🔥 infinite loop
            {
                for (int i = 0; i <= 100; i++)
                {
                    Ring.Progress = i;
                    await Task.Delay(delay);
                }

                // reset
                Ring.Progress = 0;
                await Task.Delay(300); // kleine pauze (optioneel)
            }
        }
    }
}