﻿namespace Vogel.BuildingBlocks.Application.Security
{
    public class ApplicationUser
    {
        public string Id { get; set; }
        public string? UserName { get; set; }
        public List<string> Roles { get; set; }
    }
}