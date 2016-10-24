using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NimbusACAD.Models.DB;
using NimbusACAD.Identity.Role;

namespace NimbusACAD.Controllers
{
    public class RBACPermissaoController : Controller
    {
        private PermissionStore _permissaoStore = new PermissionStore();
        private NimbusAcad_DBEntities db = new NimbusAcad_DBEntities();

        // GET: RBACPermissao/NovaPermissao
        public ActionResult NovaPermissao()
        {
            return View();
        }

        // POST: RBACPermissao/NovaPermissao
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NovaPermissao([Bind(Include = "Permissao_ID,Permissao_Nome")] RBAC_Permissao rBAC_Permissao)
        {
            if (ModelState.IsValid)
            {
                db.RBAC_Permissao.Add(rBAC_Permissao);
                db.SaveChanges();
                return RedirectToAction("Index", "RBACPerfil");
            }

            return View(rBAC_Permissao);
        }

        // GET: RBACPermissao/Editar/5
        public ActionResult Editar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RBAC_Permissao rBAC_Permissao = db.RBAC_Permissao.Find(id);
            if (rBAC_Permissao == null)
            {
                return HttpNotFound();
            }
            return View(rBAC_Permissao);
        }

        // POST: RBACPermissao/Editar/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "Permissao_ID,Permissao_Nome")] RBAC_Permissao rBAC_Permissao)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rBAC_Permissao).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "RBACPerfil");
            }
            return View(rBAC_Permissao);
        }

        // GET: RBACPermissao/Deletar/5
        public ActionResult Deletar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RBAC_Permissao rBAC_Permissao = db.RBAC_Permissao.Find(id);
            if (rBAC_Permissao == null)
            {
                return HttpNotFound();
            }
            return View(rBAC_Permissao);
        }

        // POST: RBACPermissao/Deletar/5
        [HttpPost, ActionName("Deletar")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletarConfirmacao(int id)
        {
            _permissaoStore.DeletePermissao(id);
            return RedirectToAction("Index", "RBACPerfil");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
