using Clinic_Management_System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Clinic_Management_System
{
	public static class Common
	{
		public static string ApplicationName = System.Configuration.ConfigurationManager.AppSettings["ApplicationName"];
		private static string EncrKey = System.Configuration.ConfigurationManager.AppSettings["EncrKey"];
		public static string DbConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DbConnectionString"].ConnectionString;

		private static List<UserMenuAccess> UserMenuAccess;
		private static List<UserMenuAccess> UserMenuPermission;

		private static HttpSessionStateBase session;

		public static void Set_SessionState(HttpSessionStateBase _session) => session = _session;
		public static HttpSessionStateBase Get_SessionState => session;
		public static void Clear_Session()
		{
			session.Abandon();
			session.Clear();
		}

		public static void Set_Session_Int(string key, Int64 value) => session[key] = value;
		public static long Get_Session_Int(string key) => Convert.ToInt64(session[key] ?? "0");

		public static void Set_Session(string key, string value) => session[key] = value;
		public static string Get_Session(string key) => Convert.ToString(session[key] ?? null);



		private static string controller_action;
		public static void Set_Controller_Action(string _value) => controller_action = _value;
		public static string Get_Controller_Action => controller_action;


		public static bool IsUserLogged() => session.Keys != null && session[SessionKey.USER_ID] != null && Convert.ToInt64(session[SessionKey.USER_ID]) > 0;
		public static bool IsSuperAdmin() => session.Keys != null && session[SessionKey.ROLE_ID] != null && Convert.ToInt64(session[SessionKey.ROLE_ID]) == 1;
		public static bool IsAdmin() => session.Keys != null && session[SessionKey.ROLE_ADMIN] != null && Convert.ToInt64(session[SessionKey.ROLE_ADMIN]) == 1;
		public static Int64 LoggedUser_Id() => session.Keys != null && session[SessionKey.USER_ID] != null ? Convert.ToInt64(session[SessionKey.USER_ID]) : 0;
		public static Int64 LoggedUser_RoleId() => session.Keys != null && session[SessionKey.USER_ID] != null ? Convert.ToInt64(session[SessionKey.ROLE_ID]) : 0;


		public static void Configure_UserMenuAccess(List<UserMenuAccess> userMenuAccess, List<UserMenuAccess> userMenuPermission)
		{
			UserMenuAccess = userMenuAccess;
			UserMenuPermission = userMenuPermission;
		}


		public static List<UserMenuAccess> GetUserMenuAccesses() => UserMenuAccess;
		public static List<UserMenuAccess> GetUserMenuPermission() => UserMenuPermission;

		public static string Encrypt(string strText)
		{
			byte[] byKey = { };
			byte[] IV = {
							0x12,
							0x34,
							0x56,
							0x78,
							0x90,
							0xab,
							0xcd,
							0xef
						};
			try
			{
				//byKey = System.Text.Encoding.UTF8.GetBytes(Strings.Left(strEncrKey, 8));
				byKey = System.Text.Encoding.UTF8.GetBytes(EncrKey.Substring(0, 8));
				//byKey = System.Text.Encoding.UTF8.GetBytes(Strings.Left(strEncrKey, 8));
				DESCryptoServiceProvider des = new DESCryptoServiceProvider();
				byte[] inputByteArray = Encoding.UTF8.GetBytes(strText);
				MemoryStream ms = new MemoryStream();
				CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
				cs.Write(inputByteArray, 0, inputByteArray.Length);
				cs.FlushFinalBlock();
				return Convert.ToBase64String(ms.ToArray());

			}
			catch (ExecutionEngineException ex)
			{
				return ex.Message;
			}

		}

		public static string Decrypt(string strText)
		{
			byte[] byKey = { };
			byte[] IV = {
							0x12,
							0x34,
							0x56,
							0x78,
							0x90,
							0xab,
							0xcd,
							0xef
						};
			byte[] inputByteArray = new byte[strText.Length + 1];
			try
			{
				//byKey = System.Text.Encoding.UTF8.GetBytes(Strings.Left(sDecrKey, 8));
				byKey = System.Text.Encoding.UTF8.GetBytes(EncrKey.Substring(0, 8));
				System.Security.Cryptography.DESCryptoServiceProvider des = new System.Security.Cryptography.DESCryptoServiceProvider();
				inputByteArray = Convert.FromBase64String(strText);
				MemoryStream ms = new MemoryStream();
				CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);

				cs.Write(inputByteArray, 0, inputByteArray.Length);
				cs.FlushFinalBlock();
				System.Text.Encoding encoding = System.Text.Encoding.UTF8;

				return encoding.GetString(ms.ToArray());

			}
			catch (ExecutionEngineException ex)
			{
				return ex.Message;
			}
		}
	}

	public class CurrentUser
	{
		public int UserId { get; set; }
		public string UserName { get; set; }
		public int RoleId { get; set; }
		public string Role { get; set; }
		public string UserImagePath { get; set; }
	}

	public static class Status
	{
		public static int FAILED => 0;
		public static int SUCCESS => 1;
		public static int NOT_FOUND => 2;
		public static int ALREADY_EXIST => 3;

	};

	public static class AccessType
	{
		public static int None => 0;
		public static int ReadOnly => 1;
		public static int ReadWrite => 2;

		public static int Get(AccessType_Enum x)
		{
			if (x == AccessType_Enum.ReadOnly)
				return ReadOnly;
			else if (x == AccessType_Enum.ReadWrite)
				return ReadWrite;
			else
				return None;
		}
	};

	public enum AccessType_Enum { None = 0x0, ReadOnly = 0x1, ReadWrite = 0x2 };

	public static class AccessControlType
	{
		public static bool Allow => true;
		public static bool Deny => false;
	};


}