using ProyectoIngles.Data.Models;

namespace ProyectoIngles.Data.Controllers
{
    public class DocumentosController
    {
        readonly HttpClient httpClient = new();
        private readonly string puerto = "81";
        private readonly string ip = "localhost";
        public async Task<List<Documentos>> ConsultarTodo()
        {
            List<Documentos>? datosDocumentos = await httpClient.GetFromJsonAsync<List<Documentos>>("http://" + ip + ":" + puerto + "/api/documentos/consultar-todo");
            return datosDocumentos;
        }

        public async Task<Documentos> ConsultarDocumento(int ID)
        {
            List<Documentos>? datosDocumento = await httpClient.GetFromJsonAsync<List<Documentos>>("http://" + ip + ":" + puerto + "/api/documentos/consultar-documento?ID=" + ID);
            if (datosDocumento != null && datosDocumento.Count > 0)
            {
                return datosDocumento[0];
            }
            else
            {
                return null;
            }
        }

        public bool NuevoRegistro(string? nombre)
        {
            var url = string.Format("http://" + ip + ":" + puerto + "/api/documentos/nuevo-registro?&Nombre={0}", nombre);
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
            var url = string.Format("http://" + ip + ":" + puerto + "/api/documentos/actualizar-registro?Nombre={0}&ID={1}", nombre, id);
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
            List<Documentos> documento = new();
            documento = await ConsultarTodo();
            foreach (var item in documento)
            {
                if (item.Nombre == nombre)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> CompararDocumentos(Documentos documentoWeb)
        {
            Documentos documentoDB = await ConsultarDocumento(documentoWeb.ID);

            if (documentoDB != null)
            {
                if (documentoWeb.Nombre == documentoDB.Nombre)
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

        public async Task<Tuple<bool, List<string>, Input>> Validacion(string? nombre, string? documento_nombre = "")
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
            if (documento_nombre == "" || documento_nombre != nombre)
            {
                if (await ExisteNombre(nombre))
                {
                    error = true;
                    erroresMsg.Add("El documento ingresado ya existe en el sistema.");
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
