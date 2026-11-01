using System.Security.Claims;
using AvailabilitySite.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace AvailabilitySite.Controllers;

// Inicio de sesión con Gmail (Google como proveedor de identidad externo):
//  1) el sitio manda al usuario a la página de login de Google,
//  2) Google lo devuelve con un id_token que prueba quién es,
//  3) el sitio presenta ese id_token al AuthServer y recibe el token del sistema,
//  4) guarda la identidad del usuario en una cookie para las siguientes páginas.
public class CuentaController : Controller
{
    private readonly TokenClient _tokenClient;

    public CuentaController(TokenClient tokenClient)
    {
        _tokenClient = tokenClient;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    // 1) Redirige a la página de login de Google.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult LoginGoogle(string? returnUrl)
    {
        var propiedades = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(DespuesDeGoogle), new { returnUrl })
        };
        return Challenge(propiedades, "Google");
    }
// 2) y 3) Google ya devolvió al usuario: se cambia su id_token por el token del AuthServer.
    public async Task<IActionResult> DespuesDeGoogle(string? returnUrl)
    {
        var idTokenGoogle = await HttpContext.GetTokenAsync("id_token");
        var email = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue("email");

        string? accessToken = null;
        try
        {
            if (idTokenGoogle != null)
                accessToken = await _tokenClient.ObtenerTokenUsuarioAsync(idTokenGoogle);
        }
        catch (HttpRequestException)
        {
            // El AuthServer no responde: se informa abajo.
        }

        if (accessToken == null || email == null)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Error"] = "No se pudo iniciar sesión: el AuthServer no validó la cuenta de Google.";
            return RedirectToAction(nameof(Login), new { returnUrl });
        }

        // 4) La identidad del usuario en el sistema es su email de Gmail.
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, email),
            new(ClaimTypes.Email, email),
            new("access_token", accessToken)
        };
        var identidad = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identidad));

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }
}