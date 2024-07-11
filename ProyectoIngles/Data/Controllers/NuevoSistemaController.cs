using OfficeOpenXml;
using ProyectoIngles.Data.Models;

namespace ProyectoIngles.Data.Controllers
{
    public class NuevoSistemaController
    {
        readonly HttpClient httpClient = new();
        private readonly string puerto = "81";
        private readonly string ip = "localhost";
        private CarrerasController carrerasController = new CarrerasController();
        public async Task<List<AlumnosNuevoSistema>> ConsultarTodo()
        {
            List<AlumnosNuevoSistema>? alumnosNuevoSistemas = await httpClient.GetFromJsonAsync<List<AlumnosNuevoSistema>>("http://" + ip + ":" + puerto + "/api/nuevo-sistema/consultar-todo");
            return alumnosNuevoSistemas;
        }

        public async Task<AlumnosNuevoSistema> ConsultarAlumno(int ID)
        {
            List<AlumnosNuevoSistema>? datosAlumno = await httpClient.GetFromJsonAsync<List<AlumnosNuevoSistema>>("http://" + ip + ":" + puerto + "/api/nuevo-sistema/consultar-alumno?ID=" + ID);
            if (datosAlumno != null && datosAlumno.Count > 0)
            {
                return datosAlumno[0];
            }
            else
            {
                return null;
            }
        }

        public async Task<string> ConsultarFolio()
        {
            List<Folios>? datosFolio = await httpClient.GetFromJsonAsync<List<Folios>>("http://" + ip + ":" + puerto + "/api/nuevo-sistema/consultar-folio");
            if (datosFolio.Count == 0)
            {
                return "001";
            }
            else
            {
                return (int.Parse(datosFolio[0].Folio) + 1).ToString("000");
            }
        }        

        public bool NuevoRegistro(string? folio, string? nombre, string? no_control, int carreraId, int documento_entregadoId, string? nivel_ingles, int escuela_certificadoraId, string? puntaje, string sexo)
        {
            var url = string.Format("http://" + ip + ":" + puerto + "/api/nuevo-sistema/nuevo-registro?Folio={0}&Nombre={1}&No_control={2}&CarreraID={3}&Documento_entregadoID={4}&Nivel_ingles={5}&Escuela_certificadoraID={6}&Recibido_de_CE={7}&Puntaje={8}&Sexo={9}&Fecha_constancia_generada={10}", folio, nombre, no_control, carreraId, documento_entregadoId, nivel_ingles, escuela_certificadoraId, "NO RECIBIDO", puntaje, sexo, "N/A");
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

        public bool ActualizarRegistro(string? nombre, string? no_control, int carreraId, int documento_entregadoId, string? nivel_ingles, int escuela_certificadoraId, string puntaje, string sexo, int id)
        {
            var url = string.Format("http://" + ip + ":" + puerto + "/api/nuevo-sistema/actualizar-registro?Nombre={0}&No_control={1}&CarreraID={2}&Documento_entregadoID={3}&Nivel_ingles={4}&Escuela_certificadoraID={5}&Puntaje={6}&Sexo={7}&ID={8}", nombre, no_control, carreraId, documento_entregadoId, nivel_ingles, escuela_certificadoraId, puntaje, sexo, id);
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

        public bool ActualizarRecibido(string? opcion, int id)
        {
            var url = string.Format("http://" + ip + ":" + puerto + "/api/nuevo-sistema/actualizar-recibido?Opcion={0}&ID={1}", opcion, id);
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

        public bool ActualizarFolio(int folio, int id)
        {
            var url = string.Format("http://" + ip + ":" + puerto + "/api/nuevo-sistema/actualizar-folio?Folio={0}&ID={1}", folio.ToString("000"), id);
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

        public bool ActualizarFechaConstancia(string fecha, int id)
        {
            var url = string.Format("http://" + ip + ":" + puerto + "/api/nuevo-sistema/actualizar-fecha-constancia?Fecha_constancia_generada={0}&ID={1}", fecha, id);
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

        public async Task<bool> CompararAlumnos(AlumnosNuevoSistema alumnoWeb)
        {
            AlumnosNuevoSistema alumnoDB = await ConsultarAlumno(alumnoWeb.ID);

            if (alumnoDB != null)
            {
                if (alumnoWeb.Folio == alumnoDB.Folio && alumnoWeb.Nombre == alumnoDB.Nombre && alumnoWeb.Num_control == alumnoDB.Num_control && alumnoWeb.Carrera == alumnoDB.Carrera && alumnoWeb.Documento_entregado == alumnoDB.Documento_entregado && alumnoWeb.Nivel_ingles == alumnoDB.Nivel_ingles && alumnoWeb.Escuela_certificadora == alumnoDB.Escuela_certificadora && alumnoWeb.Recibido_de_CE == alumnoDB.Recibido_de_CE && alumnoWeb.Puntaje == alumnoDB.Puntaje && alumnoWeb.Sexo == alumnoDB.Sexo && alumnoWeb.Fecha_constancia_generada == alumnoDB.Fecha_constancia_generada)
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

        private async Task<bool> ExisteNumControl(string? num_control)
        {
            List<AlumnosNuevoSistema> alumno = new();
            alumno = await ConsultarTodo();
            foreach (var item in alumno)
            {
                if (item.Num_control == num_control)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<string> NombreExcel(string? nombreCarrera)
        {
            List<Carreras> carreras = new List<Carreras>();
            carreras = await carrerasController.ConsultarTodo();
            string nombreArchivo = "";
            if (nombreCarrera == "Todas")
            {
                nombreArchivo = "DATOS - NUEVO SISTEMA.xlsx";
            }
            else
            {
                foreach (var datosCarrera in carreras)
                {
                    if (nombreCarrera == datosCarrera.Nombre)
                    {
                        nombreArchivo = "DATOS - NUEVO SISTEMA - " + datosCarrera?.Nombre?.ToUpper() + ".xlsx";
                    }
                }
            }
            return nombreArchivo;
        }

        public async Task CrearExcel(IWebHostEnvironment _env, List<AlumnosNuevoSistema>? datos, string? nombreArchivo)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Crear un nuevo archivo de Excel
            var folderPath = Path.Combine(_env.WebRootPath, "docs/" + nombreArchivo);
            var archivo = new FileInfo(folderPath);
            using (var package = new ExcelPackage(archivo))
            {
                // Agregar una nueva hoja al libro
                ExcelWorksheet hoja = package.Workbook.Worksheets.Add("Datos");

                // Encabezados de columnas
                hoja.Cells[1, 1].Value = "Folio";
                hoja.Cells[1, 2].Value = "Nombre";
                hoja.Cells[1, 3].Value = "Num Control";
                hoja.Cells[1, 4].Value = "Sexo";
                hoja.Cells[1, 5].Value = "Carrera";
                hoja.Cells[1, 6].Value = "Doc entregado";
                hoja.Cells[1, 7].Value = "Nivel de ingles";
                hoja.Cells[1, 8].Value = "Escuela certificadora";
                hoja.Cells[1, 9].Value = "Fecha de la constancia";
                hoja.Cells[1, 10].Value = "Recibido de C.E";
                hoja.Cells[1, 11].Value = "Puntaje";

                var boldStyle = hoja.Cells[1, 1, 1, 11].Style.Font.Bold = true;

                // Llenar la tabla con los datos
                int fila = 2;
                foreach (var alumno in datos)
                {
                    hoja.Cells[fila, 1].Value = alumno.Folio;
                    hoja.Cells[fila, 2].Value = alumno.Nombre;
                    hoja.Cells[fila, 3].Value = alumno.Num_control;
                    hoja.Cells[fila, 4].Value = alumno.Sexo;
                    hoja.Cells[fila, 5].Value = alumno.Carrera;
                    hoja.Cells[fila, 6].Value = alumno.Documento_entregado;
                    hoja.Cells[fila, 7].Value = alumno.Nivel_ingles;
                    hoja.Cells[fila, 8].Value = alumno.Escuela_certificadora;
                    hoja.Cells[fila, 9].Value = alumno.Fecha_constancia_generada;
                    hoja.Cells[fila, 10].Value = alumno.Recibido_de_CE;
                    hoja.Cells[fila, 11].Value = alumno.Puntaje;
                    fila++;
                }

                // Ajustar el ancho de las columnas automáticamente
                hoja.Cells.AutoFitColumns();

                // Guardar el archivo
                await package.SaveAsync();
            }
        }

        public void EliminarExcel(IWebHostEnvironment _env, string nombreArchivo)
        {
            string rutaArchivo = Path.Combine(_env.WebRootPath, "docs", nombreArchivo);
            try
            {
                File.Delete(rutaArchivo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocurrió un error:" + ex.Message);
            }
        }

        public async Task<Tuple<bool, List<string>, Input>> Validacion(string? nombre, string? num_control, string? puntaje, string? alumno_num_control = "")
        {
            bool error = false;
            List<string> erroresMsg = new List<string>();
            var clases = new Input();
            clases.Nombre = "form-control"; clases.Num_control = "form-control"; clases.Puntaje = "form-control";
            if (nombre == null || nombre == "")
            {
                error = true;
                erroresMsg.Add("El campo \"Nombre\" no puede estar vacío.");
                clases.Nombre = "form-control border-danger";
            }
            if (num_control == null || num_control == "")
            {
                error = true;
                erroresMsg.Add("El campo \"Num. control\" no puede estar vacío.");
                clases.Num_control = "form-control border-danger";
            }
            if (alumno_num_control == "" || alumno_num_control != num_control)
            {
                if (await ExisteNumControl(num_control))
                {
                    error = true;
                    erroresMsg.Add("El número de control ingresado ya ha sido asignado a otro alumno.");
                    clases.Num_control = "form-control border-danger";
                }
            }
            if (puntaje == null || puntaje == "")
            {
                error = true;
                erroresMsg.Add("El campo \"Puntaje\" no puede estar vacío.");
                clases.Puntaje = "form-control border-danger";
            }
            else if (puntaje.Any(char.IsLetter) && puntaje != "N/A")
            {
                error = true;
                erroresMsg.Add("Ingrese un valor numérico.");
                clases.Puntaje = "form-control border-danger";
            }
            else if (puntaje.Length > 5)
            {
                error = true;
                erroresMsg.Add("Ingrese un número más pequeño.");
                clases.Puntaje = "form-control border-danger";
            }
            var validacion = new Tuple<bool, List<string>, Input>(error, erroresMsg, clases);
            return validacion;
        }

        public class Input
        {
            public string? Nombre { get; set; }
            public string? Num_control { get; set; }
            public string? Puntaje { get; set; }
        }
    }
}
