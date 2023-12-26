using System;
using Microsoft.EntityFrameworkCore;

namespace ArianTel.DAL.EF.Configurations;
public static class ConfigurationMappings
{
    public static void AddCustomIdentityMappings(this ModelBuilder modelBuilder)
    {
        if (modelBuilder == null)
        {
            throw new ArgumentNullException(nameof(modelBuilder));
        }

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConfigurationMappings).Assembly);
    }
}
