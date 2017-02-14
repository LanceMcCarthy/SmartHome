using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using GHIElectronics.UWP.Shields;
using SmartShade.Converters;
using SmartShade.Models;
using Template10.Mvvm;

namespace SmartShade.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        #region Fields

        private int i;
        private FEZHAT hat;
        private DispatcherTimer timer;

        private ObservableCollection<ShadeItemViewModel> shades;
        private double lightLevelReading = 60;
        private double temperatureReading = 30;
        private bool areLedsOn;
        private bool isMonitoring;

        #endregion

        public DashboardViewModel()
        {
            if (DesignMode.DesignModeEnabled)
            {
                Shades.Add(new ShadeItemViewModel(hat.MotorA, 5, "Office Left"));
            }
        }

        #region Properties

        public ObservableCollection<ShadeItemViewModel> Shades => shades ?? (shades = new ObservableCollection<ShadeItemViewModel>());

        public double LightLevelReading
        {
            get { return lightLevelReading; }
            set { Set(ref lightLevelReading, value); }
        }

        public double TemperatureReading
        {
            get { return temperatureReading; }
            set { Set(ref temperatureReading, value); }
        }

        public bool AreLedsOn
        {
            get { return areLedsOn; }
            set { Set(ref areLedsOn, value); }
        }

        public bool IsMonitoring
        {
            get { return isMonitoring; }
            set
            {
                if (value)
                    timer?.Start();
                else
                    timer?.Stop();

                Set(ref isMonitoring, value);
            }
        }

        #endregion

        #region Methods

        private async Task Init()
        {
            hat = await FEZHAT.CreateAsync();
            
            Shades.Add(new ShadeItemViewModel(hat.MotorA, 5, "Left Window", hat.D2));
            Shades.Add(new ShadeItemViewModel(hat.MotorB, 5, "Right Window", hat.D3));

            // Main Timer - For Temp, light reading and status LEDs
            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            timer.Tick += OnTick;

            IsMonitoring = true;
        }

        private void OnTick(object sender, object e)
        {
            // Status LEDs
            if (i++ % 5 == 0) // this is true every 500ms
            {
                // If you want an LED some where on the external enclosure, use a PWN PIN
                //hat.SetPwmDutyCycle(FEZHAT.PwmPin.Pwm6, AreLedsOn ? 1.0 : 0.0);

                // On-board LED
                hat.DIO24On = AreLedsOn;
                
                AreLedsOn = !AreLedsOn;
            }

            LightLevelReading = hat.GetLightLevel() * 100;

            TemperatureReading = ConvertTemp.ConvertCelsiusToFahrenheit(hat.GetTemperature());
        }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            await Init();
        }

        #endregion
    }
}
