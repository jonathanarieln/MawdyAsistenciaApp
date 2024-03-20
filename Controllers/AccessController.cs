using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using MawdyAsistenciaApp.Models;
using AuthProject.Models;
using System.Text;
using System.Security.Cryptography;


namespace AuthProject.Controllers
{
    public class AccessController : Controller
    {
        public IActionResult Login()
        {
            ClaimsPrincipal claimUser = HttpContext.User;

            if (claimUser.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel modelLogin)
        {
            LoginModel nuevo = new LoginModel();
            bool bProbarCredencial = nuevo.ValidarUsuario(modelLogin.Email, HashPassword(modelLogin.PassWord));

            if (bProbarCredencial)
            {
                Console.WriteLine("Exito"); 
            }
            else
            {
                Console.WriteLine("Falso");
            }

            if (bProbarCredencial)
            { 
                List<Claim> claims = new List<Claim>() { 
                    new Claim(ClaimTypes.NameIdentifier, modelLogin.Email),
                    new Claim("OtherProperties","Example Role")
                
                };

                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,
                    CookieAuthenticationDefaults.AuthenticationScheme );

                AuthenticationProperties properties = new AuthenticationProperties() { 
                
                    AllowRefresh = true,
                    IsPersistent = modelLogin.KeepLoggedIn
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), properties);
            
                return RedirectToAction("Index", "Home");
            }



            ViewData["ValidateMessage"] = "Usuario o contraseña incorrecta";
            return View();
        }

        static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(bytes);

                // Convertir el hash a una cadena hexadecimal
                StringBuilder sb = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}
