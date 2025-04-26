namespace AOEOQuestEngine.WPFCoreLibrary.Services;
public class ToastQuestEnder(IOpenTimedPopup pop, IExit exit, ISpartanUtilities spartanUtilities) : ISpartaQuestEnded
{
    async void ISpartaQuestEnded.EndQuest(EnumSpartaQuestResult result, string time)
    {
        string message;
        if (result == EnumSpartaQuestResult.Completed)
        {
            message = $"Quest completed in {time}";
        }
        else if (result == EnumSpartaQuestResult.Failed)
        {
            message = $"Quest failed in {time}";
        }
        else
        {
            throw new CustomBasicException("Invalid result");
        }
        await pop.OpenPopupAsync(message, 2000);
        spartanUtilities.ExitSpartan();
        exit.ExitApp();
    }
}