using System.ComponentModel.DataAnnotations;

namespace Odin.System;

/// <summary>
/// Used to validate an attribute as being a member of a class derived from StringEnum.
/// </summary>
/// <typeparam name="TStringEnum"></typeparam>
public class StringEnumMemberAttribute<TStringEnum> : ValidationAttribute where TStringEnum : StringEnum<TStringEnum> 
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        if (value is not string str)
        {
            return new ValidationResult($"{validationContext.DisplayName} must be a string.");
        }

        var valid = StringEnum<TStringEnum>.HasValue(str);
        if (!valid.Success)
        {
            return new ValidationResult(valid.MessagesToString());
        }

        return ValidationResult.Success;
    }
}