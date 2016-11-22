using System.Web.Mvc;
using NimbusACAD.Identity.User;
using NimbusACAD.Models.DB;
using NimbusACAD.Models.ViewModels;
using System.Net;

namespace NimbusACAD.Controllers
{
    [Authorize]
    public class GerenciarController : Controller
    {
        public GerenciarController() { }

        private UserStore _userStore = new UserStore();

        //
        //GET: /Account/Index
        public ActionResult Index(ManageMassageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMassageId.ChangePasswordSuccess ? "Senha alterada com sucesso" :
                message == ManageMassageId.Error ? "Ocorreu um erro." :
                "";

            //var RU = User.Identity as RBAC_Usuario;
            //string username = RU.Username;
            var username = User.Identity.Name;
            var usuario = _userStore.GetPerfilUsuario(username);

            var model = new PerfilDeUsuarioViewModel()
            {
                PessoaID = usuario.PessoaID, //NÃO APARECE NA VIEW
                PrimeiroNome = usuario.PrimeiroNome, //APARECE NA VIEW
                Sobrenome = usuario.Sobrenome, //APARECE NA VIEW
                CPF = usuario.CPF, //APARECE NA VIEW
                RG = usuario.RG, //APARECE NA VIEW
                Sexo = usuario.Sexo, //APARECE NA VIEW
                DtNascimento = usuario.DtNascimento, //APARECE NA VIEW
                TelPrincipal = usuario.TelPrincipal, //APARECE NA VIEW
                TelSecundario = usuario.TelSecundario, //APARECE NA VIEW
                Email = usuario.Email, //APARECE NA VIEW
                EndCompleto = usuario.EndCompleto, //APARECE NA VIEW
                UsuarioID = usuario.UsuarioID, //NÃO APARECE NA VIEW
                DtModif = usuario.DtModif, //APARECE NA VIEW
                Bloqueado = usuario.Bloqueado, //APARECE NA VIEW
                Perfil = usuario.Perfil //APARECE NA VIEW
            };
            return View(model);
        }

        //
        //GET: /Account/AlterarSenha
        public ActionResult AlterarSenha()
        {
            return View();
        }

        //
        //POST: /Account/AlterarSenha
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AlterarSenha(AlterarSenhaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var RU = User.Identity as RBAC_Usuario;
            string userEmail = RU.Username;
            if (userEmail == null)
            {
                return RedirectToAction("AlterarSenhaConfirmacao", "Account");
            }

            var result = _userStore.ChangePassword(RU.Usuario_ID, model);
            if (result.Equals(OperationStatus.Success))
            {
                return RedirectToAction("AlterarSenhaConfirmacao", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        //GET: /Account/AlterarSenhaConfirmacao
        public ActionResult AlterarSenhaConfirmacao()
        {
            return View();
        }

        //
        //GET: /Account/EditarPerfil/1
        public ActionResult EditarPerfil(string email)
        {
            if (email == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var usuario = _userStore.GetPerfilUsuario(email);
            if (usuario == null)
            {
                return HttpNotFound();
            }

            return View(new PerfilDeUsuarioViewModel()
            {
                PessoaID = usuario.PessoaID,
                PrimeiroNome = usuario.PrimeiroNome,
                Sobrenome = usuario.Sobrenome,
                CPF = usuario.CPF,
                RG = usuario.RG,
                Sexo = usuario.Sexo,
                DtNascimento = usuario.DtNascimento,
                TelPrincipal = usuario.TelPrincipal,
                TelSecundario = usuario.TelSecundario,
                Email = usuario.Email,
                EndCompleto = usuario.EndCompleto,
                UsuarioID = usuario.UsuarioID,
                DtModif = usuario.DtModif,
                Bloqueado = usuario.Bloqueado,
                Perfil = usuario.Perfil
            });
        }

        //
        //POST: /Account/EditarPerfil/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarPerfil([Bind(Include = "PessoaID, PrimeiroNome, Sobrenome, CPF, RG, Sexo, DtNascimento, TelPrincipal, TelOpcional, Email, EndCompleto, UsuarioID, DtModif, Bloqueado, Perfil")]
                PerfilDeUsuarioViewModel perfilUsuario)
        {
            if (ModelState.IsValid)
            {
                _userStore.UpdateContaUsuario(perfilUsuario);
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Algo deu errado.");
            return View();
        }

        //
        //GET: /Account/EditarEndereco/1
        public ActionResult EditarEndereco(int pessoaID)
        {
            if (pessoaID == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var endereco = _userStore.GetEndereco(pessoaID);
            if (endereco == null)
            {
                return HttpNotFound();
            }

            return View(new AlterarEnderecoViewModel()
            {
                CEP = endereco.CEP,
                Complemento = endereco.Complemento,
                Numero = endereco.Numero,
                PessoaID = endereco.PessoaID,
                Logradouro = endereco.Logradouro,
                Bairro = endereco.Bairro,
                Cidade = endereco.Cidade,
                Estado = endereco.Estado,
                Pais = endereco.Pais
            });
        }

        //
        //POST: /Account/EditarEndereco/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarEndereco([Bind(Include = "CEP, Complemento, Numero, PessoaID, Logradouro, Bairro, Cidade, Estado, Pais")] AlterarEnderecoViewModel endereco)
        {
            if (ModelState.IsValid)
            {
                _userStore.UpdateEndereco(endereco);
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Algo deu errado");
            return View();
        }

        #region Helpers

        public enum ManageMassageId
        {
            ChangePasswordSuccess,
            Error
        }

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