namespace AOEOQuestEngine.WPFCoreLibrary.Services;
/// <summary>
/// Handles the shutdown process when Spartan unexpectedly exits during a quest.
/// </summary>
/// <remarks>
/// This handler is invoked by the monitoring system if Spartan closes while a quest is marked as active.
/// It performs the following steps:
/// - Stops the internal quest tracking state (via <see cref="QuestRunContainer"/>).
/// - Shows a short popup message explaining at which stage Spartan exited.
/// - If a custom fallback is provided in <see cref="QuestMonitoringEndingContainer.OnQuestFailed"/>, 
///   that action will be executed — commonly to update the UI or show a toast.
/// - If no custom action is provided, the application exits gracefully via <see cref="IExit"/>.
///
/// This class does not make assumptions about the UI layer — it only provides the mechanism to
/// clean up and optionally notify higher-level systems when Spartan exits mid-run.
/// </remarks>
public class SpartanExitHandler(QuestMonitoringEndingContainer endingContainer,
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