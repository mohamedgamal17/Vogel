﻿namespace Vogel.Domain
{
    public class Reactions : Entity
    {
        public string UserId { get; set; }
        public string PostId { get; set; }
        public ReactionType Type { get; set; }

        protected override string GetEntityPerfix()
        {
            string perfix = "rea";

            return perfix;
        }
    }

    
}
