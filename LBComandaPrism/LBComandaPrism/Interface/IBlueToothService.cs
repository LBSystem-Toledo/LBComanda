using System.Collections.Generic;
using System.Threading.Tasks;

namespace LBComandaPrism.Interface
{
    public interface IBlueToothService
    {
        string[] GetDeviceList();
        Task Print(string deviceName, string texto);
    }
}
