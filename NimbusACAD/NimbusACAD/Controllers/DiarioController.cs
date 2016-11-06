using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NimbusACAD.Models.DB;
using NimbusACAD.Models.ViewModels;
using System.Net;
using System.Data.Entity;

namespace NimbusACAD.Controllers
{
    public class DiarioController : Controller
    {
        private NimbusAcad_DBEntities db = new NimbusAcad_DBEntities();

        //GET: Diario/Index
        [RBAC]
        public ActionResult Index()
        {
            var usuario = User.Identity as RBAC_Usuario;
            int profID = usuario.Negocio_Pessoa.Negocio_Funcionario.FirstOrDefault().Funcionario_ID;

            DisciplinasProfessorVIewModel DPVM = new DisciplinasProfessorVIewModel();
            ListaDisciplinaViewModel discVM;
            List<ListaDisciplinaViewModel> discList = new List<ListaDisciplinaViewModel>();
            foreach (var d in db.Negocio_Disciplina)
            {
                if (d.Professor_ID == profID)
                {
                    discVM = new ListaDisciplinaViewModel();
                    discVM.DisciplinaID = d.Disciplina_ID;
                    discVM.DisciplinaNM = d.Disciplina_Nome;
                    discList.Add(discVM);
                }
            }
            DPVM.disciplinas = discList;

            return View(DPVM);
        }

        //Chamada - FrequenciaViewModel
        //GET: Diario/Chamada
        [RBAC]
        public ActionResult Chamada(int? discID, int? profID)
        {
            if (discID == null || profID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Disciplina disciplina = db.Negocio_Disciplina.Where(o => o.Disciplina_ID == discID && o.Professor_ID == profID).FirstOrDefault();
            if (disciplina == null)
            {
                return HttpNotFound();
            }
            PopulateMatriculasList(disciplina);
            return View(disciplina);
        }

        //POST: Diario/Chamada
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult Chamada([Bind(Include = "DisciplinaID, ProfessorID, DtAula, QtdeAulas, AulaMinistrada, Matriculas")]FrequenciaViewModel FVM)
        {
            if (ModelState.IsValid)
            {
                //Adicionando a Qtde de aulas dadas, ao Total Aulas Dadas
                Negocio_Disciplina disciplina = db.Negocio_Disciplina.Find(FVM.DisciplinaID);
                disciplina.Tot_Aulas_Dadas = disciplina.Tot_Aulas_Dadas + FVM.QtdeAulas;
                db.Entry(disciplina).State = EntityState.Modified;
                db.SaveChanges();

                Negocio_Frequencia frequencia;
                foreach (var al in FVM.Matriculas)
                {
                    //Salvando frequencia
                    frequencia = new Negocio_Frequencia();
                    frequencia.Disciplina_ID = FVM.DisciplinaID;
                    frequencia.Professor_ID = FVM.ProfessorID;
                    frequencia.Dt_Aula = FVM.DtAula;
                    frequencia.Qtde_Aula = FVM.QtdeAulas;
                    frequencia.Aula_Ministrada = FVM.AulaMinistrada;
                    frequencia.Matricula_Presente = al.MatriculaID;
                    db.Negocio_Frequencia.Add(frequencia);
                    db.SaveChanges();

                    //Atualizando a frequencia do aluno
                    Negocio_Vinculo_Disciplina vd = db.Negocio_Vinculo_Disciplina.Where(o => o.Disciplina_ID == FVM.DisciplinaID && o.Matricula_ID == al.MatriculaID).FirstOrDefault();
                    vd.Frequencia = vd.Frequencia + frequencia.Qtde_Aula;
                    db.Entry(vd).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            Negocio_Disciplina d = db.Negocio_Disciplina.Find(FVM.DisciplinaID);
            PopulateMatriculasList(d);
            return View(FVM);
        }

        //Lançar notas - LancarNotaViewModel
        //GET: Diario/LancarNotas
        [RBAC]
        public ActionResult LancarNotas(int? discID, int? profID)
        {
            if (discID == null || profID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Disciplina d = db.Negocio_Disciplina.Where(o => o.Disciplina_ID == discID && o.Professor_ID == profID).FirstOrDefault();
            if (d == null)
            {
                return HttpNotFound();
            }

            ListaLancarNotaViewModel LLNVM = new ListaLancarNotaViewModel();
            LLNVM.DisciplinaID = d.Disciplina_ID;
            LLNVM.DisciplinaNm = d.Disciplina_Nome;
            List<LancarNotaViewModel> listTemp = new List<LancarNotaViewModel>();
            LancarNotaViewModel LNVM;

            foreach (var vd in db.Negocio_Vinculo_Disciplina)
            {
                if (vd.Disciplina_ID == d.Disciplina_ID)
                {
                    LNVM = new LancarNotaViewModel();
                    LNVM.MatriculaID = vd.Negocio_Matricula_Aluno.Matricula_ID;
                    LNVM.VinculoID = vd.Vinculo_ID;
                    LNVM.AlunoNm = vd.Negocio_Matricula_Aluno.Negocio_Pessoa.Primeiro_Nome + " " + vd.Negocio_Matricula_Aluno.Negocio_Pessoa.Sobrenome;
                    LNVM.Nota1 = vd.Nota1.Value;
                    LNVM.Nota2 = vd.Nota2.Value;
                    listTemp.Add(LNVM);
                }
            }
            LLNVM.notas = listTemp;
            return View(LLNVM);
        }

        //POST: Diario/LancarNotas
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult LancarNotas([Bind(Include = "DisciplinaID, DisciplinaNm, notas")]ListaLancarNotaViewModel lancamento)
        {
            if (ModelState.IsValid)
            {
                Negocio_Vinculo_Disciplina vinculo;
                foreach (var vd in lancamento.notas)
                {
                    vinculo = db.Negocio_Vinculo_Disciplina.Where(o => o.Disciplina_ID == lancamento.DisciplinaID && o.Matricula_ID == vd.MatriculaID).FirstOrDefault();
                    vinculo.Nota1 = lancamento.notas.Where(o => o.MatriculaID == vinculo.Matricula_ID).FirstOrDefault().Nota1;
                    vinculo.Nota2 = lancamento.notas.Where(o => o.MatriculaID == vinculo.Matricula_ID).FirstOrDefault().Nota2;
                    vinculo.Media_Final = (lancamento.notas.Where(o => o.MatriculaID == vinculo.Matricula_ID).FirstOrDefault().Nota1 + lancamento.notas.Where(o => o.MatriculaID == vinculo.Matricula_ID).FirstOrDefault().Nota2) / 2;
                    db.Entry(vinculo).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("Index");
            }
            return View(lancamento);
        }

        //Corrigir notas de um único aluno
        //GET: Diario/CorrigirNota
        [RBAC]
        public ActionResult CorrigirNota(int? vdID)
        {
            if (vdID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Vinculo_Disciplina vinculo = db.Negocio_Vinculo_Disciplina.Find(vdID);
            if (vinculo == null)
            {
                return HttpNotFound();
            }
            LancarNotaViewModel LNVM = new LancarNotaViewModel();
            LNVM.MatriculaID = vinculo.Matricula_ID;
            LNVM.VinculoID = vinculo.Vinculo_ID;
            LNVM.AlunoNm = vinculo.Negocio_Matricula_Aluno.Negocio_Pessoa.Primeiro_Nome + " " + vinculo.Negocio_Matricula_Aluno.Negocio_Pessoa.Sobrenome;
            LNVM.Nota1 = vinculo.Nota1.Value;
            LNVM.Nota2 = vinculo.Nota2.Value;

            return View(LNVM);
        }
        
        //POST: Diario/CorrigirNota
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult CorrigirNota([Bind(Include = "MatriculaID, VinculoID, AlunoNm, Nota1, Nota2")]LancarNotaViewModel correcao)
        {
            if (ModelState.IsValid)
            {
                Negocio_Vinculo_Disciplina vinculo = db.Negocio_Vinculo_Disciplina.Find(correcao.VinculoID);
                vinculo.Nota1 = correcao.Nota1;
                vinculo.Nota2 = correcao.Nota2;
                vinculo.Media_Final = (correcao.Nota1 + correcao.Nota2) / 2;
                db.Entry(vinculo).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("VerNotasDisciplina", vinculo.Disciplina_ID);
            }

            return View(correcao);
        }
                
        //Listar presenças
        //GET: Diario/ListarPresencas
        [RBAC]
        public ViewResult ListarPresencas(DateTime data, int disccID)
        {
            var frequencia = from f in db.Negocio_Frequencia select f;
            if (data != null && disccID != 0)
            {
                frequencia = frequencia.Where(o => o.Dt_Aula == data.Date && o.Disciplina_ID == disccID);
                return View(frequencia.ToList());
            }
            return View();            
        }

        //Remover presença de um único aluno
        //GET: Diario/RemoverPresenca
        [RBAC]
        public ActionResult RemoverPresenca(int? freqID)
        {
            if (freqID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Frequencia frequencia = db.Negocio_Frequencia.Find(freqID);
            if (frequencia == null)
            {
                return HttpNotFound();
            }
            return View(frequencia);
        }

        //POST: Diario/RemoverPresenca
        [RBAC]
        [HttpPost, ActionName("RemoverPresenca")]
        [ValidateAntiForgeryToken]
        public ActionResult RemoverPresencaConfirmacao(int freqID)
        {
            Negocio_Frequencia frequencia = db.Negocio_Frequencia.Find(freqID);
            db.Negocio_Frequencia.Remove(frequencia);
            db.SaveChanges();
            return RedirectToAction("ListarPresencas");
        }

        //Adicionar presença para um único aluno
        //GET: Diario/AdicionarPresenca
        [RBAC]
        public ActionResult AdicionarPresenca(int? discID, int? profID)
        {
            if (discID == null || profID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Disciplina disciplina = db.Negocio_Disciplina.Where(o => o.Disciplina_ID == discID && o.Professor_ID == profID).FirstOrDefault();
            if (disciplina == null)
            {
                return HttpNotFound();
            }
            PopulateMatriculasDropDown(disciplina.Disciplina_ID);
            return View(disciplina);
        }

        //POST: Diario/AdicionarPresenca
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult AdicionarPresenca([Bind(Include = "Frequencia_ID, Disciplina_ID, Professor_ID, Dt_Aula, Qtde_Aula, Aula_Ministrada, Matricula_Presente")]Negocio_Frequencia frequencia)
        {
            if (ModelState.IsValid)
            {
                db.Negocio_Frequencia.Add(frequencia);
                db.SaveChanges();
                return RedirectToAction("VerNotasDisciplina", frequencia.Disciplina_ID);
            }
            PopulateMatriculasDropDown(frequencia.Disciplina_ID, frequencia.Matricula_Presente);
            return View(frequencia);
        }

        //Ver notas de todas as disciplinas de um único aluno - VerVinculoDisciplinaViewModel
        //GET: Diario/VerNotasAluno
        [RBAC]
        public ActionResult VerNotasAluno(int? matID)
        {
            if (matID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Matricula_Aluno aluno = db.Negocio_Matricula_Aluno.Find(matID);
            if (aluno == null)
            {
                return HttpNotFound();
            }

            List<NotasAlunoViewModel> lista = new List<NotasAlunoViewModel>();
            NotasAlunoViewModel nTemp;
            foreach (var vd in db.Negocio_Vinculo_Disciplina)
            {
                if (vd.Matricula_ID == aluno.Matricula_ID)
                {
                    nTemp = new NotasAlunoViewModel();
                    nTemp.VinculoID = vd.Vinculo_ID;
                    nTemp.ModuloNm = vd.Negocio_Disciplina.Negocio_Modulo.Modulo_Nome;
                    nTemp.DisciplinaNm = vd.Negocio_Disciplina.Disciplina_Nome;
                    nTemp.Professor = vd.Negocio_Disciplina.Negocio_Funcionario.Negocio_Pessoa.Primeiro_Nome + " " + vd.Negocio_Disciplina.Negocio_Funcionario.Negocio_Pessoa.Sobrenome;
                    nTemp.Nota1 = vd.Nota1.Value;
                    nTemp.Nota2 = vd.Nota2.Value;
                    nTemp.MediaFinal = vd.Media_Final.Value;
                    nTemp.Frequencia = vd.Frequencia.Value;
                    lista.Add(nTemp);
                }
            }
            return View(lista);
        }

        //Ver notas de todos os alunos em uma única disciplina - NotasViewModel
        //GET: Diario/VerNotasDisciplina
        [RBAC]
        public ActionResult VerNotasDisciplina(int? discID)
        {
            if (discID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Disciplina disciplina = db.Negocio_Disciplina.Find(discID);
            if (disciplina == null)
            {
                return HttpNotFound();
            }

            ListaNotasDisciplinaViewModel LNDVM = new ListaNotasDisciplinaViewModel();
            LNDVM.DisciplinaID = disciplina.Disciplina_ID;
            LNDVM.DisciplinaNm = disciplina.Disciplina_Nome;

            List<NotasDisciplinaViewModel> listTemp = new List<NotasDisciplinaViewModel>();
            NotasDisciplinaViewModel nTemp;
            foreach (var vd in db.Negocio_Vinculo_Disciplina)
            {
                if (vd.Disciplina_ID == disciplina.Disciplina_ID)
                {
                    nTemp = new NotasDisciplinaViewModel();
                    nTemp.VinculoID = vd.Vinculo_ID;
                    nTemp.AlunoNm = vd.Negocio_Matricula_Aluno.Negocio_Pessoa.Primeiro_Nome + " " + vd.Negocio_Matricula_Aluno.Negocio_Pessoa.Sobrenome;
                    nTemp.Nota1 = vd.Nota1.Value;
                    nTemp.Nota2 = vd.Nota2.Value;
                    nTemp.MediaFinal = vd.Media_Final.Value;
                    nTemp.Frequencia = vd.Frequencia.Value;
                    listTemp.Add(nTemp);
                }
            }
            LNDVM.notas = listTemp;
            return View(LNDVM);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void PopulateMatriculasList(Negocio_Disciplina disciplina)
        {
            //var allAlunos = db.Negocio_Vinculo_Disciplina.Where(o => o.Disciplina_ID == disciplina.Disciplina_ID).FirstOrDefault().Negocio_Matricula_Aluno;
            var allVinculos = db.Negocio_Vinculo_Disciplina;
            var vm = new List<ListaAlunosViewModel>();
            foreach (var al in allVinculos)
            {
                if (al.Disciplina_ID == disciplina.Disciplina_ID)
                {
                    vm.Add(new ListaAlunosViewModel
                    {
                        MatriculaID = al.Matricula_ID,
                        NomeAluno = al.Negocio_Matricula_Aluno.Negocio_Pessoa.Primeiro_Nome + " " + al.Negocio_Matricula_Aluno.Negocio_Pessoa.Sobrenome
                    });
                }
            }
            ViewBag.Matriculas = vm;
        }

        private void PopulateMatriculasDropDown(int discID, object selectedMatricula = null)
        {
            var matriculaQuery = from m in db.Negocio_Vinculo_Disciplina
                                 where m.Disciplina_ID == discID
                                 orderby m.Negocio_Matricula_Aluno.Negocio_Pessoa.Primeiro_Nome
                                 select m;
            ViewBag.Matriculas = new SelectList(matriculaQuery, "Matricula_ID", "Negocio_Matricula_Aluno.Negocio_Pessoa.Primeiro_Nome" + " " + "Negocio_Matricula_Aluno.Negocio_Pessoa.Sobrenome", selectedMatricula);
        }
    }
}