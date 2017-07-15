using SouthlandMetals.Common.Models;
using SouthlandMetals.Core.Domain.DBModels;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace SouthlandMetals.Core.Domain.Interfaces
{
    public interface IPatternMaterialRepository : IDisposable
    {
        List<SelectListItem> GetSelectablePatternMaterials();

        List<PatternMaterial> GetPatternMaterials();

        PatternMaterial GetPatternMaterial(Guid patternMaterialId);

        OperationResult SavePatternMaterial(PatternMaterial newPatternMaterial);

        OperationResult UpdatePatternMaterial(PatternMaterial patternMaterial);
    }
}
