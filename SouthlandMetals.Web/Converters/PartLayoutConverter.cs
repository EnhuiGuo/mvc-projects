using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Web.Areas.Operations.Models;
using System.Web;

namespace SouthlandMetals.Web.Converters
{
    public class PartLayoutConverter
    {
        /// <summary>
        /// convert part layout to view model
        /// </summary>
        /// <param name="layout"></param>
        /// <returns></returns>
        public LayoutViewModel ConvertToView(PartLayout layout)
        {
            LayoutViewModel model = new LayoutViewModel();

            model.LayoutId = layout.PartLayoutId;
            model.Description = (!string.IsNullOrEmpty(layout.Description)) ? layout.Description : "N/A";
            model.LastDate = layout.ModifiedDate;
            model.IsProject = false;

            return model;
        }

        /// <summary>
        /// convert part layout to pdf
        /// </summary>
        /// <param name="layout"></param>
        /// <returns></returns>
        public LayoutPdf ConvertToPdf(PartLayout layout)
        {
            LayoutPdf model = new LayoutPdf();

            model.Content = layout.Content;
            model.Type = layout.Type;
            model.Description = layout.Description;

            return model;
        }

        /// <summary>
        /// convert project part layout to part layout 
        /// </summary>
        /// <param name="projectPartLayout"></param>
        /// <returns></returns>
        public PartLayout ConvertToDomain(ProjectPartLayout projectPartLayout)
        {
            PartLayout partLayout = new PartLayout();

            partLayout.Description = projectPartLayout.Description;
            partLayout.Type = projectPartLayout.Type;
            partLayout.Length = projectPartLayout.Length;
            partLayout.Content = projectPartLayout.Content;
            partLayout.IsLatest = projectPartLayout.IsLatest;
            partLayout.IsMachined = projectPartLayout.IsMachined;
            partLayout.IsRaw = projectPartLayout.IsRaw;

            return partLayout;
        }

        /// <summary>
        /// convert posted layout to part layout domain
        /// </summary>
        /// <param name="layout"></param>
        /// <returns></returns>
        public PartLayout ConvertToDomain(HttpPostedFileBase layout)
        {
            PartLayout partLayout = new PartLayout();

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

            partLayout.Description = trimmedFileName;
            partLayout.Type = layout.ContentType;
            partLayout.Length = layout.ContentLength;
            partLayout.Content = tempFile;

            return partLayout;
        }
    }
}