﻿using NimbusACAD.Models.DB;
using NimbusACAD.Models.ViewModels;
using NimbusACAD.Identity.Security;
using NimbusACAD.Identity;
using System.Linq;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace NimbusACAD.Identity.User
{
    public class UserStore
    {
        public UserStore() { }

        public UserStore(RBAC_Usuario usuario) { }

        #region CREATE

        //Add usuario será feito no AccountController, para enviar o email com a senha temporaria.
        public int AddPessoa(RegistrarComumViewModel pessoa)
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
                NP.Email_Confirmado = false;
                NP.Tot_Notif_NL = 0;

                db.Negocio_Pessoa.Add(NP);
                db.SaveChanges();

                //Endereço --> Verificar se já existe endereço no CEP informado
                var endereco = db.Negocio_Base_Endereco.Where(o => o.CEP.Equals(pessoa.CEP)).FirstOrDefault();
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
                return GetPessoaIDporEmail(pessoa.Email);
            }
        }

        public bool IsUsuarioInPerfil(string usuarioNome, string perfilNome)
        {
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
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

        public bool IsPermissaoInPerfisDeUsuario(int usuarioID, string permission)
        {
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
            {
                if (usuarioID != 0)
                {
                    int perfilID = db.RBAC_Link_Usuario_Perfil.Where(o => o.Usuario_ID == usuarioID).FirstOrDefault().Perfil_ID;

                    var _permissao = db.RBAC_Link_Perfil_Permissao.Where(o => o.Perfil_ID == perfilID & o.RBAC_Permissao.Permissao_Nome.Equals(permission)).FirstOrDefault();
                    bool found = _permissao != null ? true : false;
                    return found;
                }
                return false;
            }
        }

        #endregion

        #region GET

        #region GET-RBAC_Usuario
        public int GetUsuarioID(string Email)
        {
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
            {
                var usuario = db.RBAC_Usuario.Where(o => o.Username.Equals(Email)).FirstOrDefault().Usuario_ID;
                //if (usuario.Any())
                //{
                //    return usuario.FirstOrDefault().Usuario_ID;
                //}
                return usuario;
            }
            //return 0;
        }

        public bool GetEmailUsernameExist(string Email)
        {
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
            {
                return db.RBAC_Usuario.Where(o => o.Username.Equals(Email)).Any();
            }
        }

        public string GetUsuarioSenha(string Email)
        {
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
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
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
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
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
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
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
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
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
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
        #endregion

        #region GET-Perfil_Completo
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
                RBAC_Usuario usuario = db.RBAC_Usuario.Where(o => o.Username.Equals(nomeUsuario)).FirstOrDefault();

                if (nomeUsuario.Equals("Admin"))
                {
                    PUVM.UsuarioID = usuario.Usuario_ID;
                    PUVM.PessoaID = 0;
                    PUVM.Email = "Sem Email";
                    PUVM.PrimeiroNome = "Nome de Usuario" + nomeUsuario;
                    PUVM.Sobrenome = "Sem sobrenome";
                    PUVM.CPF = "Sem CPF";
                    PUVM.RG = "Sem RG";
                    PUVM.Sexo = "Sem Sexo";
                    PUVM.DtNascimento = DateTime.Now;
                    PUVM.TelPrincipal = "Sem Tel Principal";
                    PUVM.TelSecundario = "Sem Tel Secundario";
                    PUVM.EndCompleto = "Sem Endereço";
                    PUVM.DtModif = DateTime.Now;
                    PUVM.Bloqueado = "Desbloqueado";
                    PUVM.Perfil = GetUsuarioPerfilRBACNome(usuario.Usuario_ID) + usuario.RBAC_Link_Usuario_Perfil.FirstOrDefault().RBAC_Perfil.RBAC_Link_Perfil_Permissao.FirstOrDefault().RBAC_Permissao.Permissao_Nome;
                }
                else
                {
                    Negocio_Pessoa pessoa = db.Negocio_Pessoa.Find(usuario.Pessoa_ID);

                    PUVM.UsuarioID = usuario.Usuario_ID;
                    PUVM.PessoaID = pessoa.Pessoa_ID;
                    PUVM.Email = pessoa.Email;
                    PUVM.PrimeiroNome = pessoa.Primeiro_Nome;
                    PUVM.Sobrenome = pessoa.Sobrenome;
                    PUVM.CPF = pessoa.CPF;
                    PUVM.RG = pessoa.RG;
                    PUVM.Sexo = pessoa.Sexo;
                    PUVM.DtNascimento = pessoa.Dt_Nascimento.Value;
                    PUVM.TelPrincipal = pessoa.Tel_Principal;
                    PUVM.TelSecundario = pessoa.Tel_Opcional;
                    PUVM.EndCompleto = GetUsuarioEndereco(pessoa.Pessoa_ID);
                    PUVM.DtModif = usuario.Dt_Ultima_Modif.Value;
                    PUVM.Bloqueado = usuario.Bloqueado.Value ? "Bloqueado" : "Desbloqueado";
                    PUVM.Perfil = GetUsuarioPerfilRBACNome(usuario.Usuario_ID);
                }
                return PUVM;
            }
        }
        #endregion

        #region GET-Auxiliares
        public string GetUsuarioPerfilRBACNome(int usuarioID)
        {
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
            {
                RBAC_Link_Usuario_Perfil linkUP = db.RBAC_Link_Usuario_Perfil.Where(o => o.Usuario_ID == usuarioID).FirstOrDefault();
                RBAC_Perfil perfilRBAC = db.RBAC_Perfil.Where(o => o.Perfil_ID == linkUP.Perfil_ID).FirstOrDefault();
                return perfilRBAC.Perfil_Nome;
            }
        }        

        public string GetUsuarioEndereco(int pessoaID)
        {
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
            {
                var endereco = db.Negocio_Endereco.Where(o => o.Pessoa_ID == pessoaID && o.Ativo == true).FirstOrDefault();
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
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
            {
                Negocio_Endereco NE = db.Negocio_Endereco.Where(o => o.Pessoa_ID == pessoaID && o.Ativo == true).FirstOrDefault();
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
        public async Task UpdateContaUsuario(PerfilDeUsuarioViewModel usuario)
        {
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
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
                        await db.SaveChangesAsync();

                        var pessoa = db.Negocio_Pessoa.Where(o => o.Pessoa_ID == usuario.PessoaID).FirstOrDefault(); ;
                        if (pessoa != null)
                        {
                            //Negocio_Pessoa p = pessoa.FirstOrDefault();
                            //pessoa.Pessoa_ID = usuario.PessoaID;
                            pessoa.Primeiro_Nome = usuario.PrimeiroNome;
                            pessoa.Sobrenome = usuario.Sobrenome;
                            pessoa.CPF = usuario.CPF;
                            pessoa.RG = usuario.RG;
                            pessoa.Sexo = usuario.Sexo;
                            pessoa.Dt_Nascimento = usuario.DtNascimento;
                            pessoa.Tel_Principal = usuario.TelPrincipal;
                            pessoa.Tel_Opcional = usuario.TelSecundario;
                            pessoa.Email = usuario.Email;
                            //p.Email_Confirmado = usuario.EmailConfirmado;
                            //pessoa.Tot_Notif_NL = pessoa.Tot_Notif_NL;

                            db.Entry(pessoa).State = EntityState.Modified;
                            await db.SaveChangesAsync();
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

        public async Task UpdateEndereco(AlterarEnderecoViewModel endereco)
        {
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        Negocio_Endereco atualizarNE = db.Negocio_Endereco.Where(o => o.Pessoa_ID == endereco.PessoaID && o.Ativo == true).FirstOrDefault();
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
                            await db.SaveChangesAsync();

                            atualizarNE.Ativo = false;
                            db.Entry(atualizarNE).State = EntityState.Modified;
                            await db.SaveChangesAsync();
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
                            await db.SaveChangesAsync();

                            Negocio_Endereco NE = new Negocio_Endereco();
                            NE.CEP = endereco.CEP;
                            NE.Complemento = endereco.Complemento;
                            NE.Numero = endereco.Numero;
                            NE.Ativo = true;
                            NE.Pessoa_ID = endereco.PessoaID;

                            db.Negocio_Endereco.Add(NE);
                            await db.SaveChangesAsync();

                            atualizarNE.Ativo = false;
                            db.Entry(atualizarNE).State = EntityState.Modified;
                            await db.SaveChangesAsync();
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
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
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
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
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
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
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

    public static class UserManager
    {
        static NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities();

        public static UserStore GetUsuario(int usuarioID)
        {
            RBAC_Usuario _usuario = db.RBAC_Usuario.Find(usuarioID);
            UserStore _userStore = new UserStore(_usuario);
            return _userStore;
        }

        public static RBAC_Usuario usuario(string username)
        {
            RBAC_Usuario _usuario = db.RBAC_Usuario.Where(o => o.Username.Equals(username)).FirstOrDefault();
            return _usuario;
        }
    }
}