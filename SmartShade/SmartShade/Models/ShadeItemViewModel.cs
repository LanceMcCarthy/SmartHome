using System;
using Windows.UI.Xaml;
using GHIElectronics.UWP.Shields;
using Template10.Mvvm;

namespace SmartShade.Models
{
    public class ShadeItemViewModel : ViewModelBase, IDisposable
    {
        #region Fields

        private string windowLocation;
        private FEZHAT.Motor motor;
        private FEZHAT.RgbLed rgbLed;
        private double speedCoefficient = 1;
        private int operationDuration;

        private DispatcherTimer operationTimer;
        private double speed;
        private string currentStatus;
        private bool isOperating;
        private double operationProgress;
        private DelegateCommand openCommand;
        private DelegateCommand closeCommand;
        private DelegateCommand cancelCommand;
        private bool isOpen;

        #endregion

        #region CTORs

        public ShadeItemViewModel(FEZHAT.Motor motor, int operationDuration)
        {
            this.motor = motor;
            this.operationDuration = operationDuration;

            operationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            operationTimer.Tick += OperationTimer_Tick;
        }

        public ShadeItemViewModel(FEZHAT.Motor motor, int operationDuration, string windowLocation)
        {
            this.motor = motor;
            this.operationDuration = operationDuration;
            this.windowLocation = windowLocation;

            operationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            operationTimer.Tick += OperationTimer_Tick;
        }

        public ShadeItemViewModel(FEZHAT.Motor motor, int operationDuration, string windowLocation, FEZHAT.RgbLed rgbLed)
        {
            this.motor = motor;
            this.operationDuration = operationDuration;
            this.windowLocation = windowLocation;
            this.rgbLed = rgbLed;

            operationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            operationTimer.Tick += OperationTimer_Tick;
        }

        #endregion

        #region Properties

        public bool IsOpen
        {
            get { return isOpen; }
            set { Set(ref isOpen, value); }
        }

        public string WindowLocation
        {
            get { return windowLocation; }
            set { Set(ref windowLocation, value); }
        }

        public FEZHAT.Motor Motor
        {
            get { return motor; }
            set { Set(ref motor, value); }
        }

        public FEZHAT.RgbLed RgbLed
        {
            get { return rgbLed; }
            set { Set(ref rgbLed, value); }
        }

        public double Speed
        {
            get { return speed; }
            private set
            {
                Motor.Speed = value;
                Set(ref speed, value);
            }
        }

        public double SpeedCoefficient
        {
            get { return speedCoefficient; }
            set { Set(ref speedCoefficient, value); }
        }

        public int OperationDuration
        {
            get { return operationDuration; }
            set { Set(ref operationDuration, value); }
        }

        public string CurrentStatus
        {
            get { return currentStatus; }
            set { Set(ref currentStatus, value); }
        }

        public double OperationProgress
        {
            get { return operationProgress; }
            set { Set(ref operationProgress, value); }
        }

        public bool IsOperating
        {
            get { return isOperating; }
            set { Set(ref isOperating, value); }
        }

        #endregion

        #region Commands

        public DelegateCommand OpenCommand => openCommand ?? (openCommand = new DelegateCommand(Open));

        public DelegateCommand CloseCommand => closeCommand ?? (closeCommand = new DelegateCommand(Close));

        public DelegateCommand CancelCommand => cancelCommand ?? (cancelCommand = new DelegateCommand(Stop));

        #endregion

        #region Methods

        public void Open()
        {
            Speed = +Speed;
            RgbLed.Color = new FEZHAT.Color(0, 255, 0);
            CurrentStatus = "Opening...";

            IsOperating = true;
            operationTimer.Start();
        }

        public void Close()
        {
            Speed = -Speed;
            RgbLed.Color = new FEZHAT.Color(255, 0, 0);
            CurrentStatus = "Closing...";

            IsOperating = true;
            operationTimer.Start();
        }

        public void Stop()
        {
            Speed = 0;
            OperationProgress = 0;
            rgbLed.Color = FEZHAT.Color.Black;

            operationTimer.Stop();

            IsOperating = false;
        }

        private void OperationTimer_Tick(object sender, object e)
        {
            OperationProgress = OperationProgress + 0.1;

            if (OperationProgress < OperationDuration)
            {
                Speed = 1 * speedCoefficient;
            }
            else if (Math.Abs(OperationProgress - OperationDuration) < 0.05)
            {
                Speed = 1 * speedCoefficient;
                IsOpen = !IsOpen;
            }
            else
            {
                Stop();
            }
        }

        public void Dispose()
        {
            Speed = 0;
            RgbLed.Color = FEZHAT.Color.Black;

            if (operationTimer.IsEnabled)
            {
                operationTimer.Stop();
            }

            operationTimer.Tick -= OperationTimer_Tick;
            operationTimer = null;

            motor.Dispose();
        }
        
        #endregion
    }
}
