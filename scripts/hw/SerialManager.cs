using Godot;
using System.IO.Ports;
using System.Linq;

public partial class SerialManager : Node
{
    private SerialPort _serial;

    public override void _Ready()
    {
        string[] ports = SerialPort.GetPortNames();
        
        if (ports.Length > 0)
        {
            // Automatically select the first available port
            string portName = ports[0];
            _serial = new SerialPort(portName, 115200);
            
            try 
            {
                _serial.Open();
                GD.Print($"SerialManager: Successfully connected to {portName}");
            }
            catch (System.Exception e)
            {
                GD.PrintErr($"SerialManager: Could not open {portName}. Error: {e.Message}");
            }
        }
        else
        {
            GD.PrintErr("SerialManager: No serial ports detected. Check USB hardware connection.");
        }
    }

    public override void _Process(double delta)
    {
        if (_serial != null && _serial.IsOpen && _serial.BytesToRead > 0)
        {
            try
            {
                string data = _serial.ReadLine();
                if (data.StartsWith("S1:"))
                {
                    float val = float.Parse(data.Split(':')[1]);
                    // Direct communication with your normalized signal pipeline
                    float clean = GetNode<SensorCalibrator>("/root/SensorCalibrator").ProcessSignal(val);
                    
                    // Trigger data flow to the Mirror
                    // SomaticStateMirror.Update(clean);
                }
            }
            catch (System.Exception e)
            {
                GD.PrintErr($"SerialManager: Read error: {e.Message}");
            }
        }
    }
}
