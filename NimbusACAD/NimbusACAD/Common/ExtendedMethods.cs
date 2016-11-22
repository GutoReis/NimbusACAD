using NimbusACAD.Identity.User;
using NimbusACAD.Models.DB;
using System;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Mvc;

public enum OperationStatus
{
    Success,
    LockedOut,
    RequiresVerification,
    Failure
}

public static class ExtendedMethods
{
    #region HELPER_FUNCTIONS

    public static bool GetConfigSettingAsBool(string _name, bool _defaultValue = false)
    {
        bool _retVal = _defaultValue;
        try
        {
            _retVal = Convert.ToBoolean(ConfigurationManager.AppSettings[_name].ToString());
        }
        catch (Exception)
        {
        }
        return _retVal;
    }

    public static int GetConfigSettingAsInt(string _name, int _defaultValue = 0)
    {
        int _retVal = _defaultValue;
        try
        {
            _retVal = Convert.ToInt32(ConfigurationManager.AppSettings[_name].ToString());
        }
        catch (Exception)
        {
        }
        return _retVal;
    }

    public static double GetConfigSettingAsDouble(string _name, double _defaultValue = 0)
    {
        double _retVal = _defaultValue;
        try
        {
            _retVal = Convert.ToDouble(ConfigurationManager.AppSettings[_name].ToString());
        }
        catch (Exception)
        {
        }
        return _retVal;
    }

    public static string GetConfigSetting(string _name)
    {
        string _retVal = string.Empty;
        try
        {
            _retVal = ConfigurationManager.AppSettings[_name].ToString();
        }
        catch (Exception)
        {
        }
        return _retVal;
    }

    #endregion

    #region RBAC_FUNCTIONS

    public static bool HasPermission(this IPrincipal _principal, string _requiredPermission)
    {
        bool _retVal = false;
        try
        {
            if (_principal != null && _principal.Identity.IsAuthenticated)
            {
                NimbusAcad_DBEntities db = new NimbusAcad_DBEntities();
                string username = _principal.Identity.Name;
                int _userID = db.RBAC_Usuario.Where(o => o.Username.Equals(username)).FirstOrDefault().Usuario_ID;

                if (_userID != 0)
                {
                    UserStore _authenticatedUser = UserManager.GetUsuario(_userID);
                    _retVal = _authenticatedUser.IsPermissaoInPerfisDeUsuario(_userID, _requiredPermission);
                }
            }
        }
        catch (Exception)
        {
        }
        return _retVal;
    }

    #endregion
}