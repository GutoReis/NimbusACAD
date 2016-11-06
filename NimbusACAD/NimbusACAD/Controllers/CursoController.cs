using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using NimbusACAD.Models.DB;
using NimbusACAD.Models.ViewModels;

namespace NimbusACAD.Controllers
{
    public class CursoController : Controller
    {
        private NimbusAcad_DBEntities db = new NimbusAcad_DBEntities();

        // GET: Curso
        [RBAC]
        public ViewResult Index(string searchString)
        {
            var cursos = from c in db.Negocio_Curso select c;
            cursos = cursos.Include(n => n.Negocio_Funcionario);
            if (!String.IsNullOrEmpty(searchString))
            {
                cursos = cursos.Where(o => o.Curso_Nome.ToUpper().Contains(searchString.ToUpper()));
            }
            //var negocio_Curso = db.Negocio_Curso.Include(n => n.Negocio_Funcionario);
            return View(cursos.ToList());
        }

        // GET: Curso/Detalhes/5
        [RBAC]
        public ActionResult Detalhes(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Curso negocio_Curso = db.Negocio_Curso.Find(id);
            if (negocio_Curso == null)
            {
                return HttpNotFound();
            }
            VerCursoViewModel VCVM = new VerCursoViewModel();
            VCVM.CursoID = negocio_Curso.Curso_ID;
            VCVM.CursoNm = negocio_Curso.Curso_Nome;
            VCVM.Descricao = negocio_Curso.Descricao;
            VCVM.Periodo = negocio_Curso.Periodo;
            VCVM.CoordenadorNM = negocio_Curso.Negocio_Funcionario.Negocio_Pessoa.Primeiro_Nome + " " + negocio_Curso.Negocio_Funcionario.Negocio_Pessoa.Sobrenome;
            VCVM.CoordenadorEmail = negocio_Curso.Negocio_Funcionario.Negocio_Pessoa.Email;
            VCVM.CargaHoraria = negocio_Curso.Carga_Horaria.Value;

            List<ListaModulosViewModel> listTemp = new List<ListaModulosViewModel>();
            Negocio_Modulo mTemp;
            ListaModulosViewModel modVM;

            foreach (var modulo in db.Negocio_Modulo)
            {
                if (modulo.Curso_ID == negocio_Curso.Curso_ID)
                {
                    mTemp = db.Negocio_Modulo.Find(modulo.Modulo_ID);
                    modVM = new ListaModulosViewModel();
                    modVM.ModuloID = mTemp.Modulo_ID;
                    modVM.ModuloNome = mTemp.Modulo_Nome;
                    listTemp.Add(modVM);
                }
            }
            VCVM.Modulos = listTemp;

            return View(VCVM);
        }

        // GET: Curso/NovoCurso
        [RBAC]
        public ActionResult NovoCurso()
        {
            //ViewBag.Coordenador_ID = new SelectList(db.Negocio_Funcionario, "Funcionario_ID", "Funcionario_ID");
            PopulateFuncionarioDropDownList();
            return View();
        }

        // POST: Curso/NovoCurso
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult NovoCurso([Bind(Include = "Curso_ID,Curso_Nome,Descricao,Periodo,Coordenador_ID,Carga_Horaria")] Negocio_Curso negocio_Curso)
        {
            if (ModelState.IsValid)
            {
                db.Negocio_Curso.Add(negocio_Curso);
                db.SaveChanges();
                int cID = db.Negocio_Curso.Where(o => o.Curso_Nome == negocio_Curso.Curso_Nome).FirstOrDefault().Curso_ID;
                return RedirectToAction("Detalhes", "Curso", cID);
            }

            ViewBag.Coordenador_ID = new SelectList(db.Negocio_Funcionario, "Funcionario_ID", "Funcionario_ID", negocio_Curso.Coordenador_ID);
            PopulateFuncionarioDropDownList(negocio_Curso.Coordenador_ID);
            return View(negocio_Curso);
        }

        // GET: Curso/Editar/5
        [RBAC]
        public ActionResult Editar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Curso negocio_Curso = db.Negocio_Curso.Find(id);
            if (negocio_Curso == null)
            {
                return HttpNotFound();
            }
            ViewBag.Coordenador_ID = new SelectList(db.Negocio_Funcionario, "Funcionario_ID", "Funcionario_ID", negocio_Curso.Coordenador_ID);
            PopulateFuncionarioDropDownList();
            return View(negocio_Curso);
        }

        // POST: Curso/Editar/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "Curso_ID,Curso_Nome,Descricao,Periodo,Coordenador_ID,Carga_Horaria")] Negocio_Curso negocio_Curso)
        {
            if (ModelState.IsValid)
            {
                db.Entry(negocio_Curso).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Detlalhes", "Curso", negocio_Curso.Curso_ID);
            }
            ViewBag.Coordenador_ID = new SelectList(db.Negocio_Funcionario, "Funcionario_ID", "Funcionario_ID", negocio_Curso.Coordenador_ID);
            PopulateFuncionarioDropDownList(negocio_Curso.Coordenador_ID);
            return View(negocio_Curso);
        }

        // GET: Curso/Deletar/5
        [RBAC]
        public ActionResult Deletar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Curso negocio_Curso = db.Negocio_Curso.Find(id);
            if (negocio_Curso == null)
            {
                return HttpNotFound();
            }
            return View(negocio_Curso);
        }

        // POST: Curso/Deletar/5
        [HttpPost, ActionName("Deletar")]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult DeletarConfirmacao(int id)
        {
            Negocio_Curso negocio_Curso = db.Negocio_Curso.Find(id);
            if (negocio_Curso.Negocio_Modulo != null || negocio_Curso.Negocio_Matricula_Aluno != null)
            {
                return RedirectToAction("Error");
            }
            db.Negocio_Curso.Remove(negocio_Curso);
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

        private void PopulateFuncionarioDropDownList(object selectedFuncionario = null)
        {
            var funcionarioQuery = from f in db.Negocio_Funcionario
                                   orderby f.Negocio_Pessoa.Primeiro_Nome
                                   select f;
            ViewBag.Funcionario_ID = new SelectList(funcionarioQuery,
                "Funcionario_ID", "Negocio_Pessoa.Primeiro_Nome" + " " + "Negocio_Pessoa.Sobrenome", selectedFuncionario);
        }
    }
}
