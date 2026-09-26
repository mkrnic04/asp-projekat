using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.DTO;

namespace BookReview.Application.Commands.Categories
{
    public interface ICreateCategoryCommand
    {
        string Name { get; }

        string Id { get; }

        void Execute(CreateCategoryDTO data);
    }
}
