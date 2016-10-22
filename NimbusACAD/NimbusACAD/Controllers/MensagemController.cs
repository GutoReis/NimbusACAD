using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NimbusACAD.Models.DB;
using NimbusACAD.Models.ViewModels;
using System.Net;

namespace NimbusACAD.Controllers
{
    public class MensagemController : Controller
    {
        private NimbusAcad_DBEntities db = new NimbusAcad_DBEntities();
        

        // GET: Mensagem
        public ActionResult Index()
        {
            int pID = GetPessoaId();

            var msgs = db.Negocio_Notificacao.Where(o => o.Pessoa_Receptor_ID == pID);
            return View(msgs.ToList());
        }

        //GET: Mensagem/Detalhes/5
        public ActionResult Detalhes(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Notificacao notificacao = db.Negocio_Notificacao.Find(id);
            if (notificacao == null)
            {
                return HttpNotFound();
            }
            return View(notificacao);
        }

        //GET: Mensagem/Escrever
        public ActionResult Escrever()
        {
            PopulatePessoaDropDownList();
            return View();
        }

        //POST: Mensagem/Escrever
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Escrever([Bind(Include = "Notificacao_ID, Pessoa_Emissor_ID, Pessoa_Receptor_ID, Assunto, Corpo, Lida")] Negocio_Notificacao notificacao)
        {
            if (ModelState.IsValid)
            {
                db.Negocio_Notificacao.Add(notificacao);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            PopulatePessoaDropDownList(notificacao.Pessoa_Receptor_ID);
            return View(notificacao);
        }

        //GET: Mensagem/Deletar/5
        public ActionResult Deletar(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Negocio_Notificacao notificacao = db.Negocio_Notificacao.Find(id);
            if (notificacao == null)
            {
                return HttpNotFound();
            }
            return View(notificacao);
        }

        //POST: Mensagem/Deletar/5
        [HttpPost, ActionName("Deletar")]
        [ValidateAntiForgeryToken]
        public ActionResult DeletarConfirmacao(int id)
        {
            Negocio_Notificacao notificacao = db.Negocio_Notificacao.Find(id);
            db.Negocio_Notificacao.Remove(notificacao);
            return RedirectToAction("Index");
        }

        private int GetPessoaId()
        {
            var pessoa = User.Identity as Negocio_Pessoa;
            return pessoa.Pessoa_ID;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) { db.Dispose(); }
            base.Dispose(disposing);
        }

        private void PopulatePessoaDropDownList(object selectedPessoa = null)
        {
            var pessoaQuery = from c in db.Negocio_Pessoa
                              orderby c.Primeiro_Nome
                              select c;
            ViewBag.Pessoa_ID = new SelectList(pessoaQuery,
                "Pessoa_ID", "Primeiro_Nome" + "Sobrenome", selectedPessoa);
        }
    }
}