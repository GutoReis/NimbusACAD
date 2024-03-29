﻿using System;
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

        private NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities();

        // GET: RBACUsuario
        [RBAC]
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
        [RBAC]
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
        [RBAC]
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
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult Bloquear([Bind(Include = "usuarioID, Bloqueado")] BloquearUsuarioViewModel bloquearUsuario)
        {
            if (ModelState.IsValid)
            {
                RBAC_Usuario RU = db.RBAC_Usuario.Find(bloquearUsuario.usuarioID);
                RU.Bloqueado = bloquearUsuario.Bloqueado;
                db.Entry(RU).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Detalhes", new { id = bloquearUsuario.usuarioID });
            }
            return View(bloquearUsuario);
        }

        //GET: RBACUsuario/Deletar/5
        [RBAC]
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
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult DeletarConfirmacao(int id)
        {
            _userStore.DeleteUsuario(id);
            return RedirectToAction("Index");
        }

        //Perfil de acesso
        //GET: RBACUsuario/NovoPerfilDeAcesso/5
        [RBAC]
        public ActionResult NovoPerfilDeAcesso(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RBAC_Usuario rBAC_Usuario = db.RBAC_Usuario.Find(id);
            if (rBAC_Usuario == null)
            {
                return HttpNotFound();
            }

            RBAC_Link_Usuario_Perfil linkUP = new RBAC_Link_Usuario_Perfil();
            linkUP.Usuario_ID = rBAC_Usuario.Usuario_ID;

            PopulatePerfilDropDownList();
            return View(linkUP);
        }

        //POST: RBACUsuario/NovoPerfilDeAcesso/5
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult NovoPerfilDeAcesso([Bind(Include = "Link_ID, Usuario_ID, Perfil_ID")] RBAC_Link_Usuario_Perfil linkUP)
        {
            if (ModelState.IsValid)
            {
                _roleStore.AddUsuarioPerfil(linkUP);
                return RedirectToAction("Detalhes", new { id = linkUP.Usuario_ID });
            }
            PopulatePerfilDropDownList(linkUP.Perfil_ID);
            return View(linkUP);
        }

        //GET: RBACUsuario/RemoverPerfilDeAcesso/5
        [RBAC]
        public ActionResult RemoverPerfilDeAcesso(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RBAC_Link_Usuario_Perfil lup = db.RBAC_Link_Usuario_Perfil.Where(o => o.Link_ID == id).FirstOrDefault();
            if (lup == null)
            {
                return HttpNotFound();
            }
            return View(lup);
        }

        //POST: RBACUsuario/RemoverPerfilDeAcesso/5
        [HttpPost, ActionName("RemoverPerfilDeAcesso")]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult RemoverPerfilDeAcessoConfirmacao(int id)
        {
            RBAC_Link_Usuario_Perfil lup = db.RBAC_Link_Usuario_Perfil.Where(o => o.Link_ID == id).FirstOrDefault();
            _roleStore.RemoveUsuarioPerfil(lup.Usuario_ID, lup.Perfil_ID);
            return RedirectToAction("Detalhes", new { id = lup.Usuario_ID });
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