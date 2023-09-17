using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMSPortal.Core.Model;

namespace WMSPortal.Core.Validators
{
    public class OrdersValidator : AbstractValidator<Orders>
    {
        public OrdersValidator()
        {
            RuleFor(x => x.StorerKey).NotNull().WithMessage("Storer is required");
            RuleFor(x => x.ExternOrderKey).NotEmpty();
        }
    }
}
