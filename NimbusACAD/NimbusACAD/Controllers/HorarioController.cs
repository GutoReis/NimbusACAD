﻿using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using NimbusACAD.Models.DB;

namespace NimbusACAD.Controllers
{
    public class HorarioController : Controller
    {
        private NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities();

        // GET: Horario
        [RBAC]
        public ActionResult Index(string searchString)
        {
            var horarios = from h in db.Negocio_Quadro_Horario select h;
            horarios = horarios.Include(n => n.Negocio_Disciplina);
            if (!String.IsNullOrEmpty(searchString))
            {
                horarios = horarios.Where(o => o.Negocio_Disciplina.Disciplina_Nome.ToUpper().Contains(searchString.ToUpper()));
            }
            //var negocio_Quadro_Horario = db.Negocio_Quadro_Horario.Include(n => n.Negocio_Disciplina);
            return View(horarios.ToList());
        }

        // GET: Horario/DefinirHorario
        [RBAC]
        public ActionResult DefinirHorario(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Disciplina disciplina = db.Negocio_Disciplina.Find(id);
            if (disciplina == null)
            {
                return HttpNotFound();
            }
            Negocio_Quadro_Horario NQH = new Negocio_Quadro_Horario();
            NQH.Disciplina_ID = disciplina.Disciplina_ID;
            //ViewBag.Disciplina_ID = new SelectList(db.Negocio_Disciplina, "Disciplina_ID", "Disciplina_Nome");
            return View(NQH);
        }

        // POST: Horario/DefinirHorario
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult DefinirHorario([Bind(Include = "Quadro_Horario_ID,Disciplina_ID,Dia_Semana,Hora_Inicio,Hora_Fim")] Negocio_Quadro_Horario negocio_Quadro_Horario)
        {
            if (ModelState.IsValid)
            {
                db.Negocio_Quadro_Horario.Add(negocio_Quadro_Horario);
                db.SaveChanges();
                return RedirectToAction("Detalhes", "Disciplina", new { id = negocio_Quadro_Horario.Disciplina_ID });
            }

            ViewBag.Disciplina_ID = new SelectList(db.Negocio_Disciplina, "Disciplina_ID", "Disciplina_Nome", negocio_Quadro_Horario.Disciplina_ID);
            return View(negocio_Quadro_Horario);
        }

        // GET: Horario/Editar/5
        [RBAC]
        public ActionResult Editar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Quadro_Horario negocio_Quadro_Horario = db.Negocio_Quadro_Horario.Find(id);
            if (negocio_Quadro_Horario == null)
            {
                return HttpNotFound();
            }
            ViewBag.Disciplina_ID = new SelectList(db.Negocio_Disciplina, "Disciplina_ID", "Disciplina_Nome", negocio_Quadro_Horario.Disciplina_ID);
            return View(negocio_Quadro_Horario);
        }

        // POST: Horario/Editar/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "Quadro_Horario_ID,Disciplina_ID,Dia_Semana,Hora_Inicio,Hora_Fim")] Negocio_Quadro_Horario negocio_Quadro_Horario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(negocio_Quadro_Horario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Detalhes", "Disciplina", new { id = negocio_Quadro_Horario.Disciplina_ID });
            }
            ViewBag.Disciplina_ID = new SelectList(db.Negocio_Disciplina, "Disciplina_ID", "Disciplina_Nome", negocio_Quadro_Horario.Disciplina_ID);
            return View(negocio_Quadro_Horario);
        }

        // GET: Horario/Deletar/5
        [RBAC]
        public ActionResult Deletar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Quadro_Horario negocio_Quadro_Horario = db.Negocio_Quadro_Horario.Find(id);
            if (negocio_Quadro_Horario == null)
            {
                return HttpNotFound();
            }
            return View(negocio_Quadro_Horario);
        }

        // POST: Horario/Deletar/5
        [HttpPost, ActionName("Deletar")]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult DeletarConfirmacao(int id)
        {
            Negocio_Quadro_Horario negocio_Quadro_Horario = db.Negocio_Quadro_Horario.Find(id);
            db.Negocio_Quadro_Horario.Remove(negocio_Quadro_Horario);
            db.SaveChanges();
            return RedirectToAction("Detalhes", "Disciplina", new { id = negocio_Quadro_Horario.Disciplina_ID });
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
