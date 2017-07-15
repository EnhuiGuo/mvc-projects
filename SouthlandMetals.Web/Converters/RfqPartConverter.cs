using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using System;
using System.Linq;

namespace SouthlandMetals.Web.Converters
{
    public class RfqPartConverter
    {
        /// <summary>
        /// convert project part to rfq part view model
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public RfqPartViewModel ConvertToView(ProjectPart part)
        {
            RfqPartViewModel model = new RfqPartViewModel();

            var _materialRepository = new MaterialRepository();

            var material = _materialRepository.GetMaterial(part.MaterialId);

            model.ProjectPartId = part.ProjectPartId;
            model.PartId = part.PartId;
            model.PartNumber = (!string.IsNullOrEmpty(part.Number)) ? part.Number : "N/A";
            model.RevisionNumber = (!string.IsNullOrEmpty(part.RevisionNumber)) ? part.RevisionNumber : "N/A";
            model.PartDescription = (!string.IsNullOrEmpty(part.Description)) ? part.Description : "N/A";
            model.CustomerId = part.CustomerId;
            model.FoundryId = part.FoundryId;
            model.IsRaw = part.IsRaw;
            model.IsMachined = part.IsMachined;
            model.Weight = part.Weight;
            model.AnnualUsage = part.AnnualUsage;
            model.MaterialId = part.MaterialId;
            model.MaterialDescription = (material != null && !string.IsNullOrEmpty(material.Description)) ? material.Description : "N/A";
            model.PartTypeId = part.PartTypeId;
            model.PartStatusId = part.PartStatusId;
            model.DestinationId = part.DestinationId;
            model.SurchargeId = part.SurchargeId;

            if (_materialRepository != null)
            {
                _materialRepository.Dispose();
                _materialRepository = null;
            }

            return model;
        }

        /// <summary>
        /// convert part to rfq part view model
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public RfqPartViewModel ConvertToView(Part part)
        {
            RfqPartViewModel model = new RfqPartViewModel();

            var _partRepository = new PartRepository();
            var _dynamicsPartRepository = new PartDynamicsRepository();
            var _materialRepository = new MaterialRepository();

            var dynamicsPart = _dynamicsPartRepository.GetPartMaster(part.Number);
            var material = _materialRepository.GetMaterial(part.MaterialId);
            var partDrawing = _partRepository.GetPartDrawings(part.PartId).FirstOrDefault(x => x.IsLatest);

            model.PartId = part.PartId;
            model.PartNumber = (!string.IsNullOrEmpty(part.Number)) ? part.Number : "N/A";
            model.PartDescription = (dynamicsPart != null && !string.IsNullOrEmpty(dynamicsPart.ITEMDESC)) ? dynamicsPart.ITEMDESC : "N/A";
            model.CustomerId = part.CustomerId;
            model.FoundryId = part.FoundryId;
            model.Weight = (dynamicsPart != null) ? (dynamicsPart.ITEMSHWT / 100.00m) : 0.00m;
            model.AnnualUsage = part.AnnualUsage;
            model.IsMachined = part.IsMachined;
            model.MaterialId = part.MaterialId;
            model.MaterialDescription = (material != null && !string.IsNullOrEmpty(material.Description)) ? material.Description : "N/A";
            model.RevisionNumber = (partDrawing != null && !string.IsNullOrEmpty(partDrawing.RevisionNumber)) ? partDrawing.RevisionNumber : "N/A";
            model.PartDrawingId = (partDrawing != null) ? partDrawing.PartDrawingId : Guid.Empty;
            model.PartTypeId = part.PartTypeId;
            model.PartStatusId = part.PartStatusId;
            model.DestinationId = part.DestinationId;
            model.SurchargeId = part.SurchargeId;
            model.SubFoundryId = part.SubFoundryId;

            if (_partRepository != null)
            {
                _partRepository.Dispose();
                _partRepository = null;
            }

            if (_dynamicsPartRepository != null)
            {
                _dynamicsPartRepository.Dispose();
                _dynamicsPartRepository = null;
            }

            if (_materialRepository != null)
            {
                _materialRepository.Dispose();
                _materialRepository = null;
            }

            return model;
        }
    }
}