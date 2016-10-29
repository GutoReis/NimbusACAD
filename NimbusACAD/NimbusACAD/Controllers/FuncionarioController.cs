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
        private NimbusAcad_DBEntities db = new NimbusAcad_DBEntities();

        // GET: Funcionario
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
        public ActionResult Detalhes(int? id)
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

            VerFuncionarioViewModel VFVM = new VerFuncionarioViewModel();
            VFVM.funcionarioID = negocio_Funcionario.Funcionario_ID;
            VFVM.pessoaID = negocio_Funcionario.Pessoa_ID;
            VFVM.cargoID = negocio_Funcionario.Cargo_ID;
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
        public ActionResult NovaPessoa()
        {
            PopulatePerfilDropDownList();
            return View();
        }

        //POST: Funcionario/NovaPessoa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NovaPessoa([Bind(Include = "PrimeiroNome, Sobrenome, CPF, RG, Sexo, DtNascimento, TelPrincipal, TelOpcional, Email, CEP, Logradouro, Complemento, Numero, Bairro, Cidade, Estado, Pais, PerfilID")]RegistrarComumViewModel novaPessoa)
        {
            if (ModelState.IsValid)
            {
                //Criando pessoa
                int pID = -_userStore.AddPessoa(novaPessoa);

                //Criando usuario
                RBAC_Usuario RU = new RBAC_Usuario();
                RU.Pessoa_ID = pID;
                RU.Username = novaPessoa.Email;

                //Gernado Senha tempraria e salt
                string tempToken = SecurityMethods.GenerateTempTokenAccess();
                string tempSalt = SecurityMethods.GenerateSalt();
                string tempPass = SecurityMethods.HashPasswordPBKDF2(tempToken, tempSalt);

                //Criando usuario
                RU.Senha_Hash = tempToken;
                RU.Salt = tempSalt;
                RU.Dt_Criacao = DateTime.Now.Date;
                RU.Dt_Ultima_Modif = DateTime.Now.Date;
                RU.Bloqueado = false;

                db.SaveChanges();

                //Enviando Email
                EmailMessage message = new EmailMessage(novaPessoa.Email, "Bem-vindo ao NimbusAcad!", "Olá, seja bem-vindo ao NimbusAcad \n Esta é sua senha: " + tempToken + "\nRecomenda-se que altere a senha para uma, com fácil memorização.");
                _emailService.Send(message);

                //Assimilando perfil de acesso
                int uID = _userStore.GetUsuarioID(novaPessoa.Email);
                VinculoPerfilUsuarioViewModel VPUVM = new VinculoPerfilUsuarioViewModel();
                VPUVM.UsuarioID = uID;
                VPUVM.PerfilID = novaPessoa.PerfilID;
                _roleStore.AddUsuarioPerfil(VPUVM);

                return RedirectToAction("RegistrarDocumento", pID);
            }
            PopulatePerfilDropDownList(novaPessoa.PerfilID);
            return View(novaPessoa);
        }

        //Registrar Documentos -> Registrar Funcionario
        //GET: Funcionario/RegistrarDocumento
        public ActionResult RegistrarDocumento(int? pID)
        {
            if (pID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Pessoa pessoa = db.Negocio_Pessoa.Find(pID);
            if (pessoa == null)
            {
                return HttpNotFound();
            }
            PopulateDocumentoDropDownList();
            return ViewBag(pessoa);
        }

        //POST: Funcionario/RegistrarDocumento
        [HttpPost]
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

                return RedirectToAction("NovoFuncionario", curriculo.PessoaID);
            }
            PopulateDocumentoDropDownList(curriculo.DocumentoID);
            return View(curriculo);
        }

        // GET: Funcionario/NovoFuncionario
        public ActionResult NovoFuncionario(int? pID)
        {
            //ViewBag.Pessoa_ID = new SelectList(db.Negocio_Pessoa, "Pessoa_ID", "Primeiro_Nome");
            //ViewBag.Cargo_ID = new SelectList(db.Negocio_Tipo_Funcionario, "Tipo_Funcionario_ID", "Cargo");
            if (pID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Pessoa pessoa = db.Negocio_Pessoa.Find(pID);
            if (pessoa == null)
            {
                return HttpNotFound();
            }
            PopulateCargoDropDownList();
            return View();
        }

        // POST: Funcionario/NovoFuncionario
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NovoFuncionario([Bind(Include = "PrimeiroNome, Sobrenome, CPF, RG, Sexo, DtNascimento, TelPrincipal, TelOpcional, Email, CEP, Logradouro, Numero, Bairro, Cidade, Estado, Pais")] RegistrarComumViewModel novaPessoa, [Bind(Include = "Funcionario_ID, Pessoa_ID, Cargo_ID")] Negocio_Funcionario funcionario)
        {
            if (ModelState.IsValid)
            {
                int pID = _userStore.AddPessoa(novaPessoa);
                funcionario.Pessoa_ID = pID;
                db.Negocio_Funcionario.Add(funcionario);
                return RedirectToAction("Index");
            }

            //ViewBag.Pessoa_ID = new SelectList(db.Negocio_Pessoa, "Pessoa_ID", "Primeiro_Nome", negocio_Funcionario.Pessoa_ID);
            //ViewBag.Cargo_ID = new SelectList(db.Negocio_Tipo_Funcionario, "Tipo_Funcionario_ID", "Cargo", negocio_Funcionario.Cargo_ID);
            PopulateCargoDropDownList(funcionario.Cargo_ID);
            return View(novaPessoa);
        }

        // GET: Funcionario/Editar/5
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
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "Funcionario_ID,Pessoa_ID,Cargo_ID")] Negocio_Funcionario negocio_Funcionario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(negocio_Funcionario).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.Pessoa_ID = new SelectList(db.Negocio_Pessoa, "Pessoa_ID", "Primeiro_Nome", negocio_Funcionario.Pessoa_ID);
            //ViewBag.Cargo_ID = new SelectList(db.Negocio_Tipo_Funcionario, "Tipo_Funcionario_ID", "Cargo", negocio_Funcionario.Cargo_ID);
            PopulateCargoDropDownList(negocio_Funcionario.Cargo_ID);
            return View(negocio_Funcionario);
        }

        // GET: Funcionario/Deletar/5
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
        [ValidateAntiForgeryToken]
        public ActionResult DeletarConfirmacao(int id)
        {
            Negocio_Funcionario negocio_Funcionario = db.Negocio_Funcionario.Find(id);
            db.Negocio_Funcionario.Remove(negocio_Funcionario);
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

        //GET: Funcionario/NovoDocumento
        public ActionResult NovoDocumento(int? pID)
        {
            if (pID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Pessoa pessoa = db.Negocio_Pessoa.Find(pID);
            if (pessoa == null)
            {
                return HttpNotFound();
            }
            PopulateDocumentoDropDownList();
            return View(pessoa);
        }

        //POST: Funcionario/NovoDocumento
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult NovoDocumento([Bind(Include = "PessoaID, DocumentoID, OrgaoEmissor, DtEmissao, Cidade, Estado, Pais")]CurriculoViewModel curriculo)
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
                return RedirectToAction("Detalhes", curriculo.PessoaID);
            }
            PopulateDocumentoDropDownList(curriculo.DocumentoID);
            return View(curriculo);
        }

        private void PopulateCargoDropDownList(object selectedCargo = null)
        {
            var cargoQuery = from c in db.Negocio_Tipo_Funcionario
                             orderby c.Cargo
                             select c;
            ViewBag.Tipo_Funcionario_ID = new SelectList(cargoQuery,
                "Tipo_Funcionario_ID", "Cargo", selectedCargo);
        }

        private void PopulatePerfilDropDownList(object selectedPerfil = null)
        {
            var perfilQuery = from p in db.RBAC_Perfil
                              orderby p.Perfil_Nome
                              select p;
            ViewBag.Perfil_ID = new SelectList(perfilQuery, "Perfil_ID", "Perfil_Nome", selectedPerfil);
        }

        private void PopulateDocumentoDropDownList(object selectedDocumento = null)
        {
            var documentoQuery = from d in db.Negocio_Documento
                                 orderby d.Documento_Nome
                                 select d;
            ViewBag.Documento_ID = new SelectList(documentoQuery, "Documento_ID", "Documento_Nome", selectedDocumento);
        }
    }
}
