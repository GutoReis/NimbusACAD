using NimbusACAD.Models.DB;
using NimbusACAD.Models.ViewModels;
using NimbusACAD.Identity.Security;
using NimbusACAD.Identity;
using System.Linq;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Collections.Generic;
using System;

namespace NimbusACAD.Identity.User
{
    public class UserStore
    {
        #region CREATE

        //Add usuario será feito no AccountController, para enviar o email com a senha temporaria.
        public void AddPessoa(RegistrarComumViewModel pessoa)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
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

        public bool IsUsuarioInPerfil(string usuarioNome, string perfilNome)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                RBAC_Usuario usuario = db.RBAC_Usuario.Where(o => o.Username.ToLower().Equals(usuarioNome))?.FirstOrDefault();
                if (usuario != null)
                {
                    var perfilRBAC = from q in db.RBAC_Link_Usuario_Perfil
                                     join r in db.RBAC_Perfil
                                     on q.Perfil_ID equals r.Perfil_ID
                                     where r.Perfil_Nome.Equals(perfilNome) && q.Usuario_ID.Equals(usuario.Usuario_ID)
                                     select r.Perfil_Nome;
                    if (perfilRBAC != null)
                    {
                        return perfilRBAC.Any();
                    }
                }
                return false;
            }
        }

        #endregion

        #region GET

        #region GET-RBAC_Usuario
        public int GetUsuarioID(string Email)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                var usuario = db.RBAC_Usuario.Where(o => o.Username.Equals(Email));
                if (usuario.Any())
                {
                    return usuario.FirstOrDefault().Usuario_ID;
                }
            }
            return 0;
        }

        public bool GetEmailUsernameExist(string Email)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                return db.RBAC_Usuario.Where(o => o.Username.Equals(Email)).Any();
            }
        }

        public string GetUsuarioSenha(string Email)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                var usuario = db.RBAC_Usuario.Where(o => o.Username.Equals(Email));
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

        public string GetUsuarioSalt(string Email)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                var usuario = db.RBAC_Usuario.Where(o => o.Username.Equals(Email));
                if (usuario.Any())
                {
                    return usuario.FirstOrDefault().Salt;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public bool GetUsuarioBloqueado(string Email)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                var usuario = db.RBAC_Usuario.Where(o => o.Username.Equals(Email));
                if (usuario.Any())
                {
                    return usuario.FirstOrDefault().Bloqueado.Value;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool GetUsuarioEmailVerificado(string Email)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                var usuario = db.RBAC_Usuario.Where(o => o.Username.Equals(Email));
                if (usuario.Any())
                {
                    var pessoa = db.Negocio_Pessoa.Where(o => o.Pessoa_ID == usuario.FirstOrDefault().Pessoa_ID);
                    if (pessoa.Any())
                    {
                        return pessoa.FirstOrDefault().Email_Confirmado.Value;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public string GetEmailUsernameByNome(string nmCompleto)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                int pessoaID = GetPessoaIDporNome(nmCompleto);
                var usuario = db.RBAC_Usuario.Where(o => o.Pessoa_ID == pessoaID);
                if (usuario.Any())
                {
                    return usuario.FirstOrDefault().Username;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        #endregion

        #region GET-Negocio_Pessoa
        public int GetPessoaIDporEmail(string Email)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
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
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                var pessoa = db.Negocio_Pessoa.Where(o => (o.Primeiro_Nome + o.Sobrenome).Equals(nmCompleto));
                if (pessoa.Any())
                {
                    return pessoa.FirstOrDefault().Pessoa_ID;
                }
            }
            return 0;
        }
        #endregion

        #region GET-Perfil_Completo
        public List<ListaPerfisViewModel> GetAllPerfilUsuario()
        {
            List<ListaPerfisViewModel> perfis = new List<ListaPerfisViewModel>();
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
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

            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                var usuario = db.RBAC_Usuario.Where(o => o.Username.Equals(nomeUsuario)).FirstOrDefault();

                PUVM.UsuarioID = usuario.Usuario_ID;
                PUVM.PessoaID = usuario.Pessoa_ID.Value;
                PUVM.Email = usuario.Username;
                PUVM.PrimeiroNome = usuario.Negocio_Pessoa.Primeiro_Nome;
                PUVM.Sobrenome = usuario.Negocio_Pessoa.Sobrenome;
                PUVM.CPF = usuario.Negocio_Pessoa.CPF;
                PUVM.RG = usuario.Negocio_Pessoa.RG;
                PUVM.Sexo = usuario.Negocio_Pessoa.Sexo;
                PUVM.DtNascimento = usuario.Negocio_Pessoa.Dt_Nascimento.Value;
                PUVM.TelPrincipal = usuario.Negocio_Pessoa.Tel_Principal;
                PUVM.TelSecundario = usuario.Negocio_Pessoa.Tel_Opcional;
                PUVM.EndCompleto = GetUsuarioEndereco(usuario.Pessoa_ID.Value);
                PUVM.DtModif = usuario.Dt_Ultima_Modif.Value;
                PUVM.Bloqueado = usuario.Bloqueado.Value ? "Bloqueado" : "Desbloqueado";
                PUVM.Perfil = GetUsuarioPerfilRBACNome(usuario.Usuario_ID);

                return PUVM;
            }
        }
        #endregion

        #region GET-Auxiliares
        public string GetUsuarioPerfilRBACNome(int usuarioID)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                RBAC_Link_Usuario_Perfil linkUP = db.RBAC_Link_Usuario_Perfil.Where(o => o.Usuario_ID == usuarioID).FirstOrDefault();
                RBAC_Perfil perfilRBAC = db.RBAC_Perfil.Where(o => o.Perfil_ID == linkUP.Perfil_ID).FirstOrDefault();
                return perfilRBAC.Perfil_Nome;
            }
        }        

        public string GetUsuarioEndereco(int pessoaID)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
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

        public AlterarEnderecoViewModel GetEndereco(int pessoaID)
        {
            AlterarEnderecoViewModel AEVM = new AlterarEnderecoViewModel();
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                Negocio_Endereco NE = db.Negocio_Endereco.Where(o => o.Pessoa_ID == pessoaID).FirstOrDefault();
                AEVM.PessoaID = NE.Pessoa_ID;
                AEVM.CEP = NE.CEP;
                AEVM.Complemento = NE.Complemento;
                AEVM.Numero = NE.Numero.Value;
                AEVM.Logradouro = NE.Negocio_Base_Endereco.Logradouro;
                AEVM.Bairro = NE.Negocio_Base_Endereco.Bairro;
                AEVM.Cidade = NE.Negocio_Base_Endereco.Cidade;
                AEVM.Estado = NE.Negocio_Base_Endereco.Estado;
                AEVM.Pais = NE.Negocio_Base_Endereco.Pais;

                return AEVM;
            }
        }
        #endregion

        #endregion

        #region UPDATE

        #region UPDATE-Usuario
        public void UpdateContaUsuario(PerfilDeUsuarioViewModel usuario)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        RBAC_Usuario u = db.RBAC_Usuario.Find(usuario.UsuarioID);

                        u.Usuario_ID = usuario.UsuarioID;
                        u.Username = usuario.Email;
                        //u.Senha_Hash = usuario.SenhaHash; (Foram retirados de PerfilDeUsuarioViewModel (AccountViewModel.cs))
                        //u.Salt = usuario.Salt;
                        //u.Dt_Criacao = usuario.DtCriacao;
                        u.Dt_Ultima_Modif = DateTime.Now;
                        u.Bloqueado = usuario.Bloqueado == "Bloqueado" ? true : false;

                        db.Entry(u).State = EntityState.Modified;
                        db.SaveChanges();

                        var pessoa = db.Negocio_Pessoa.Where(o => o.Pessoa_ID == usuario.PessoaID);
                        if (pessoa.Any())
                        {
                            Negocio_Pessoa p = pessoa.FirstOrDefault();
                            p.Pessoa_ID = usuario.PessoaID;
                            p.Primeiro_Nome = usuario.PrimeiroNome;
                            p.Sobrenome = usuario.Sobrenome;
                            p.CPF = usuario.CPF;
                            p.RG = usuario.RG;
                            p.Sexo = usuario.Sexo;
                            p.Dt_Nascimento = usuario.DtNascimento;
                            p.Tel_Principal = usuario.TelPrincipal;
                            p.Tel_Opcional = usuario.TelSecundario;
                            p.Email = usuario.Email;
                            //p.Email_Confirmado = usuario.EmailConfirmado;
                            p.Tot_Notif_NL = p.Tot_Notif_NL;

                            db.Entry(p).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        dbContextTransaction.Commit();
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }
        }
        #endregion

        #region UPDATE-ENDERECO

        public void UpdateEndereco(AlterarEnderecoViewModel endereco)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var atualizarNE = db.Negocio_Endereco.Where(o => o.Pessoa_ID == endereco.PessoaID).FirstOrDefault();
                        Negocio_Base_Endereco NBE = db.Negocio_Base_Endereco.Where(o => o.CEP == endereco.CEP).FirstOrDefault();
                        if (NBE != null)
                        {
                            Negocio_Endereco NE = new Negocio_Endereco();
                            NE.CEP = endereco.CEP;
                            NE.Complemento = endereco.Complemento;
                            NE.Numero = endereco.Numero;
                            NE.Ativo = true;
                            NE.Pessoa_ID = endereco.PessoaID;

                            db.Negocio_Endereco.Add(NE);
                            db.SaveChanges();

                            atualizarNE.Ativo = false;
                            db.Entry(atualizarNE).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            Negocio_Base_Endereco novoNBE = new Negocio_Base_Endereco();
                            novoNBE.CEP = endereco.CEP;
                            novoNBE.Logradouro = endereco.Logradouro;
                            novoNBE.Bairro = endereco.Bairro;
                            novoNBE.Cidade = endereco.Cidade;
                            novoNBE.Estado = endereco.Estado;
                            novoNBE.Pais = endereco.Pais;

                            db.Negocio_Base_Endereco.Add(novoNBE);
                            db.SaveChanges();

                            Negocio_Endereco NE = new Negocio_Endereco();
                            NE.CEP = endereco.CEP;
                            NE.Complemento = endereco.Complemento;
                            NE.Numero = endereco.Numero;
                            NE.Ativo = true;
                            NE.Pessoa_ID = endereco.PessoaID;

                            db.Negocio_Endereco.Add(NE);
                            db.SaveChanges();

                            atualizarNE.Ativo = false;
                            db.Entry(atualizarNE).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        dbContextTransaction.Commit();
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }
        }

        #endregion

        #region UPDATE-PASSWORD

        #region FORGOT-PASSWORD
        public string ForgotPassword(int usuarioID)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                var usuario = db.RBAC_Usuario.Find(usuarioID);
                if(usuario != null)
                {
                    string newSalt = SecurityMethods.GenerateSalt();
                    string newToken = SecurityMethods.GenerateTempTokenAccess();
                    string newTokenEncrypted = SecurityMethods.HashPasswordPBKDF2(newToken, newSalt);

                    usuario.Senha_Hash = newTokenEncrypted;
                    usuario.Salt = newSalt;
                    db.Entry(usuario).State = EntityState.Modified;
                    db.SaveChanges();

                    return newToken;
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        #endregion

        #region CHANGE-PASSWORD
        public OperationStatus ChangePassword(int usuarioID, AlterarSenhaViewModel model)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                var usuario = db.RBAC_Usuario.Find(usuarioID);
                if (usuario != null)
                {
                    string newSalt = SecurityMethods.GenerateSalt();
                    string newEncrypted = SecurityMethods.HashPasswordPBKDF2(model.Senha, newSalt);

                    usuario.Senha_Hash = newEncrypted;
                    usuario.Salt = newSalt;

                    db.Entry(usuario).State = EntityState.Modified;
                    db.SaveChanges();

                    return OperationStatus.Success;
                }
                else
                {
                    return OperationStatus.Failure;
                }
            }
        }
        #endregion

        #endregion

        #endregion

        #region DELETE

        public void DeleteUsuario(int usuarioID)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (RBAC_Link_Usuario_Perfil lup in db.RBAC_Link_Usuario_Perfil)
                        {
                            if (lup.Usuario_ID == usuarioID)
                            {
                                db.RBAC_Link_Usuario_Perfil.Remove(lup);
                                db.SaveChanges();
                            }
                        }

                        //var linkUP = db.RBAC_Link_Usuario_Perfil.Where(o => o.Usuario_ID == usuarioID);
                        //if (linkUP.Any())
                        //{
                        //    db.RBAC_Link_Usuario_Perfil.Remove(linkUP.FirstOrDefault());
                        //    db.SaveChanges();
                        //}

                        var usuario = db.RBAC_Usuario.Where(o => o.Usuario_ID == usuarioID);
                        if (usuario.Any())
                        {
                            db.RBAC_Usuario.Remove(usuario.FirstOrDefault());
                            db.SaveChanges();
                        }

                        dbContextTransaction.Commit();
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }
        }

        #endregion
    }
}