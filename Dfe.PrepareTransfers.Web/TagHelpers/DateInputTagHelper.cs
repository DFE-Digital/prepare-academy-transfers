using Dfe.PrepareTransfers.Models;
using Dfe.PrepareTransfers.Services;
using Dfe.PrepareTransfers.Web.Models.Forms;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

namespace Dfe.PrepareTransfers.TagHelpers;

[HtmlTargetElement("govuk-date-input", TagStructure = TagStructure.WithoutEndTag)]
public class DateInputTagHelper : InputTagHelperBase
{
    private readonly ErrorService _errorService;

    public DateInputTagHelper(IHtmlHelper htmlHelper, ErrorService errorService) : base(htmlHelper)
    {
        _errorService = errorService;
    }

   public bool HeadingLabel { get; set; }

   protected override async Task<IHtmlContent> RenderContentAsync()
   {
      DateInputViewModel model = ValidateRequest();

      return await _htmlHelper.PartialAsync("_DateInput", model);
   }

   private DateInputViewModel ValidateRequest()
   {
      if (For.ModelExplorer.ModelType != typeof(DateTime?))
      {
         throw new ArgumentException("ModelType is not a DateTime?");
      }

      DateTime? date = For.Model as DateTime?;
      DateInputViewModel model = new()
      {
         Id = Id,
         Name = Name,
         Label = Label,
         HeadingLabel = HeadingLabel,
         Hint = Hint
      };

      if (date.HasValue)
      {
         model.Day = date.Value.Day.ToString();
         model.Month = date.Value.Month.ToString();
         model.Year = date.Value.Year.ToString();
      }

      Error error = _errorService.GetError(Name);
      if (error is not null)
      {
         model.ErrorMessage = error.Message;
         model.DayInvalid = error.InvalidInputs.Contains($"{Name}-day");
         if (ViewContext.HttpContext.Request.Form.TryGetValue($"{Name}-day", out StringValues dayValue))
         {
            model.Day = dayValue;
         }

         model.MonthInvalid = error.InvalidInputs.Contains($"{Name}-month");
         if (ViewContext.HttpContext.Request.Form.TryGetValue($"{Name}-month", out StringValues monthValue))
         {
            model.Month = monthValue;
         }

         model.YearInvalid = error.InvalidInputs.Contains($"{Name}-year");
         if (ViewContext.HttpContext.Request.Form.TryGetValue($"{Name}-year", out StringValues yearValue))
         {
            model.Year = yearValue;
         }

         if (!model.DayInvalid && !model.MonthInvalid && model.YearInvalid)
         {
            model.DayInvalid = model.MonthInvalid = model.YearInvalid = true;
         }
      }

      return model;
   }
}
