using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Web.Areas.Operations.Models;
using System.Web;

namespace SouthlandMetals.Web.Converters
{
    public class ProjectPartLayoutConverter
    {
        /// <summary>
        /// convert project part layout to view model
        /// </summary>
        /// <param name="layout"></param>
        /// <returns></returns>
        public LayoutViewModel ConvertToView(ProjectPartLayout layout)
        {
            LayoutViewModel model = new LayoutViewModel();

            model.LayoutId = layout.ProjectPartLayoutId;
            model.Description = (!string.IsNullOrEmpty(layout.Description)) ? layout.Description : "N/A";
            model.LastDate = layout.ModifiedDate;
            model.IsProject = true;

            return model;
        }

        /// <summary>
        /// convert project part layout to pdf
        /// </summary>
        /// <param name="layout"></param>
        /// <returns></returns>
        public LayoutPdf ConvertToPdf(ProjectPartLayout layout)
        {
            LayoutPdf model = new LayoutPdf();

            model.Content = layout.Content;
            model.Type = layout.Type;
            model.Description = layout.Description;

            return model;
        }

        /// <summary>
        /// convert posted layout to project part layout
        /// </summary>
        /// <param name="layout"></param>
        /// <returns></returns>
        public ProjectPartLayout ConvertToDomain(HttpPostedFileBase layout)
        {
            ProjectPartLayout projectPartLayout = new ProjectPartLayout();

            byte[] tempFile = new byte[layout.ContentLength];

            var trimmedFileName = string.Empty;

            if (layout.FileName.EndsWith("png"))
            {
                trimmedFileName = layout.FileName.Replace(".png", "");
            }
            else if (layout.FileName.EndsWith("jpg"))
            {
                trimmedFileName = layout.FileName.Replace(".jpg", "");
            }
            else if (layout.FileName.EndsWith("pdf"))
            {
                trimmedFileName = layout.FileName.Replace(".pdf", "");
            }

            projectPartLayout.Description = trimmedFileName;
            projectPartLayout.Type = layout.ContentType;
            projectPartLayout.Length = layout.ContentLength;
            projectPartLayout.Content = tempFile;

            return projectPartLayout;
        }
    }
}