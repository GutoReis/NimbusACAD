using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NimbusACAD.Models.DB;
using NimbusACAD.Models.ViewModels;
using System.Net;
using System.Data.Entity;
using NimbusACAD.Identity.User;
using NimbusACAD.Identity.Role;

namespace NimbusACAD.Controllers
{
    public class RBACUsuarioController : Controller
    {
        private UserStore _userStore = new UserStore();
        private RoleStore _roleStore = new RoleStore();

        private NimbusAcad_DBEntities db = new NimbusAcad_DBEntities();

        // GET: RBACUsuario
        public ViewResult Index(string searchString)
        {
            var usuariosRBAC = from u in db.RBAC_Usuario select u;
            if (!String.IsNullOrEmpty(searchString))
            {
                usuariosRBAC = usuariosRBAC.Where(o => o.Negocio_Pessoa.Primeiro_Nome.ToUpper().Contains(searchString.ToUpper()) || o.Negocio_Pessoa.Sobrenome.ToUpper().Contains(searchString.ToUpper()));
            }
            return View(usuariosRBAC.ToList());
        }

        //GET: RBACUsuario/Detalhes/5
        public ActionResult Detalhes(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RBAC_Usuario usuario = db.RBAC_Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        //GET: RBACUsuario/Bloquear/5
        public ActionResult Bloquear(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RBAC_Usuario usuario = db.RBAC_Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(new BloquearUsuarioViewModel()
            {
                usuarioID = usuario.Usuario_ID,
                Bloqueado = usuario.Bloqueado.Value
            });
        }

        //POST: RBACUsuario/Bloquear/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Bloquear([Bind(Include = "usuarioID, Bloqueado")] BloquearUsuarioViewModel bloquearUsuario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(bloquearUsuario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(bloquearUsuario);
        }

        //GET: RBACUsuario/Deletar/5
        public ActionResult Deletar(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RBAC_Usuario usuario = db.RBAC_Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        //POST: RBACUsuario/Deletar/5
        [HttpPost, ActionName("Deletar")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletarConfirmacao(int id)
        {
            _userStore.DeleteUsuario(id);
            return RedirectToAction("Index");
        }

        //Perfil de acesso
        //GET: RBACUsuario/NovoPerfilDeAcesso/5
        public ActionResult NovoPerfilDeAcesso()
        {
            PopulatePerfilDropDownList();
            return View();
        }

        //POST: RBACUsuario/NovoPerfilDeAcesso/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NovoPerfilDeAcesso([Bind(Include = "UsuarioID, PerfilID")] VinculoPerfilUsuarioViewModel vpuvm)
        {
            if (ModelState.IsValid)
            {
                _roleStore.AddUsuarioPerfil(vpuvm);
                return RedirectToAction("Index");
            }
            PopulatePerfilDropDownList(vpuvm.PerfilID);
            return View(vpuvm);
        }

        //GET: RBACUsuario/RemoverPerfilDeAcesso/5
        public ActionResult RemoverPerfilDeAcesso(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RBAC_Link_Usuario_Perfil lup = db.RBAC_Link_Usuario_Perfil.Where(o => o.Usuario_ID == id).FirstOrDefault();
            if (lup == null)
            {
                return HttpNotFound();
            }
            return View(lup);
        }

        //POST: RBACUsuario/RemoverPerfilDeAcesso/5
        [HttpPost, ActionName("RemoverPerfilDeAcesso")]
        [ValidateAntiForgeryToken]
        public ActionResult RemoverPerfilDeAcessoConfirmacao(int id)
        {
            RBAC_Link_Usuario_Perfil lup = db.RBAC_Link_Usuario_Perfil.Where(o => o.Usuario_ID == id).FirstOrDefault();
            _roleStore.RemoveUsuarioPerfil(lup.Usuario_ID, lup.Perfil_ID);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void PopulatePessoaDropDownList(object selectedPessoa = null)
        {
            var pessoaQuery = from c in db.Negocio_Pessoa
                              orderby c.Primeiro_Nome
                              select c;
            ViewBag.Pessoa_ID = new SelectList(pessoaQuery,
                "Pessoa_ID", "Primeiro_Nome" + "Sobrenome", selectedPessoa);
        }

        private void PopulatePerfilDropDownList(object selectedPerfil = null)
        {
            var perfilQuery = from c in db.RBAC_Perfil
                              orderby c.Perfil_Nome
                              select c;
            ViewBag.Perfil_ID = new SelectList(perfilQuery,
                "Perfil_ID", "Perfil_Nome", selectedPerfil);
        }
    }
}