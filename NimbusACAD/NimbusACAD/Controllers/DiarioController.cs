﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NimbusACAD.Models.DB;
using NimbusACAD.Models.ViewModels;
using System.Net;
using System.Data.Entity;
using System.Threading.Tasks;

namespace NimbusACAD.Controllers
{
    public class DiarioController : Controller
    {
        private NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities();

        //GET: Diario/Index
        [RBAC]
        public ActionResult Index()
        {
            var name = User.Identity.Name;
            RBAC_Usuario usuario = db.RBAC_Usuario.Where(o => o.Username.Equals(name)).FirstOrDefault();
            int profID = usuario.Negocio_Pessoa.Negocio_Funcionario.FirstOrDefault().Funcionario_ID;

            DisciplinasProfessorVIewModel DPVM = new DisciplinasProfessorVIewModel();
            DPVM.ProfessorID = profID;
            ListaDisciplinaViewModel discVM;
            List<ListaDisciplinaViewModel> discList = new List<ListaDisciplinaViewModel>();
            foreach (var d in db.Negocio_Disciplina)
            {
                if (d.Funcionario_ID == profID)
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
            Negocio_Disciplina disciplina = db.Negocio_Disciplina.Where(o => o.Disciplina_ID == discID && o.Funcionario_ID == profID).FirstOrDefault();
            if (disciplina == null)
            {
                return HttpNotFound();
            }
            FrequenciaViewModel FVM = new FrequenciaViewModel();
            FVM.ProfessorID = disciplina.Funcionario_ID;
            FVM.DisciplinaID = disciplina.Disciplina_ID;
            FVM.MatriculasPresentes = disciplina.Negocio_Vinculo_Disciplina.Select(o => o.Matricula_ID);
            ICollection<ListaAlunosViewModel> mats = PopulateMatriculasList(disciplina);
            FVM.Matriculas = mats.Select(o => new SelectListItem
            {
                Value = o.MatriculaID.ToString(),
                Text = o.NomeAluno,
            }).ToList();
            return View(FVM);
        }

        //POST: Diario/Chamada
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Chamada([Bind(Include = "DisciplinaID, ProfessorID, DtAula, QtdeAulas, AulaMinistrada, Matriculas, MatriculasPresentes")]FrequenciaViewModel FVM)
        {
            if (ModelState.IsValid)
            {
                //Adicionando a Qtde de aulas dadas, ao Total Aulas Dadas
                Negocio_Disciplina disciplina = db.Negocio_Disciplina.Find(FVM.DisciplinaID);
                int totAula = disciplina.Tot_Aulas_Dadas.Value;
                disciplina.Tot_Aulas_Dadas = totAula + FVM.QtdeAulas;
                db.Entry(disciplina).State = EntityState.Modified;
                await db.SaveChangesAsync();

                Negocio_Frequencia frequencia;
                foreach (var al in FVM.MatriculasPresentes.ToList())
                {
                    //Salvando frequencia
                    frequencia = new Negocio_Frequencia();
                    frequencia.Disciplina_ID = FVM.DisciplinaID;
                    frequencia.Funcionario_ID = FVM.ProfessorID;
                    frequencia.Dt_Aula = FVM.DtAula;
                    frequencia.Qtde_Aula = FVM.QtdeAulas;
                    frequencia.Aula_Ministrada = FVM.AulaMinistrada;
                    frequencia.Matricula_ID = al;
                    db.Negocio_Frequencia.Add(frequencia);
                    await db.SaveChangesAsync();

                    //Atualizando a frequencia do aluno
                    Negocio_Vinculo_Disciplina vd = db.Negocio_Vinculo_Disciplina.Where(o => o.Disciplina_ID == FVM.DisciplinaID && o.Matricula_ID == al).FirstOrDefault();
                    vd.Frequencia = vd.Frequencia + frequencia.Qtde_Aula;
                    db.Entry(vd).State = EntityState.Modified;
                    await db.SaveChangesAsync();
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
            Negocio_Disciplina d = db.Negocio_Disciplina.Where(o => o.Disciplina_ID == discID && o.Funcionario_ID == profID).FirstOrDefault();
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
        public async Task<ActionResult> LancarNotas([Bind(Include = "DisciplinaID, DisciplinaNm, notas")]ListaLancarNotaViewModel lancamento)
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
                    await db.SaveChangesAsync();
                }

                return RedirectToAction("Index");
            }
            return View(lancamento);
        }

        //Corrigir notas de um único aluno
        //GET: Diario/CorrigirNota
        [RBAC]
        public ActionResult CorrigirNota(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Vinculo_Disciplina vinculo = db.Negocio_Vinculo_Disciplina.Find(id);
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

                return RedirectToAction("VerNotasDisciplina", new { discID = vinculo.Disciplina_ID });
            }

            return View(correcao);
        }
                
        //Listar presenças
        //GET: Diario/ListarPresencas
        [RBAC]
        public ViewResult ListarPresencas(/*DateTime? data, int? disccID*/)
        {
            //var frequencia = from f in db.Negocio_Frequencia select f;
            //if (data != null && disccID != 0)
            //{
            //    frequencia = frequencia.Where(o => o.Dt_Aula == data.Value.Date && o.Disciplina_ID == disccID);
            //    return View(frequencia.ToList());
            //}
            var freq = from f in db.Negocio_Frequencia select f;
            return View(freq.ToList());
        }

        //Remover presença de um único aluno
        //GET: Diario/RemoverPresenca
        [RBAC]
        public ActionResult RemoverPresenca(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Frequencia frequencia = db.Negocio_Frequencia.Find(id);
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
        public async Task<ActionResult> RemoverPresencaConfirmacao(int id)
        {
            Negocio_Frequencia frequencia = db.Negocio_Frequencia.Find(id);

            Negocio_Vinculo_Disciplina vinculo = db.Negocio_Vinculo_Disciplina.Where(o => o.Disciplina_ID == frequencia.Disciplina_ID && o.Matricula_ID == frequencia.Matricula_ID).FirstOrDefault();
            vinculo.Frequencia = vinculo.Frequencia.Value - frequencia.Qtde_Aula.Value;
            db.Entry(vinculo).State = EntityState.Modified;
            await db.SaveChangesAsync();

            db.Negocio_Frequencia.Remove(frequencia);
            await db.SaveChangesAsync();

            return RedirectToAction("ListarPresencas");
        }

        //Ver notas de todas as disciplinas de um único aluno - VerVinculoDisciplinaViewModel
        //GET: Diario/VerNotasAluno
        [RBAC]
        public ActionResult VerNotasAluno(/*int? matID*/)
        {
            //if (matID == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //Negocio_Matricula_Aluno aluno = db.Negocio_Matricula_Aluno.Find(matID);
            //if (aluno == null)
            //{
            //    return HttpNotFound();
            //}

            var name = User.Identity.Name;
            RBAC_Usuario usuario = db.RBAC_Usuario.Where(o => o.Username.Equals(name)).FirstOrDefault();
            var matID = usuario.Negocio_Pessoa.Negocio_Matricula_Aluno.Where(o => o.Ativo == true).FirstOrDefault().Matricula_ID;

            if (matID == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Matricula_Aluno aluno = db.Negocio_Matricula_Aluno.Find(matID);
            if (aluno == null)
            {
                return HttpNotFound();
            }

            ListarNotasAlunoViewModel LNAVM = new ListarNotasAlunoViewModel();
            LNAVM.MatriculaID = aluno.Matricula_ID;

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
            LNAVM.notas = lista;

            return View(LNAVM);
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

        private ICollection<ListaAlunosViewModel> PopulateMatriculasList(Negocio_Disciplina disciplina)
        {
            //var allAlunos = db.Negocio_Vinculo_Disciplina.Where(o => o.Disciplina_ID == disciplina.Disciplina_ID).FirstOrDefault().Negocio_Matricula_Aluno;
            var allVinculos = db.Negocio_Vinculo_Disciplina;
            var vm = new List<ListaAlunosViewModel>();
            foreach (var al in allVinculos.ToList())
            {
                if (al.Disciplina_ID == disciplina.Disciplina_ID)
                {
                    string PN = al.Negocio_Matricula_Aluno.Negocio_Pessoa.Primeiro_Nome;
                    string SN = al.Negocio_Matricula_Aluno.Negocio_Pessoa.Sobrenome;
                    string nome = PN + " " + SN;
                    vm.Add(new ListaAlunosViewModel
                    {
                        MatriculaID = al.Matricula_ID,
                        NomeAluno = nome
                    });
                }
            }
            //ViewBag.Matriculas = vm;
            return vm;
        }

        private void PopulateMatriculasDropDown(int discID, object selectedMatricula = null)
        {
            var matriculaQuery = from m in db.Negocio_Vinculo_Disciplina
                                 where m.Disciplina_ID == discID
                                 orderby m.Negocio_Matricula_Aluno.Negocio_Pessoa.Primeiro_Nome
                                 select m;
            ViewBag.Matriculas = new SelectList(matriculaQuery, "Matricula_ID", "Negocio_Matricula_Aluno.Negocio_Pessoa.Primeiro_Nome", selectedMatricula);
        }
    }
}