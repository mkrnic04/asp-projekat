using System;
using System.Collections.Generic;
using System.Text;

using BookReview.Application.DTO;

namespace BookReview.Application.Commands.Favorites
{
    public interface ICreateFavoriteCommand
    {
        string Name { get; }

        string Id { get; }

        void Execute(CreateFavoriteDTO data);
    }
}
