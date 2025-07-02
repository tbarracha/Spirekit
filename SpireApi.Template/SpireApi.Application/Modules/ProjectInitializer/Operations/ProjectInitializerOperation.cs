using Microsoft.AspNetCore.Mvc;
using SpireApi.Application.Modules.ProjectInitializer.Domain.Models;
using SpireApi.Application.Modules.ProjectInitializer.Dtos;
using SpireApi.Application.Modules.ProjectInitializer.Services;
using SpireCore.API.Operations;

namespace SpireApi.Application.Modules.ProjectInitializer.Operations;

[OperationProducesFile(OperationFileContentType.Custom)]
public class ProjectInitializerDownloadOperation : IOperation<ProjectInitializerRequestDto, IActionResult>
{
    private readonly IProjectInitializerService _projectInitializerService;

    public ProjectInitializerDownloadOperation(IProjectInitializerService projectInitializerService)
    {
        _projectInitializerService = projectInitializerService;
    }

    public async Task<IActionResult> ExecuteAsync(ProjectInitializerRequestDto request)
    {
        var projectRequest = ProjectInitializerMapper.MapDtoToDomain(request);
        var project = await _projectInitializerService.InitializeProjectAsync(projectRequest);

        var fileBytes = project.ZipFile; // byte[]
        var fileName = string.IsNullOrWhiteSpace(project.FileName) ? "Project.zip" : project.FileName;
        var contentType = "application/zip"; // You could use a property if you store it

        // This will make the browser download the file!
        return new FileContentResult(fileBytes, contentType)
        {
            FileDownloadName = fileName
        };
    }
}
