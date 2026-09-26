using System;
using System.Collections.Generic;
using System.Text;
using BookReview.DataAccess;

namespace BookReview.Implementation.UseCases
{
    public abstract class EfUseCase
    {
        protected readonly BookReviewDbContext ctx;

        protected EfUseCase(BookReviewDbContext context)
        {
            ctx = context;
        }
    }
}
