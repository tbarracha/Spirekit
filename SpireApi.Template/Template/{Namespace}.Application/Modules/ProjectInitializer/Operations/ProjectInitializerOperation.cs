
using {Namespace}.Application.Modules.ProjectInitializer.Domain.Models;
using {Namespace}.Application.Modules.ProjectInitializer.Services;
using {Namespace}.Contracts.Dtos.Modules.ProjectInitializer;
using SpireCore.API.Operations;

namespace {Namespace}.Application.Modules.ProjectInitializer.Operations;

public class ProjectInitializerOperation : IOperation<ProjectInitializerRequestDto, ProjectInitializerResponseDto>
{
    private IProjectInitializerService _projectInitializerService;

    public ProjectInitializerOperation(IProjectInitializerService projectInitializerService)
    {
        _projectInitializerService = projectInitializerService;
    }

    public async Task<ProjectInitializerResponseDto> ExecuteAsync(ProjectInitializerRequestDto request)
    {
        var projectRequest = ProjectInitializerMapper.MapDtoToDomain(request);
        var project = await _projectInitializerService.InitializeProjectAsync(projectRequest);
        var result = ProjectInitializerMapper.MapDomainToDto(project);

        return await Task.FromResult(result);
    }
}

