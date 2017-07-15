using SouthlandMetals.Core.Domain.DBModels;
using SouthlandMetals.Core.Domain.Models;

namespace SouthlandMetals.Web.Converters
{
    public class PartOperationConverter
    {
        /// <summary>
        /// convert project part to part model
        /// </summary>
        /// <param name="projectPart"></param>
        /// <returns></returns>
        public PartOperationModel ConvertFromProjectPart(ProjectPart projectPart)
        {
            PartOperationModel operationPart = new PartOperationModel();

            operationPart.ProjectPartId = projectPart.ProjectPartId;
            operationPart.PartId = projectPart.PartId;
            operationPart.PartNumber = projectPart.Number;
            operationPart.PartDescription = projectPart.Description;
            operationPart.AccountCode = projectPart.AccountCode;
            operationPart.RevisionNumber = projectPart.RevisionNumber;
            operationPart.CustomerId = projectPart.CustomerId;
            operationPart.FoundryId = projectPart.FoundryId;
            operationPart.SubFoundryId = projectPart.SubFoundryId;
            operationPart.PartTypeId = projectPart.PartTypeId;
            operationPart.HtsNumberId = projectPart.HtsNumberId;
            operationPart.MaterialId = projectPart.MaterialId;
            operationPart.ShipmentTermId = projectPart.ShipmentTermId;
            operationPart.MaterialSpecificationId = projectPart.MaterialSpecificationId;
            operationPart.PaymentTermId = projectPart.PaymentTermId;
            operationPart.IsMachined = projectPart.IsMachined;
            operationPart.PartStatusId = projectPart.PartStatusId;
            operationPart.DestinationId = projectPart.DestinationId;
            operationPart.Weight = projectPart.Weight;
            operationPart.Cost = projectPart.Cost;
            operationPart.Price = projectPart.Price;
            operationPart.AdditionalCost = projectPart.AdditionalCost;
            operationPart.SurchargeId = projectPart.SurchargeId;
            operationPart.PalletQuantity = projectPart.PalletQuantity;
            operationPart.CustomerAddressId = projectPart.CustomerAddressId;
            operationPart.SiteId = projectPart.SiteId;
            operationPart.CoatingTypeId = projectPart.CoatingTypeId;
            operationPart.FixtureDate = projectPart.FixtureDate;
            operationPart.FixtureCost = projectPart.FixtureCost;
            operationPart.FixturePrice = projectPart.FixturePrice;
            operationPart.PatternDate = projectPart.PatternDate;
            operationPart.PatternCost = projectPart.PatternCost;
            operationPart.PatternPrice = projectPart.PatternPrice;
            operationPart.IsFamilyPattern = projectPart.IsFamilyPattern;
            operationPart.PatternMaterialId = projectPart.PatternMaterialId;
            operationPart.Notes = projectPart.Notes;
            operationPart.AnnualUsage = projectPart.AnnualUsage;
            operationPart.IsRaw = projectPart.IsRaw;
            operationPart.FoundryOrderId = projectPart.FoundryOrderId;
            operationPart.ToolingDescription = projectPart.ToolingDescription;

            return operationPart;
        }
    }
}