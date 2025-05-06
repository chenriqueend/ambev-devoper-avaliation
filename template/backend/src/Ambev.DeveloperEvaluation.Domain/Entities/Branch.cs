using System;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Entities
{
    public class Branch : AggregateRoot
    {
        public string Name { get; private set; } = string.Empty;
        public string Address { get; private set; } = string.Empty;
        public string Phone { get; private set; } = string.Empty;

        protected Branch() { }

        public Branch(string name, string address, string phone)
        {
            Name = name;
            Address = address;
            Phone = phone;

            Validate();
        }

        public void Update(string name, string address, string phone)
        {
            Name = name;
            Address = address;
            Phone = phone;

            Validate();
        }

        private void Validate()
        {
            StringValidation.ValidateName(Name, "Branch name");
            StringValidation.ValidateAddress(Address);
            StringValidation.ValidatePhone(Phone);
        }
    }
}