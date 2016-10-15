using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NimbusACAD.Models.ViewModels;
using NimbusACAD.Models.DB;
using System.Data.Entity;

namespace NimbusACAD.Identity.Role
{
    public class RoleStore
    {
        #region CREATE

        public void AddRole(CriarPerfilRBACViewModel perfil)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                RBAC_Perfil rbacPerfil = new RBAC_Perfil();
                rbacPerfil.Perfil_Nome = perfil.PerfilNm;
                rbacPerfil.Descricao = perfil.Descricao;

                db.RBAC_Perfil.Add(rbacPerfil);
                db.SaveChanges();
            }
        }

        public bool IsPermissaoEmPerfil(string nmPerfil, string nmPermissao)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                RBAC_Permissao permissao = db.RBAC_Permissao.Where(o => o.Permissao_Nome.ToLower().Equals(nmPermissao)).FirstOrDefault();
                if (permissao != null)
                {
                    var perfilRBAC = from q in db.RBAC_Link_Perfil_Permissao
                                     join r in db.RBAC_Perfil
                                     on q.Perfil_ID equals r.Perfil_ID
                                     where r.Perfil_Nome.Equals(nmPerfil) && q.Permissao_ID.Equals(permissao.Permissao_ID)
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

        public int GetPerfilID(string nmPerfil)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                var perfil = db.RBAC_Perfil.Where(o => o.Perfil_Nome.Equals(nmPerfil));
                if (perfil.Any())
                {
                    return perfil.FirstOrDefault().Perfil_ID;
                }
                else
                {
                    return 0;
                }
            }
        }

        public string GetPerfilNome(int id)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                var perfil = db.RBAC_Perfil.Where(o => o.Perfil_ID == id);
                if (perfil.Any())
                {
                    return perfil.FirstOrDefault().Perfil_Nome;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string GetPerfilDescricao(int id)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                var perfil = db.RBAC_Perfil.Where(o => o.Perfil_ID == id);
                if (perfil.Any())
                {
                    return perfil.FirstOrDefault().Descricao;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public string GetPerfilDescricao(string nmPerfil)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                var perfil = db.RBAC_Perfil.Where(o => o.Perfil_Nome.Equals(nmPerfil));
                if (perfil.Any())
                {
                    return perfil.FirstOrDefault().Descricao;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public List<ListaUsuariosPerfilViewModel> GetUsuariosDePerfil(string nmPerfil)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                List<ListaUsuariosPerfilViewModel> usuarios = new List<ListaUsuariosPerfilViewModel>();
                ListaUsuariosPerfilViewModel LUPVM;
                RBAC_Usuario uTemp;
                Negocio_Pessoa pTemp;
                int pid = db.RBAC_Perfil.Where(o => o.Perfil_Nome.Equals(nmPerfil)).FirstOrDefault().Perfil_ID;
                foreach (RBAC_Link_Usuario_Perfil lup in db.RBAC_Link_Usuario_Perfil)
                {
                    if (lup.Perfil_ID == pid)
                    {
                        uTemp = db.RBAC_Usuario.Find(lup.Usuario_ID);
                        pTemp = db.Negocio_Pessoa.Find(uTemp.Pessoa_ID);

                        LUPVM = new ListaUsuariosPerfilViewModel();
                        LUPVM.usuarioID = uTemp.Usuario_ID;
                        LUPVM.Email = uTemp.Username;
                        LUPVM.UsuarioNome = pTemp.Primeiro_Nome + " " + pTemp.Sobrenome;

                        usuarios.Add(LUPVM);
                    }
                }
                return usuarios;
            }
        }

        public List<ListaPermissoesPerfilViewModel> GetPermicoesDoPerfil(string nmPerfil)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                List<ListaPermissoesPerfilViewModel> permissoes = new List<ListaPermissoesPerfilViewModel>();
                ListaPermissoesPerfilViewModel LPPVM;
                RBAC_Permissao pTemp;
                int perfilID = db.RBAC_Perfil.Where(o => o.Perfil_Nome.Equals(nmPerfil)).FirstOrDefault().Perfil_ID;
                foreach(RBAC_Link_Perfil_Permissao lpp in db.RBAC_Link_Perfil_Permissao)
                {
                    if (lpp.Perfil_ID == perfilID)
                    {
                        pTemp = db.RBAC_Permissao.Find(lpp.Permissao_ID);

                        LPPVM = new ListaPermissoesPerfilViewModel();
                        LPPVM.permisssaoID = pTemp.Permissao_ID;
                        LPPVM.PermissaoNome = pTemp.Permissao_Nome;

                        permissoes.Add(LPPVM);
                    }
                }
                return permissoes;
            }
        }

        #endregion

        #region MANAGE-USER

        public void AddUsuarioPerfil(VinculoPerfilUsuarioViewModel vinculo)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                RBAC_Link_Usuario_Perfil linkUP = new RBAC_Link_Usuario_Perfil();
                linkUP.Perfil_ID = vinculo.PerfilID;
                linkUP.Usuario_ID = vinculo.UsuarioID;

                db.RBAC_Link_Usuario_Perfil.Add(linkUP);
                db.SaveChanges();
            }
        }

        public void RemoveUsuarioPerfil(int uID, int pID)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var linkUP = db.RBAC_Link_Usuario_Perfil.Where(o => o.Perfil_ID == pID && o.Usuario_ID == uID);
                        if (linkUP.Any())
                        {
                            db.RBAC_Link_Usuario_Perfil.Remove(linkUP.FirstOrDefault());
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

        #region UPDATE

        public void UpdatePerfil(CriarPerfilRBACViewModel perfil)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        RBAC_Perfil p = db.RBAC_Perfil.Where(o => o.Perfil_ID == perfil.PerfilID).FirstOrDefault();
                        p.Perfil_Nome = perfil.PerfilNm;
                        p.Descricao = perfil.Descricao;

                        db.Entry(p).State = EntityState.Modified;
                        db.SaveChanges();

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

        #region DELETE

        public void DeletePerfil(int perfilID)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach(RBAC_Link_Usuario_Perfil lup in db.RBAC_Link_Usuario_Perfil)
                        {
                            if (lup.Perfil_ID == perfilID)
                            {
                                db.RBAC_Link_Usuario_Perfil.Remove(lup);
                                db.SaveChanges();
                            }
                        }

                        foreach(RBAC_Link_Perfil_Permissao lpp in db.RBAC_Link_Perfil_Permissao)
                        {
                            if (lpp.Perfil_ID == perfilID)
                            {
                                db.RBAC_Link_Perfil_Permissao.Remove(lpp);
                                db.SaveChanges();
                            }
                        }

                        var perfil = db.RBAC_Perfil.Find(perfilID);
                        if (perfil != null)
                        {
                            db.RBAC_Perfil.Remove(perfil);
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