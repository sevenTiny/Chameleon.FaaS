using Chameleon.Faas.Management.Infrastructure.Configs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chameleon.Faas.Management.Entities
{
    public class FaasDbContext : DbContext
    {
        public virtual DbSet<FaasScript> FaasScript { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySQL(ConnectionStrings.Instance.ChameleonFaas);
            }
        }
    }
}
