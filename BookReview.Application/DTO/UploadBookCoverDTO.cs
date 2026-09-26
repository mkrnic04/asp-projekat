using System;
using System.Collections.Generic;
using System.Text;

namespace BookReview.Application.DTO
{
    public class UploadBookCoverDTO
    {
        public byte[] File { get; set; } = Array.Empty<byte>();

        public string FileName { get; set; } = string.Empty;

        public string ContentType { get; set; } = string.Empty;
    }
}
