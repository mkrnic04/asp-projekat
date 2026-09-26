using System;
using System.Collections.Generic;
using System.Text;

namespace BookReview.Application.DTO
{
    public class PaginationDTO<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }
    }
}
