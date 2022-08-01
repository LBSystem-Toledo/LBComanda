using Android.Bluetooth;
using Java.Util;
using LBComandaPrism.Droid;
using LBComandaPrism.Interface;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(AndroidBlueToothService))]
namespace LBComandaPrism.Droid
{
    public class AndroidBlueToothService : IBlueToothService
    {
        BluetoothAdapter bluetoothAdapter = BluetoothAdapter.DefaultAdapter;

        public string[] GetDeviceList()
        {
            var btdevice = bluetoothAdapter?.BondedDevices.Select(p => p.Name).ToArray();
            return btdevice;
        }
        public async Task Print(string deviceName, string Texto)
        {
            BluetoothDevice device = (from bd in bluetoothAdapter?.BondedDevices
                                      where bd?.Name == deviceName
                                      select bd).FirstOrDefault();
            try
            {
                await Task.Delay(1000);
                BluetoothSocket bluetoothSocket = device?.
                    CreateRfcommSocketToServiceRecord(
                    UUID.FromString("00001101-0000-1000-8000-00805f9b34fb"));
                bluetoothSocket?.Connect();
                byte[] buffer = Encoding.UTF8.GetBytes(Texto);
                bluetoothSocket?.OutputStream.Write(buffer, 0, buffer.Length);
                await Task.Delay(1000);
                bluetoothSocket.Close();
            }
            catch
            { }
        }
    }
}