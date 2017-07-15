using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Web.Areas.Administration.Models;
using System;

namespace SouthlandMetals.Web.Converters
{
    public class HtsNumberConverter
    {
        /// <summary>
        /// convert htsNumber to view model
        /// </summary>
        /// <param name="htsNumber"></param>
        /// <returns></returns>
        public HtsNumberViewModel ConvertToView(HtsNumber htsNumber)
        {
            HtsNumberViewModel model = new HtsNumberViewModel();

            model.HtsNumberId = htsNumber.HtsNumberId;
            model.HtsNumberDescription = (!string.IsNullOrEmpty(htsNumber.Description)) ? htsNumber.Description : "N/A";
            model.HtsNumberDutyRate = (htsNumber.DutyRate * 100).ToString();
            model.IsActive = htsNumber.IsActive;

            return model;
        }

        /// <summary>
        /// convert htsNumber view model to domain
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public HtsNumber ConvertToDomain(HtsNumberViewModel model)
        {
            HtsNumber htsNumber = new HtsNumber();

            htsNumber.HtsNumberId = model.HtsNumberId;
            htsNumber.Description = model.HtsNumberDescription;
            htsNumber.DutyRate = Convert.ToDouble(model.HtsNumberDutyRate) / 100;
            htsNumber.IsActive = model.IsActive;

            return htsNumber;
        }
    }
}
