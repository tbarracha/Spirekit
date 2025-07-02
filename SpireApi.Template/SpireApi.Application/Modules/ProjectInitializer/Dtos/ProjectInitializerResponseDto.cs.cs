using SpireCore.API.Operations.Dtos;

namespace SpireApi.Application.Modules.ProjectInitializer.Dtos;

public class ProjectInitializerResponseDto : FileDownloadResponseDto
{
    public required string ProjectType { get; set; }
}
