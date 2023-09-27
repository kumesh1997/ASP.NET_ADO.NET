namespace WebApplication1.DTO
{
    public class CustomerDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedOn { get; set; }
        public int IsActive { get; set; }
    }
}
