namespace Appleseed.Portal.Domain.Services
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public static class Validation
    {
        public static List<ValidationResult> Validate(object objectToValidate)
        {
            var errors = new List<ValidationResult>();
            var ctx = new ValidationContext(objectToValidate, null, null);

            // Performs actual validation
            Validator.TryValidateObject(objectToValidate, ctx, errors, true);

            return errors;
        }
    }
}
