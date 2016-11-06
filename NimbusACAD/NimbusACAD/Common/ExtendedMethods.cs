using NimbusACAD.Identity.User;
using NimbusACAD.Models.DB;
using System;
using System.Configuration;
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
                var ci = _principal.Identity as ClaimsIdentity;
                string _userID = ci != null ? ci.FindFirstValue(ClaimTypes.NameIdentifier) : null;
                if (!string.IsNullOrEmpty(_userID))
                {
                    UserStore _authenticatedUser = UserManager.GetUsuario(int.Parse(_userID));
                    _retVal = _authenticatedUser.IsPermissaoInPerfisDeUsuario(int.Parse(_userID), _requiredPermission);                    
                }
            }
        }
        catch (Exception)
        {
        }
        return _retVal;
    }

    public static string FindFirstValue(this ClaimsIdentity identity, string claimType)
    {
        string _retVal = string.Empty;
        try
        {
            if (identity != null)
            {
                var claim = identity.FindFirst(claimType);
                _retVal = claim != null ? claim.Value : null;
            }
        }
        catch (Exception)
        {
        }
        return _retVal;
    }

    #endregion
}