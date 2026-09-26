using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.DTO;
using FluentValidation;

namespace BookReview.Implementation.UseCases.Validators
{
    public class UploadBookCoverValidator
        : AbstractValidator<UploadBookCoverDTO>
    {
        private const long MaxFileSize = 5 * 1024 * 1024;

        public UploadBookCoverValidator()
        {
            this.RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.File)
                .NotEmpty()
                .WithMessage("File is required.")
                .Must(x => x.Length <= MaxFileSize)
                .WithMessage("File cannot be larger than 5 MB.");

            RuleFor(x => x.FileName)
                .NotEmpty()
                .WithMessage("File name is required.");

            RuleFor(x => x.ContentType)
                .Must(x =>
                    x == "image/jpeg" ||
                    x == "image/jpg" ||
                    x == "image/png" ||
                    x == "image/webp")
                .WithMessage("Only JPG, PNG and WEBP images are allowed.");
        }
    }
}
