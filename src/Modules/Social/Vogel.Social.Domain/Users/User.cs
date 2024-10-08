﻿using Vogel.BuildingBlocks.Domain.Auditing;
using Vogel.Social.Shared.Common;
namespace Vogel.Social.Domain.Users
{
    public class User : AuditedAggregateRoot<string>
    {
        public User(string id)
        {
            Id = id;
        }
        public User()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string? AvatarId { get; set; }
    }
}
