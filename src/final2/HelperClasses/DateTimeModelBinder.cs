using Microsoft.AspNet.Mvc.ModelBinding;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace toysRus.HelperClasses
{
    public class DateTimeModelBinder : IModelBinder
    {
        public Task<ModelBindingResult> BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext.ModelType != typeof(DateTime))
            {
                return ModelBindingResult.NoResultAsync;
            }

            ValueProviderResult value = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            CultureInfo cultureInf = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            cultureInf.DateTimeFormat.ShortDatePattern = "yyyy/MM/dd";

            var givenDate = value.ConvertTo(typeof(DateTime));
            return ModelBindingResult.SuccessAsync(bindingContext.ModelName, givenDate);
        }
    }
}
