﻿namespace MIST155.Models.DTO
{
    public class SearchDTO
    {
        public int? categoryId { get; set; }
        public int? page { get; set; }
        public int? pageSize { get; set; }
        public string? keyword  { get; set; }
        public string? sortType { get; set; }
        public string? sortBy { get; set; }
        public string? categoryname {  get; set; }

    }
}
