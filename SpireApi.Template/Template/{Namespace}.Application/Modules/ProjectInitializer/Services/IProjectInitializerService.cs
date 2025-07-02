
using {Namespace}.Application.Modules.ProjectInitializer.Domain.Models;

namespace {Namespace}.Application.Modules.ProjectInitializer.Services;

public interface IProjectInitializerService
{
    Task<InitializedProject> InitializeProjectAsync(ProjectInitializerRequest request);
}

