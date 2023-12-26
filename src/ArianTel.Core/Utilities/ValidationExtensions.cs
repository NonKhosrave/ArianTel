using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace ArianTel.Core.Utilities;
public static class ValidationExtensions
{
    public static string GetValidationErrors(this DbContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        var errors = new StringBuilder();
        var entities = context.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified)
            .Select(e => e.Entity);
        foreach (var entity in entities)
        {
            var validationContext = new ValidationContext(entity);
            var validationResults = new List<ValidationResult>();
            if (!Validator.TryValidateObject(entity, validationContext, validationResults, true))
            {
                foreach (var validationResult in validationResults)
                {
                    var names = validationResult.MemberNames.Aggregate((s1, s2) => $"{s1}, {s2}");
                    errors.AppendFormat(CultureInfo.InvariantCulture, "{0}: {1}", names, validationResult.ErrorMessage);
                }
            }
        }

        return errors.ToString();
    }
}
