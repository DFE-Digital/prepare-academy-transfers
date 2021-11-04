using Frontend.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Threading.Tasks;

namespace Frontend.ModelBinders
{
    public class FeaturesViewModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            var values = bindingContext.ValueProvider.GetValue("Value");
            if (values.Length == 0)
                return Task.CompletedTask;

            var splitData = values.FirstValue.Split(new char[] { '|' });
            //if (splitData.Length >= 2)
            //{
            //    var result = new User
            //    {
            //        Id = Convert.ToInt32(splitData[0]),
            //        Name = splitData[1]
            //    };

            var result = new FeaturesViewModel();
            bindingContext.Result = ModelBindingResult.Success(result);
            //}

            return Task.CompletedTask;
        }
    }
}
