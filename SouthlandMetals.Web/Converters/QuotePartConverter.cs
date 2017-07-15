using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Repositories;
using SouthlandMetals.Dynamics.Domain.Repositories;
using SouthlandMetals.Web.Areas.Operations.Models;
using System.Linq;

namespace SouthlandMetals.Web.Converters
{
    public class QuotePartConverter
    {
        /// <summary>
        /// convert project part to quote part view model
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public QuotePartViewModel ConvertToView(ProjectPart part)
        {
            QuotePartViewModel model = new QuotePartViewModel();

            model.ProjectPartId = part.ProjectPartId;
            model.PartId = part.PartId;
            model.PartNumber = (!string.IsNullOrEmpty(part.Number)) ? part.Number : "N/A";
            model.PartDescription = (!string.IsNullOrEmpty(part.Description)) ? part.Description : "N/A";
            model.CustomerId = part.CustomerId;
            model.FoundryId = part.FoundryId;
            model.RevisionNumber = (!string.IsNullOrEmpty(part.RevisionNumber)) ? part.RevisionNumber : "N/A";
            model.Weight = part.Weight;
            model.AnnualUsage = part.AnnualUsage;
            model.Price = part.Price;
            model.Cost = part.Cost;
            model.PatternPrice = part.PatternPrice;
            model.PatternCost = part.PatternCost;
            model.FixturePrice = part.FixturePrice;
            model.FixtureCost = part.FixtureCost;
            model.IsRaw = part.IsRaw;
            model.IsMachined = part.IsMachined;

            return model;
        }

        /// <summary>
        /// convert part to quote part view model
        /// </summary>
        /// <param name="part"></param>
        /// <returns></returns>
        public QuotePartViewModel ConvertToView(Part part)
        {
            QuotePartViewModel model = new QuotePartViewModel();

            var _partRepository = new PartRepository();
            var _dynamicsPartRepository = new PartDynamicsRepository();
            var _partStatusRepository = new PartStatusRepository();

            var dynamicsPart = _dynamicsPartRepository.GetPartMaster(part.Number);
            var dynamicsPartCurrency = _dynamicsPartRepository.GetPartCurrency(part.Number);
            var partStatus = _partStatusRepository.GetPartStatus(part.PartStatusId);
            var partDrawing = _partRepository.GetPartDrawings(part.PartId).FirstOrDefault(x => x.IsLatest);

            model.PartId = part.PartId;
            model.PartNumber = (!string.IsNullOrEmpty(part.Number)) ? part.Number : "N/A";
            model.PartDescription = (dynamicsPart != null && !string.IsNullOrEmpty(dynamicsPart.ITEMDESC)) ? dynamicsPart.ITEMDESC : "N/A";
            model.CustomerId = part.CustomerId;
            model.FoundryId = part.FoundryId;
            model.RevisionNumber = (partDrawing != null && !string.IsNullOrEmpty(partDrawing.RevisionNumber)) ? partDrawing.RevisionNumber : "N/A";
            model.Weight = (dynamicsPart != null) ? (dynamicsPart.ITEMSHWT / 100.00m) : 0.00m;
            model.AnnualUsage = part.AnnualUsage;
            model.Price = (dynamicsPartCurrency != null) ? (decimal)dynamicsPartCurrency.LISTPRCE : 0.00m;
            model.PatternPrice = part.PatternPrice;
            model.FixturePrice = part.FixturePrice;
            model.Cost = (dynamicsPart != null) ? (decimal)dynamicsPart.STNDCOST : 0.00m;
            model.PatternCost = part.PatternCost;
            model.FixtureCost = part.FixtureCost;
            model.IsRaw = part.IsRaw;
            model.IsMachined = part.IsMachined;

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
            if (_partStatusRepository != null)
            {
                _partStatusRepository.Dispose();
                _partStatusRepository = null;
            }

            return model;
        }
    }
}