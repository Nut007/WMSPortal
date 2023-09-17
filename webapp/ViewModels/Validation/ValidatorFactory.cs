using FluentValidation;
using System;
using System.Collections.Generic;
using WMSPortal.ViewModels;
using WMSPortal.ViewModels.Validation.FluentValidationApplication.Validation;

namespace FluentValidationApplication.Validation
{
    public class ValidatorFactory : ValidatorFactoryBase
    {
        private static Dictionary<Type, IValidator> validators = new Dictionary<Type, IValidator>();

        static ValidatorFactory()
        {
            validators.Add(typeof(IValidator<OrdersViewModel>), new OrderViewModelValidator());
            validators.Add(typeof(IValidator<LoadingViewModel>), new LoadingViewModelValidator());
        }

        public override IValidator CreateInstance(Type validatorType)
        {
            IValidator validator;
            if (validators.TryGetValue(validatorType, out validator))
            {
                return validator;
            }
            return validator;
        }
    }
}