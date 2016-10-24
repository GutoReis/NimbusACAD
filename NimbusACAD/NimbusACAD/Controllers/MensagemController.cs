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
        public ActionResult Detalhes(int? id)
        {
            if (id == null)
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
        public ActionResult Escrever([Bind(Include = "EmissorID, ReceptorID, Assunto, Corpo")] EscreverNotifacaoViewModel notificacao)
        {
            if (ModelState.IsValid)
            {
                Negocio_Notificacao NN = new Negocio_Notificacao();
                NN.Pessoa_Emissor_ID = notificacao.EmissorID;
                NN.Pessoa_Receptor_ID = notificacao.ReceptorID;
                NN.Assunto = notificacao.Assunto;
                NN.Corpo = notificacao.Corpo;
                NN.Lida = false;

                Negocio_Pessoa NPReceptor = db.Negocio_Pessoa.Find(notificacao.ReceptorID);
                NPReceptor.Tot_Notif_NL = NPReceptor.Tot_Notif_NL + 1;

                db.Negocio_Notificacao.Add(NN);
                db.Entry(NPReceptor).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            PopulatePessoaDropDownList(notificacao.ReceptorID);
            return View(notificacao);
        }

        //GET: Mensagem/Deletar/5
        public ActionResult Deletar(int? id)
        {
            if (id == null)
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
            db.SaveChanges();
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