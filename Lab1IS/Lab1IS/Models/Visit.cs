namespace Lab1IS.Models
{
    public class Visit
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime DateVisited { get; set; }

        public int Duration { get; set; }

        public int Rating { get; set; }

        public Guid VisitorId { get; set; }
        public Visitor Visitor { get; set; } = null!;

        public Guid ArtifactId { get; set; }
        public Artifact Artifact { get; set; } = null!;
    }
}
