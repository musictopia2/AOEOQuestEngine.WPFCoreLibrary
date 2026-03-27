namespace AOEOQuestEngine.WPFCoreLibrary.Services;
/// <summary>
/// Handles Spartan exit by treating it as a full quest failure.
/// </summary>
/// <remarks>
/// This handler is invoked when Spartan closes unexpectedly during an active quest,
/// and the system requires the exit to be processed as a real failed quest.
///
/// Behavior:
/// - Displays a short popup message indicating at which stage Spartan exited.
/// - Delegates to <see cref="ISpartaQuestEnded"/> using a failed result,
///   ensuring the standard quest failure pipeline is executed.
///
/// The failure pipeline typically includes:
/// - Stopping the active quest (via <see cref="QuestRunContainer"/>).
/// - Updating replay/session state (e.g., marking the quest as failed).
/// - Clearing any pending persistence state.
/// - Evaluating civilization changes.
/// - Invoking <see cref="QuestMonitoringEndingContainer.OnQuestFailed"/> to update the UI.
///
/// Notes:
/// - No completion time is available when Spartan exits early; a default or empty value
///   may be passed to the failure pipeline.
/// - This ensures consistent behavior between monitored failures and manual exits,
///   avoiding partial or inconsistent application state.
///
/// Use this handler in scenarios where quest progression, rewards, or UI state depend on
/// the full failure logic being executed (e.g., replay, progression systems).
/// </remarks>
public class QuestFailureSpartanExitHandler(ISpartaQuestEnded ends) : ISpartanExitHandler
{
    async Task ISpartanExitHandler.ExitSpartanAsync(EnumSpartaExitStage stage)
    {
        await ends.EndQuestAsync(EnumSpartaQuestResult.Failed, "");
    }
}