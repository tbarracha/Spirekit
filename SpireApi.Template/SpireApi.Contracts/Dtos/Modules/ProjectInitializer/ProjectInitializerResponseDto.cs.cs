namespace SpireApi.Contracts.Dtos.Modules.ProjectInitializer;

public class ProjectInitializerResponseDto
{
    public required string ProjectType { get; set; }
    public required string FileName { get; set; }
    public required byte[] ZipFile { get; set; }
}
