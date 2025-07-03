using SpireApi.Application.Modules.ProjectInitializer.Domain.Models;

namespace SpireApi.Application.Modules.ProjectInitializer.Domain.Services;

public interface IProjectInitializerService
{
    Task<InitializedProject> InitializeProjectAsync(ProjectInitializerRequest request);
}
