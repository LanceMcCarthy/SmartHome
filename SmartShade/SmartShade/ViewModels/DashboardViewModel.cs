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
        private FEZHAT hat;
        private DispatcherTimer timer;

        private int i;
        private double lightLevelReading = 60;
        private double temperatureReading = 30;
        private bool areLedsOn;
        private bool isButton18Pressed;
        private bool isButton22Pressed;
        private string acclerometerReading;
        private double analogReading;
        private double motorASpeed = 0.5;
        private double motorBSpeed = -0.7;
        private double servo1Position = 100;
        private double servo2Position = 45;
        private bool isMonitoring;
        //private Random rand;
        private ObservableCollection<ShadeItemViewModel> shades;

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

        public bool IsButton18Pressed
        {
            get { return isButton18Pressed; }
            set { Set(ref isButton18Pressed, value); }
        }

        public bool IsButton22Pressed
        {
            get { return isButton22Pressed; }
            set { Set(ref isButton22Pressed, value); }
        }

        public string AcclerometerReading
        {
            get { return acclerometerReading; }
            set { Set(ref acclerometerReading, value); }
        }

        public double AnalogReading
        {
            get { return analogReading; }
            set { Set(ref analogReading, value); }
        }

        public double MotorASpeed
        {
            get { return motorASpeed; }
            set
            {
                // Determines if we need to reverse the motor direction
                var updatedValue = MotorADirection ? value : -value;
                
                Set(ref motorASpeed, updatedValue);
            }
        }

        public bool MotorADirection { get; set; }

        public double MotorBSpeed
        {
            get { return motorBSpeed; }
            set { Set(ref motorBSpeed, value); }
        }

        public double Servo1Position
        {
            get { return servo1Position; }
            set { Set(ref servo1Position, value); }
        }

        public double Servo2Position
        {
            get { return servo2Position; }
            set { Set(ref servo2Position, value); }
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

        public bool UseCelcius { get; set; }

        #endregion

        #region Methods

        private async void Init()
        {
            hat = await FEZHAT.CreateAsync();

            //hat.S1.SetLimits(500, 2400, 0, 180);
            //hat.S2.SetLimits(500, 2400, 0, 180);

            Shades.Add(new ShadeItemViewModel(hat.MotorA, 5, "Office Left", hat.D2));
            
            //rand = new Random();

            // Main Timer
            timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            timer.Tick += OnTick;
        }

        private void OnTick(object sender, object e)
        {
            double x, y, z;

            hat.GetAcceleration(out x, out y, out z);

            LightLevelReading = hat.GetLightLevel();
            AcclerometerReading = $"({x:N2}, {y:N2}, {z:N2})";

            IsButton18Pressed = hat.IsDIO18Pressed();
            IsButton22Pressed = hat.IsDIO22Pressed();

            AnalogReading = hat.ReadAnalog(FEZHAT.AnalogPin.Ain1);

            var celcius = hat.GetTemperature();
            TemperatureReading = UseCelcius ? celcius : ConvertTemp.ConvertCelsiusToFahrenheit(celcius);
            
            // ----------- Servos ----------- //

            //if (IsButton18Pressed)
            //{
            //    hat.S1.Position += 5.0;
            //    hat.S2.Position += 5.0;

            //    if (hat.S1.Position >= 180.0)
            //    {
            //        hat.S1.Position = 0.0;
            //        hat.S2.Position = 0.0;
            //    }
            //}

            //Servo1Position = hat.S1.Position;
            //Servo2Position = hat.S2.Position;


            // ----------- Motors ----------- //

            if (IsButton18Pressed)
            {
                //Shades[0].Motor.Speed = MotorASpeed;
                //Shades[0].RgbLed.Color = new FEZHAT.Color((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255));
                //hat.D2.Color = new FEZHAT.Color((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255));

                Shades[0]?.Open();
            }
            else
            {
                //Shades[0].Motor.Speed = 0.0;
                //hat.D2.Color = FEZHAT.Color.Black;
            }

            if (IsButton22Pressed)
            {
                //Shades[1].Motor.Speed = MotorBSpeed;
                //hat.D3.Color = new FEZHAT.Color((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255));

                Shades[0]?.Close();
            }
            else
            {
                //Shades[1].Motor.Speed = 0.0;
                //hat.D3.Color = FEZHAT.Color.Black;
            }


            // ----------- LEDs and Pins ----------- //

            //if (i++ % 5 == 0)
            //{
            //    hat.DIO24On = AreLedsOn;

            //    hat.WriteDigital(FEZHAT.DigitalPin.DIO16, AreLedsOn);
            //    hat.WriteDigital(FEZHAT.DigitalPin.DIO26, AreLedsOn);

            //    hat.SetPwmDutyCycle(FEZHAT.PwmPin.Pwm5, AreLedsOn ? 1.0 : 0.0);
            //    hat.SetPwmDutyCycle(FEZHAT.PwmPin.Pwm6, AreLedsOn ? 1.0 : 0.0);
            //    hat.SetPwmDutyCycle(FEZHAT.PwmPin.Pwm7, AreLedsOn ? 1.0 : 0.0);
            //    hat.SetPwmDutyCycle(FEZHAT.PwmPin.Pwm11, AreLedsOn ? 1.0 : 0.0);
            //    hat.SetPwmDutyCycle(FEZHAT.PwmPin.Pwm12, AreLedsOn ? 1.0 : 0.0);

            //    AreLedsOn = !AreLedsOn;
            //}
        }
        

        public override Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> state)
        {
            Init();
            return base.OnNavigatedToAsync(parameter, mode, state);
        }

        #endregion

    }
}
