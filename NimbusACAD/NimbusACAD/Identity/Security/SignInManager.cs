using System.Linq;
using NimbusACAD.Models.DB;
using System.Web.Security;
using NimbusACAD.Identity.User;
using NimbusACAD.Identity.Email;
using System.Data.Entity;

namespace NimbusACAD.Identity.Security
{
    public class SignInManager
    {
        #region LOGIN

        UserStore US = new UserStore();

        public OperationStatus PasswordSignIn(string username, string password)
        {
            bool exist = US.GetEmailUsernameExist(username);
            if (exist)
            {
                string passDB = US.GetUsuarioSenha(username);
                string salt = US.GetUsuarioSalt(username);
                string access = SecurityMethods.HashPasswordPBKDF2(password, salt);

                if (access.Equals(passDB))
                {
                    if (US.GetUsuarioEmailVerificado(username))
                    {
                        if (US.GetUsuarioBloqueado(username))
                        {
                            return OperationStatus.LockedOut;
                        }
                        else
                        {
                            FormsAuthentication.SetAuthCookie(username, false);
                            return OperationStatus.Success;
                        }
                    }
                    else
                    {
                        return OperationStatus.RequiresVerification;
                    }
                }
                else
                {
                    return OperationStatus.Failure;
                }
            }
            else
            {
                return OperationStatus.Failure;
            }
        }

        #endregion

        #region CONFIRM-EMAIL

        public void SendEmail(string _userName, string _subject, string _body)
        {
            EmailMessage message = new EmailMessage(_userName, _subject, _body);
            EmailService service = new EmailService();
            service.Send(message);
        }

        public bool ConfirmEmail(string email)
        {
            using (NimbusAcad_DBEntities db = new NimbusAcad_DBEntities())
            {
                var pessoa = db.Negocio_Pessoa.Where(o => o.Email.Equals(email)).FirstOrDefault();
                if (!pessoa.Email_Confirmado.Value)
                {
                    pessoa.Email_Confirmado = true;
                    db.Entry(pessoa).State = EntityState.Modified;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion

        #region LOGOUT

        public void LogOut()
        {
            FormsAuthentication.SignOut();
        }

        #endregion
    }
}