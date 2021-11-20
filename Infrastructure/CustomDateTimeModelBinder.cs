using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace SC.DevChallenge.Api.Infrastructure
{
    public class CustomDateTimeModelBinder : IModelBinder
    {
        private readonly IModelBinder fallbackBinder;

        private List<string> propertyNames = new List<string>()
        {
            "date",
            "startDate",
            "endDate"
        };

        public CustomDateTimeModelBinder(IModelBinder fallbackBinder)
        {
            this.fallbackBinder = fallbackBinder;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            ValueProviderResult dateTimeValues = new ValueProviderResult();

            dateTimeValues = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);


            if (dateTimeValues == ValueProviderResult.None)
            {
                return fallbackBinder.BindModelAsync(bindingContext);
            }

            var dateTime = dateTimeValues.FirstValue;
            if (DateTime.TryParse(dateTime, CultureInfo.CreateSpecificCulture("fr-FR"),
                DateTimeStyles.None, out DateTime newDateTime))
            {
                bindingContext.Result = ModelBindingResult.Success(newDateTime);
            }

            return Task.CompletedTask;
        }
    }
}
