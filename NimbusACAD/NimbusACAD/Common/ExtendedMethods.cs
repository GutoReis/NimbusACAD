using System;
using System.Configuration;

public enum OperationStatus
{
    Success,
    LockedOut,
    RequiresVerification,
    Failure
}

public class ExtendedMethods
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
}