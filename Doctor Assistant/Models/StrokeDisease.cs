using System.ComponentModel.DataAnnotations.Schema;

namespace Doctor_Assistant.Models
{
    public class StrokeDisease
    {
        public int Id { get; set; }

        [ForeignKey("Doctor")]
        public int DoctorId { get; set; }

        [ForeignKey("Patient")]
        public int PatientId { get; set; }
        public int Gender { get; set; }
        public int Age { get; set; }
        public bool Hypertension { get; set; }
        public bool HeartDisease { get; set; }
        public bool IsMarried { get; set; }
        public string WorkType { get; set; }
        public bool ResidenceType { get; set; }
        public float AvrGlucoseLevel { get; set; }
        public float BMI { get; set; }
        public string SmokingStatus { get; set; }
        public bool Stroke { get; set; }
        public string Result { get; set; }


    }
}
