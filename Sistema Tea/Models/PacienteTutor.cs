using System.ComponentModel.DataAnnotations;

namespace Sistema_Tea.Models
{
    public class PacienteTutor
    {
        [Required]
        public Paciente Paciente { get; set; } = new Paciente();

        public Tutor Tutor { get; set; } = new Tutor();
    }
}
