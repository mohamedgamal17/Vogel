﻿namespace Vogel.BuildingBlocks.Shared.Models
{
    public class Paging<T>
    {
        public List<T> Data { get; set; }
        public PagingInfo Info { get; set; }
    }
}
