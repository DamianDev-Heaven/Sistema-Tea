namespace Sistema_Tea.Models
{
    public class TestInicial
    {
        public int Id { get; set; } 
        public string Texto { get; set; }
        public List<string> Opciones { get; set; }

        // Diccionario para mapear el texto de la opción al puntaje que otorga
        // Ejemplo: {"Sí", 1}, {"No", 0} o {"Nunca", 0}, {"A veces", 1}, {"Siempre", 2}
        public Dictionary<string, int> ScoresPorOpcion { get; set; }

        public string AyudaAdicional { get; set; }

        public TestInicial()
        {
            Opciones = new List<string>();
            ScoresPorOpcion = new Dictionary<string, int>();
        }
    }
}