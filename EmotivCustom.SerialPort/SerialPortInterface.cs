using System;
using System.IO.Ports;
namespace EmotivCustom.SerialPortDriver
{
  public  class SerialPortInterface
    {
        private readonly SerialPort _port;
        private const int MAX_SIGNAL_ANALOG = 230;
        public SerialPortInterface(string serialPort)
        {
            _port = new SerialPort(serialPort);
            _port.BaudRate = 115200;
            _port.Open();
        }
        private  void Write(string message)
        {
            _port.Write(message);
        }

        public void Write(int pint, int force)
        {
            _port.Write($"C=W,P={pint},V={force}|");
        }
        public void Write(int pint, float power)
        {
            var powerAdd = MAX_SIGNAL_ANALOG * power;
            _port.Write($"C=W,P={pint},V={powerAdd}|");
        }
    }
}
