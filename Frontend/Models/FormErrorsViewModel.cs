using System.Collections.Generic;
using System.Linq;

namespace Frontend.Models
{
    public class FormErrorsViewModel
    {
        public List<FormError> Errors { get; }
        public bool HasErrors => Errors.Count > 0;

        public FormErrorsViewModel()
        {
            Errors = new List<FormError>();
        }

        public void AddError(string elementId, string fieldName, string errorMessage)
        {
            Errors.Add(new FormError {ErrorElementId = elementId, FieldName = fieldName, ErrorMessage = errorMessage});
        }

        public bool HasErrorForField(string fieldName)
        {
            return Errors.Any(error => error.FieldName == fieldName);
        }

        public FormError ErrorForField(string fieldName)
        {
            return Errors.Find(error => error.FieldName == fieldName);
        }

        public string FormClassesForField(string fieldName)
        {
            return HasErrorForField(fieldName) ? "govuk-form-group--error" : "";
        }
    }
}