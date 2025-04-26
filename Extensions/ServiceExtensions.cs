namespace AOEOQuestEngine.WPFCoreLibrary.Extensions;
public static class ServiceExtensions
{
    /// <summary>
    /// Registers all necessary services for running the AOEO Quest Engine with OCR and autoclicking support.
    /// </summary>
    /// <typeparam name="I">The implementation of <see cref="ICaptureGrayScaleMask"/> used to capture screen regions.</typeparam>
    /// <typeparam name="O">The implementation of <see cref="IOcrProcessor"/> used to extract text from screen captures.</typeparam>
    /// <typeparam name="P">The implementation of <see cref="IClickLocationProvider"/> used to define click positions on the UI.</typeparam>
    /// <param name="services">The service collection to register the dependencies with.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> for chaining.</returns>
    /// <remarks>
    /// This method should be used when the full OCR-driven automation flow is required, including:
    /// launching, detecting readiness via OCR, automatic clicking, and quest result monitoring.
    ///
    /// It is intended for Windows-specific environments using WPF, where screen interaction is required.
    /// </remarks>

    public static IServiceCollection RegisterQuestEngineWindowsOcrAutoclicking<I, O, P>(this IServiceCollection services)
        where I : class, ICaptureGrayScaleMask
        where O : class, IOcrProcessor
        where P : class, IClickLocationProvider
    {
        services.AddSingleton<ICaptureGrayScaleMask, I>()
            .AddSingleton<IOcrProcessor, O>()
            .RegisterAutoClickServices<P>()
            .RegisterSpartanMonitoring()
            .AddSingleton<ISpartanLaunchHandler, OcrDetectionSpartanLaunchHandler>()
            .AddSingleton<ISpartanQuestRequested, SpartanQuestResultMonitor>()
            .AddSingleton<ISpartanReady, QuestAutoClicker>()
            .RegisterToastQuestEndingServices()
            ;
        return services;
    }
    /// <summary>
    /// Registers the services needed for quest automation via autoclicking only (without OCR).
    /// </summary>
    /// <typeparam name="P">The implementation of <see cref="IClickLocationProvider"/> used to define click positions on the UI.</typeparam>
    /// <param name="services">The service collection to register the dependencies with.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> for chaining.</returns>
    /// <remarks>
    /// This setup is for scenarios where the user manually triggers readiness (e.g., via popup) instead of OCR detection.
    /// Useful when you want basic automation (like autoclicking) but prefer manual input over OCR.
    ///
    /// It includes popup registration, click container management, and quest monitoring stub logic.
    /// </remarks>
    public static IServiceCollection RegisterQuestEngineWindowsAutoclickingOnly<P>(this IServiceCollection services)
        where P : class, IClickLocationProvider
    {
        services.RegisterAutoClickServices<P>()
            .AddSingleton<ISpartanLaunchHandler, SpartanLaunchPopupHandler>()
            .AddSingleton<IAfterCloseSimplePopup, QuestAutoClicker>()
            .AddSingleton<ISpartanQuestRequested, DoNothingQuestMonitor>()
            .RegisterSpartanMonitoring()
            ;
        return services;
    }
    /// <summary>
    /// Registers services required for supporting autoclicking functionality.
    /// </summary>
    /// <typeparam name="P">The implementation of <see cref="IClickLocationProvider"/> for click location setup.</typeparam>
    /// <param name="services">The service collection to register the dependencies with.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> for chaining.</returns>
    /// <remarks>
    /// This method includes the click location container, popup UI support, and the specified click location provider.
    /// It is used by both OCR and non-OCR based automation setups.
    /// </remarks>
    private static IServiceCollection RegisterAutoClickServices<P>(this IServiceCollection services)
        where P : class, IClickLocationProvider
    {
        services.AddSingleton<ClickLocationContainer>()
            .RegisterDefaultPopups()
            .AddSingleton<IClickLocationProvider, P>();
        return services;
    }
    /// <summary>
    /// Registers the default quest ending handler that uses toast messages to notify the user.
    /// </summary>
    /// <param name="services">The service collection to register the dependency with.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> for chaining.</returns>
    /// <remarks>
    /// This handler shows a simple toast popup when a quest ends, either successfully or by failure.
    /// </remarks>
    private static IServiceCollection RegisterToastQuestEndingServices(this IServiceCollection services)
    {
        services.AddSingleton<ISpartaQuestEnded, ToastQuestEnder>();
        return services;
    }
    /// <summary>
    /// Registers default popup implementations for both timed and interactive popups.
    /// </summary>
    /// <param name="services">The service collection to register the dependencies with.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> for chaining.</returns>
    /// <remarks>
    /// These popups are used for prompting the user and providing timed feedback in WPF UI.
    /// </remarks>
    private static IServiceCollection RegisterDefaultPopups(this IServiceCollection services)
    {
        services.AddSingleton<IOpenTimedPopup, TimerWPFPopupClass>()
            .AddSingleton<IOpenSimplePopup, SimpleWPFPopupClass>();
        return services;
    }
    /// <summary>
    /// Registers the monitoring system for Spartan's runtime status and its associated exit handling services.
    /// </summary>
    /// <param name="services">The service collection to register the dependencies with.</param>
    /// <returns>The updated <see cref="IServiceCollection"/> for chaining.</returns>
    /// <remarks>
    /// This system watches for the Spartan application while a quest is active. If Spartan exits unexpectedly:
    /// - It automatically invokes utility logic to cleanly shut down the quest state.
    /// - If a custom action is provided (via <see cref="QuestMonitoringEndingContainer.OnQuestFailed"/>),
    ///   that action will be executed instead of exiting the application — typically used to update the UI
    ///   or inform the user through a toast or message.
    ///
    /// This system does not assume how the UI is built or how it should respond — it only exposes the
    /// extension point to allow the app to respond accordingly.
    /// </remarks>
    private static IServiceCollection RegisterSpartanMonitoring(this IServiceCollection services)
    {
        services.AddSingleton<ISpartanMonitor, SpartanMonitorService>()
            .AddSingleton<ISpartanUtilities, SpartanUtilities>()
            .AddSingleton<ISpartanExitHandler, SpartanExitHandler>();
        return services;
    }
}