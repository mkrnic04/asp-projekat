using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.Commands.Categories;
using BookReview.Application.DTO;
using BookReview.DataAccess;
using BookReview.Domain;
using BookReview.Implementation.UseCases.Validators;
using FluentValidation;

namespace BookReview.Implementation.UseCases.Commands.Categories
{
    public class EfUpdateCategoryCommand : EfUseCase, IUpdateCategoryCommand
    {
        private readonly UpdateCategoryValidator _validator;

        public EfUpdateCategoryCommand(
            UpdateCategoryValidator validator,
            BookReviewDbContext context)
            : base(context)
        {
            _validator = validator;
        }

        public string Name => "Update category";

        public string Id => "update-category";

        public void Execute(int id, UpdateCategoryDTO data)
        {
            _validator.ValidateAndThrow(data);

            var category = ctx.Categories
                .FirstOrDefault(x => x.Id == id);

            if (category == null)
            {
                throw new KeyNotFoundException("Category not found.");
            }

            var categoryExists = ctx.Categories.Any(x =>
                x.Name == data.Name &&
                x.Id != id);

            if (categoryExists)
            {
                throw new ValidationException(
                    "Category with this name already exists.");
            }

            category.Name = data.Name;

            ctx.SaveChanges();
        }
    }
}