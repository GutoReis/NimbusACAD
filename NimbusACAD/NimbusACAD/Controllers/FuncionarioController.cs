﻿using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using NimbusACAD.Models.DB;
using NimbusACAD.Models.ViewModels;
using NimbusACAD.Identity.User;
using NimbusACAD.Identity.Email;
using NimbusACAD.Identity.Role;
using NimbusACAD.Identity.Security;

namespace NimbusACAD.Controllers
{
    public class FuncionarioController : Controller
    {
        private UserStore _userStore = new UserStore();
        private EmailService _emailService = new EmailService();
        private RoleStore _roleStore = new RoleStore();
        private NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities();

        // GET: Funcionario
        [RBAC]
        public ViewResult Index(string searchString)
        {
            var funcionarios = from f in db.Negocio_Funcionario select f;
            funcionarios = funcionarios.Include(o => o.Negocio_Pessoa).Include(o => o.Negocio_Tipo_Funcionario);
            if (!String.IsNullOrEmpty(searchString))
            {
                funcionarios = funcionarios.Where(o => o.Negocio_Pessoa.Primeiro_Nome.ToUpper().Contains(searchString.ToUpper()));
            }
            //var negocio_Funcionario = db.Negocio_Funcionario.Include(n => n.Negocio_Pessoa).Include(n => n.Negocio_Tipo_Funcionario);
            return View(funcionarios.ToList());
        }

        // GET: Funcionario/Detalhes/5
        [RBAC]
        public ActionResult Detalhes(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Funcionario negocio_Funcionario = db.Negocio_Funcionario.Where(o => o.Pessoa_ID == id).FirstOrDefault();
            if (negocio_Funcionario == null)
            {
                return HttpNotFound();
            }

            VerFuncionarioViewModel VFVM = new VerFuncionarioViewModel();
            VFVM.funcionarioID = negocio_Funcionario.Funcionario_ID;
            VFVM.pessoaID = negocio_Funcionario.Pessoa_ID;
            VFVM.cargoID = negocio_Funcionario.Tipo_Funcionario_ID;
            VFVM.FuncionarioNM = negocio_Funcionario.Negocio_Pessoa.Primeiro_Nome + " " + negocio_Funcionario.Negocio_Pessoa.Sobrenome;
            VFVM.Email = negocio_Funcionario.Negocio_Pessoa.Email;
            VFVM.TelefonePrincipal = negocio_Funcionario.Negocio_Pessoa.Tel_Principal;
            VFVM.TelefoneSecundario = negocio_Funcionario.Negocio_Pessoa.Tel_Opcional;
            VFVM.Endereco = _userStore.GetUsuarioEndereco(negocio_Funcionario.Pessoa_ID);
            VFVM.Cargo = negocio_Funcionario.Negocio_Tipo_Funcionario.Cargo;

            return View(VFVM);
        }

        //Registrar Pessoa -> Registrar Documentos
        //GET: Funcionario/NovaPessoa
        [RBAC]
        public ActionResult NovaPessoa()
        {
            PopulatePerfilDropDownList();
            return View();
        }

        //POST: Funcionario/NovaPessoa
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<ActionResult> NovaPessoa([Bind(Include = "PrimeiroNome, Sobrenome, CPF, RG, Sexo, DtNascimento, TelPrincipal, TelOpcional, Email, CEP, Logradouro, Complemento, Numero, Bairro, Cidade, Estado, Pais, PerfilID")]RegistrarComumViewModel novaPessoa)
        {
            if (ModelState.IsValid)
            {
                //Criando pessoa
                int pID = _userStore.AddPessoa(novaPessoa);

                //Criando usuario
                RBAC_Usuario RU = new RBAC_Usuario();
                RU.Pessoa_ID = pID;
                RU.Username = novaPessoa.Email;

                //Gernado Senha tempraria e salt
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

                //Enviando Email da senha
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

                return RedirectToAction("RegistrarDocumento", "Funcionario", new { id = pID });
            }
            PopulatePerfilDropDownList(novaPessoa.PerfilID);
            return View(novaPessoa);
        }

        //Registrar Documentos -> Registrar Funcionario
        //GET: Funcionario/RegistrarDocumento
        [RBAC]
        public ActionResult RegistrarDocumento(int? id)
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
            CurriculoViewModel CVM = new CurriculoViewModel();
            CVM.PessoaID = pessoa.Pessoa_ID;
            PopulateDocumentoDropDownList();
            return View(CVM);
        }

        //POST: Funcionario/RegistrarDocumento
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarDocumento([Bind(Include = "PessoaID, DocumentoID, DtEmissao, OrgaoEmissor, DtEmissao, Cidade, Estado, Pais")] CurriculoViewModel curriculo)
        {
            if (ModelState.IsValid)
            {
                Negocio_Curriculo NC = new Negocio_Curriculo();
                NC.Pessoa_ID = curriculo.PessoaID;
                NC.Documento_ID = curriculo.DocumentoID;
                NC.Orgao_Emissor = curriculo.OrgaoEmissor;
                NC.Dt_Emissao = curriculo.DtEmissao;
                NC.Cidade_Emissao = curriculo.Cidade;
                NC.Estado_Emissao = curriculo.Estado;
                NC.Pais_Emissao = curriculo.Pais;

                db.Negocio_Curriculo.Add(NC);
                db.SaveChanges();

                return RedirectToAction("NovoFuncionario", new { id = curriculo.PessoaID });
            }
            PopulateDocumentoDropDownList(curriculo.DocumentoID);
            return View(curriculo);
        }

        // GET: Funcionario/NovoFuncionario
        [RBAC]
        public ActionResult NovoFuncionario(int? id)
        {
            //ViewBag.Pessoa_ID = new SelectList(db.Negocio_Pessoa, "Pessoa_ID", "Primeiro_Nome");
            //ViewBag.Cargo_ID = new SelectList(db.Negocio_Tipo_Funcionario, "Tipo_Funcionario_ID", "Cargo");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Pessoa pessoa = db.Negocio_Pessoa.Find(id);
            if (pessoa == null)
            {
                return HttpNotFound();
            }
            Negocio_Funcionario func = new Negocio_Funcionario();
            func.Pessoa_ID = pessoa.Pessoa_ID;
            PopulateCargoDropDownList();
            return View(func);
        }

        // POST: Funcionario/NovoFuncionario
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult NovoFuncionario([Bind(Include = "Funcionario_ID, Pessoa_ID, Tipo_Funcionario_ID")] Negocio_Funcionario funcionario)
        {
            if (ModelState.IsValid)
            {
                db.Negocio_Funcionario.Add(funcionario);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.Pessoa_ID = new SelectList(db.Negocio_Pessoa, "Pessoa_ID", "Primeiro_Nome", negocio_Funcionario.Pessoa_ID);
            //ViewBag.Cargo_ID = new SelectList(db.Negocio_Tipo_Funcionario, "Tipo_Funcionario_ID", "Cargo", negocio_Funcionario.Cargo_ID);
            PopulateCargoDropDownList(funcionario.Tipo_Funcionario_ID);
            return View(funcionario);
        }

        // GET: Funcionario/Editar/5
        [RBAC]
        public ActionResult Editar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Funcionario negocio_Funcionario = db.Negocio_Funcionario.Find(id);
            if (negocio_Funcionario == null)
            {
                return HttpNotFound();
            }
            //ViewBag.Pessoa_ID = new SelectList(db.Negocio_Pessoa, "Pessoa_ID", "Primeiro_Nome", negocio_Funcionario.Pessoa_ID);
            //ViewBag.Cargo_ID = new SelectList(db.Negocio_Tipo_Funcionario, "Tipo_Funcionario_ID", "Cargo", negocio_Funcionario.Cargo_ID);
            PopulateCargoDropDownList();
            return View(negocio_Funcionario);
        }

        // POST: Funcionario/Editar/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "Funcionario_ID,Pessoa_ID,Tipo_Funcionario_ID")] Negocio_Funcionario negocio_Funcionario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(negocio_Funcionario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.Pessoa_ID = new SelectList(db.Negocio_Pessoa, "Pessoa_ID", "Primeiro_Nome", negocio_Funcionario.Pessoa_ID);
            //ViewBag.Cargo_ID = new SelectList(db.Negocio_Tipo_Funcionario, "Tipo_Funcionario_ID", "Cargo", negocio_Funcionario.Cargo_ID);
            PopulateCargoDropDownList(negocio_Funcionario.Tipo_Funcionario_ID);
            return View(negocio_Funcionario);
        }

        // GET: Funcionario/Deletar/5
        [RBAC]
        public ActionResult Deletar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Funcionario negocio_Funcionario = db.Negocio_Funcionario.Find(id);
            if (negocio_Funcionario == null)
            {
                return HttpNotFound();
            }
            return View(negocio_Funcionario);
        }

        // POST: Funcionario/Deletar/5
        [HttpPost, ActionName("Deletar")]
        [RBAC]
        [ValidateAntiForgeryToken]
        public ActionResult DeletarConfirmacao(int id)
        {
            Negocio_Funcionario negocio_Funcionario = db.Negocio_Funcionario.Find(id);
            db.Negocio_Funcionario.Remove(negocio_Funcionario);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET: Funcionario/NovoDocumento
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
            PopulateDocumentoDropDownList();
            return View(NC);
        }

        //POST: Funcionario/NovoDocumento
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
                return RedirectToAction("Detalhes", new { id = NC.Pessoa_ID });
            }
            PopulateDocumentoDropDownList(curriculo.Documento_ID);
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

        private void PopulateCargoDropDownList(object selectedCargo = null)
        {
            var cargoQuery = from c in db.Negocio_Tipo_Funcionario
                             orderby c.Cargo
                             select c;
            ViewBag.Cargos = new SelectList(cargoQuery,
                "Tipo_Funcionario_ID", "Cargo", selectedCargo);
        }

        private void PopulatePerfilDropDownList(object selectedPerfil = null)
        {
            var perfilQuery = from p in db.RBAC_Perfil
                              orderby p.Perfil_Nome
                              select p;
            ViewBag.Perfis = new SelectList(perfilQuery, "Perfil_ID", "Perfil_Nome", selectedPerfil);
        }

        private void PopulateDocumentoDropDownList(object selectedDocumento = null)
        {
            var documentoQuery = from d in db.Negocio_Documento
                                 orderby d.Documento_Nome
                                 select d;
            ViewBag.Documentos = new SelectList(documentoQuery, "Documento_ID", "Documento_Nome", selectedDocumento);
        }
    }
}
