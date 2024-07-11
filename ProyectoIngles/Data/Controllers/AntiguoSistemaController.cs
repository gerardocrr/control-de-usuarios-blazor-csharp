using OfficeOpenXml;
using ProyectoIngles.Data.Models;

namespace ProyectoIngles.Data.Controllers
{
    public class AntiguoSistemaController
    {
        readonly HttpClient httpClient = new();
        private readonly string puerto = "81";
        private readonly string ip = "localhost";
        private CarrerasController carrerasController = new CarrerasController();
        public async Task<List<AlumnosAntiguoSistema>> ConsultarTodo()
        {
            List<AlumnosAntiguoSistema>? alumnosAntiguoSistemas = await httpClient.GetFromJsonAsync<List<AlumnosAntiguoSistema>>("http://" + ip + ":" + puerto + "/api/antiguo-sistema/consultar-todo");
            return alumnosAntiguoSistemas;
        }

        public async Task<AlumnosAntiguoSistema> ConsultarAlumno(int ID)
        {
            List<AlumnosAntiguoSistema>? datosAlumno = await httpClient.GetFromJsonAsync<List<AlumnosAntiguoSistema>>("http://" + ip + ":" + puerto + "/api/antiguo-sistema/consultar-alumno?ID=" + ID);
            if (datosAlumno != null && datosAlumno.Count > 0)
            {
                return datosAlumno[0];
            }
            else
            {
                return null;
            }
        }

        public bool NuevoRegistro(string? Folio_acta, string? Num_control, string? Nombre, string? Calificacion, string? Carrera, string? Semestre, string? Documento_entregado, string? Emite, string? Ciclo, string? Fecha_entrega_acta, string Sexo)
        {
            var url = string.Format("http://" + ip + ":" + puerto + "/api/antiguo-sistema/nuevo-registro?Folio_acta={0}&Num_control={1}&Nombre={2}&Calificacion={3}&Carrera={4}&Semestre={5}&Documento_entregado={6}&Emite={7}&Ciclo={8}&Fecha_entrega_acta={9}&Sexo={10}", Folio_acta, Num_control, Nombre, Calificacion, Carrera, Semestre, Documento_entregado, Emite, Ciclo, Fecha_entrega_acta, Sexo);
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

        public bool ActualizarRegistro(string? Folio_acta, string? Num_control, string? Nombre, string? Calificacion, string? Carrera, string? Semestre, string? Documento_entregado, string? Emite, string? Ciclo, string? Fecha_entrega_acta, string? Sexo, int Id)
        {
            var url = string.Format("http://" + ip + ":" + puerto + "/api/antiguo-sistema/actualizar-registro?Folio_acta={0}&Num_control={1}&Nombre={2}&Calificacion={3}&Carrera={4}&Semestre={5}&Documento_entregado={6}&Emite={7}&Ciclo={8}&Fecha_entrega_acta={9}&Sexo={10}&ID={11}", Folio_acta, Num_control, Nombre, Calificacion, Carrera, Semestre, Documento_entregado, Emite, Ciclo, Fecha_entrega_acta, Sexo, Id);
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

        public bool EliminarRegistro(int id)
        {
            var url = string.Format("http://" + ip + ":" + puerto + "/api/antiguo-sistema/eliminar-registro?ID={0}", id);
            HttpResponseMessage response = httpClient.DeleteAsync(url).Result;
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> CompararAlumnos(AlumnosAntiguoSistema alumnoWeb)
        {
            AlumnosAntiguoSistema alumnoDB = await ConsultarAlumno(alumnoWeb.ID);

            if (alumnoDB != null)
            {
                if (alumnoWeb.Folio_acta == alumnoDB.Folio_acta && alumnoWeb.Num_control == alumnoDB.Num_control && alumnoWeb.Nombre == alumnoDB.Nombre && alumnoWeb.Calificacion == alumnoDB.Calificacion && alumnoWeb.Carrera == alumnoDB.Carrera && alumnoWeb.Semestre == alumnoDB.Semestre && alumnoWeb.Documento_entregado == alumnoDB.Documento_entregado && alumnoWeb.Emite == alumnoDB.Emite && alumnoWeb.Ciclo == alumnoDB.Ciclo && alumnoWeb.Fecha_entrega_acta == alumnoDB.Fecha_entrega_acta)
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

        public async Task<string> NombreExcel(string? nombreCarrera)
        {
            List<Carreras> carreras = new List<Carreras>();
            carreras = await carrerasController.ConsultarTodo();
            string nombreArchivo = "";
            if (nombreCarrera == "Todas")
            {
                nombreArchivo = "DATOS - ANTIGUO SISTEMA.xlsx";
            }
            else
            {
                foreach (var datosCarrera in carreras)
                {
                    if (nombreCarrera == datosCarrera.Nombre)
                    {
                        nombreArchivo = "DATOS - ANTIGUO SISTEMA - " + datosCarrera.Nombre.ToUpper() + ".xlsx";
                    }
                }
            }
            return nombreArchivo;
        }

        public async Task CrearExcel(IWebHostEnvironment _env, List<AlumnosAntiguoSistema>? datos, string? nombreArchivo)
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
                hoja.Cells[1, 2].Value = "Num Control";
                hoja.Cells[1, 3].Value = "Nombre";
                hoja.Cells[1, 4].Value = "Sexo";
                hoja.Cells[1, 5].Value = "Calificación";
                hoja.Cells[1, 6].Value = "Carrera";
                hoja.Cells[1, 7].Value = "Semestre";
                hoja.Cells[1, 8].Value = "Doc entregado";
                hoja.Cells[1, 9].Value = "Emite";
                hoja.Cells[1, 10].Value = "Ciclo";
                hoja.Cells[1, 11].Value = "Fecha entregada";

                var boldStyle = hoja.Cells[1, 1, 1, 11].Style.Font.Bold = true;

                // Llenar la tabla con los datos
                int fila = 2;
                foreach (var alumno in datos)
                {
                    hoja.Cells[fila, 1].Value = alumno.Folio_acta;
                    hoja.Cells[fila, 2].Value = alumno.Num_control;
                    hoja.Cells[fila, 3].Value = alumno.Nombre;
                    hoja.Cells[fila, 4].Value = alumno.Sexo;
                    hoja.Cells[fila, 5].Value = alumno.Calificacion;
                    hoja.Cells[fila, 6].Value = alumno.Carrera;
                    hoja.Cells[fila, 7].Value = alumno.Semestre;
                    hoja.Cells[fila, 8].Value = alumno.Documento_entregado;
                    hoja.Cells[fila, 9].Value = alumno.Emite;
                    hoja.Cells[fila, 10].Value = alumno.Ciclo;
                    hoja.Cells[fila, 11].Value = alumno.Fecha_entrega_acta;
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

        public Tuple<bool, List<string>, Input> Validacion(string? folio_acta, string? num_control, string? nombre, string? calificacion, string? carrera, string? semestre, string? documento_entregado, string? emite, string? ciclo, string? fecha_entregada)
        {
            bool error = false;
            List<string> erroresMsg = new List<string>();
            var clases = new Input();
            clases.Folio_acta = "form-control"; clases.Num_control = "form-control"; clases.Nombre = "form-control"; clases.Calificacion = "form-control"; clases.Carrera = "form-control"; clases.Semestre = "form-control"; clases.Doc_entregado = "form-control"; clases.Emite = "form-control"; clases.Ciclo = "form-control"; clases.Fecha_entregada = "form-control";
            if (folio_acta == null || folio_acta == "")
            {
                error = true;
                erroresMsg.Add("El campo \"Folio del acta\" no puede estar vacío.");
                clases.Folio_acta = "form-control border-danger";
            }
            if (num_control == null || num_control == "")
            {
                error = true;
                erroresMsg.Add("El campo \"Num. control\" no puede estar vacío.");
                clases.Num_control = "form-control border-danger";
            }
            if (nombre == null || nombre == "")
            {
                error = true;
                erroresMsg.Add("El campo \"Nombre\" no puede estar vacío.");
                clases.Nombre = "form-control border-danger";
            }
            if (calificacion == null || calificacion == "")
            {
                error = true;
                erroresMsg.Add("El campo \"Calificación\" no puede estar vacío.");
                clases.Calificacion = "form-control border-danger";
            }
            if (carrera == null || carrera == "")
            {
                error = true;
                erroresMsg.Add("El campo \"Carrera\" no puede estar vacío.");
                clases.Carrera = "form-control border-danger";
            }
            if (semestre == null || semestre == "")
            {
                error = true;
                erroresMsg.Add("El campo \"Semestre\" no puede estar vacío.");
                clases.Semestre = "form-control border-danger";
            }
            if (documento_entregado == null || documento_entregado == "")
            {
                error = true;
                erroresMsg.Add("El campo \"Documento entregado\" no puede estar vacío.");
                clases.Doc_entregado = "form-control border-danger";
            }
            if (emite == null || emite == "")
            {
                error = true;
                erroresMsg.Add("El campo \"Emite\" no puede estar vacío.");
                clases.Emite = "form-control border-danger";
            }
            if (ciclo == null || ciclo == "")
            {
                error = true;
                erroresMsg.Add("El campo \"Ciclo\" no puede estar vacío.");
                clases.Ciclo = "form-control border-danger";
            }
            if (fecha_entregada == null || fecha_entregada == "")
            {
                error = true;
                erroresMsg.Add("El campo \"Fecha entregada\" no puede estar vacío.");
                clases.Fecha_entregada = "form-control border-danger";
            }

            var validacion = new Tuple<bool, List<string>, Input>(error, erroresMsg, clases);
            return validacion;
        }

        public class Input
        {
            public string? Folio_acta { get; set; }
            public string? Num_control { get; set; }
            public string? Nombre { get; set; }
            public string? Calificacion { get; set; }
            public string? Carrera { get; set; }
            public string? Semestre { get; set; }
            public string? Doc_entregado { get; set; }
            public string? Emite { get; set; }
            public string? Ciclo { get; set; }
            public string? Fecha_entregada { get; set; }
        }
    }
}
