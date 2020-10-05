﻿#pragma warning disable 8618
using System.Collections.Generic;

namespace XivApi.Pagination
{
    public class PaginationInfo
    {
        public int Page { get; set; }
        public int? PageNext { get; set; }
        public int? PagePrev { get; set; }
        public int PageTotal { get; set; }
        public int Results { get; set; }
        public int ResultsPerPage { get; set; }
        public int ResultsTotal { get; set; }
    }

    public class PaginatedResponse<T>
    {
        public PaginationInfo Pagination { get; set; }
        public List<T> Results { get; set; }
    }
}