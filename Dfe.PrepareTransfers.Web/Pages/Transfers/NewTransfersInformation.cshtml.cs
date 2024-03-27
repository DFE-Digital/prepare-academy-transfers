using Microsoft.AspNetCore.Mvc;

namespace Dfe.PrepareTransfers.Web.Pages.Transfers
{
    public class NewTransfersInformation : TransfersPageModel
    {

        public IActionResult OnGet()
        {
            return Page();
        }
    }
}
