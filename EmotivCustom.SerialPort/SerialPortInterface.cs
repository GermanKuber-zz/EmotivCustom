using System;
using System.IO.Ports;
namespace EmotivCustom.SerialPortDriver
{
  public  class SerialPortInterface
    {
        private readonly SerialPort _port;

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
    }
}
