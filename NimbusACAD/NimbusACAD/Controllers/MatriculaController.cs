﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using NimbusACAD.Models.DB;
using NimbusACAD.Models.ViewModels;
using NimbusACAD.Identity.User;
using NimbusACAD.Identity.Security;
using NimbusACAD.Identity.Email;
using NimbusACAD.Identity.Role;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;

namespace NimbusACAD.Controllers
{
    public class MatriculaController : Controller
    {
        private UserStore _userStore = new UserStore();
        private EmailService _emailService = new EmailService();
        private RoleStore _roleStore = new RoleStore();
        private NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities();

        //GET: Matricula
        [RBAC]
        public ViewResult Index(string searchString)
        {
            var matriculas = from m in db.Negocio_Matricula_Aluno select m;
            matriculas = matriculas.Include(o => o.Negocio_Pessoa);
            if (!String.IsNullOrEmpty(searchString))
            {
                matriculas = matriculas.Where(o => o.Negocio_Pessoa.Primeiro_Nome.ToUpper().Contains(searchString.ToUpper()));
            }
            return View(matriculas.ToList());
        }

        //Registrar Pessoa -> Registro de Documentos
        //GET: Matricula/NovaPessoa
        [RBAC]
        public ActionResult NovaPessoa()
        {
            PopulatePerfilDropDownList();
            return View();
        }
        
        //POST: Matricula/NovaPessoa
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NovaPessoa([Bind(Include = "PrimeiroNome, Sobrenome, CPF, RG, Sexo, DtNascimento, TelPrincipal, TelOpcional, Email, CEP, Logradouro, Complemento, Numero, Bairro, Cidade, Estado, Pais, PerfilID")] RegistrarComumViewModel novaPessoa)
        {
            if (ModelState.IsValid)
            {
                //Criando Pessoa
                int pID = _userStore.AddPessoa(novaPessoa);

                //Criando usuario
                RBAC_Usuario RU = new RBAC_Usuario();
                RU.Pessoa_ID = pID;
                RU.Username = novaPessoa.Email;

                //Gerando senha temporaria, e salt
                string tempToken = SecurityMethods.GenerateTempTokenAccess();
                string tempSalt = SecurityMethods.GenerateSalt();
                string tempPass = SecurityMethods.HashPasswordPBKDF2(tempToken, tempSalt);

                //Criando usuario
                RU.Senha_Hash = tempPass;
                RU.Salt = tempSalt;
                RU.Dt_Criacao = DateTime.Now.Date;
                RU.Dt_Ultima_Modif = DateTime.Now.Date;
                RU.Bloqueado = false;

                db.RBAC_Usuario.Add(RU);
                db.SaveChanges();

                //Enviando Email de senha gerada
                EmailMessage message = new EmailMessage(novaPessoa.Email, "Bem-vindo ao NimbusAcad!", "Olá, seja bem-vindo ao NimbusAcad \n Esta é sua senha: " + tempToken + "\nRecomenda-se que altere a senha para uma, com fácil memorização.");
                await _emailService.Send(message);

                //Enviando Email de confirmação de email
                var callbackUrl = Url.Action("ConfirmarEmail", "Account", new { novaPessoa.Email }, null);
                EmailMessage confirmacao = new EmailMessage(novaPessoa.Email, "Confirmar email", "Por favor, confirme seu email clicando neste link: <a href=\"" + callbackUrl + "\">Confirmar</a>");
                await _emailService.Send(confirmacao);

                //Assimilando perfil de acesso
                int uID = _userStore.GetUsuarioID(novaPessoa.Email);
                //VinculoPerfilUsuarioViewModel VPUVM = new VinculoPerfilUsuarioViewModel();
                //VPUVM.UsuarioID = uID;
                //VPUVM.PerfilID = novaPessoa.PerfilID;
                RBAC_Link_Usuario_Perfil linkUP = new RBAC_Link_Usuario_Perfil();
                linkUP.Usuario_ID = uID;
                linkUP.Perfil_ID = novaPessoa.PerfilID;
                _roleStore.AddUsuarioPerfil(linkUP);

                return RedirectToAction("RegistrarDocumento", "Matricula", new { id = pID });
            }
            PopulatePerfilDropDownList(novaPessoa.PerfilID);
            return View(novaPessoa);
        }

        //Registrar Documentos -> Registro Matricula
        //GET: Matricula/RegistrarDocumento
        [RBAC]
        public ActionResult RegistrarDocumento(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Pessoa negocio_Pessoa = db.Negocio_Pessoa.Find(id);
            if (negocio_Pessoa == null)
            {
                return HttpNotFound();
            }
            CurriculoViewModel CVM = new CurriculoViewModel();
            CVM.PessoaID = negocio_Pessoa.Pessoa_ID;
            PopulateDocumentosDropDownList();
            return View(CVM);
        }

        //POST: Matricula/RegistrarDocumento
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarDocumento([Bind(Include = "PessoaID, DocumentoID, OrgaoEmissor, DtEmissao, Cidade, Estado, Pais")] CurriculoViewModel novoCurriculo)
        {
            if (ModelState.IsValid)
            {
                Negocio_Curriculo curriculo = new Negocio_Curriculo();
                curriculo.Pessoa_ID = novoCurriculo.PessoaID;
                curriculo.Documento_ID = novoCurriculo.DocumentoID;
                curriculo.Orgao_Emissor = novoCurriculo.OrgaoEmissor;
                curriculo.Dt_Emissao = novoCurriculo.DtEmissao;
                curriculo.Cidade_Emissao = novoCurriculo.Cidade;
                curriculo.Estado_Emissao = novoCurriculo.Estado;
                curriculo.Pais_Emissao = novoCurriculo.Pais;
                db.Negocio_Curriculo.Add(curriculo);
                db.SaveChanges();

                return RedirectToAction("NovaMatricula", "Matricula", new { id = novoCurriculo.PessoaID });
            }
            PopulateDocumentosDropDownList(novoCurriculo.DocumentoID);
            return View(novoCurriculo);
        }

        //Registrar Matricula -> Matricula_Aluno.Deve_Documento ? Registro Doc_Devente : Index
        //GET: Matricula/NovaMatricula
        [RBAC]
        public ActionResult NovaMatricula(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Pessoa negocio_Pessoa = db.Negocio_Pessoa.Find(id);
            if (negocio_Pessoa == null)
            {
                return HttpNotFound();
            }
            RegistrarAlunoViewModel RAVM = new RegistrarAlunoViewModel();
            RAVM.PessoaID = negocio_Pessoa.Pessoa_ID;
            ViewBag.CursosList = db.Negocio_Curso;
            PopulateCursoDropDownList();
            //PopulateModuloDropDownList(cID);
            //PopulateModuloMatriculaDropDownList(cID);
            return View(RAVM);
        }

        //POST: Matricula/NovaMatricula
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> NovaMatricula([Bind(Include = "PessoaID, CursoID, Ano, DeveDocumento")] RegistrarAlunoViewModel novoAluno)
        {
            if (ModelState.IsValid)
            {
                Negocio_Matricula_Aluno mat = new Negocio_Matricula_Aluno();
                mat.Pessoa_ID = novoAluno.PessoaID;
                mat.Curso_ID = novoAluno.CursoID;
                mat.Ano = novoAluno.Ano;
                mat.Ativo = true;
                mat.Deve_Documento = novoAluno.DeveDocumento;
                db.Negocio_Matricula_Aluno.Add(mat);
                await db.SaveChangesAsync();

                //int matID = db.Negocio_Matricula_Aluno.Where(o => o.Pessoa_ID == novoAluno.PessoaID && o.Curso_ID == novoAluno.CursoID).FirstOrDefault().Matricula_ID;
                //Negocio_Vinculo_Modulo modulo = new Negocio_Vinculo_Modulo();
                //modulo.Modulo_ID = novoAluno.ModuloID;
                //modulo.Matricula_ID = matID;
                //modulo.Status_Vinculo = "Em Curso";
                //db.Negocio_Vinculo_Modulo.Add(modulo);
                //await db.SaveChangesAsync();

                //Negocio_Vinculo_Disciplina disciplina;
                //foreach (var d in db.Negocio_Disciplina.ToList())
                //{
                //    if (d.Modulo_ID == novoAluno.ModuloID)
                //    {
                //        disciplina = new Negocio_Vinculo_Disciplina();
                //        disciplina.Disciplina_ID = d.Disciplina_ID;
                //        disciplina.Matricula_ID = matID;
                //        disciplina.Frequencia = 0;
                //        disciplina.Num_Chamada = 0;
                //        disciplina.Nota1 = 0;
                //        disciplina.Nota2 = 0;
                //        disciplina.Media_Final = 0;
                //        db.Negocio_Vinculo_Disciplina.Add(disciplina);
                //        await db.SaveChangesAsync();
                //    }
                //}
                //if (mat.Deve_Documento.Value)
                //{
                //    return RedirectToAction("DeveDocumento", "Matricula", new { id = matID });
                //}
                //return RedirectToAction("Index");
                return RedirectToAction("VincularMatModulo", "Matricula", new { id = mat.Matricula_ID });
            }
            PopulateCursoDropDownList(novoAluno.CursoID);
            //PopulateModuloDropDownList(novoAluno.ModuloID);
            return View(novoAluno);
        }

        //GET: Matricula/VincularMatModulo
        [RBAC]
        public ActionResult VincularMatModulo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Matricula_Aluno aluno = db.Negocio_Matricula_Aluno.Find(id);
            if (aluno == null)
            {
                return HttpNotFound();
            }
            //Negocio_Vinculo_Modulo NVM = new Negocio_Vinculo_Modulo();
            //NVM.Matricula_ID = aluno.Matricula_ID;
            CriarVinculoModuloViewModel CVMVM = new CriarVinculoModuloViewModel();
            CVMVM.MatriculaID = aluno.Matricula_ID;
            PopulateModuloMatriculaDropDownList(aluno.Curso_ID);

            return View(CVMVM);
        }

        //POST: Matricula/VincularMatModulo
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VincularMatModulo([Bind(Include = "ModuloID, MatriculaID, StatusVinculo")]CriarVinculoModuloViewModel CVMVM)
        {
            if (ModelState.IsValid)
            {
                Negocio_Vinculo_Modulo NVM = new Negocio_Vinculo_Modulo();
                NVM.Matricula_ID = CVMVM.MatriculaID;
                NVM.Modulo_ID = CVMVM.ModuloID;
                NVM.Status_Vinculo = CVMVM.StatusVinculo;
                db.Negocio_Vinculo_Modulo.Add(NVM);
                await db.SaveChangesAsync();

                Negocio_Vinculo_Disciplina disciplina;
                foreach (var d in db.Negocio_Disciplina.ToList())
                {
                    if (d.Modulo_ID == CVMVM.ModuloID)
                    {
                        disciplina = new Negocio_Vinculo_Disciplina();
                        disciplina.Disciplina_ID = d.Disciplina_ID;
                        disciplina.Matricula_ID = CVMVM.MatriculaID;
                        disciplina.Frequencia = 0;
                        disciplina.Num_Chamada = 0;
                        disciplina.Nota1 = 0;
                        disciplina.Nota2 = 0;
                        disciplina.Media_Final = 0;
                        db.Negocio_Vinculo_Disciplina.Add(disciplina);
                        await db.SaveChangesAsync();
                    }
                }
                bool DeveDoc = db.Negocio_Matricula_Aluno.Find(CVMVM.MatriculaID).Deve_Documento.Value;
                if (DeveDoc)
                {
                    return RedirectToAction("DeveDocumento", "Matricula", new { id = CVMVM.MatriculaID });
                }
                return RedirectToAction("Index");
            }
            int cursoID = db.Negocio_Matricula_Aluno.Find(CVMVM.MatriculaID).Curso_ID;
            PopulateModuloMatriculaDropDownList(cursoID, CVMVM.ModuloID);
            return View(CVMVM);
        }

        //GET: Matricula/DeveDocumento
        [RBAC]
        public ActionResult DeveDocumento(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Matricula_Aluno mat = db.Negocio_Matricula_Aluno.Find(id);
            if (mat == null)
            {
                return HttpNotFound();
            }
            Negocio_Doc_Devente NDD = new Negocio_Doc_Devente();
            NDD.Matricula_ID = mat.Matricula_ID;
            PopulateDocumentosDropDownList();
            return View(NDD);
        }

        //POST: Matricula/DeveDocumento
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult DeveDocumento([Bind(Include = "Doc_Devente_ID, Documento_ID, Matricula_ID")]Negocio_Doc_Devente docDevente)
        {
            if (ModelState.IsValid)
            {
                db.Negocio_Doc_Devente.Add(docDevente);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            PopulateCursoDropDownList(docDevente.Documento_ID);
            return View(docDevente);
        }

        //Vincular à modulo (a todas as disciplinas do modulo)
        //GET: Matricula/VincularModulo
        [RBAC]
        public ActionResult VincularModulo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Matricula_Aluno aluno = db.Negocio_Matricula_Aluno.Find(id);
            if (aluno == null)
            {
                return HttpNotFound();
            }
            Negocio_Vinculo_Modulo NVM = new Negocio_Vinculo_Modulo();
            NVM.Matricula_ID = aluno.Matricula_ID;
            PopulateModuloDropDownList(aluno.Matricula_ID);
            return View(NVM);
        }

        //POST: Matricula/VincularModulo
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VincularModulo([Bind(Include = "Vinculo_ID, Modulo_ID, Matricula_ID, Status_Vinculo")] Negocio_Vinculo_Modulo vm)
        {
            if (ModelState.IsValid)
            {
                int c = 0;
                foreach (var st in db.Negocio_Vinculo_Modulo)
                {
                    if (st.Matricula_ID == vm.Matricula_ID)
                    {
                        if (st.Status_Vinculo == "Matricula Trancada" && st.Status_Vinculo == "Reprovado")
                        {
                            c += 1;
                        }
                    }
                }
                if (c > 0)
                {
                    return RedirectToAction("Error");
                }
                //vm.Status_Vinculo = "Em Curso";
                db.Negocio_Vinculo_Modulo.Add(vm);
                await db.SaveChangesAsync();

                Negocio_Vinculo_Disciplina vd;
                foreach (var d in db.Negocio_Disciplina.ToList())
                {
                    if (d.Modulo_ID == vm.Modulo_ID)
                    {
                        vd = new Negocio_Vinculo_Disciplina();
                        vd.Disciplina_ID = d.Disciplina_ID;
                        vd.Matricula_ID = vm.Matricula_ID;
                        vd.Frequencia = 0;
                        vd.Nota1 = 0;
                        vd.Nota2 = 0;
                        vd.Media_Final = 0;
                        db.Negocio_Vinculo_Disciplina.Add(vd);
                        await db.SaveChangesAsync();
                    }
                }
                return RedirectToAction("VerAluno", new { id = vm.Matricula_ID });
            }
            PopulateModuloDropDownList(vm.Matricula_ID, vm.Modulo_ID);
            return View(vm);
        }

        //Vincular à disciplina (Isolada)
        //GET: Matricula/VincularDisciplina
        [RBAC]
        public ActionResult VincularDisciplina(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Matricula_Aluno aluno = db.Negocio_Matricula_Aluno.Find(id);
            if (aluno == null)
            {
                return HttpNotFound();
            }
            CriarVinculoDisciplinaViewModel CVDVM = new CriarVinculoDisciplinaViewModel();
            CVDVM.MatriculaID = aluno.Matricula_ID;
            PopulateDisciplinaDropDownList(aluno.Matricula_ID);
            return View(CVDVM);
        }

        //POST: Matricula/VincularDisciplina
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult VincularDisciplina([Bind(Include = "DisciplinaID, MatriculaID, NumChamada")] CriarVinculoDisciplinaViewModel vd)
        {
            if (ModelState.IsValid)
            {
                Negocio_Vinculo_Disciplina novoVD = new Negocio_Vinculo_Disciplina();
                novoVD.Disciplina_ID = vd.DisciplinaID;
                novoVD.Matricula_ID = vd.MatriculaID;
                novoVD.Num_Chamada = 0;
                novoVD.Frequencia = 0;
                novoVD.Nota1 = 0;
                novoVD.Nota2 = 0;
                novoVD.Media_Final = 0;
                db.Negocio_Vinculo_Disciplina.Add(novoVD);
                db.SaveChanges();
                return RedirectToAction("VerAluno", new { id = vd.MatriculaID });
            }
            PopulateDisciplinaDropDownList(vd.MatriculaID, vd.DisciplinaID);
            return View(vd);
        }

        //Ver Aluno
        //GET: Matricula/VerAluno
        [RBAC]
        public ActionResult VerAluno(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Matricula_Aluno aluno = db.Negocio_Matricula_Aluno.Find(id);
            if (aluno == null)
            {
                return HttpNotFound();
            }

            VerAlunoViewModel VAVM = new VerAlunoViewModel();
            VAVM.pID = aluno.Pessoa_ID;
            VAVM.matID = aluno.Matricula_ID;
            VAVM.Nome = aluno.Negocio_Pessoa.Primeiro_Nome + " " + aluno.Negocio_Pessoa.Sobrenome;
            VAVM.Email = aluno.Negocio_Pessoa.Email;
            VAVM.Ativo = aluno.Ativo.Value ? "Ativada" : "Desativada";
            VAVM.DeveDocumento = aluno.Deve_Documento.Value ? "Sim" : "Não";
            VAVM.cID = aluno.Curso_ID;
            VAVM.Curso = aluno.Negocio_Curso.Curso_Nome;

            Negocio_Modulo NM;
            ListaVinculoModuloViewModel VVMVM;
            List<ListaVinculoModuloViewModel> listTemp = new List<ListaVinculoModuloViewModel>();
            foreach (var vm in db.Negocio_Vinculo_Modulo)
            {
                if (vm.Matricula_ID == aluno.Matricula_ID)
                {
                    NM = db.Negocio_Modulo.Find(vm.Modulo_ID);
                    VVMVM = new ListaVinculoModuloViewModel();
                    VVMVM.VinculoID = vm.Vinculo_ID;
                    VVMVM.ModuloNM = NM.Modulo_Nome;
                    VVMVM.StatusVinculo = vm.Status_Vinculo;
                    listTemp.Add(VVMVM);
                }
            }
            VAVM.modulos = listTemp;

            Negocio_Documento ND;
            VerDocsDeventesViewModel VDDVM;
            List<VerDocsDeventesViewModel> docList = new List<VerDocsDeventesViewModel>();
            foreach (var dd in db.Negocio_Doc_Devente)
            {
                if (dd.Matricula_ID == aluno.Matricula_ID)
                {
                    ND = db.Negocio_Documento.Find(dd.Documento_ID);
                    VDDVM = new VerDocsDeventesViewModel();
                    VDDVM.DocumentoID = ND.Documento_ID;
                    VDDVM.DocumentoNM = ND.Documento_Nome;
                    docList.Add(VDDVM);
                }
            }
            VAVM.documentos = docList;

            return View(VAVM);
        }

        //Ver vinculoModulo
        //GET: Matricula/VerVinculoModulo
        [RBAC]
        public ActionResult VerVinculoModulo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Vinculo_Modulo vinculo = db.Negocio_Vinculo_Modulo.Find(id);
            if (vinculo == null)
            {
                return HttpNotFound();
            }

            VerVinculoModuloViewModel VVMVM = new VerVinculoModuloViewModel();
            VVMVM.VinculoID = vinculo.Vinculo_ID;
            VVMVM.MatID = vinculo.Matricula_ID;
            VVMVM.ModuloNM = vinculo.Negocio_Modulo.Modulo_Nome;
            VVMVM.StatusVinculo = vinculo.Status_Vinculo;
            
            ListaDisciplinasViewModel LDVM;
            List<ListaDisciplinasViewModel> listTemp = new List<ListaDisciplinasViewModel>();
            foreach (var d in db.Negocio_Disciplina)
            {
                if (d.Modulo_ID == vinculo.Modulo_ID)
                {
                    LDVM = new ListaDisciplinasViewModel();
                    LDVM.DisciplinaID = d.Disciplina_ID;
                    LDVM.DisciplinaNM = d.Disciplina_Nome;
                    listTemp.Add(LDVM);
                }
            }
            VVMVM.disciplinas = listTemp;

            return View(VVMVM);
        }

        //Editar vinculo modulo
        //GET: Matricula/EditarVinculoModulo
        [RBAC]
        public ActionResult EditarVinculoModulo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Vinculo_Modulo vm = db.Negocio_Vinculo_Modulo.Find(id);
            if (vm == null)
            {
                return HttpNotFound();
            }
            return View(vm);
        }

        //POST: Matricula/EditarVinculoModulo
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult EditarVinculoModulo([Bind(Include = "Vinculo_ID, Modulo_ID, Matricula_ID, Status_Vinculo")]Negocio_Vinculo_Modulo vm)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Entry(vm).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("VerAluno", new { id = vm.Matricula_ID });
                }
                return View(vm);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
        }

        //Remover VinculoModulo
        //GET: Matricula/RemoverVinculoModulo
        [RBAC]
        public ActionResult RemoverVinculoModulo(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Vinculo_Modulo vinculo = db.Negocio_Vinculo_Modulo.Find(id);
            if (vinculo == null)
            {
                return HttpNotFound();
            }
            return View(vinculo);
        }

        //POST: Matricula/RemoverVinculoModulo
        [HttpPost, ActionName("RemoverVinculoModulo")]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult RemoverVinculoModuloConfirmacao(int id)
        {
            Negocio_Vinculo_Modulo vinculo = db.Negocio_Vinculo_Modulo.Find(id);
            db.Negocio_Vinculo_Modulo.Remove(vinculo);
            db.SaveChanges();
            return RedirectToAction("VerAluno", new { id = vinculo.Matricula_ID });
        }

        //Ver VinculoDisciplina
        //GET: Matricula/VerVinculoDisciplina
        [RBAC]
        public ActionResult VerVinculoDisciplina(int? discID, int? matID)
        {
            if (discID == null || matID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Vinculo_Disciplina vinculo = db.Negocio_Vinculo_Disciplina.Where(o => o.Disciplina_ID == discID && o.Matricula_ID == matID).FirstOrDefault();
            if (vinculo == null)
            {
                return HttpNotFound();
            }

            VerVinculoDisciplinaViewModel VVDVM = new VerVinculoDisciplinaViewModel();
            VVDVM.DisciplinaNm = vinculo.Negocio_Disciplina.Disciplina_Nome;
            VVDVM.Professor = vinculo.Negocio_Disciplina.Negocio_Funcionario.Negocio_Pessoa.Primeiro_Nome + " " + vinculo.Negocio_Disciplina.Negocio_Funcionario.Negocio_Pessoa.Sobrenome;
            VVDVM.NmAluno = vinculo.Negocio_Matricula_Aluno.Negocio_Pessoa.Primeiro_Nome + " " + vinculo.Negocio_Matricula_Aluno.Negocio_Pessoa.Sobrenome;
            VVDVM.NumChamada = 0;
            VVDVM.Nota1 = vinculo.Nota1.Value;
            VVDVM.Nota2 = vinculo.Nota2.Value;
            VVDVM.MediaFinal = vinculo.Media_Final.Value;
            VVDVM.TotAulasDadas = vinculo.Negocio_Disciplina.Tot_Aulas_Dadas.Value;
            VVDVM.Frequencia = vinculo.Frequencia.Value;
            VVDVM.VinculoID = vinculo.Vinculo_ID;
            VVDVM.MatriculaID = vinculo.Matricula_ID;

            ListaHorarioViewModel hTemp;
            List<ListaHorarioViewModel> listTemp = new List<ListaHorarioViewModel>();
            foreach (var h in db.Negocio_Quadro_Horario)
            {
                if (h.Disciplina_ID == vinculo.Disciplina_ID)
                {
                    hTemp = new ListaHorarioViewModel();
                    hTemp.horarioID = h.Quadro_Horario_ID;
                    hTemp.DiaSemana = h.Dia_Semana;
                    hTemp.HoraInicio = h.Hora_Inicio.Value;
                    hTemp.HoraFim = h.Hora_Fim.Value;
                    listTemp.Add(hTemp);
                }
            }

            VVDVM.horarios = listTemp;
            return View(VVDVM);
        }

        //Remover VinculoDisciplina
        //GET: Matricula/RemoverVinculoDisciplina
        [RBAC]
        public ActionResult RemoverVinculoDisciplina(int? id)
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
            return View(vinculo);
        }

        //POST: Matricula/RemoverVinculoDisciplina
        [HttpPost, ActionName("RemoverVinculoDisciplina")]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult RemoverVinculoDisciplinaConfimacao(int id)
        {
            Negocio_Vinculo_Disciplina vinculo = db.Negocio_Vinculo_Disciplina.Find(id);
            db.Negocio_Vinculo_Disciplina.Remove(vinculo);
            db.SaveChanges();
            return RedirectToAction("VerAluno", new { id = vinculo.Matricula_ID });
        }

        //Adicionar NovoDocumento (Remover se o doc for devente)
        //GET: Matricula/NovoDocumento
        [RBAC]
        public ActionResult NovoDocumento(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Pessoa pessoa = db.Negocio_Pessoa.Find(id);
            if (pessoa == null)
            {
                return HttpNotFound();
            }
            Negocio_Curriculo NC = new Negocio_Curriculo();
            NC.Pessoa_ID = pessoa.Pessoa_ID;
            PopulateDocumentosDropDownList();
            return View(NC);
        }

        //POST: Matricula/NovoDocumento
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult NovoDocumento([Bind(Include = "Pessoa_ID, Documento_ID, Orgao_Emissor, Dt_Emissao, Cidade_Emissao, Estado_Emissao, Pais_Emissao")]Negocio_Curriculo curriculo)
        {
            if (ModelState.IsValid)
            {
                Negocio_Curriculo NC = new Negocio_Curriculo();
                NC.Pessoa_ID = curriculo.Pessoa_ID;
                NC.Documento_ID = curriculo.Documento_ID;
                NC.Orgao_Emissor = curriculo.Orgao_Emissor;
                NC.Dt_Emissao = curriculo.Dt_Emissao;
                NC.Cidade_Emissao = curriculo.Cidade_Emissao;
                NC.Estado_Emissao = curriculo.Estado_Emissao;
                NC.Pais_Emissao = curriculo.Pais_Emissao;

                db.Negocio_Curriculo.Add(NC);
                db.SaveChanges();

                var aluno = db.Negocio_Matricula_Aluno.Where(o => o.Pessoa_ID == curriculo.Pessoa_ID && o.Ativo == true).FirstOrDefault();
                int matID = aluno.Matricula_ID;
                Negocio_Doc_Devente docDevente = db.Negocio_Doc_Devente.Where(o => o.Matricula_ID == matID && o.Documento_ID == curriculo.Documento_ID).FirstOrDefault();
                if (docDevente != null)
                {
                    db.Negocio_Doc_Devente.Remove(docDevente);
                    aluno.Deve_Documento = false;
                    db.Entry(aluno).State = EntityState.Modified;
                    db.SaveChanges();
                }
                return RedirectToAction("VerAluno", new { id = matID });
            }
            PopulateDocumentosDropDownList(curriculo.Documento_ID);
            return View(curriculo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void PopulateCursoDropDownList(object selectedCurso = null)
        {
            var cursoQuery = from c in db.Negocio_Curso
                             orderby c.Curso_Nome
                             select c;
            ViewBag.Cursos = new SelectList(cursoQuery,
                "Curso_ID", "Curso_Nome", selectedCurso);
            //return cursoQuery.FirstOrDefault().Curso_ID;
        }

        private void PopulateModuloDropDownList(int mID, object selectedModulo = null)
        {
            var cursoMatriculaQuery = from ma in db.Negocio_Matricula_Aluno
                                      where ma.Matricula_ID == mID
                                      select ma.Curso_ID;
            var moduloQuery = from m in db.Negocio_Modulo
                              where m.Curso_ID == cursoMatriculaQuery.FirstOrDefault()
                              orderby m.Modulo_Nome
                              select m;
            ViewBag.Modulos = new SelectList(moduloQuery, "Modulo_ID", "Modulo_Nome", selectedModulo);
        }

        private void PopulateModuloMatriculaDropDownList(int cID, object selectedModulo = null)
        {
            var moduloQuery = from m in db.Negocio_Modulo
                              where m.Curso_ID == cID
                              orderby m.Modulo_Nome
                              select m;
            ViewBag.Modulos = new SelectList(moduloQuery, "Modulo_ID", "Modulo_Nome", selectedModulo);
        }

        private void PopulateDisciplinaDropDownList(int mID, object selectedDisciplina = null)
        {
            var cursoMatriculaQuery = from ma in db.Negocio_Matricula_Aluno
                                      where ma.Matricula_ID == mID
                                      select ma.Curso_ID;
            var disciplinaQuery = from d in db.Negocio_Disciplina
                                  where d.Negocio_Modulo.Curso_ID == cursoMatriculaQuery.FirstOrDefault()
                                  orderby d.Disciplina_Nome
                                  select d;
            ViewBag.Disciplinas = new SelectList(disciplinaQuery, "Disciplina_ID", "Disciplina_Nome", selectedDisciplina);
        }

        private void PopulateDocumentosDropDownList(object selectedDocumento = null)
        {
            var documentoQuery = from d in db.Negocio_Documento
                                 orderby d.Documento_Nome
                                 select d;
            ViewBag.Documentos = new SelectList(documentoQuery, "Documento_ID", "Documento_Nome", selectedDocumento);
        }

        private void PopulatePerfilDropDownList(object selectedPerfil = null)
        {
            var perfilQuery = from p in db.RBAC_Perfil
                              orderby p.Perfil_Nome
                              select p;
            ViewBag.Perfis = new SelectList(perfilQuery, "Perfil_ID", "Perfil_Nome", selectedPerfil);
        }
    }
}