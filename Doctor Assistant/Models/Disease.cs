using System.ComponentModel.DataAnnotations.Schema;

namespace Doctor_Assistant.Models
{
    public class Disease
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        [ForeignKey("department")]
        public int DepartmentId { get; set; }

    }
}
