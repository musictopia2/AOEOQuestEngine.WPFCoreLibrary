namespace AOEOQuestEngine.WPFCoreLibrary.Services;
public partial class SpartanQuestResultMonitor(
    ICaptureGrayScaleMask capture
    , IOcrProcessor ocr,
    ISpartanMonitor monitor,
    ISpartaQuestEnded end,
    IQuestResultPersistenceService possibleRecover
    ) : ISpartanQuestRequested
{
    async void ISpartanQuestRequested.Monitor()
    {
        CancellationToken token = monitor.RegisterWatcher(EnumSpartaExitStage.PlayingQuest);
        await RunCaptureLoopAsync(token);
    }
    private async Task RunCaptureLoopAsync(CancellationToken token)
    {
        EnumSpartaQuestResult result;
        while (!token.IsCancellationRequested)
        {
            result = await GetResultsAsync(token);
            if (result != EnumSpartaQuestResult.Ongoing)
            {
                string time = await GetTimeAsync(token);
                const int maxRetries = 20;
                int retries = 0;
                //try up to 20 times here.
                while (string.IsNullOrEmpty(time) && retries < maxRetries)
                {
                    await Task.Delay(1000, token); // wait 1 sec between retries
                    time = await GetTimeAsync(token);
                    retries++;
                }

                if (string.IsNullOrEmpty(time))
                {
                    // Could not get time after retries
                    // You can either:
                    // - Save with a placeholder time (e.g. "Unknown")
                    // - Log a warning or handle accordingly
                    time = "Unknown"; //makes the system crash but gives me hints
                }

                //put somewhere so i can recover later.
                QuestResultModel item = new()
                {
                    Result = result,
                    Time = time
                };
                //has to figure out how to save the results.
                await possibleRecover.SaveAsync(item);
                await end.EndQuestAsync(result, time);
                return;

            }
            try
            {
                await Task.Delay(5000, token); // optional: cancels delay if token is canceled
            }
            catch (TaskCanceledException)
            {
                return;
            }
        }
    }
    private async Task<string> GetTimeAsync(CancellationToken token)
    {
        // Offload capture and OCR to a background thread
        return await Task.Run(async () =>
        {
            Rectangle bounds = OcrConfiguration.TimerRegion;
            using Bitmap bitmap = capture.CaptureMaskedBitmap(bounds);
            string text = await ocr.GetTextAsync(bitmap, token);
            var timeMatch = TimeMatch().Match(text);
            if (timeMatch.Success)
            {
                return timeMatch.Value;
            }
            return "";
        }, token);
    }
    private async Task<EnumSpartaQuestResult> GetResultsAsync(CancellationToken token)
    {
        // Offload capture and OCR to a background thread
        return await Task.Run(async () =>
        {
            Rectangle bounds = OcrConfiguration.QuestStatusRegion;
            using Bitmap bitmap = capture.CaptureMaskedBitmap(bounds);
            string text = await ocr.GetTextAsync(bitmap, token);

            if (text.Contains(OcrConfiguration.SuccessMessage))
            {
                return EnumSpartaQuestResult.Completed;
            }
            if (text.Contains(OcrConfiguration.FailureMessage))
            {
                return EnumSpartaQuestResult.Failed;
            }
            return EnumSpartaQuestResult.Ongoing;
        }, token);
    }
    [GeneratedRegex(@"\b\d{2}:\d{2}:\d{2}\b")]
    private static partial Regex TimeMatch();
}