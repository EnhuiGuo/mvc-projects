using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Web.Areas.Operations.Models;
using System;

namespace SouthlandMetals.Web.Converters
{
    public class ProjectNoteConverter
    {
        public ProjectNoteViewModel ConvertToView(ProjectNote note)
        {
            ProjectNoteViewModel model = new ProjectNoteViewModel();

            model.ProjectNoteId = note.ProjectNoteId;
            model.ProjectId = note.ProjectId;
            model.Note = (!string.IsNullOrEmpty(note.Note)) ? note.Note : "N/A";
            model.CreatedDate = (note.CreatedDate != null) ? note.CreatedDate : DateTime.MinValue;
            model.CreatedBy = (!string.IsNullOrEmpty(note.CreatedBy)) ? note.CreatedBy : "N/A";

            return model;
        }

        public ProjectNote ConvertToDomain(ProjectNoteViewModel model)
        {
            ProjectNote note = new ProjectNote();

            note.ProjectId = model.ProjectId;
            note.Note = model.Note;
            note.CreatedDate = model.CreatedDate;
            note.CreatedBy = model.CreatedBy;

            return note;
        }
    }
}