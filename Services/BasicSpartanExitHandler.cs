namespace AOEOQuestEngine.WPFCoreLibrary.Services;
/// </summary>
/// <remarks>
/// This handler is invoked by the monitoring system if Spartan closes while a quest is marked as active.
/// It performs minimal cleanup and notification only.
///
/// Behavior:
/// - Stops the internal quest tracking state (via <see cref="QuestRunContainer"/>).
/// - Displays a short popup message indicating at which stage Spartan exited.
/// - Invokes <see cref="QuestMonitoringEndingContainer.OnQuestFailed"/> if provided,
///   allowing the UI layer to react (e.g., reload screen or show a toast).
/// - If no failure callback is provided, the application exits via <see cref="IExit"/>.
///
/// Important:
/// This handler does NOT execute the full quest failure pipeline (e.g., persistence cleanup,
/// civilization evaluation, or replay state updates). It is intended for lightweight scenarios
/// such as balancing or testing where full failure processing is not required.
///
/// This class does not make assumptions about the UI layer — it only provides minimal cleanup
/// and optional notification when Spartan exits mid-run.
/// </remarks>
public class BasicSpartanExitHandler(QuestMonitoringEndingContainer endingContainer,
    IOpenTimedPopup pop,
    QuestRunContainer runContainer,
    IExit exit
    ) : ISpartanExitHandler
{
    async Task ISpartanExitHandler.ExitSpartanAsync(EnumSpartaExitStage stage)
    {
        if (runContainer.IsPlaying == false)
        {
            // Early exit if no quest is currently playing
            return;
        }
        runContainer.StopPlaying(); // Stop any ongoing quest
        // Generate a specific message based on the exit stage
        string message = stage switch
        {
            EnumSpartaExitStage.Open => "Quest was initialized, but no actions were taken.",
            EnumSpartaExitStage.AutoClicking => "Auto-clicking was in progress but was interrupted.",
            EnumSpartaExitStage.PlayingQuest => "The quest was in progress when the exit occurred.",
            EnumSpartaExitStage.Ending => "The quest was in the ending phase when it was interrupted.",
            _ => $"Spartan exited at an unknown stage: {stage}", // Default case for any unrecognized stage
        };

        // Display the message using the popup
        await pop.OpenPopupAsync(message, 500);

        // Handle the quest failure case if no custom action is provided
        if (endingContainer.OnQuestFailed is null)
        {
            exit.ExitApp();  // Exit the app if no custom failure logic is defined
            return;
        }

        // Invoke the custom quest failure handler if specified
        endingContainer.OnQuestFailed.Invoke();
    }
}