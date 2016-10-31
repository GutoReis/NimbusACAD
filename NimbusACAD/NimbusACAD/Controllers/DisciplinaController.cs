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
    public class DisciplinaController : Controller
    {
        private NimbusAcad_DBEntities db = new NimbusAcad_DBEntities();        

        // GET: Disciplina/Detalhes/5
        public ActionResult Detalhes(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Disciplina negocio_Disciplina = db.Negocio_Disciplina.Find(id);
            if (negocio_Disciplina == null)
            {
                return HttpNotFound();
            }

            VerDisciplinaViewModel VDVM = new VerDisciplinaViewModel();
            VDVM.DisciplinaID = negocio_Disciplina.Disciplina_ID;
            VDVM.ModuloNM = negocio_Disciplina.Negocio_Modulo.Modulo_Nome;
            VDVM.DisciplinaNM = negocio_Disciplina.Disciplina_Nome;
            VDVM.Descricao = negocio_Disciplina.Descricao;
            VDVM.ProfessorNM = negocio_Disciplina.Negocio_Funcionario.Negocio_Pessoa.Primeiro_Nome + " " + negocio_Disciplina.Negocio_Funcionario.Negocio_Pessoa.Sobrenome;
            VDVM.Email = negocio_Disciplina.Negocio_Funcionario.Negocio_Pessoa.Email;
            VDVM.CargaHoraria = negocio_Disciplina.Carga_Horaria.Value;

            List<ListaHorarioViewModel> listTemp = new List<ListaHorarioViewModel>();
            Negocio_Quadro_Horario hTemp;
            ListaHorarioViewModel horVM;

            foreach (var horario in db.Negocio_Quadro_Horario)
            {
                if (horario.Disciplina_ID == negocio_Disciplina.Disciplina_ID)
                {
                    hTemp = db.Negocio_Quadro_Horario.Find(horario.Quadro_Horario_ID);
                    horVM = new ListaHorarioViewModel();
                    horVM.horarioID = hTemp.Quadro_Horario_ID;
                    horVM.DiaSemana = hTemp.Dia_Semana;
                    horVM.HoraInicio = hTemp.Hora_Inicio.Value;
                    horVM.HoraFim = hTemp.Hora_Fim.Value;
                    listTemp.Add(horVM);
                }
            }
            VDVM.horariosAula = listTemp;

            return View(VDVM);
        }

        // GET: Disciplina/NovaDisciplina
        public ActionResult NovaDisciplina(int? mID)
        {
            //ViewBag.Professor_ID = new SelectList(db.Negocio_Funcionario, "Funcionario_ID", "Funcionario_ID");
            //ViewBag.Modulo_ID = new SelectList(db.Negocio_Modulo, "Modulo_ID", "Modulo_Nome");

            if (mID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.Modulo_ID = mID;
            PopulateFuncionarioDropDown();

            return View();
        }

        // POST: Disciplina/NovaDisciplina
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NovaDisciplina([Bind(Include = "Disciplina_ID,Modulo_ID,Disciplina_Nome,Descricao,Professor_ID,Tot_Aulas_Dadas,Carga_Horaria")] Negocio_Disciplina negocio_Disciplina)
        {
            if (ModelState.IsValid)
            {
                negocio_Disciplina.Tot_Aulas_Dadas = 0;
                db.Negocio_Disciplina.Add(negocio_Disciplina);
                db.SaveChanges();
                return RedirectToAction("Detalhes", "Modulo", negocio_Disciplina.Modulo_ID);
            }

            //ViewBag.Professor_ID = new SelectList(db.Negocio_Funcionario, "Funcionario_ID", "Funcionario_ID", negocio_Disciplina.Professor_ID);
            ViewBag.Modulo_ID = new SelectList(db.Negocio_Modulo, "Modulo_ID", "Modulo_Nome", negocio_Disciplina.Modulo_ID);
            PopulateFuncionarioDropDown(negocio_Disciplina.Professor_ID);
            return View(negocio_Disciplina);
        }

        // GET: Disciplina/Editar/5
        public ActionResult Editar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Disciplina negocio_Disciplina = db.Negocio_Disciplina.Find(id);
            if (negocio_Disciplina == null)
            {
                return HttpNotFound();
            }
            ViewBag.Professor_ID = new SelectList(db.Negocio_Funcionario, "Funcionario_ID", "Funcionario_ID", negocio_Disciplina.Professor_ID);
            ViewBag.Modulo_ID = new SelectList(db.Negocio_Modulo, "Modulo_ID", "Modulo_Nome", negocio_Disciplina.Modulo_ID);
            return View(negocio_Disciplina);
        }

        // POST: Disciplina/Editar/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "Disciplina_ID,Modulo_ID,Disciplina_Nome,Descricao,Professor_ID,Tot_Aulas_Dadas,Carga_Horaria")] Negocio_Disciplina negocio_Disciplina)
        {
            if (ModelState.IsValid)
            {
                db.Entry(negocio_Disciplina).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Detalhes", "Disciplina", negocio_Disciplina.Disciplina_ID);
            }
            ViewBag.Professor_ID = new SelectList(db.Negocio_Funcionario, "Funcionario_ID", "Funcionario_ID", negocio_Disciplina.Professor_ID);
            ViewBag.Modulo_ID = new SelectList(db.Negocio_Modulo, "Modulo_ID", "Modulo_Nome", negocio_Disciplina.Modulo_ID);
            return View(negocio_Disciplina);
        }

        // GET: Disciplina/Deletar/5
        public ActionResult Deletar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Disciplina negocio_Disciplina = db.Negocio_Disciplina.Find(id);
            if (negocio_Disciplina == null)
            {
                return HttpNotFound();
            }
            return View(negocio_Disciplina);
        }

        // POST: Disciplina/Deletar/5
        [HttpPost, ActionName("Deletar")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletarConfirmacao(int id)
        {
            Negocio_Disciplina negocio_Disciplina = db.Negocio_Disciplina.Find(id);
            db.Negocio_Disciplina.Remove(negocio_Disciplina);
            db.SaveChanges();
            return RedirectToAction("Detalhes", "Modulo", negocio_Disciplina.Modulo_ID);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void PopulateFuncionarioDropDown(object selectedFuncionario = null)
        {
            var funcionarioQuery = from f in db.Negocio_Funcionario
                                   orderby f.Negocio_Pessoa.Primeiro_Nome
                                   select f;
            ViewBag.Funcionario_ID = new SelectList(funcionarioQuery,
                "Funcionario_ID", "Negocio_Pessoa.Primeiro_Nome" + " " + "Negocio_Pessoa.Sobrenome", selectedFuncionario);
        }
    }
}
