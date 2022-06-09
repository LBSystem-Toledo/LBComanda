using LBComandaPrism.Services;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing.Mobile;

[assembly: Dependency(typeof(LBComandaPrism.Droid.QrCodeScanningService))]
namespace LBComandaPrism.Droid
{
    public class QrCodeScanningService : IQrCodeScanningService
    {
        public async Task<string> ScanAsync()
        {
            var optionsCustom = new MobileBarcodeScanningOptions()
            {
                //UseFrontCameraIfAvailable = true
            };
            var scanner = new MobileBarcodeScanner()
            {
                TopText = "Aproxime a câmera do QR Code",
                BottomText = "Toque na tela para focar"
            };

            var scanResults = await scanner.Scan(optionsCustom);

            return (scanResults != null) ? scanResults.Text : string.Empty;
        }
    }
}