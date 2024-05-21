using Dfe.PrepareTransfers.Web.Models;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;

namespace Dfe.PrepareTransfers.Web.Pages.Transfers
{
    public class PreferredTrustModel : PageModel
    {
        [BindProperty] 
        public PreferredTrustViewModel PreferredTrustViewModel { get; set; } = new PreferredTrustViewModel();

        public IList<RadioButtonViewModel> RadioButtonsYesNo { get; set; }

        public IActionResult OnGet(bool change = false)
        {
            ViewData["ChangeLink"] = change;

            RadioButtonsYesNo = GetRadioButtons(PreferredTrustViewModel.HasPreferredTrust);
            return Page();
        }


        public IActionResult OnPost() 
        {
            if (!ModelState.IsValid)
            {
                return OnGet();
            }

            var redirectPage = PreferredTrustViewModel.HasPreferredTrust.HasValue && PreferredTrustViewModel.HasPreferredTrust.Value ? "/Transfers/IncomingTrust" : "/Transfers/checkyouranswers";

            return RedirectToPage(redirectPage);
        }        
        
        private IList<RadioButtonViewModel> GetRadioButtons(bool? valueSelected)
        {
            var list = new List<RadioButtonViewModel>
            {
                new RadioButtonViewModel
                {
                    DisplayName = "Yes",
                    Name = $"{nameof(PreferredTrustViewModel.HasPreferredTrust)}",
                    Value = "true",
                    Checked = valueSelected is true
                },
                new RadioButtonViewModel
                {
                    DisplayName = "No",
                    Name = $"{nameof(PreferredTrustViewModel.HasPreferredTrust)}",
                    Value = "false",
                    Checked = valueSelected is false
                }
            };

            var selectedRadio =
                list.FirstOrDefault(c => c.Value == PreferredTrustViewModel.HasPreferredTrust.ToString());
            if (selectedRadio != null)
            {
                selectedRadio.Checked = true;
            }

            return list;
        }
    }
}
