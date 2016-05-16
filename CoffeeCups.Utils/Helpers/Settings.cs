// Helpers/Settings.cs
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;

namespace CoffeeCups.Utils
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        const string LastSyncKey = "last_sync";
        static readonly DateTime LastSyncDefault = DateTime.Now.AddDays(-30);


        const string UserIdKey = "userid";
        static readonly string UserIdDefault = string.Empty;

        const string AuthTokenKey = "authtoken";
        static readonly string AuthTokenDefault = string.Empty;

        const string LoginAttemptsKey = "login_attempts";
        const int LoginAttemptsDefault = 0;

        const string NeedsSyncKey = "needs_sync";
        const bool NeedsSyncDefault = true;

        const string HasSyncedDataKey = "has_synced";
        const bool HasSyncedDataDefault = false;

        #endregion

        #if DEBUG
        public static bool NeedsSync
        {
            get { return true; }
            set { }
        }
        #else
        public static bool NeedsSync
        {
        get { return AppSettings.GetValueOrDefault<bool>(NeedsSyncKey, NeedsSyncDefault) || LastSync < DateTime.Now.AddDays(-1); }
        set { AppSettings.AddOrUpdateValue<bool>(NeedsSyncKey, value); }

        }
        #endif

        public static bool HasSyncedData
        {
            get { return AppSettings.GetValueOrDefault<bool>(HasSyncedDataKey, HasSyncedDataDefault); }
            set { AppSettings.AddOrUpdateValue<bool>(HasSyncedDataKey, value); }

        }

        public static DateTime LastSync
        {
            get
            {
                return AppSettings.GetValueOrDefault<DateTime>(LastSyncKey, LastSyncDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<DateTime>(LastSyncKey, value);
            }
        } 


        public static string AuthToken
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(AuthTokenKey, AuthTokenDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(AuthTokenKey, value);
            }
        }

        public static string UserId
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(UserIdKey, UserIdDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(UserIdKey, value);
            }
        }

        public static int LoginAttempts
        {
            get
            {
                return AppSettings.GetValueOrDefault<int>(LoginAttemptsKey, LoginAttemptsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<int>(LoginAttemptsKey, value);
            }
        }


        public static bool IsLoggedIn { get { return !string.IsNullOrWhiteSpace(UserId); } }




    }
}