namespace AOEOQuestEngine.WPFCoreLibrary.Services;
/// <summary>
/// Handles the automatic clicking process for quests after determining the method chosen (OCR, popup, or direct).
/// - For OCR: Waits for screen elements (via OCR) to appear and clicks the necessary locations.
/// - For Popup: Waits for the popup to close, then continues the quest process.
/// - For Direct: If no locations are specified, starts the quest directly without waiting for other events.
/// </summary>
public class QuestAutoClicker(ClickLocationContainer locationContainer
    , ISpartanMonitor monitor,
    IWindowStateManager stateManager,
    ISpartanQuestRequested quest
    ) : ISpartanReady, IAfterCloseSimplePopup
{
    async void IAfterCloseSimplePopup.FinishProcess()
    {
        await LaunchAsync();
    }
    private async Task LaunchAsync()
    {
        if (locationContainer.Locations.Count == 0)
        {
            StartQuest();
            return; //just do nothing
        }
        CancellationToken token = monitor.RegisterWatcher(EnumSpartaExitStage.AutoClicking);
        await aa1.ClickSeveralLocationsAsync(locationContainer.Locations, 500, token);
        stateManager.MinimizeWindow();
        if (token.IsCancellationRequested)
        {
            //the monitor already handled this.
            return;
        }
        monitor.StopWatching();
        StartQuest();
    }
    async Task ISpartanReady.LoadQuestAsync()
    {
        await LaunchAsync();
    }
    private void StartQuest()
    {
        stateManager.MinimizeWindow();
        quest.Monitor();
    }
}