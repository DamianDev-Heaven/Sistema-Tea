using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Sistema_Tea.Filters
{
    public class SesionActivaAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var usuarioId = session.GetInt32("UsuarioID");

            // Si no está logueado y no está en la acción Login, redirige
            var controller = context.RouteData.Values["controller"]?.ToString();
            var action = context.RouteData.Values["action"]?.ToString();

            if (usuarioId == null && !(controller == "Cuenta" && action == "Login"))
            {
                context.Result = new RedirectToActionResult("Login", "Cuenta", null);
            }
            base.OnActionExecuting(context);
        }
    }
}