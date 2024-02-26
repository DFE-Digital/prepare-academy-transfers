using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace Dfe.PrepareTransfers.Web.Pages.Transfers
{
    public class IsFormAMatModel : PageModel
    {
        [BindProperty] 
        public IsFormAMatViewModel IsFormAMatViewModel { get; set; } = new IsFormAMatViewModel();

        public IList<RadioButtonViewModel> RadioButtonsYesNo { get; set; }

        public IActionResult OnGet(bool change = false)
        {
            ViewData["ChangeLink"] = change;

            RadioButtonsYesNo = GetRadioButtons(IsFormAMatViewModel.IsFormAMat);
            return Page();
        }


        public IActionResult OnPost() 
        {
            if (!ModelState.IsValid)
            {
                return OnGet();
            }

            var redirectPage = IsFormAMatViewModel.IsFormAMat.HasValue && IsFormAMatViewModel.IsFormAMat.Value ? "Transfers/ProposedTrustName" : "/Transfers/IncomingTrust";

            return RedirectToPage(redirectPage);
        }        
        
        private IList<RadioButtonViewModel> GetRadioButtons(bool? valueSelected)
        {
            var list = new List<RadioButtonViewModel>
            {
                new RadioButtonViewModel
                {
                    DisplayName = "Yes",
                    Name = $"{nameof(IsFormAMatViewModel.IsFormAMat)}",
                    Value = "true",
                    Checked = valueSelected is true
                },
                new RadioButtonViewModel
                {
                    DisplayName = "No",
                    Name = $"{nameof(IsFormAMatViewModel.IsFormAMat)}",
                    Value = "false",
                    Checked = valueSelected is false
                }
            };

            var selectedRadio =
                list.FirstOrDefault(c => c.Value == IsFormAMatViewModel.IsFormAMat.ToString());
            if (selectedRadio != null)
            {
                selectedRadio.Checked = true;
            }

            return list;
        }
    }
}
