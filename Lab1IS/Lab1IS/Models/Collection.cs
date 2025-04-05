using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Lab1IS.Models
{
    public class Collection
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Currator { get; set; } = string.Empty;

        public ICollection<Artifact> Artifacts { get; set; } = new List<Artifact>();
    }
}
