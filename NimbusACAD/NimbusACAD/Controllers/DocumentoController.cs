using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NimbusACAD.Models.DB;

namespace NimbusACAD.Controllers
{
    public class DocumentoController : Controller
    {
        private NimbusAcad_DBEntities db = new NimbusAcad_DBEntities();

        // GET: Documento
        public ActionResult Index(string searchString)
        {
            var documentos = from d in db.Negocio_Documento select d;
            if (!String.IsNullOrEmpty(searchString))
            {
                documentos = documentos.Where(o => o.Documento_Nome.ToUpper().Contains(searchString.ToUpper()));
            }
            return View(documentos);
        }

        // GET: Documento/Detalhes/5
        public ActionResult Detalhes(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Documento negocio_Documento = db.Negocio_Documento.Find(id);
            if (negocio_Documento == null)
            {
                return HttpNotFound();
            }
            return View(negocio_Documento);
        }

        // GET: Documento/NovoDocumento
        public ActionResult NovoDocumento()
        {
            return View();
        }

        // POST: Documento/NovoDocumento
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NovoDocumento([Bind(Include = "Documento_ID,Documento_Nome")] Negocio_Documento negocio_Documento)
        {
            if (ModelState.IsValid)
            {
                db.Negocio_Documento.Add(negocio_Documento);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(negocio_Documento);
        }

        // GET: Documento/Editar5
        public ActionResult Editar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Documento negocio_Documento = db.Negocio_Documento.Find(id);
            if (negocio_Documento == null)
            {
                return HttpNotFound();
            }
            return View(negocio_Documento);
        }

        // POST: Documento/Editar/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "Documento_ID,Documento_Nome")] Negocio_Documento negocio_Documento)
        {
            if (ModelState.IsValid)
            {
                db.Entry(negocio_Documento).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(negocio_Documento);
        }

        // GET: Documento/Deletar/5
        public ActionResult Deletar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Documento negocio_Documento = db.Negocio_Documento.Find(id);
            if (negocio_Documento == null)
            {
                return HttpNotFound();
            }
            return View(negocio_Documento);
        }

        // POST: Documento/Deletar/5
        [HttpPost, ActionName("Deletar")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletarConfirmacao(int id)
        {
            Negocio_Documento negocio_Documento = db.Negocio_Documento.Find(id);
            db.Negocio_Documento.Remove(negocio_Documento);
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
