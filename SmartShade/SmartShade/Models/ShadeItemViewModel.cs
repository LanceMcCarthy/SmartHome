using System;
using System.Runtime.Serialization;
using Windows.UI.Xaml;
using GHIElectronics.UWP.Shields;
using Template10.Mvvm;

namespace SmartShade.Models
{
    [DataContract]
    public class ShadeItemViewModel : ViewModelBase, IDisposable
    {
        #region Fields

        private string windowName;
        private FEZHAT.Motor shadeMotor;
        private FEZHAT.RgbLed rgbLed;
        private double motorSpeed; // ranges from -1 to 1, 0 is full stop
        private double speedCoefficient = 1; // this can be used to tweak the speed
        private double operationDuration;
        private string currentStatus;
        private double shadePosition;
        private bool isOperating;
        private bool isShadeOpen = true;

        private DelegateCommand openCommand;
        private DelegateCommand closeCommand;
        private DelegateCommand cancelCommand;

        private DispatcherTimer operationTimer;
        private double operationProgress;

        #endregion

        #region CTORs
        
        public ShadeItemViewModel(FEZHAT.Motor shadeMotor, int operationDuration, string windowName)
        {
            this.shadeMotor = shadeMotor;
            this.operationDuration = operationDuration;
            this.windowName = windowName;

            operationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            operationTimer.Tick += OperationTimer_Tick;
        }

        public ShadeItemViewModel(FEZHAT.Motor shadeMotor, int operationDuration, string windowName, FEZHAT.RgbLed rgbLed)
        {
            this.shadeMotor = shadeMotor;
            this.operationDuration = operationDuration;
            this.windowName = windowName;
            this.rgbLed = rgbLed;

            operationTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
            operationTimer.Tick += OperationTimer_Tick;
        }

        #endregion

        #region Properties
        
        public string WindowName
        {
            get { return windowName; }
            set { Set(ref windowName, value); }
        }

        public FEZHAT.Motor ShadeMotor
        {
            get { return shadeMotor; }
            set { Set(ref shadeMotor, value); }
        }

        public FEZHAT.RgbLed RgbLed
        {
            get { return rgbLed; }
            set { Set(ref rgbLed, value); }
        }

        public double MotorSpeed
        {
            get { return motorSpeed; }
            private set
            {
                ShadeMotor.Speed = value;
                Set(ref motorSpeed, value);
            }
        }

        public double SpeedCoefficient
        {
            get { return speedCoefficient; }
            set { Set(ref speedCoefficient, value); }
        }

        public double OperationDuration
        {
            get { return operationDuration; }
            set { Set(ref operationDuration, value); }
        }

        public string CurrentStatus
        {
            get { return currentStatus; }
            set { Set(ref currentStatus, value); }
        }

        public double ShadePosition
        {
            get { return shadePosition; }
            set { Set(ref shadePosition, value); }
        }

        public bool IsOperating
        {
            get { return isOperating; }
            set { Set(ref isOperating, value); }
        }

        public bool IsShadeOpen
        {
            get { return isShadeOpen; }
            set { Set(ref isShadeOpen, value); }
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
            if (RgbLed != null)
                RgbLed.Color = new FEZHAT.Color(0, 255, 0);
            
            MotorSpeed = 1 * speedCoefficient;
            IsOperating = true;
            operationTimer.Start();
        }

        public void Close()
        {
            if (RgbLed != null)
                RgbLed.Color = new FEZHAT.Color(255, 0, 0);
            
            MotorSpeed = -1 * speedCoefficient;
            IsOperating = true;
            operationTimer.Start();
        }

        public void Stop()
        {
            RgbLed?.TurnOff();
            MotorSpeed = 0;
            IsOperating = false;
            operationTimer.Stop();
        }

        private void OperationTimer_Tick(object sender, object e)
        {
            // Increment the progress counter
            operationProgress = operationProgress + 0.1;

            // Calculate the percentage complete
            int percentComplete = Convert.ToInt16(Math.Ceiling((operationProgress / operationDuration) * 100));

            // This is to move the slider to visualize the shade's current position
            // eventually it will be used for a window animation
            UpdateShadePosition(percentComplete);

            if (percentComplete < 100)
            {
                var operation = IsShadeOpen ? "Closing" : "Opening";
                CurrentStatus = $"{operation} - {percentComplete}% Complete";
            }
            else if (percentComplete == 100)
            {
                operationProgress = 0;

                // toggle completion
                IsShadeOpen = !IsShadeOpen; 

                // update status
                CurrentStatus = IsShadeOpen ? "Open" : "Closed";

                Stop();
            }
        }
        
        private void UpdateShadePosition(double percentComplete)
        {
            if (IsShadeOpen)
                ShadePosition = percentComplete;
            else
                ShadePosition = 100 - percentComplete;
        }

        public void Dispose()
        {
            MotorSpeed = 0;
            RgbLed.Color = FEZHAT.Color.Black;

            if (operationTimer.IsEnabled)
            {
                operationTimer.Stop();
            }

            operationTimer.Tick -= OperationTimer_Tick;
            operationTimer = null;

            shadeMotor.Dispose();
        }
        
        #endregion
    }
}
