namespace Ambev.DeveloperEvaluation.WebApi.Models.Customers
{
    public class CustomerResponse
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
    }
}