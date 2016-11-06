using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using NimbusACAD.Models.DB;

namespace NimbusACAD.Controllers
{
    public class CargoController : Controller
    {
        private NimbusAcad_DBEntities db = new NimbusAcad_DBEntities();

        // GET: Cargo
        [RBAC]
        public ViewResult Index(string searchString)
        {
            var cargos = from c in db.Negocio_Tipo_Funcionario select c;
            if (!String.IsNullOrEmpty(searchString))
            {
                cargos = cargos.Where(o => o.Cargo.ToUpper().Contains(searchString.ToUpper()));
            }
            return View(cargos.ToList());
        }

        // GET: Cargo/Detalhes/5
        [RBAC]
        public ActionResult Detalhes(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Tipo_Funcionario negocio_Tipo_Funcionario = db.Negocio_Tipo_Funcionario.Find(id);
            if (negocio_Tipo_Funcionario == null)
            {
                return HttpNotFound();
            }
            return View(negocio_Tipo_Funcionario);
        }

        // GET: Cargo/NovoCargo
        [RBAC]
        public ActionResult NovoCargo()
        {
            return View();
        }

        // POST: Cargo/NovoCargo
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult NovoCargo([Bind(Include = "Tipo_Funcionario_ID,Cargo,Descricao")] Negocio_Tipo_Funcionario negocio_Tipo_Funcionario)
        {
            if (ModelState.IsValid)
            {
                db.Negocio_Tipo_Funcionario.Add(negocio_Tipo_Funcionario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(negocio_Tipo_Funcionario);
        }

        // GET: Cargo/Editar/5
        [RBAC]
        public ActionResult Editar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Tipo_Funcionario negocio_Tipo_Funcionario = db.Negocio_Tipo_Funcionario.Find(id);
            if (negocio_Tipo_Funcionario == null)
            {
                return HttpNotFound();
            }
            return View(negocio_Tipo_Funcionario);
        }

        // POST: Cargo/Editar/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "Tipo_Funcionario_ID,Cargo,Descricao")] Negocio_Tipo_Funcionario negocio_Tipo_Funcionario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(negocio_Tipo_Funcionario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(negocio_Tipo_Funcionario);
        }

        // GET: Cargo/Deletar/5
        [RBAC]
        public ActionResult Deletar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Tipo_Funcionario negocio_Tipo_Funcionario = db.Negocio_Tipo_Funcionario.Find(id);
            if (negocio_Tipo_Funcionario == null)
            {
                return HttpNotFound();
            }
            return View(negocio_Tipo_Funcionario);
        }

        // POST: Cargo/Deletar/5
        [HttpPost, ActionName("Deletar")]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult DeletarConfirmacao(int id)
        {
            Negocio_Tipo_Funcionario negocio_Tipo_Funcionario = db.Negocio_Tipo_Funcionario.Find(id);
            db.Negocio_Tipo_Funcionario.Remove(negocio_Tipo_Funcionario);
            db.SaveChanges();
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
