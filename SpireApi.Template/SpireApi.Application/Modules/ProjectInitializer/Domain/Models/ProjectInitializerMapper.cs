using SpireApi.Contracts.Dtos.Modules.ProjectInitializer;

namespace SpireApi.Application.Modules.ProjectInitializer.Domain.Models;

public static class ProjectInitializerMapper
{
    public static ProjectInitializerRequest MapDtoToDomain(ProjectInitializerRequestDto dto)
    {
        return new ProjectInitializerRequest
        {
            Namespace = dto.Namespace,
            ProjectType = dto.ProjectType,
            Modules = dto.Modules.Select(m => new ModuleSelection
            {
                Name = m.Name,
                Enabled = m.Enabled
            }).ToList()
        };
    }

    public static ProjectInitializerResponseDto MapDomainToDto(InitializedProject model)
    {
        return new ProjectInitializerResponseDto
        {
            ProjectType = model.ProjectType,
            FileName = model.FileName,
            ZipFile = model.ZipFile
        };
    }
}
