namespace AOEOQuestEngine.WPFCoreLibrary.Services;
/// <summary>
/// Handles the launch of Spartan quests using a popup instead of OCR.
/// This class listens for a user action (key press) to trigger the next step in the process.
/// 
/// It is used when the user wants to manually acknowledge that the Spartan quest is ready to proceed,
/// instead of automatically detecting the UI element via OCR. This method relies on the user interacting with
/// a popup and pressing a specific key to resume the processing of the quest.
/// </summary>
public class SpartanLaunchPopupHandler(IOpenSimplePopup pop,
    ISpartanMonitor monitor
    ) : ISpartanLaunchHandler
{
    private CancellationToken _cts;
    void ISpartanLaunchHandler.OnSpartanLaunched()
    {
        if (_cts.CanBeCanceled)
        {
            monitor.StopWatching(); // clean up existing if any
        }
        _cts = monitor.RegisterWatcher(EnumSpartaExitStage.Open);
        pop.OpenPopup(SpartanPopupConfiguration.Key, SpartanPopupConfiguration.Message, _cts, Closed);
    }
    void Closed()
    {
        //this means the popup was closed.
        monitor.StopWatching();
    }
}