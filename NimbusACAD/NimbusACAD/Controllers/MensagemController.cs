using System.Linq;
using System.Web.Mvc;
using NimbusACAD.Models.DB;
using System.Net;
using System.Threading.Tasks;

namespace NimbusACAD.Controllers
{
    [Authorize]
    public class MensagemController : Controller
    {
        private NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities();
        
        // GET: Mensagem
        public ActionResult Index()
        {
            //int pID = GetPessoaId();
            var nome = User.Identity.Name;
            int pID = db.RBAC_Usuario.Where(o => o.Username.Equals(nome)).FirstOrDefault().Pessoa_ID.Value;

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
            Negocio_Notificacao NN = new Negocio_Notificacao();

            var nome = User.Identity.Name;
            int pID = db.RBAC_Usuario.Where(o => o.Username.Equals(nome)).FirstOrDefault().Pessoa_ID.Value;

            NN.Pessoa_Emissor_ID = pID;
            NN.Lida = false;

            PopulatePessoaDropDownList();
            return View(NN);
        }

        //POST: Mensagem/Escrever
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Escrever([Bind(Include = "Notificacao_ID, Pessoa_Emissor_ID, Pessoa_Receptor_ID, Assunto, Corpo, Lida")] Negocio_Notificacao notificacao)
        {
            if (ModelState.IsValid)
            {
                Negocio_Notificacao NN = new Negocio_Notificacao();

                //var nome = User.Identity.Name;
                //int pID = db.RBAC_Usuario.Where(o => o.Username.Equals(nome)).FirstOrDefault().Pessoa_ID.Value;

                NN.Pessoa_Emissor_ID = notificacao.Pessoa_Emissor_ID; /*GetPessoaId();*/
                NN.Pessoa_Receptor_ID = notificacao.Pessoa_Receptor_ID;
                NN.Assunto = notificacao.Assunto;
                NN.Corpo = notificacao.Corpo;
                NN.Lida = notificacao.Lida;

                Negocio_Pessoa NPReceptor = db.Negocio_Pessoa.Find(notificacao.Pessoa_Receptor_ID);
                NPReceptor.Tot_Notif_NL = NPReceptor.Tot_Notif_NL.Value + 1;

                db.Negocio_Notificacao.Add(NN);
                db.Entry(NPReceptor).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            PopulatePessoaDropDownList(notificacao.Pessoa_Receptor_ID);
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
            ViewBag.Pessoas = new SelectList(pessoaQuery,
                "Pessoa_ID", "Primeiro_Nome", selectedPessoa);
        }
    }
}