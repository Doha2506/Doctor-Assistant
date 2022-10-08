using System.ComponentModel.DataAnnotations.Schema;

namespace Doctor_Assistant.Models
{
    public class PatientJoinRay
    {

        public Patient patient { get; set; }
        public Ray ray { get; set; }

    }
}
