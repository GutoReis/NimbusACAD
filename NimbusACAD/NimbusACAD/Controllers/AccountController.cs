﻿using System.Web.Mvc;
using NimbusACAD.Identity.Security;
using NimbusACAD.Identity.User;
using NimbusACAD.Models.ViewModels;
using System.Web.Security;

namespace NimbusACAD.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController() { }

        private UserStore _userStore = new UserStore();
        private SignInManager _signInManager = new SignInManager();

        //
        //GET: /Account/
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        //POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = _signInManager.PasswordSignIn(model.Username, model.Password);
            switch (result)
            {
                case OperationStatus.Success:
                    //FormsAuthentication.SetAuthCookie(model.Username, true);
                    return RedirectToLocal(returnUrl);
                case OperationStatus.RequiresVerification:
                    ModelState.AddModelError("", "Email não confirmado, por favor confirme o email para continuar o login.");
                    return View(model);
                case OperationStatus.LockedOut:
                    ModelState.AddModelError("", "Usuário bloqueado, contate o administrador para mais informações.");
                    return View(model);
                case OperationStatus.Failure:
                default:
                    ModelState.AddModelError("", "Login inválido.");
                    return View(model);
            }
        }

        //
        //GET: /Account/ConfirmarEmail
        [AllowAnonymous]
        public ActionResult ConfirmarEmail(string email)
        {
            if (email == null)
            {
                return View("Error");
            }
            bool result = _signInManager.ConfirmEmail(email);
            return View(result ? "ConfirmarEmail" : "Error");
        }

        //
        //GET: /Account/EsqueceuSenha
        [AllowAnonymous]
        public ActionResult EsqueceuSenha()
        {
            return View();
        }

        //
        //POST: /Account/EsqueceuSenha
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> EsqueceuSenha(EsqueceuSenhaViewModel model)
        {
            if (ModelState.IsValid)
            {
                var usuario = _userStore.GetPerfilUsuario(model.Email);
                if (usuario == null || !(_userStore.GetUsuarioEmailVerificado(usuario.Email)))
                {
                    return View("EsqueceuSenhaConfirmacao");
                }

                string _tempAcess = _userStore.ForgotPassword(usuario.UsuarioID);
                await _signInManager.SendEmail(usuario.Email, "Senha alterada", "Aqui está sua nova senha: " + _tempAcess + ", por favor altere esta senha para sua senha própria");
                return View("EsqueceuSenhaCOnfirmacao");
            }
            return View(model);
        }

        //
        //GET: /Account/EsqueceuSenhaConfirmacao
        [AllowAnonymous]
        public ActionResult EsqueceuSenhaConfirmacao()
        {
            return View();
        }

        //
        //POST: /Account/LogOff
        public ActionResult LogOff()
        {
            _signInManager.LogOut();
            return RedirectToAction("Index", "Home");
        }


        #region Helpers

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private void AddErrors(OperationStatus result)
        {
            ModelState.AddModelError("", result.ToString());
        }

        #endregion
    }
}