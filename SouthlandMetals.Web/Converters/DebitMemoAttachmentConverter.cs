using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Web.Areas.Operations.Models;

namespace SouthlandMetals.Web.Converters
{
    public class DebitMemoAttachmentConverter
    {
        /// <summary>
        /// convert attachment to view model
        /// </summary>
        /// <param name="attachment"></param>
        /// <returns></returns>
        public DebitMemoAttachmentViewModel ConvertToView(DebitMemoAttachment attachment)
        {
            DebitMemoAttachmentViewModel model = new DebitMemoAttachmentViewModel();

            model.DebitMemoAttachmentId = attachment.DebitMemoAttachmentId;
            model.AttachmentName = (!string.IsNullOrEmpty(attachment.Name)) ? attachment.Name : "N/A";

            return model;
        }

        /// <summary>
        /// convert attachment view model to domain
        /// </summary>
        /// <param name="attachmentModel"></param>
        /// <returns></returns>
        public DebitMemoAttachment ConvertToDomain(AttachmentModel attachmentModel)
        {
            DebitMemoAttachment attachment = new DebitMemoAttachment();

            byte[] tempFile = new byte[attachmentModel.Attachment.ContentLength];

            var trimmedFileName = string.Empty;

            if (attachmentModel.Attachment.FileName.EndsWith("png"))
            {
                trimmedFileName = attachmentModel.Attachment.FileName.Replace(".png", "");
            }
            else if (attachmentModel.Attachment.FileName.EndsWith("jpg"))
            {
                trimmedFileName = attachmentModel.Attachment.FileName.Replace(".jpg", "");
            }
            else if (attachmentModel.Attachment.FileName.EndsWith("pdf"))
            {
                trimmedFileName = attachmentModel.Attachment.FileName.Replace(".pdf", "");
            }

            attachment.DebitMemoId = attachmentModel.DebitMemoId;
            attachment.Name = trimmedFileName;
            attachment.Type = attachmentModel.Attachment.ContentType;
            attachment.Length = attachmentModel.Attachment.ContentLength;
            attachment.Content = tempFile;

            return attachment;
        }
    }
}