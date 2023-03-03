namespace Doctor_Assistant.Models
{
    public class RayJoinPatientJoinDoctor
    {
        public Doctor doctor { get; set; }
        public Ray ray { get; set; }
        public Patient patient { get; set; }
    }
}
