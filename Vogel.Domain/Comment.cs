﻿namespace Vogel.Domain
{
    public class Comment : Entity
    {
        public string Content { get; set; }
        public string PostId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        protected override string GetEntityPerfix()
        {
            string perfix = "com";

            return perfix;
        }
    }
}
