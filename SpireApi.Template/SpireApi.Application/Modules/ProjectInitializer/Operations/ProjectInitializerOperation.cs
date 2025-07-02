
using SpireApi.Application.Modules.ProjectInitializer.Domain.Models;
using SpireApi.Application.Modules.ProjectInitializer.Services;
using SpireApi.Contracts.Dtos.Modules.ProjectInitializer;
using SpireCore.API.Operations;

namespace SpireApi.Application.Modules.ProjectInitializer.Operations;

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
