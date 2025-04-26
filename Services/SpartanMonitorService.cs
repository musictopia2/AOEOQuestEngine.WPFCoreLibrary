namespace AOEOQuestEngine.WPFCoreLibrary.Services;
public class SpartanMonitorService(ISpartanExitHandler exit
    , QuestRunContainer statusContainer,
    ISpartanUtilities spartanUtilities
    ) : ISpartanMonitor, IDisposable
{
    private CancellationTokenSource? _cts; // Holds the token source to control cancellation
    protected Task? _watchTask; // Represents the background task that monitors Spartan
    /// <summary>
    /// Registers a new watcher to monitor Spartan's exit stage.
    /// </summary>
    /// <param name="stage">The stage at which to monitor Spartan's exit (e.g., when Spartan is finished).</param>
    /// <returns>A cancellation token that can be used to stop the monitoring.</returns>
    CancellationToken ISpartanMonitor.RegisterWatcher(EnumSpartaExitStage stage)
    {
        // Cancel and dispose of the previous monitoring if any
        _cts?.Cancel();
        _cts?.Dispose();

        // Create a new cancellation token source for the new monitoring task
        _cts = new CancellationTokenSource();

        // Start the monitoring task to check Spartan's status
        _watchTask = MonitorAsync(stage, _cts.Token);
        return _cts.Token;
    }
    /// <summary>
    /// The main asynchronous method that continuously checks the status of Spartan.
    /// </summary>
    /// <param name="stage">The stage at which to exit Spartan (e.g., end of quest).</param>
    /// <param name="token">The cancellation token to monitor the cancellation of the task.</param>
    /// <returns>An asynchronous task that monitors Spartan until it's cancelled or finished.</returns>
    private async Task MonitorAsync(EnumSpartaExitStage stage, CancellationToken token)
    {
        // Keep monitoring Spartan as long as the task is not cancelled
        while (!token.IsCancellationRequested)
        {
            // If the quest is not playing, stop monitoring (someone else is in charge of exiting Spartan)
            if (statusContainer.IsPlaying == false)
            {
                StopWatching(); // Stop the monitoring process
                break;
            }

            // If Spartan is no longer running, exit the Spartan process
            if (spartanUtilities.IsSpartanRunning() == false)
            {
                StopWatching(); // Stop monitoring
                await exit.ExitSpartanAsync(stage); // Trigger the exit action
                break;
            }

            // Delay before checking again (to prevent excessive polling)
            await Task.Delay(1000, token);
        }
    }
    /// <summary>
    /// Stops the monitoring task and cancels any ongoing operations.
    /// </summary>
    private void StopWatching()
    {
        _cts?.Cancel(); // Cancel the cancellation token
        _watchTask = null; // Clear the watch task (no longer running)
    }
    /// <summary>
    /// Stops the monitoring process (public method from ISpartanMonitor interface).
    /// </summary>
    void ISpartanMonitor.StopWatching()
    {
        StopWatching(); // Delegate to the private method to clean up
    }
    /// <summary>
    /// Dispose of the resources, canceling any ongoing tasks and cleaning up.
    /// </summary>
    public void Dispose()
    {
        // Cancel and dispose the token source to release resources
        _cts?.Cancel();
        _cts?.Dispose();

        // Suppress the finalization to prevent GC from doing it again
        GC.SuppressFinalize(this);
    }
}