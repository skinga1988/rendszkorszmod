namespace RKM_Server.DTO
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string RoleType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Password { get; set; }
    }
}
