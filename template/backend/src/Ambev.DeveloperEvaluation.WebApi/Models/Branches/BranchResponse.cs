namespace Ambev.DeveloperEvaluation.WebApi.Models.Branches
{
    public class BranchResponse
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required string Phone { get; set; }
    }
}