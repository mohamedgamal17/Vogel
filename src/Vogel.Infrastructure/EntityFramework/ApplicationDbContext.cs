﻿using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Vogel.BuildingBlocks.EntityFramework;

namespace Vogel.Infrastructure.EntityFramework
{
    public class ApplicationDbContext : VogelDbContext<ApplicationDbContext>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

    }
}