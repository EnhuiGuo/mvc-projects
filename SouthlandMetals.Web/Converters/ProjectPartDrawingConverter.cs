using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Web.Areas.Operations.Models;
using System.Web;

namespace SouthlandMetals.Web.Converters
{
    public class ProjectPartDrawingConverter
    {
        /// <summary>
        /// convert project part drawing to view model
        /// </summary>
        /// <param name="drawing"></param>
        /// <returns></returns>
        public DrawingViewModel ConvertToView(ProjectPartDrawing drawing)
        {
            DrawingViewModel model = new DrawingViewModel();

            model.DrawingId = drawing.ProjectPartDrawingId;
            model.RevisionNumber = (!string.IsNullOrEmpty(drawing.RevisionNumber)) ? drawing.RevisionNumber : "N/A";
            model.LastDate = drawing.ModifiedDate;
            model.IsProject = true;

            return model;
        }

        /// <summary>
        /// convert part drawing to project part drawing
        /// </summary>
        /// <param name="drawing"></param>
        /// <returns></returns>
        public ProjectPartDrawing ConvertToDomain(PartDrawing drawing)
        {
            ProjectPartDrawing projectPartDrawing = new ProjectPartDrawing();

            projectPartDrawing.RevisionNumber = drawing.RevisionNumber;
            projectPartDrawing.Type = drawing.Type;
            projectPartDrawing.Length = drawing.Length;
            projectPartDrawing.Content = drawing.Content;

            return projectPartDrawing;
        }

        /// <summary>
        /// convert posted drawing to project part drawing
        /// </summary>
        /// <param name="drawing"></param>
        /// <returns></returns>
        public ProjectPartDrawing ConvertToDomain(HttpPostedFileBase drawing)
        {
            ProjectPartDrawing projectPartDrawing = new ProjectPartDrawing();

            byte[] tempFile = new byte[drawing.ContentLength];

            var trimmedFileName = string.Empty;

            if (drawing.FileName.EndsWith("png"))
            {
                trimmedFileName = drawing.FileName.Replace(".png", "");
            }
            else if (drawing.FileName.EndsWith("jpg"))
            {
                trimmedFileName = drawing.FileName.Replace(".jpg", "");
            }
            else if (drawing.FileName.EndsWith("pdf"))
            {
                trimmedFileName = drawing.FileName.Replace(".pdf", "");
            }

            projectPartDrawing.RevisionNumber = trimmedFileName;
            projectPartDrawing.Type = drawing.ContentType;
            projectPartDrawing.Length = drawing.ContentLength;
            projectPartDrawing.Content = tempFile;

            return projectPartDrawing;
        }

        /// <summary>
        /// convert project part drawing to pdf
        /// </summary>
        /// <param name="drawing"></param>
        /// <returns></returns>
        public DrawingPdf ConvertToPdf(ProjectPartDrawing drawing)
        {
            DrawingPdf model = new DrawingPdf();

            model.Content = drawing.Content;
            model.Type = drawing.Type;
            model.RevisionNumber = drawing.RevisionNumber;

            return model;
        }
    }
}