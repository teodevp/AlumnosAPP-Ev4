using System.Collections.ObjectModel;
using Firebase.Database;
using Firebase.Database.Query;
using static AlumnosAPP.RegistroAlumnos.Modelos;

namespace AlumnosAPP.Vistas
{
    public partial class ListarAlumnos : ContentPage
    {
        public ObservableCollection<Estudiante> ListaAlumnos { get; set; }
        private ObservableCollection<Estudiante> TodosLosAlumnos { get; set; }

        public ListarAlumnos()
        {
            InitializeComponent();
            TodosLosAlumnos = new ObservableCollection<Estudiante>();
            ListaAlumnos = new ObservableCollection<Estudiante>();
            BindingContext = this;

            _ = CargarDatosDesdeFirebase();
        }

        private async Task CargarDatosDesdeFirebase()
        {
            try
            {
                var firebaseClient = new FirebaseClient("https://eva4-8e56b-default-rtdb.firebaseio.com/");
                var estudiantes = await firebaseClient
                    .Child("Estudiantes")
                    .OnceAsync<Estudiante>();

                TodosLosAlumnos.Clear();

                foreach (var estudiante in estudiantes)
                {
                    estudiante.Object.Id = estudiante.Key;
                    TodosLosAlumnos.Add(estudiante.Object);
                }

                ActualizarListaAlumnos();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudieron cargar los datos: {ex.Message}", "OK");
            }
        }

        private void filtroSearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = e.NewTextValue?.ToLower() ?? string.Empty;

            var listaFiltrada = TodosLosAlumnos
                .Where(estudiante =>
                    estudiante.Nombre.ToLower().Contains(searchText) ||
                    estudiante.Correo.ToLower().Contains(searchText))
                .ToList();

            ListaAlumnos.Clear();
            foreach (var estudiante in listaFiltrada)
            {
                if (estudiante.Activo)
                {
                    ListaAlumnos.Add(estudiante);
                }
            }
        }

        private void ActualizarListaAlumnos()
        {
            ListaAlumnos.Clear();
            foreach (var estudiante in TodosLosAlumnos.Where(e => e.Activo))
            {
                ListaAlumnos.Add(estudiante);
            }
        }

        private async void NuevoEstudianteBoton_Clicked(object sender, EventArgs e)
        {
            var firebaseClient = new FirebaseClient("https://eva4-8e56b-default-rtdb.firebaseio.com/");
            var agregarEstudiantePage = new AgregarEstudiante(firebaseClient);

            agregarEstudiantePage.StudentAdded += (s, nuevoEstudiante) =>
            {
                TodosLosAlumnos.Add(nuevoEstudiante);
                ActualizarListaAlumnos();
            };

            await Navigation.PushAsync(agregarEstudiantePage);
        }

        private async void editarButton_Clicked(object sender, EventArgs e)
        {
            var boton = sender as ImageButton;
            var estudiante = boton?.BindingContext as Estudiante;

            if (estudiante != null)
            {
                var firebaseClient = new FirebaseClient("https://eva4-8e56b-default-rtdb.firebaseio.com/");
                var editarEstudiantePage = new AgregarEstudiante(firebaseClient, estudiante);

                editarEstudiantePage.StudentUpdated += (s, estudianteActualizado) =>
                {
                    var index = TodosLosAlumnos.IndexOf(estudiante);
                    if (index >= 0)
                    {
                        TodosLosAlumnos[index] = estudianteActualizado;
                        ActualizarListaAlumnos();
                    }
                };

                await Navigation.PushAsync(editarEstudiantePage);
            }
        }

        private async void deshabilitarButton_Clicked(object sender, EventArgs e)
        {
            var boton = sender as ImageButton;
            var estudiante = boton?.BindingContext as Estudiante;

            if (estudiante != null)
            {
                bool confirmacion = await DisplayAlert(
                    "Confirmar",
                    $"¿Estás seguro de deshabilitar a {estudiante.Nombre}?",
                    "Sí",
                    "No"
                );

                if (confirmacion)
                {
                    estudiante.Activo = false;

                    var firebaseClient = new FirebaseClient("https://eva4-8e56b-default-rtdb.firebaseio.com/");
                    var estudianteKey = estudiante.Id;

                    await firebaseClient
                        .Child("Estudiantes")
                        .Child(estudianteKey)
                        .PutAsync(estudiante);

                    var index = TodosLosAlumnos.IndexOf(estudiante);
                    if (index >= 0)
                    {
                        TodosLosAlumnos[index] = estudiante;
                        ActualizarListaAlumnos();
                    }
                }
            }
        }
    }
}
