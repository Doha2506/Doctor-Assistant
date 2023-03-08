using System.ComponentModel.DataAnnotations.Schema;

namespace Doctor_Assistant.Models
{
    public class PatientJoinStrokeDisease
    {

        public Patient patient { get; set; }
        public StrokeDisease stroke { get; set; }

    }
}
