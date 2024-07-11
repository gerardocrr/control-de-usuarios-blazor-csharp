using ProyectoIngles.Data.Models;

namespace ProyectoIngles.Data.Controllers
{
    public class CarrerasController
    {
        readonly HttpClient httpClient = new();
        private readonly string puerto = "81";
        private readonly string ip = "localhost";
        public async Task<List<Carreras>> ConsultarTodo()
        {
            List<Carreras>? datosCarreras = await httpClient.GetFromJsonAsync<List<Carreras>>("http://" + ip + ":" + puerto + "/api/carreras/consultar-todo");
            return datosCarreras;
        }

        public async Task<Carreras> ConsultarCarrera(int ID)
        {
            List<Carreras>? datosCarrera = await httpClient.GetFromJsonAsync<List<Carreras>>("http://" + ip + ":" + puerto + "/api/carreras/consultar-carrera?ID=" + ID);
            if (datosCarrera != null && datosCarrera.Count > 0)
            {
                return datosCarrera[0];
            }
            else
            {
                return null;
            }
        }

        public bool NuevoRegistro(string? nombre)
        {
            var url = string.Format("http://" + ip + ":" + puerto + "/api/carreras/nuevo-registro?&Nombre={0}", nombre);
            HttpResponseMessage response = httpClient.PostAsync(url, null).Result;
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ActualizarRegistro(string? nombre, int id)
        {
            var url = string.Format("http://" + ip + ":" + puerto + "/api/carreras/actualizar-registro?Nombre={0}&ID={1}", nombre, id);
            HttpResponseMessage response = httpClient.PutAsync(url, null).Result;
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> ExisteNombre(string? nombre)
        {
            List<Carreras> carrera = new();
            carrera = await ConsultarTodo();
            foreach (var item in carrera)
            {
                if (item.Nombre == nombre)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> CompararCarreras(Carreras carreraWeb)
        {
            Carreras carreraDB = await ConsultarCarrera(carreraWeb.ID);

            if (carreraDB != null)
            {
                if (carreraWeb.Nombre == carreraDB.Nombre)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public async Task<Tuple<bool, List<string>, Input>> Validacion(string? nombre, string? carrera_nombre = "")
        {
            bool error = false;
            List<string> erroresMsg = new List<string>();
            var clases = new Input();
            clases.Nombre = "form-control";
            if (nombre == null || nombre == "")
            {
                error = true;
                erroresMsg.Add("El campo \"Nombre\" no puede estar vacío.");
                clases.Nombre = "form-control border-danger";
            }
            if (carrera_nombre == "" || carrera_nombre != nombre)
            {
                if (await ExisteNombre(nombre))
                {
                    error = true;
                    erroresMsg.Add("La carrera ingresada ya existe en el sistema.");
                    clases.Nombre = "form-control border-danger";
                }
            }
            var validacion = new Tuple<bool, List<string>, Input>(error, erroresMsg, clases);
            return validacion;
        }

        public class Input
        {
            public string? Nombre { get; set; }
        }
    }
}
