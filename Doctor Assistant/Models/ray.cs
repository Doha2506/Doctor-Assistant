using System.ComponentModel.DataAnnotations.Schema;

namespace Doctor_Assistant.Models
{
    public class Ray
    {
        public int Id { get; set; }

        [ForeignKey("patient")]
        public int patientId { get; set; }

        [ForeignKey("Doctor")]
        public int doctorId { get; set; }

        public string result { get; set; }


        public string ImageName { get; set; }

        [NotMapped]
        public IFormFile ImageFile { get; set; }
    }
}
