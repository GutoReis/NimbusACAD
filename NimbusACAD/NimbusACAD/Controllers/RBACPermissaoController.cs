using System.Linq;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using NimbusACAD.Models.DB;
using NimbusACAD.Identity.Role;
using System;

namespace NimbusACAD.Controllers
{
    public class RBACPermissaoController : Controller
    {
        private PermissionStore _permissaoStore = new PermissionStore();
        private NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities();

        //GET: RBACPermissao
        [RBAC]
        public ViewResult Index(string searchString)
        {
            var permissoesRBAC = from p in db.RBAC_Permissao select p;
            if (!String.IsNullOrEmpty(searchString))
            {
                permissoesRBAC = permissoesRBAC.Where(o => o.Permissao_Nome.ToUpper().Contains(searchString.ToUpper()));
            }
            return View(permissoesRBAC.ToList());
        }

        // GET: RBACPermissao/NovaPermissao
        [RBAC]
        public ActionResult NovaPermissao()
        {
            return View();
        }

        // POST: RBACPermissao/NovaPermissao
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult NovaPermissao([Bind(Include = "Permissao_ID,Permissao_Nome")] RBAC_Permissao rBAC_Permissao)
        {
            if (ModelState.IsValid)
            {
                db.RBAC_Permissao.Add(rBAC_Permissao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rBAC_Permissao);
        }

        // GET: RBACPermissao/Editar/5
        [RBAC]
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
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "Permissao_ID,Permissao_Nome")] RBAC_Permissao rBAC_Permissao)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rBAC_Permissao).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rBAC_Permissao);
        }

        // GET: RBACPermissao/Deletar/5
        [RBAC]
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
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult DeletarConfirmacao(int id)
        {
            _permissaoStore.DeletePermissao(id);
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
    }
}
