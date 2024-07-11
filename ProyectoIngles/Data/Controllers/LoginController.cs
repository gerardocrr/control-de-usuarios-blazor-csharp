using Microsoft.JSInterop;
using System.Security.Cryptography;
using System.Text;

namespace ProyectoIngles.Data.Controllers
{    
    public class LoginController
    {
        private readonly HttpClient httpClient = new();
        private readonly IJSRuntime _jsRuntime;
        private readonly string puerto = "81";
        private readonly string ip = "localhost";

        public LoginController(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public string ConsultarHash(string? Nombre)
        {
            var url = string.Format("http://" + ip + ":" + puerto + "/api/usuarios/consultar-hash?&Nombre={0}", Nombre);
            HttpResponseMessage response = httpClient.GetAsync(url).Result;
            var hash = response.Content.ReadAsStringAsync().Result;
            return hash;
        }

        public string HashPassword(string password)
        {
            byte[] salt = GenerateSalt();
            return HashPassword(password, salt);
        }

        private string HashPassword(string password, byte[] salt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] combined = Encoding.UTF8.GetBytes(password);
                byte[] combinedWithSalt = new byte[combined.Length + salt.Length];
                Buffer.BlockCopy(combined, 0, combinedWithSalt, 0, combined.Length);
                Buffer.BlockCopy(salt, 0, combinedWithSalt, combined.Length, salt.Length);
                
                byte[] hash = sha256.ComputeHash(combinedWithSalt);
                
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("x2"));
                }
                builder.Append(":");
                builder.Append(Convert.ToBase64String(salt));
                return builder.ToString();
            }
        }

        private byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        public bool VerifyPassword(string? userInput, string hashedPassword)
        {
            string[] parts = hashedPassword.Split(':');
            byte[] storedSalt = Convert.FromBase64String(parts[1]);            
            string hashedInput = HashPassword(userInput, storedSalt);
            
            return string.Equals(hashedInput, hashedPassword);
        }

        public async Task SetLocalStorage(string nombre)
        {
            string hashToken = HashPassword("accept");
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userToken", hashToken);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userName", nombre);
        }

        public async Task<bool> ReadLocalStorage()
        {
            string hashToken = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userToken");
            if (hashToken == null)
            {
                return false;
            }
            return VerifyPassword("accept", hashToken);
        }

        public async Task<string> ReadLocalStorageName()
        {
            string name = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userName");
            if (name == null)
            {
                return "";
            }
            return name;
        }

        public async Task DeleteLocalStorage()
        {
            await _jsRuntime.InvokeAsync<string>("localStorage.removeItem", "userToken");
            await _jsRuntime.InvokeAsync<string>("localStorage.removeItem", "userName");
        }
    }
}
