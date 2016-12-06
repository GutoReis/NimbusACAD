using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using NimbusACAD.Models.DB;
using NimbusACAD.Models.ViewModels;
using NimbusACAD.Identity.Role;

namespace NimbusACAD.Controllers
{
    public class RBACPerfilController : Controller
    {

        private RoleStore _roleStore = new RoleStore();
        private NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities();

        // GET: RBACPerfil
        [RBAC]
        public ActionResult Index(string searchString)
        {
            var perfisRBAC = from p in db.RBAC_Perfil select p;
            if (!String.IsNullOrEmpty(searchString))
            {
                perfisRBAC = perfisRBAC.Where(o => o.Perfil_Nome.ToUpper().Contains(searchString.ToUpper()));
            }
            return View(perfisRBAC.ToList());
        }

        // GET: RBACPerfil/Detalhes/5
        [RBAC]
        public ActionResult Detalhes(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RBAC_Perfil rBAC_Perfil = db.RBAC_Perfil.Find(id);

            if (rBAC_Perfil == null)
            {
                return HttpNotFound();
            }

            VerPerfilViewModel VPVM = new VerPerfilViewModel();
            VPVM.PerfilID = rBAC_Perfil.Perfil_ID;
            VPVM.PerfilNome = rBAC_Perfil.Perfil_Nome;

            List<RBAC_Permissao> tempList = new List<RBAC_Permissao>();
            RBAC_Permissao pTemp;

            foreach (var lpp in db.RBAC_Link_Perfil_Permissao)
            {
                if (lpp.Perfil_ID == rBAC_Perfil.Perfil_ID)
                {
                    pTemp = db.RBAC_Permissao.Find(lpp.Permissao_ID);
                    tempList.Add(pTemp);
                }
            }
            VPVM.Permissoes = tempList;

            return View(VPVM);
        }

        // GET: RBACPerfil/NovoPerfilDeAcesso
        [RBAC]
        public ActionResult NovoPerfilDeAcesso()
        {
            return View();
        }

        // POST: RBACPerfil/NovoPerfilDeAcesso
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult NovoPerfilDeAcesso([Bind(Include = "Perfil_ID,Perfil_Nome,Descricao")] RBAC_Perfil rBAC_Perfil)
        {
            if (ModelState.IsValid)
            {
                db.RBAC_Perfil.Add(rBAC_Perfil);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(rBAC_Perfil);
        }

        // GET: RBACPerfil/Editar/5
        [RBAC]
        public ActionResult Editar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RBAC_Perfil rBAC_Perfil = db.RBAC_Perfil.Find(id);
            if (rBAC_Perfil == null)
            {
                return HttpNotFound();
            }
            return View(rBAC_Perfil);
        }

        // POST: RBACPerfil/Editar/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "Perfil_ID,Perfil_Nome,Descricao")] RBAC_Perfil rBAC_Perfil)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rBAC_Perfil).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rBAC_Perfil);
        }

        // GET: RBACPerfil/Deletar/5
        [RBAC]
        public ActionResult Deletar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RBAC_Perfil rBAC_Perfil = db.RBAC_Perfil.Find(id);
            if (rBAC_Perfil == null)
            {
                return HttpNotFound();
            }
            return View(rBAC_Perfil);
        }

        // POST: RBACPerfil/Deletar/5
        [HttpPost, ActionName("Deletar")]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult DeletarConfirmado(int id)
        {
            _roleStore.DeletePerfil(id);
            return RedirectToAction("Index");
        }

        //GET: RBACPerfil/VincularPermissao/5
        [RBAC]
        public ActionResult VincularPermissao(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RBAC_Perfil rBAC_Perfil = db.RBAC_Perfil.Find(id);
            if (rBAC_Perfil == null)
            {
                return HttpNotFound();
            }
            VinculoPerfilPermissaoViewModel VPPVM = new VinculoPerfilPermissaoViewModel();
            VPPVM.PerfilID = rBAC_Perfil.Perfil_ID;

            PopulatePermissaoDropDownList();
            return View(VPPVM);
        }

        //POST: RBACPerfil/VincularPermissao/5
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult VincularPermissao([Bind(Include = "PerfilID, PermissaoID")]VinculoPerfilPermissaoViewModel VPPVM)
        {
            if (ModelState.IsValid)
            {
                RBAC_Link_Perfil_Permissao lpp = new RBAC_Link_Perfil_Permissao();
                lpp.Perfil_ID = VPPVM.PerfilID;
                lpp.Permissao_ID = VPPVM.PermissaoID;
                db.RBAC_Link_Perfil_Permissao.Add(lpp);
                db.SaveChanges();
                return RedirectToAction("Detalhes", new { id = VPPVM.PerfilID });
            }
            PopulatePermissaoDropDownList(VPPVM.PermissaoID);
            return View(VPPVM);
        }

        //Remover permissão
        //GET: RBACPerfil/RemoverPermissao/5
        [RBAC]
        public ActionResult RemoverPermissao(int? pfid, int? pmid)
        {
            if (pfid == null || pmid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RBAC_Link_Perfil_Permissao lpp = db.RBAC_Link_Perfil_Permissao.Where(o => o.Perfil_ID == pfid && o.Permissao_ID == pmid).FirstOrDefault();
            if (lpp == null)
            {
                return HttpNotFound();
            }
            return View(lpp);
        }

        //POST: RBACPerfil/RemoverPermissao/5
        [HttpPost, ActionName("RemoverPermissao")]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult RemoverPermissaoConfirmacao(int pfid, int pmid)
        {
            RBAC_Link_Perfil_Permissao lpp = db.RBAC_Link_Perfil_Permissao.Where(o => o.Perfil_ID == pfid && o.Permissao_ID == pmid).FirstOrDefault();
            db.RBAC_Link_Perfil_Permissao.Remove(lpp);
            db.SaveChanges();
            return RedirectToAction("Detalhes", new { id = pfid });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void PopulatePermissaoDropDownList(object selectedPermissao = null)
        {
            var permissaoQuery = from p in db.RBAC_Permissao
                                 orderby p.Permissao_Nome
                                 select p;
            ViewBag.Permissoes = new SelectList(permissaoQuery,
                "Permissao_ID", "Permissao_Nome", selectedPermissao);
        }
    }
}
