﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnlabMvc.Models;

namespace AnlabMvc.Data
{
    // Run on app startup for now to seed DB with initial values
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Look for any students.
            if (context.Orders.Any())
            {
                return;   // DB has been seeded
            }

            // Seed with orders here, and maybe create users to test with
        }
    }
}
