namespace AOEOQuestEngine.WPFCoreLibrary.Services;
/// <summary>
/// Spartan launch handler that uses OCR to detect when a known UI element appears on the screen.
/// This is specifically designed for the Windows environment, where OCR and screen capture are possible.
/// 
/// **Note:** This functionality is part of the WPF+Blazor integration, but still leverages WPF APIs for graphical manipulation,
/// such as using <see cref="System.Drawing.Bitmap"/> and <see cref="System.Drawing.Rectangle"/> for capturing regions of the screen.
/// 
/// The handler does NOT perform actions like autoclicking but only detects when the system is ready to proceed.
/// </summary>
public class OcrDetectionSpartanLaunchHandler(ICaptureGrayScaleMask capture
    , IOcrProcessor ocr,
    ISpartanMonitor monitor,
    ISpartanReady ready
    ) : ISpartanLaunchHandler
{
    async void ISpartanLaunchHandler.OnSpartanLaunched()
    {
        OcrConfiguration.DoubleCheck();
        CancellationToken checkToken = monitor.RegisterWatcher(EnumSpartaExitStage.Open);
        await RunCaptureLoopAsync(checkToken);
    }
    private async Task RunCaptureLoopAsync(CancellationToken token)
    {

        while (!token.IsCancellationRequested)
        {
            if (await IsLoadedAsync(token))
            {
                break;
            }

            await Task.Delay(5000, token); // optional: cancels delay if token is canceled
        }
        if (token.IsCancellationRequested)
        {
            return;
        }
        await ready.LoadQuestAsync();
    }
    private async Task<bool> IsLoadedAsync(CancellationToken token)
    {
        // Offload capture and OCR to a background thread
        return await Task.Run(async () =>
        {
            Rectangle bounds = OcrConfiguration.IsLoadedRegion;
            using Bitmap bitmap = capture.CaptureMaskedBitmap(bounds);
            string text = await ocr.GetTextAsync(bitmap, token);
            return text.Contains(OcrConfiguration.LoadIdentifier);
        }, token);
    }
}