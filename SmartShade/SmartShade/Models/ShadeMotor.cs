namespace SmartShade.Models
{
    public class ShadeMotor
    {
        public string WindowLocation { get; set; }

        public double MotorSpeed { get; set; }

        public bool IsServo { get; set; }

        public double ServoPosition { get; set; }
    }
}
