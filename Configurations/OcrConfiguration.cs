namespace AOEOQuestEngine.WPFCoreLibrary.Configurations;
public class OcrConfiguration
{
    public static Rectangle TimerRegion { get; set; }
    public static Rectangle QuestStatusRegion { get; set; }
    public static Rectangle IsLoadedRegion { get; set; }
    public static string SuccessMessage { get; set; } = "COMPLETE";
    public static string FailureMessage { get; set; } = "FAILED";
    //says QUEST but if using the ocr library recommended, then shows this text that means the quest button showed up.
    public static string LoadIdentifier { get; set; } = "11158";
    public static void DoubleCheck()
    {
        //since someone may use a different system, a person is now on their own if they did not set the ocr path.
        // Checking if the Rectangle properties are set to their default value (i.e., Empty).
        if (TimerRegion == Rectangle.Empty)
        {
            throw new CustomBasicException("TimerRegion rectangle is not set.");
        }
        if (QuestStatusRegion == Rectangle.Empty)
        {
            throw new CustomBasicException("QuestStatusRegion rectangle is not set.");
        }
        if (IsLoadedRegion == Rectangle.Empty)
        {
            throw new CustomBasicException("IsLoadedRegion rectangle is not set.");
        }
    }
}