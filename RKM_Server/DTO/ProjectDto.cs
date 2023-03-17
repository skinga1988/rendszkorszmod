namespace RKM_Server.DTO
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string ProjectType { get; set; }
        public string ProjectDescription { get; set; }
        public string Place { get; set; }
        public int OrdererId { get; set; }
        public int UserId { get; set; }

    }
}
