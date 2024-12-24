namespace AlumnosAPP
{
    public class RegistroAlumnos
    {
        public class Modelos
        {
            public class Estudiante
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
