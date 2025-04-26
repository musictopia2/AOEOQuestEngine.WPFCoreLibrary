namespace AOEOQuestEngine.WPFCoreLibrary.Services;
public class SpartanReadyAfterPopup(ISpartanReady ready) : IAfterCloseSimplePopup
{
    async void IAfterCloseSimplePopup.FinishProcess()
    {
        await ready.LoadQuestAsync();
    }
}