using Core.DTOs;

namespace Logic.IServices
{
    public interface IProjectService
    {
        Task<Core.ViewModels.HeplerResponseVM> CreateProjectAsync(ProjectCreateDto dto);
        List<ProjectDto> GetAllProjects();
        Task<ProjectDto?> GetProjectByIdAsync(string id);
        Task<ProjectDetailsDto?> GetProjectDetailsAsync(string id);
        Task<Core.ViewModels.HeplerResponseVM> UpdateProjectAsync(string id, ProjectCreateDto dto);
        Task<Core.ViewModels.HeplerResponseVM> DeleteProjectAsync(string id);
        Task<Core.ViewModels.HeplerResponseVM> CreateBuildingDesignAsync(string projectId, BuildingDesignCreateDto dto);
        Task<BuildingDesignDto?> GetBuildingDesignByIdAsync(string id);
        List<BuildingDesignDto> GetBuildingDesignsByProjectId(string projectId);
        Task<Core.ViewModels.HeplerResponseVM> UpdateBuildingDesignAsync(string id, BuildingDesignCreateDto dto);
        Task<Core.ViewModels.HeplerResponseVM> DeleteBuildingDesignAsync(string id);
    }
}
