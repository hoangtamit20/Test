using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using serverapi.Entity;

namespace serverapi.Validator
{
    public class PaymentValidator : AbstractValidator<Payment>
    {
        // public PaymentValidator()
        // {
        //     RuleFor(x => x.Amount)
        //         .NotNull()
        //         .WithMessage("Amount is required.")
        //         .GreaterThan(0)
        //         .WithMessage("Amount must be greater than 0.");

        //     RuleFor(x => x.PaymentMethod)
        //         .NotNull()
        //         .WithMessage("PaymentMethod is required.")
        //         .Length(1, 255)
        //         .WithMessage("PaymentMethod must be between 1 and 255 characters.");

        //     RuleFor(x => x.Status)
        //         .NotNull()
        //         .WithMessage("Status is required.")
        //         .Length(1, 50)
        //         .WithMessage("Status must be between 1 and 50 characters.");

        //     RuleFor(x => x.CreateAt)
        //         .NotNull()
        //         .WithMessage("CreateAt is required.");

        //     RuleFor(x => x.OrderId)
        //         .NotNull()
        //         .WithMessage("OrderId is required.");
        // }
    }
}