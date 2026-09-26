using BookReview.Application.Commands.Categories;
using BookReview.Application.DTO;
using BookReview.DataAccess;
using BookReview.Domain;
using BookReview.Implementation.UseCases.Validators;
using FluentValidation;

namespace BookReview.Implementation.UseCases.Commands.Categories
{
    public class EfCreateCategoryCommand : EfUseCase, ICreateCategoryCommand
    {
        private readonly CreateCategoryValidator _validator;

        public EfCreateCategoryCommand(
            CreateCategoryValidator validator,
            BookReviewDbContext context)
            : base(context)
        {
            _validator = validator;
        }

        public string Name => "Create new category";

        public string Id => "add-category";

        public void Execute(CreateCategoryDTO data)
        {
            _validator.ValidateAndThrow(data);

            Category category = new Category
            {
                Name = data.Name
            };

            ctx.Categories.Add(category);
            ctx.SaveChanges();
        }
    }
}