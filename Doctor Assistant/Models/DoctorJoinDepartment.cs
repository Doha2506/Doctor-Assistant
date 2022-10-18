using System.ComponentModel.DataAnnotations.Schema;

namespace Doctor_Assistant.Models
{
    public class DoctorJoinDepartment
    {

        public Doctor doctor { get; set; }
        public Department department { get; set; }

    }
}
