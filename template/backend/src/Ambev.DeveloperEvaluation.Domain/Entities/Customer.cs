using System;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class Customer : AggregateRoot
    {
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string Phone { get; private set; } = string.Empty;

        protected Customer() { }

        public Customer(string name, string email, string phone)
        {
            Name = name;
            Email = email;
            Phone = phone;

            Validate();
        }

        public void Update(string name, string email, string phone)
        {
            Name = name;
            Email = email;
            Phone = phone;

            Validate();
        }

        private void Validate()
        {
            StringValidation.ValidateName(Name, "Customer name");
            StringValidation.ValidateEmail(Email);
            StringValidation.ValidatePhone(Phone);
        }
    }
}