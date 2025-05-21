namespace Sistema_Tea.Models
{
    public class Tutor
    {
        public int TutorID { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Telefono { get; set; }
        public DateTime FechaRegistro { get; set; }

        // Relaciones
        public ICollection<Paciente> Pacientes { get; set; }
    }
}
