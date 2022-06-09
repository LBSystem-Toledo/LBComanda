using System.Threading.Tasks;

namespace LBComandaPrism.Services
{
    public interface IQrCodeScanningService
    {
        Task<string> ScanAsync();
    }
}
