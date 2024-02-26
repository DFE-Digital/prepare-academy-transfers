using Dfe.PrepareTransfers.Web.Models.Benefits;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dfe.PrepareTransfers.Web.Pages.Transfers
{
    public class IsFormAMatModel : PageModel
    {
        [BindProperty]
        public bool? IsFormAMat { get; set; }

        public IList<RadioButtonViewModel> RadioButtonsYesNo { get; set; }

        public IActionResult OnGet(bool change = false)
        {
            ViewData["ChangeLink"] = change;

            RadioButtonsYesNo = GetRadioButtons(IsFormAMat);
            return Page();
        }


        public IActionResult OnPost() 
        {
            if (!ModelState.IsValid)
            {
                RadioButtonsYesNo = GetRadioButtons(IsFormAMat);
                return Page();
            }

            var redirectPage = IsFormAMat.HasValue && IsFormAMat.Value ? "Transfers/ProposedTrustName" : "/Transfers/SearchIncomingTrust";

            return RedirectToPage(redirectPage);
        }        
        
        private IList<RadioButtonViewModel> GetRadioButtons(bool? valueSelected)
        {
            var list = new List<RadioButtonViewModel>
            {
                new RadioButtonViewModel
                {
                    DisplayName = "Yes",
                    Name = $"{nameof(IsFormAMat)}",
                    Value = "true",
                    Checked = valueSelected is true
                },
                new RadioButtonViewModel
                {
                    DisplayName = "No",
                    Name = $"{nameof(IsFormAMat)}",
                    Value = "false",
                    Checked = valueSelected is false
                }
            };

            var selectedRadio =
                list.FirstOrDefault(c => c.Value == IsFormAMat.ToString());
            if (selectedRadio != null)
            {
                selectedRadio.Checked = true;
            }

            return list;
        }
    }
}
