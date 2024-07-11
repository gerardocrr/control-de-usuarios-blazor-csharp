using Xceed.Words.NET;

namespace ProyectoIngles.Data.Controllers
{
    public class ConstanciasController
    {
        private readonly IWebHostEnvironment _env;
        public ConstanciasController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task GenerarConstancia(string? fecha, string? folio, string? nombre, string? num_control, string? carrera, string? documento, string? escuela, string? resultado_obtenido, string? nivel_ingles, string? fecha_2)
        {
            try
            {
                string rutaArchivo = Path.Combine(_env.WebRootPath, "docs/constancias", "Constancia_ejemplo.docx");
                // Carga el documento de Word
                var doc = DocX.Load(rutaArchivo);

                // Reemplaza el marcador con el nuevo nombre
                doc.ReplaceText("<fecha>", fecha);
                doc.ReplaceText("<folio>", folio);
                doc.ReplaceText("<nombre>", nombre);
                doc.ReplaceText("<no_control>", num_control);
                doc.ReplaceText("<carrera>", carrera);
                doc.ReplaceText("<documento>", documento);
                doc.ReplaceText("<escuela>", escuela);
                doc.ReplaceText("<resultado_obtenido>", resultado_obtenido);
                doc.ReplaceText("<nivel_ingles>", nivel_ingles);
                doc.ReplaceText("<fecha_2>", fecha_2);

                // Guarda los cambios en un nuevo archivo
                doc.SaveAs(Path.Combine(_env.WebRootPath, "docs/constancias", "CONSTANCIA - ") + nombre.ToUpper() + ".docx");
                
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        public void EliminarArchivo(string nombre)
        {
            string nombreArchivo = "CONSTANCIA - " + nombre.ToUpper() + ".docx";
            string rutaArchivo = Path.Combine(_env.WebRootPath, "docs/constancias", nombreArchivo);
            try
            {
                File.Delete(rutaArchivo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ocurrió un error:" + ex.Message);
            }
        }

        public Tuple<bool, List<string>, Input> Validacion(string? nombre, string? num_control, string? carrera, string? documento, string? escuela, string? nivel_ingles, string? resultado_obtenido)
        {
            bool error = false;
            List<string> erroresMsg = new List<string>();
            var clases = new Input();
            clases.Nombre = "form-control"; clases.Num_control = "form-control"; clases.Carrera = "form-control"; clases.Docuemnto = "form-control"; clases.Escuela = "form-control"; clases.Nivel_ingles = "form-control"; clases.Resultado_obtenido = "form-control";
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
            if (carrera == null || carrera == "")
            {
                error = true;
                erroresMsg.Add("El campo \"Carrera\" no puede estar vacío.");
                clases.Carrera = "form-control border-danger";
            }
            if (documento == null || documento == "")
            {
                error = true;
                erroresMsg.Add("El campo \"Documento entregadó\" no puede estar vacío.");
                clases.Docuemnto = "form-control border-danger";
            }
            if (escuela == null || escuela == "")
            {
                error = true;
                erroresMsg.Add("El campo \"Escuela certificadora\" no puede estar vacío.");
                clases.Escuela = "form-control border-danger";
            }            
            if (nivel_ingles == null || nivel_ingles == "")
            {
                error = true;
                erroresMsg.Add("El campo \"Nivel de inglés\" no puede estar vacío.");
                clases.Nivel_ingles = "form-control border-danger";
            }
            if (resultado_obtenido == null || resultado_obtenido == "")
            {
                error = true;
                erroresMsg.Add("El campo \"Resultado\" no puede estar vacío.");
                clases.Resultado_obtenido = "form-control border-danger";
            }

            var validacion = new Tuple<bool, List<string>, Input>(error, erroresMsg, clases);
            return validacion;
        }

        public class Input
        {
            public string? Nombre { get; set; }
            public string? Num_control { get; set; }
            public string? Carrera { get; set; }
            public string? Docuemnto { get; set; }
            public string? Escuela { get; set; }            
            public string? Nivel_ingles { get; set; }
            public string? Resultado_obtenido { get; set; }
        }
    }
}
