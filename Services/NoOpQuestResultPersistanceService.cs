namespace AOEOQuestEngine.WPFCoreLibrary.Services;
public class NoOpQuestResultPersistanceService : IQuestResultPersistenceService
{
    Task IQuestResultPersistenceService.ClearPendingAsync()
    {
        return Task.CompletedTask;
    }

    Task<QuestResultModel?> IQuestResultPersistenceService.LoadPendingAsync()
    {
        return Task.FromResult<QuestResultModel?>(null);
    }

    Task IQuestResultPersistenceService.SaveAsync(QuestResultModel result)
    {
        return Task.CompletedTask;
    }
}