using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using Clinic_Management_System;
using Newtonsoft.Json;

namespace Clinic_Management_System
{
	public static class LogEntry
	{
		public static void InsertLogEntry(Exception ex)
		{
			SqlParameter[] spCol = new SqlParameter[]
			{
				new SqlParameter("@ErrorCode", Common.Get_Controller_Action),
				new SqlParameter("@Errordesc", JsonConvert.SerializeObject(ex)),
				new SqlParameter("@Created_By", Common.LoggedUser_Id())
			};

			ExecuteSPForLogEntry("SP_Insertlog", spCol);
		}


		public static object ExecuteSPForLogEntry(string sp, SqlParameter[] spCol)
		{
			try
			{
				object result = string.Empty;
				using (SqlConnection con = new SqlConnection(Common.DbConnectionString))
				{
					using (SqlCommand cmd = new SqlCommand(sp, con))
					{
						cmd.CommandType = CommandType.StoredProcedure;
						if (spCol != null && spCol.Length > 0)
							cmd.Parameters.AddRange(spCol);

						con.Open();
						int InsertState = cmd.ExecuteNonQuery();
						return InsertState;
					}
				}
			}
			catch (Exception ex) { return null; }

		}


		public static void ErrorWrite(string code, string error, Exception ex = null)
		{
			try
			{
				var CreatedBy = HttpContext.Current.Session["UserId"] == null ? "" : HttpContext.Current.Session["UserId"].ToString();

				if (ex != null)
				{
					error = error + Environment.NewLine;

					if (ex.InnerException != null)
					{
						error = error + "InnerException: " + ex.InnerException.ToString().Substring(0, 1000);
						error = error + Environment.NewLine;
					}

					if (ex.StackTrace != null)
					{
						error = error + "StackTrace: " + ex.StackTrace.ToString().Substring(0, 1000);
						error = error + Environment.NewLine;
					}

					if (ex.Source != null)
					{
						error = error + "Source: " + ex.Source.ToString().Substring(0, 1000);
						error = error + Environment.NewLine;
					}

					if (ex.StackTrace == null && ex.Source == null)
					{
						error = error + "Exception: " + ex.ToString().Substring(0, 3000);
						error = error + Environment.NewLine;
					}
				}

				SqlParameter[] spCol = new SqlParameter[]
				{
				new SqlParameter("@ErrorCode",code),
				new SqlParameter("@Errordesc",error),
				new SqlParameter("@Created_By", CreatedBy)
				};

				ExecuteSPForLogEntry("SP_Insertlog", spCol);
			}
			catch { }
		}
	}
}