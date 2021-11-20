using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace SC.DevChallenge.Api.Infrastructure
{
    public class CustomDateTimeModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            ILoggerFactory loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
            IModelBinder binder = new CustomDateTimeModelBinder(new SimpleTypeModelBinder(typeof(DateTime?), loggerFactory));
            return context.Metadata.ModelType == typeof(DateTime?) ? binder : null;
        }
    }
}
