using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Interfaces
{
    public interface IProjectRepository : IDisposable
    {
        List<SelectListItem> GetSelectablePartProjects(Guid partId);

        List<SelectListItem> GetSelectableProjects();

        List<SelectListItem> GetSelectableActiveProjects();

        List<Project> GetProjects();

        List<ProjectNote> GetProjectNotes(Guid projectId);

        Project GetProject(Guid projectId);

        Project GetProject(Guid? projectId);

        Project GetProject(string projectName);

        ProjectNote GetProjectNote(Guid projectNoteId);

        OperationResult SaveProject(Project newProject);

        OperationResult SaveProjectNote(ProjectNote newProjectNote);

        OperationResult UpdateProject(Project project);

        OperationResult DeleteProject(Guid projectId);

        OperationResult DeleteProjectNote(Guid projectNoteId);
    }
}
