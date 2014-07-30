﻿using System.Data.Entity;
using CarbonKnown.Factors.Constants;
using CarbonKnown.Factors.Models;

namespace CarbonKnown.Factors.DAL
{
    public class DataContext : DbContext
    {
        public DataContext()
            : base(Constant.ConnectionStringName)
        {
        }

        public virtual DbSet<CourierRouteDistance> CourierRouteDistances { get; set; }
        public virtual DbSet<AirRouteDistance> AirRouteDistances { get; set; }
        public virtual DbSet<FactorValue> FactorValues { get; set; }
        public virtual DbSet<Factor> Factors { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FactorValue>().Property(e => e.Value).HasPrecision(22, 8);
            base.OnModelCreating(modelBuilder);
        }
    }
}
