using System;
using System.Collections.Generic;
using System.Linq;
using NimbusACAD.Models.ViewModels;
using NimbusACAD.Models.DB;
using System.Data.Entity;

namespace NimbusACAD.Identity.Role
{
    public class PermissionStore
    {
        #region CREATE

        public void AddPermission(CriarPermissaoRBACViewModel permissao)
        {
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
            {
                RBAC_Permissao rbacPermissao = new RBAC_Permissao();
                rbacPermissao.Permissao_Nome = permissao.PermissaoNm;

                db.RBAC_Permissao.Add(rbacPermissao);
                db.SaveChanges();
            }
        }

        #endregion

        #region GET

        public int GetPermissaoID(string nmPermissao)
        {
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
            {
                var permissao = db.RBAC_Permissao.Where(o => o.Permissao_Nome.Equals(nmPermissao));
                if (permissao.Any())
                {
                    return permissao.FirstOrDefault().Permissao_ID;
                }
                else
                {
                    return 0;
                }
            }
        }

        public string GetPermissaoNome(int id)
        {
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
            {
                var permissao = db.RBAC_Permissao.Where(o => o.Permissao_ID == id);
                if (permissao.Any())
                {
                    return permissao.FirstOrDefault().Permissao_Nome;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public List<ListaPerfisPermissaoViewModel> GetPerfisDePermissao(string nmPermissao)
        {
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
            {
                List<ListaPerfisPermissaoViewModel> perfis = new List<ListaPerfisPermissaoViewModel>();
                ListaPerfisPermissaoViewModel LPPVM;
                RBAC_Perfil pTemp;
                int permissaoID = db.RBAC_Permissao.Where(o => o.Permissao_Nome.Equals(nmPermissao)).FirstOrDefault().Permissao_ID;
                foreach (RBAC_Link_Perfil_Permissao lpp in db.RBAC_Link_Perfil_Permissao)
                {
                    if (lpp.Permissao_ID == permissaoID)
                    {
                        pTemp = db.RBAC_Perfil.Find(lpp.Perfil_ID);

                        LPPVM = new ListaPerfisPermissaoViewModel();
                        LPPVM.perfilID = pTemp.Perfil_ID;
                        LPPVM.PerfilNm = pTemp.Perfil_Nome;

                        perfis.Add(LPPVM);
                    }
                }
                return perfis;
            }
        }

        #endregion

        #region MANAGE-ROLE

        public void AddPermissaoPerfil(VinculoPerfilPermissaoViewModel vinculo)
        {
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
            {
                RBAC_Link_Perfil_Permissao linkPP = new RBAC_Link_Perfil_Permissao();
                linkPP.Perfil_ID = vinculo.PerfilID;
                linkPP.Permissao_ID = vinculo.PermissaoID;

                db.RBAC_Link_Perfil_Permissao.Add(linkPP);
                db.SaveChanges();
            }
        }
        
        public void RemovePermissaoPerfil(int pmID, int pfID)
        {
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var linkPP = db.RBAC_Link_Perfil_Permissao.Where(o => o.Perfil_ID == pfID && o.Permissao_ID == pmID);
                        if (linkPP.Any())
                        {
                            db.RBAC_Link_Perfil_Permissao.Remove(linkPP.FirstOrDefault());
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

        public void UpdatePermissao(CriarPermissaoRBACViewModel permissao)
        {
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        RBAC_Permissao p = db.RBAC_Permissao.Where(o => o.Permissao_ID == permissao.PermissaoID).FirstOrDefault();
                        p.Permissao_Nome = permissao.PermissaoNm;

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

        public void DeletePermissao(int permissaoID)
        {
            using (NimbusAcad_DB_Entities db = new NimbusAcad_DB_Entities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach(RBAC_Link_Perfil_Permissao lpp in db.RBAC_Link_Perfil_Permissao)
                        {
                            if (lpp.Permissao_ID == permissaoID)
                            {
                                db.RBAC_Link_Perfil_Permissao.Remove(lpp);
                                db.SaveChanges();
                            }
                        }

                        var permissao = db.RBAC_Permissao.Find(permissaoID);
                        if (permissao != null)
                        {
                            db.RBAC_Permissao.Remove(permissao);
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