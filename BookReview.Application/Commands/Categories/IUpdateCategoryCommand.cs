using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.DTO;

namespace BookReview.Application.Commands.Categories
{
    public interface IUpdateCategoryCommand
    {
        string Name { get; }

        string Id { get; }

        void Execute(int id, UpdateCategoryDTO data);
    }
}
