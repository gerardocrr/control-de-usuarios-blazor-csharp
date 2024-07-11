using ProyectoIngles.Data.Models;

namespace ProyectoIngles.Data.Controllers
{
    public class EscuelasController
    {
        readonly HttpClient httpClient = new();
        private readonly string puerto = "81";
        private readonly string ip = "localhost";
        public async Task<List<Escuelas>> ConsultarTodo()
        {
            List<Escuelas>? datosEscuelas = await httpClient.GetFromJsonAsync<List<Escuelas>>("http://" + ip + ":" + puerto + "/api/escuelas/consultar-todo");
            return datosEscuelas;
        }

        public async Task<Escuelas> ConsultarEscuela(int ID)
        {
            List<Escuelas>? datosEscuela = await httpClient.GetFromJsonAsync<List<Escuelas>>("http://" + ip + ":" + puerto + "/api/escuelas/consultar-escuela?ID=" + ID);
            if (datosEscuela != null && datosEscuela.Count > 0)
            {
                return datosEscuela[0];
            }
            else
            {
                return null;
            }
        }

        public bool NuevoRegistro(string? nombre)
        {
            var url = string.Format("http://" + ip + ":" + puerto + "/api/escuelas/nuevo-registro?&Nombre={0}", nombre);
            HttpResponseMessage response = httpClient.PostAsync(url, null).Result;
            if (response.IsSuccessStatusCode)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool ActualizarRegistro(string? nombre, int id)
        {
            var url = string.Format("http://" + ip + ":" + puerto + "/api/escuelas/actualizar-registro?Nombre={0}&ID={1}", nombre, id);
            HttpResponseMessage response = httpClient.PutAsync(url, null).Result;
            if (response.IsSuccessStatusCode)
            {
                var resultado = response.Content.ReadAsStringAsync().Result;
                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task<bool> ExisteNombre(string? nombre)
        {
            List<Escuelas> escuela = new();
            escuela = await ConsultarTodo();
            foreach (var item in escuela)
            {
                if (item.Nombre == nombre)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> CompararEscuelas(Escuelas escuelaoWeb)
        {
            Escuelas escuelaDB = await ConsultarEscuela(escuelaoWeb.ID);

            if (escuelaDB != null)
            {
                if (escuelaoWeb.Nombre == escuelaDB.Nombre)
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

        public async Task<Tuple<bool, List<string>, Input>> Validacion(string? nombre, string? escuela_nombre = "")
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
            if (escuela_nombre == "" || escuela_nombre != nombre)
            {
                if (await ExisteNombre(nombre))
                {
                    error = true;
                    erroresMsg.Add("La escuela ingresada ya existe en el sistema.");
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
