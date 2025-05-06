using System.Text.RegularExpressions;
using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Validation
{
    public static class StringValidation
    {
        public const int MaxNameLength = 100;
        public const int MaxEmailLength = 100;
        public const int MaxPhoneLength = 20;
        public const int MaxAddressLength = 200;

        private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public static void ValidateName(string name, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException($"{fieldName} cannot be empty");

            if (name.Length > MaxNameLength)
                throw new DomainException($"{fieldName} cannot be longer than {MaxNameLength} characters");
        }

        public static void ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email cannot be empty");

            if (email.Length > MaxEmailLength)
                throw new DomainException($"Email cannot be longer than {MaxEmailLength} characters");

            if (!EmailRegex.IsMatch(email))
                throw new DomainException("Invalid email format");
        }

        public static void ValidatePhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                throw new DomainException("Phone cannot be empty");

            if (phone.Length > MaxPhoneLength)
                throw new DomainException($"Phone cannot be longer than {MaxPhoneLength} characters");
        }

        public static void ValidateAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new DomainException("Address cannot be empty");

            if (address.Length > MaxAddressLength)
                throw new DomainException($"Address cannot be longer than {MaxAddressLength} characters");
        }
    }
}