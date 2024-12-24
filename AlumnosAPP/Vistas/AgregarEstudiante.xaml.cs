using Firebase.Database;
using Firebase.Database.Query;
using System.Text.RegularExpressions;
using static AlumnosAPP.RegistroAlumnos.Modelos;

namespace AlumnosAPP.Vistas
{
    public partial class AgregarEstudiante : ContentPage
    {
        private readonly FirebaseClient _firebaseClient;

        public event EventHandler<Estudiante> StudentAdded;
        public event EventHandler<Estudiante> StudentUpdated;

        public bool IsNuevoEstudiante { get; set; }
        private string EstudianteId { get; set; }

        public AgregarEstudiante(FirebaseClient firebaseClient)
        {
            InitializeComponent();
            _firebaseClient = firebaseClient;
            IsNuevoEstudiante = true;
            BindingContext = this;
        }

        public AgregarEstudiante(FirebaseClient firebaseClient, Estudiante student)
        {
            InitializeComponent();
            _firebaseClient = firebaseClient;
            IsNuevoEstudiante = false;

            nombreEntry.Text = student.Nombre;
            correoEntry.Text = student.Correo;
            edadEntry.Text = student.Edad.ToString();
            cursoEntry.Text = student.Curso;
            activoSwitch.IsToggled = student.Activo;
            EstudianteId = student.Id;

            activoSwitch.IsVisible = IsNuevoEstudiante;
        }

        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(nombreEntry.Text) ||
                string.IsNullOrWhiteSpace(correoEntry.Text) ||
                string.IsNullOrWhiteSpace(edadEntry.Text) ||
                string.IsNullOrWhiteSpace(cursoEntry.Text))
            {
                await DisplayAlert("Error", "Todos los campos son obligatorios.", "OK");
                return;
            }

            if (!int.TryParse(edadEntry.Text, out var edad) || edad <= 0)
            {
                await DisplayAlert("Error", "La edad debe ser un número válido mayor que 0.", "OK");
                return;
            }

            var correo = correoEntry.Text;
            if (!IsValidEmail(correo))
            {
                await DisplayAlert("Error", "El correo electrónico no es válido. Debe contener '@' y '.'", "OK");
                return;
            }

            if (IsNuevoEstudiante && !activoSwitch.IsToggled)
            {
                await DisplayAlert("Error", "No se puede crear un estudiante deshabilitado.", "OK");
                return;
            }

            var estudiante = new Estudiante
            {
                Id = EstudianteId,
                Nombre = nombreEntry.Text,
                Correo = correoEntry.Text,
                Edad = edad,
                Curso = cursoEntry.Text,
                Activo = IsNuevoEstudiante ? activoSwitch.IsToggled : true
            };

            bool confirmacion = await DisplayAlert(
                "Confirmar",
                $"¿Estás seguro de guardar a {estudiante.Nombre}?",
                "Sí",
                "No"
            );

            if (confirmacion)
            {
                try
                {
                    if (IsNuevoEstudiante)
                    {
                        var result = await _firebaseClient
                            .Child("Estudiantes")
                            .PostAsync(estudiante);

                        estudiante.Id = result.Key;
                        StudentAdded?.Invoke(this, estudiante);

                        await DisplayAlert("Éxito", "Estudiante guardado exitosamente.", "OK");
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(estudiante.Id))
                        {
                            await _firebaseClient
                                .Child("Estudiantes")
                                .Child(estudiante.Id)
                                .PutAsync(estudiante);

                            StudentUpdated?.Invoke(this, estudiante);

                            await DisplayAlert("Éxito", "Estudiante actualizado exitosamente.", "OK");
                        }
                        else
                        {
                            await DisplayAlert("Error", "No se pudo encontrar el estudiante para actualizar.", "OK");
                        }
                    }

                    await Navigation.PopAsync();
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"No se pudo guardar al estudiante: {ex.Message}", "OK");
                }
            }
        }

        private bool IsValidEmail(string email)
        {
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }
    }
}
