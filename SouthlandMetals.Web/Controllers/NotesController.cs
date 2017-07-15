using SouthlandMetals.Web.Helpers;
using SouthlandMetals.Web.Models;
using System.Web.Mvc;

namespace SouthlandMetals.Web.Controllers
{
    public class NotesController : ApplicationBaseController
    {
        /// <summary>
        /// add hold notes modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _AddHoldNotes()
        {
            return PartialView("~/Views/Shared/_AddHoldNotes.cshtml");
        }

        /// <summary>
        /// add cancel notes modal
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _AddCancelNotes()
        {
            return PartialView("~/Views/Shared/_AddCancelNotes.cshtml");
        }

        /// <summary>
        /// view hold notes modal
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _ViewHoldNotes(NotesViewModel model)
        {
            return PartialView("~/Views/Shared/_ViewHoldNotes.cshtml", model);
        }

        /// <summary>
        /// view cancale notes modal
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [CustomAuthorize(Roles = "Accounting, Admin, Standard")]
        public ActionResult _ViewCancelNotes(NotesViewModel model)
        {
            return PartialView("~/Views/Shared/_ViewCancelNotes.cshtml", model);
        }
    }
}