using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Web.Areas.Operations.Models;
using System.Web;

namespace SouthlandMetals.Web.Converters
{
    public class PartDrawingConverter
    {
        /// <summary>
        /// convert part drawing to view model
        /// </summary>
        /// <param name="drawing"></param>
        /// <returns></returns>
        public DrawingViewModel ConvertToView(PartDrawing drawing)
        {
            DrawingViewModel model = new DrawingViewModel();

            model.DrawingId = drawing.PartDrawingId;
            model.RevisionNumber = (!string.IsNullOrEmpty(drawing.RevisionNumber)) ? drawing.RevisionNumber : "N/A";
            model.LastDate = drawing.ModifiedDate;
            model.IsProject = false;
            model.Type = drawing.Type;
            model.Length = drawing.Length;
            model.Content = drawing.Content;
            model.IsActive = model.IsActive;

            return model;
        }

        /// <summary>
        /// convert part drawing to pdf
        /// </summary>
        /// <param name="drawing"></param>
        /// <returns></returns>
        public DrawingPdf ConvertToPdf(PartDrawing drawing)
        {
            DrawingPdf model = new DrawingPdf();

            model.Content = drawing.Content;
            model.Type = drawing.Type;
            model.RevisionNumber = drawing.RevisionNumber;

            return model;
        }

        /// <summary>
        /// convert project part drawing to part drawing
        /// </summary>
        /// <param name="projectPartDrawing"></param>
        /// <returns></returns>
        public PartDrawing ConvertToDomain(ProjectPartDrawing projectPartDrawing)
        {
            PartDrawing partDrawing = new PartDrawing();

            partDrawing.RevisionNumber = projectPartDrawing.RevisionNumber;
            partDrawing.Type = projectPartDrawing.Type;
            partDrawing.Length = projectPartDrawing.Length;
            partDrawing.Content = projectPartDrawing.Content;
            partDrawing.IsLatest = projectPartDrawing.IsLatest;
            partDrawing.IsMachined = projectPartDrawing.IsMachined;
            partDrawing.IsRaw = projectPartDrawing.IsRaw;

            return partDrawing;
        }

        /// <summary>
        /// convert posted drawing to part drawing
        /// </summary>
        /// <param name="drawing"></param>
        /// <returns></returns>
        public PartDrawing ConvertToDomain(HttpPostedFileBase drawing)
        {
            PartDrawing partDrawing = new PartDrawing();

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

            partDrawing.RevisionNumber = trimmedFileName;
            partDrawing.Type = drawing.ContentType;
            partDrawing.Length = drawing.ContentLength;
            partDrawing.Content = tempFile;

            return partDrawing;
        }
    }
}