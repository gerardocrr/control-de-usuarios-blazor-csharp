using ProyectoIngles.Data.Models;

namespace ProyectoIngles.Data.Controllers
{
    public class UsuariosController
    {
        readonly HttpClient httpClient = new();
        private readonly string puerto = "81";
        private readonly string ip = "localhost";

        public async Task<List<Usuarios>> ConsultarTodo()
        {
            List<Usuarios>? datosUsuarios = await httpClient.GetFromJsonAsync<List<Usuarios>>("http://" + ip + ":" + puerto + "/api/usuarios/consultar-todo");
            return datosUsuarios;
        }

        public async Task<Usuarios> ConsultarUsuario(string? Nombre)
        {
            List<Usuarios>? datosUsuarios = await httpClient.GetFromJsonAsync<List<Usuarios>>("http://" + ip + ":" + puerto + "/api/usuarios/consultar-todo");
            return datosUsuarios.Where(x => x.Nombre == Nombre).First();
        }

        private async Task<bool> ExisteNombre(string? nombre)
        {
            List<Usuarios> usuario = new();
            usuario = await ConsultarTodo();
            foreach (var item in usuario)
            {
                if (item.Nombre == nombre)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ActualizarNombre(string? Nombre, int Id)
        {
            var url = string.Format("http://" + ip + ":" + puerto + "/api/usuarios/actualizar-nombre?Nombre={0}&ID={1}", Nombre, Id);
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

        public bool ActualizarContraseña(string? Contraseña, int Id)
        {
            var url = string.Format("http://" + ip + ":" + puerto + "/api/usuarios/actualizar-contraseña?Contraseña={0}&ID={1}", Contraseña, Id);
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

        public async Task<Tuple<bool, List<string>, Input>> ValidacionNombre(string? nombre)
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
            if (await ExisteNombre(nombre))
            {
                error = true;
                erroresMsg.Add("Ya existe un usuario con este nombre.");
                clases.Nombre = "form-control border-danger";
            }
            var validacion = new Tuple<bool, List<string>, Input>(error, erroresMsg, clases);
            return validacion;
        }

        public Tuple<bool, List<string>, Input> ValidacionContraseña(string? contraseña_actual, string? nueva_contraseña, string? repetir_contraseña)
        {
            bool error = false;
            List<string> erroresMsg = new List<string>();
            var clases = new Input();
            clases.Contraseña_actual = "form-control"; clases.Nueva_contraseña = "form-control"; clases.Repetir_contraseña = "form-control";
            if (contraseña_actual == null || contraseña_actual == "")
            {
                error = true;
                erroresMsg.Add("El campo \"Contraseña actual\" no puede estar vacío.");
                clases.Contraseña_actual = "form-control border-danger";
            }
            if (nueva_contraseña == null || nueva_contraseña == "")
            {
                error = true;
                erroresMsg.Add("El campo \"Nueva contraseña\" no puede estar vacío.");
                clases.Nueva_contraseña = "form-control border-danger";
            }
            if (nueva_contraseña?.Length < 6)
            {
                error = true;
                erroresMsg.Add("Su nueva contraseña debe tener al menos 6 caracteres.");
                clases.Nueva_contraseña = "form-control border-danger";
            }
            if (repetir_contraseña == null || repetir_contraseña == "")
            {
                error = true;
                erroresMsg.Add("El campo \"Repetir contraseña\" no puede estar vacío.");
                clases.Repetir_contraseña = "form-control border-danger";
            }
            if (nueva_contraseña != repetir_contraseña)
            {
                error = true;
                erroresMsg.Add("Las contraseñas no coinciden.");
                clases.Repetir_contraseña = "form-control border-danger";
            }
            var validacion = new Tuple<bool, List<string>, Input>(error, erroresMsg, clases);
            return validacion;
        }

        public class Input
        {
            public string? Nombre { get; set; }
            public string? Contraseña_actual { get; set; }
            public string? Nueva_contraseña { get; set; }
            public string? Repetir_contraseña { get; set; }
        }
    }
}
