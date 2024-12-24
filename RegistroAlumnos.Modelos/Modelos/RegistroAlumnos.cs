namespace RegistroAlumnos.Modelos.Modelos
{
    internal class RegistroAlumnos
    {
        internal class Modelos
        {
            internal class Estudiante
            {
                public string Id { get; set; }
                public string Nombre { get; set; }
                public string Correo { get; set; }
                public int Edad { get; set; }
                public string Curso { get; set; }
                public bool Activo { get; set; }
            }
        }
    }
}