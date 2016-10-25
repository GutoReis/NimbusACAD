using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NimbusACAD.Models.DB;
using NimbusACAD.Models.ViewModels;

namespace NimbusACAD.Controllers
{
    public class ModuloController : Controller
    {
        private NimbusAcad_DBEntities db = new NimbusAcad_DBEntities();

        // GET: Modulo/Detalhes/5
        public ActionResult Detalhes(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Modulo negocio_Modulo = db.Negocio_Modulo.Find(id);
            if (negocio_Modulo == null)
            {
                return HttpNotFound();
            }

            VerModuloViewModel VMVM = new VerModuloViewModel();
            VMVM.CursoNM = negocio_Modulo.Negocio_Curso.Curso_Nome;
            VMVM.ModuloNome = negocio_Modulo.Modulo_Nome;
            VMVM.MaxAlunos = negocio_Modulo.Max_Alunos.Value;
            VMVM.TotAlunos = negocio_Modulo.Tot_Inscritos.Value;
            VMVM.CargaHoraria = negocio_Modulo.Carga_Horaria.Value;

            List<ListaDisciplinaViewModel> listTemp = new List<ListaDisciplinaViewModel>();
            Negocio_Disciplina dTemp;
            ListaDisciplinaViewModel discVM;

            foreach (var disciplina in db.Negocio_Disciplina)
            {
                dTemp = db.Negocio_Disciplina.Find(disciplina.Disciplina_ID);
                if (dTemp.Modulo_ID == negocio_Modulo.Modulo_ID)
                {
                    discVM = new ListaDisciplinaViewModel();
                    discVM.DisciplinaID = dTemp.Disciplina_ID;
                    discVM.DisciplinaNM = dTemp.Disciplina_Nome;
                    listTemp.Add(discVM);
                }
            }
            VMVM.disciplinas = listTemp;

            return View(negocio_Modulo);
        }

        // GET: Modulo/NovoModulo
        public ActionResult NovoModulo(int? id)
        {
            //ViewBag.Curso_ID = new SelectList(db.Negocio_Curso, "Curso_ID", "Curso_Nome");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.Curso_ID = id;
            return View();
        }

        // POST: Modulo/NovoModulo
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NovoModulo([Bind(Include = "Modulo_ID,Modulo_Nome,Curso_ID,Max_Alunos,Tot_Inscritos,Carga_Horaria")] Negocio_Modulo negocio_Modulo)
        {
            if (ModelState.IsValid)
            {
                negocio_Modulo.Tot_Inscritos = 0;
                db.Negocio_Modulo.Add(negocio_Modulo);
                db.SaveChanges();
                return RedirectToAction("NovaDisciplina", "Disciplina");
            }

            ViewBag.Curso_ID = new SelectList(db.Negocio_Curso, "Curso_ID", "Curso_Nome", negocio_Modulo.Curso_ID);
            return View(negocio_Modulo);
        }

        // GET: Modulo/Editar/5
        public ActionResult Editar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Modulo negocio_Modulo = db.Negocio_Modulo.Find(id);
            if (negocio_Modulo == null)
            {
                return HttpNotFound();
            }
            ViewBag.Curso_ID = new SelectList(db.Negocio_Curso, "Curso_ID", "Curso_Nome", negocio_Modulo.Curso_ID);
            return View(negocio_Modulo);
        }

        // POST: Modulo/Editar/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "Modulo_ID,Modulo_Nome,Curso_ID,Max_Alunos,Tot_Inscritos,Carga_Horaria")] Negocio_Modulo negocio_Modulo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(negocio_Modulo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Curso");
            }
            ViewBag.Curso_ID = new SelectList(db.Negocio_Curso, "Curso_ID", "Curso_Nome", negocio_Modulo.Curso_ID);
            return View(negocio_Modulo);
        }

        // GET: Modulo/Deletar/5
        public ActionResult Deletar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Modulo negocio_Modulo = db.Negocio_Modulo.Find(id);
            if (negocio_Modulo == null)
            {
                return HttpNotFound();
            }
            return View(negocio_Modulo);
        }

        // POST: Modulo/Deletar/5
        [HttpPost, ActionName("Deletar")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletarConfirmacao(int id)
        {
            Negocio_Modulo negocio_Modulo = db.Negocio_Modulo.Find(id);
            if (negocio_Modulo.Negocio_Disciplina != null || negocio_Modulo.Negocio_Vinculo_Modulo != null)
            {
                return RedirectToAction("Error");
            }
            db.Negocio_Modulo.Remove(negocio_Modulo);
            db.SaveChanges();
            return RedirectToAction("Index", "Curso");
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
