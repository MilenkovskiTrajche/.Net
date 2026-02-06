using System.ComponentModel.DataAnnotations;

namespace Lab1IS.Models
{
    public class Artifact
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Origin { get; set; } = string.Empty;

        public int? Year { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;

        public Guid? CollectionId { get; set; }
        public Collection? Collection { get; set; } = null;
    }
}
