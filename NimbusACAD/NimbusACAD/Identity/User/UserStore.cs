using NimbusACAD.Models;
using NimbusACAD.Models.ViewModels;
using NimbusACAD.Identity;
using System.Linq;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Collections.Generic;

namespace NimbusACAD.Identity.User
{
    public class UserStore
    {
        #region USER

        public void AddPessoa(RegistrarComumViewModel pessoa)
        {
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
            {
                Negocio_Pessoa NP = new Negocio_Pessoa();
                Negocio_Endereco NE = new Negocio_Endereco();
                Negocio_Base_Endereco NBE = new Negocio_Base_Endereco();

                NP.Primeiro_Nome = pessoa.PrimeiroNome;
                NP.Sobrenome = pessoa.Sobrenome;
                NP.CPF = pessoa.CPF;
                NP.RG = pessoa.RG;
                NP.Sexo = pessoa.Sexo;
                NP.Dt_Nascimento = pessoa.DtNascimento;
                NP.Tel_Principal = pessoa.TelPrincipal;
                NP.Tel_Opcional = pessoa.TelOpcional;
                NP.Email = pessoa.Email;

                db.Negocio_Pessoa.Add(NP);
                db.SaveChanges();

                //Endereço --> Verificar se já existe endereço no CEP informado
                var endereco = db.Negocio_Base_Endereco.Where(o => o.CEP.Equals(pessoa.CEP));
                if (endereco == null)
                {
                    //Negocio_Base_Endereco
                    NBE.CEP = pessoa.CEP;
                    NBE.Logradouro = pessoa.Logradouro;
                    NBE.Bairro = pessoa.Bairro;
                    NBE.Cidade = pessoa.Cidade;
                    NBE.Estado = pessoa.Estado;
                    NBE.Pais = pessoa.Pais;

                    db.Negocio_Base_Endereco.Add(NBE);
                    db.SaveChanges();

                    //Negocio_Endereco
                    int pessoaID = GetPessoaIDporEmail(pessoa.Email);
                    if (pessoaID != 0)
                    {
                        NE.CEP = pessoa.CEP;
                        NE.Complemento = pessoa.Complemento;
                        NE.Numero = pessoa.Numero;
                        NE.Ativo = true;
                        NE.Pessoa_ID = pessoaID;

                        db.Negocio_Endereco.Add(NE);
                        db.SaveChanges();
                    }
                }
                else
                {
                    int pessoaID = GetPessoaIDporEmail(pessoa.Email);
                    if (pessoaID != 0)
                    {
                        NE.CEP = pessoa.CEP;
                        NE.Complemento = pessoa.Complemento;
                        NE.Numero = pessoa.Numero;
                        NE.Ativo = true;
                        NE.Pessoa_ID = pessoaID;

                        db.Negocio_Endereco.Add(NE);
                        db.SaveChanges();
                    }
                }
            }
        }
        
        public bool IsEmailExist(string emailValidar)
        {
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
            {
                return db.Negocio_Pessoa.Where(o => o.Email.Equals(emailValidar)).Any();
            }
        }

        public string GetUsuarioSenha(string Email)
        {
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
            {
                var usuario = db.RBAC_Usuario.Where(o => o.Username.ToLower().Equals(Email));
                if (usuario.Any())
                {
                    return usuario.FirstOrDefault().Senha_Hash;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        
        #endregion

        #region GET

        public int GetPessoaIDporEmail(string Email)
        {
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
            {
                var pessoa = db.Negocio_Pessoa.Where(o => o.Email.Equals(Email));
                if (pessoa.Any())
                {
                    return pessoa.FirstOrDefault().Pessoa_ID;
                }
            }
            return 0;
        }

        public int GetPessoaIDporNome(string nmCompleto)
        {
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
            {
                var pessoa = db.Negocio_Pessoa.Where(o => (o.Primeiro_Nome + o.Sobrenome).Equals(nmCompleto));
                if (pessoa.Any())
                {
                    return pessoa.FirstOrDefault().Pessoa_ID;
                }
            }
            return 0;
        }

        public int GetUsuarioID(string Email)
        {
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
            {
                var usuario = db.RBAC_Usuario.Where(o => o.Username.Equals(Email));
                if (usuario.Any())
                {
                    return usuario.FirstOrDefault().Usuario_ID;
                }
            }
            return 0;
        }

        public List<ListaPerfisViewModel> GetAllPerfilUsuario()
        {
            List<ListaPerfisViewModel> perfis = new List<ListaPerfisViewModel>();
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
            {
                ListaPerfisViewModel LPVM;
                var pessoas = db.Negocio_Pessoa.ToList();
                var usuarios = db.RBAC_Usuario.ToList();

                //u = RBAC_Usuario | p = Negocio_Pessoa | r = RBAC_Perfil
                foreach (RBAC_Usuario u in db.RBAC_Usuario)
                {
                    LPVM = new ListaPerfisViewModel();
                    LPVM.Email = u.Username;
                    LPVM.Bloqueado = u.Bloqueado == true ? "Bloqueado" : "desbloqueado";

                    var p = db.Negocio_Pessoa.Find(u.Pessoa_ID);

                    if (p != null)
                    {
                        LPVM.NmCompleto = p.Primeiro_Nome + " " + p.Sobrenome;
                        LPVM.CPF = p.CPF;
                        LPVM.RG = p.RG;
                        LPVM.TelPrincipal = p.Tel_Principal;
                    }                    

                    perfis.Add(LPVM);
                }
            }
            return perfis;
        }

        public PerfilDeUsuarioViewModel GetPerfilUsuario(string nomeUsuario)
        {
            PerfilDeUsuarioViewModel PUVM = new PerfilDeUsuarioViewModel();

            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
            {
                var usuario = db.RBAC_Usuario.Where(o => o.Username.Equals(nomeUsuario)).FirstOrDefault();

                PUVM.Email = usuario.Username;
                PUVM.NmCompleto = usuario.Negocio_Pessoa.Primeiro_Nome + " " + usuario.Negocio_Pessoa.Sobrenome;
                PUVM.CPF = usuario.Negocio_Pessoa.CPF;
                PUVM.RG = usuario.Negocio_Pessoa.RG;
                PUVM.Sexo = usuario.Negocio_Pessoa.Sexo;
                PUVM.DtNascimento = usuario.Negocio_Pessoa.Dt_Nascimento.Value;
                PUVM.TelPrincipal = usuario.Negocio_Pessoa.Tel_Principal;
                PUVM.TelSecundario = usuario.Negocio_Pessoa.Tel_Opcional;
                PUVM.EndCompleto = GetUsuarioEndereco(usuario.Pessoa_ID.Value);
                PUVM.DtModif = usuario.Dt_Ultima_Modif.Value;
                PUVM.Bloqueado = usuario.Bloqueado == true ? "Bloqueado" : "Desbloqueado";
                PUVM.Perfil = usuario.RBAC_Perfil.FirstOrDefault().Perfil_Nome;

                return PUVM;
            }
        }

        public string GetUsuarioEndereco(int pessoaID)
        {
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
            {
                var endereco = db.Negocio_Endereco.Where(o => o.Pessoa_ID == pessoaID).FirstOrDefault();
                var bsEnd = db.Negocio_Base_Endereco.Where(o => o.CEP.Equals(endereco.CEP)).FirstOrDefault();
                string end = endereco.CEP + "\n" +
                             bsEnd.Logradouro + ", " + endereco.Numero.ToString() + ", " + endereco.Complemento + " - " + bsEnd.Bairro + "\n" +
                             bsEnd.Cidade + " - " + bsEnd.Estado + "\n" +
                             bsEnd.Pais;
                return end;
            }
        }



        #endregion
    }
}