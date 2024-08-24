﻿using Microsoft.EntityFrameworkCore;
using Vogel.BuildingBlocks.EntityFramework;
using Vogel.Social.Infrastructure.EntityFramework.Constants;

namespace Vogel.Social.Infrastructure.EntityFramework
{
    public class SocialDbContext : VogelDbContext<SocialDbContext>
    {
        public SocialDbContext(DbContextOptions<SocialDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(SocialDbConstants.Schema);

            base.OnModelCreating(modelBuilder);
       
        }
    }
}