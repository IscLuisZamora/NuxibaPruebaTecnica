namespace AppTestNuxiba.Models
{
    public class Usuario
    {
        public int UserId { get; set; }
        public string? Login { get; set; }
        public string? Nombre { get; set; }
        public string? Paterno { get; set; }
        public string? Materno { get; set; }
        public double Sueldo { get; set; }
        public DateTime FechaIngreso { get; set; }
    }
}
