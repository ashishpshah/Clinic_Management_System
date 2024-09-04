using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using Clinic_Management_System;
using System.Xml.Linq;
using System.Data.Entity;
using System.Runtime;

namespace Clinic_Management_System
{
	public static class DataContext_Command
	{
		public static DateTime? nullDateTime = null;
		public static DataTable ExecuteQuery_DataTable(string query)
		{
			try
			{
				DataTable dt = new DataTable();

				SqlConnection connection = new SqlConnection(Common.DbConnectionString);

				SqlDataAdapter oraAdapter = new SqlDataAdapter(query, connection);

				oraAdapter.Fill(dt);

				return dt;
			}
			catch (Exception ex)
			{
				//LogService.LogInsert("ExecuteQuery_DataTable - DataContext", "", ex);
				return null;
			}

		}

		public static DataSet ExecuteQuery_DataSet(string sqlquerys)
		{
			DataSet ds = new DataSet();

			try
			{
				DataTable dt = new DataTable();

				SqlConnection connection = new SqlConnection(Common.DbConnectionString);

				foreach (var sqlquery in sqlquerys.Split(';'))
				{
					dt = new DataTable();

					SqlDataAdapter oraAdapter = new SqlDataAdapter(sqlquery, connection);

					SqlCommandBuilder oraBuilder = new SqlCommandBuilder(oraAdapter);

					oraAdapter.Fill(dt);

					if (dt != null)
						ds.Tables.Add(dt);
				}

			}
			catch (Exception ex)
			{
				//LogService.LogInsert("ExecuteQuery_DataSet - DataContext", "", ex);
				return null;
			}

			return ds;
		}

		public static DataTable ExecuteStoredProcedure_DataTable(string query, SqlParameter[] parameters = null)
		{
			DataTable dt = new DataTable();

			try
			{
				using (SqlConnection conn = new SqlConnection(Common.DbConnectionString))
				{
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						cmd.CommandType = CommandType.StoredProcedure;

						if (parameters != null)
							foreach (SqlParameter param in parameters)
								cmd.Parameters.Add(param);

						SqlDataAdapter da = new SqlDataAdapter(cmd);

						da.Fill(dt);

						parameters = null;
					}
				}
			}
			catch (Exception ex)
			{
				//LogService.LogInsert("ExecuteStoredProcedure_DataTable - DataContext", "", ex);
				return null;
			}

			return dt;
		}

		public static DataSet ExecuteStoredProcedure_DataSet(string sp, SqlParameter[] spCol = null)
		{
			try
			{
				using (SqlConnection con = new SqlConnection(Common.DbConnectionString))
				{
					using (SqlCommand cmd = new SqlCommand(sp, con))
					{
						cmd.CommandType = CommandType.StoredProcedure;

						if (spCol != null && spCol.Length > 0)
							cmd.Parameters.AddRange(spCol);

						using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
						{
							DataSet ds = new DataSet();
							adp.Fill(ds);
							return ds;
						}
					}
				}
			}
			catch (Exception ex)
			{
				//LogService.LogInsert("ExecuteStoredProcedure_DataSet - DataContext", "", ex);
				return null;
			}
		}

		public static bool ExecuteNonQuery(string query, SqlParameter[] parameters = null)
		{
			try
			{
				using (SqlConnection con = new SqlConnection(Common.DbConnectionString))
				{
					con.Open();

					SqlCommand cmd = con.CreateCommand();

					cmd.CommandType = CommandType.Text;
					cmd.CommandText = query;

					if (parameters != null)
						foreach (SqlParameter param in parameters)
							cmd.Parameters.Add(param);

					cmd.ExecuteNonQuery();
				}

				return true;
			}
			catch (Exception ex)
			{
				//LogService.LogInsert("ExecuteNonQuery - DataContext", "", ex);
				return false;
			}
		}

		public static dynamic ExecuteStoredProcedure(string query, SqlParameter[] parameters = null, bool IsOutParam = true)
		{
			try
			{
				dynamic exo = new System.Dynamic.ExpandoObject();

				using (SqlConnection con = new SqlConnection(Common.DbConnectionString))
				{
					con.Open();

					SqlCommand cmd = con.CreateCommand();

					cmd.CommandType = CommandType.StoredProcedure;
					cmd.CommandText = query;

					if (parameters != null && parameters.Count() > 0)
						foreach (SqlParameter param in parameters)
							cmd.Parameters.Add(param);

					if (IsOutParam == true)
						cmd.Parameters.Add(new SqlParameter("@response", SqlDbType.NVarChar, 1000) { Direction = ParameterDirection.Output });

					cmd.ExecuteNonQuery();

					SqlParameterCollection paramCollection = cmd.Parameters;

					for (int i = 0; i < paramCollection.Count; i++)
						if (paramCollection[i].Direction == ParameterDirection.Output)
							((IDictionary<String, Object>)exo).Add(paramCollection[i].ParameterName.Replace("@", ""), paramCollection[i].Value);
				}

				if (((IDictionary<String, Object>)exo) != null && ((IDictionary<String, Object>)exo).Count == 1 && ((IDictionary<String, Object>)exo).ContainsKey("response"))
					return Convert.ToString(((IDictionary<String, Object>)exo).FirstOrDefault().Value);

				return exo;
			}
			catch (Exception ex)
			{
				//LogService.LogInsert("ExecuteStoredProcedure - DataContext", "", ex);
				return null;
			}
		}

		public static bool ExecuteNonQuery_Delete(string query, SqlParameter[] parameters = null)
		{
			try
			{
				using (SqlConnection con = new SqlConnection(Common.DbConnectionString))
				{
					con.Open();

					SqlCommand cmd = con.CreateCommand();
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = query;

					if (parameters != null)
						foreach (SqlParameter param in parameters)
							cmd.Parameters.Add(param);

					cmd.ExecuteNonQuery();
				}

				return true;
			}
			catch (Exception ex)
			{
				//LogService.LogInsert("ExecuteNonQuery_Delete - DataContext", "", ex);
				return false;
			}
		}


		public static List<Department> Department_Get(long id = 0)
		{
			var listObj = new List<Department>();

			try
			{
				var parameters = new List<SqlParameter>();
				parameters.Add(new SqlParameter("Id", SqlDbType.BigInt) { Value = id, Direction = ParameterDirection.Input, IsNullable = true });
				parameters.Add(new SqlParameter("Name", SqlDbType.NVarChar) { Value = "", Direction = ParameterDirection.Input, IsNullable = true });
				parameters.Add(new SqlParameter("IsActive", SqlDbType.NVarChar) { Value = null, Direction = ParameterDirection.Input, IsNullable = true });
				parameters.Add(new SqlParameter("Operated_By", SqlDbType.BigInt) { Value = Common.Get_Session_Int(SessionKey.USER_ID), Direction = ParameterDirection.Input, IsNullable = true });
				parameters.Add(new SqlParameter("Operated_RoleId", SqlDbType.BigInt) { Value = Common.Get_Session_Int(SessionKey.ROLE_ID), Direction = ParameterDirection.Input, IsNullable = true });
				parameters.Add(new SqlParameter("Operated_MenuId", SqlDbType.BigInt) { Value = Common.Get_Session_Int(SessionKey.CURRENT_MENU_ID), Direction = ParameterDirection.Input, IsNullable = true });

				var dt = ExecuteStoredProcedure_DataTable("SP_Department_GET", parameters.ToArray());

				if (dt != null && dt.Rows.Count > 0)
					foreach (DataRow dr in dt.Rows)
						listObj.Add(new Department()
						{
							Id = dr["Id"] != DBNull.Value ? Convert.ToInt64(dr["Id"]) : 0,
							Name = dr["Name"] != DBNull.Value ? Convert.ToString(dr["Name"]) : "",
							IsActive = dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false
						});
			}

			catch (Exception ex) { /*LogService.LogInsert(GetCurrentAction(), "", ex);*/ }

			return listObj;
		}

		public static (bool, string, long) Department_Save(Department obj = null)
		{
			if (obj != null)
				try
				{
					var parameters = new List<SqlParameter>();
					parameters.Add(new SqlParameter("Id", SqlDbType.BigInt) { Value = obj.Id, Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("Name", SqlDbType.NVarChar) { Value = obj.Name, Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("IsActive", SqlDbType.NVarChar) { Value = obj.IsActive, Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("Operated_By", SqlDbType.BigInt) { Value = Common.Get_Session_Int(SessionKey.USER_ID), Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("Operated_RoleId", SqlDbType.BigInt) { Value = Common.Get_Session_Int(SessionKey.ROLE_ID), Direction = ParameterDirection.Input, IsNullable = true });
					//parameters.Add(new SqlParameter("Operated_DepartmentId", SqlDbType.BigInt) { Value = Common.LoggedUser_DepartmentId(), Direction = ParameterDirection.Input, IsNullable = true });
					//parameters.Add(new SqlParameter("Operated_BranchId", SqlDbType.BigInt) { Value = Common.LoggedUser_BranchId(), Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("Operated_MenuId", SqlDbType.BigInt) { Value = Common.Get_Session_Int(SessionKey.CURRENT_MENU_ID), Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("Action", SqlDbType.BigInt) { Value = obj.Id > 0 ? "UPDATE" : "INSERT", Direction = ParameterDirection.Input, IsNullable = true });

					var response = ExecuteStoredProcedure("SP_Department_Save", parameters.ToArray());

					var msgtype = response.Split('|').Length > 0 ? response.Split('|')[0] : "";
					var message = response.Split('|').Length > 1 ? response.Split('|')[1].Replace("\"", "") : "";
					var strid = response.Split('|').Length > 2 ? response.Split('|')[2].Replace("\"", "") ?? "0" : "0";

					return (msgtype.Contains("S"), message, Convert.ToInt64(strid));

				}
				catch (Exception ex) { /*LogService.LogInsert(GetCurrentAction(), "", ex);*/ }

			return (false, ResponseStatusMessage.Error, 0);
		}

		public static (bool, string, long) User_Save(User obj = null)
		{
			if (obj != null)
				try
				{
					var parameters = new List<SqlParameter>();
					parameters.Add(new SqlParameter("Id", SqlDbType.BigInt) { Value = obj.Id, Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("UserName", SqlDbType.NVarChar) { Value = obj.UserName, Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("Password", SqlDbType.NVarChar) { Value = obj.Password, Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("IsActive", SqlDbType.NVarChar) { Value = obj.IsActive, Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("Operated_By", SqlDbType.BigInt) { Value = Common.Get_Session_Int(SessionKey.USER_ID), Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("Operated_RoleId", SqlDbType.BigInt) { Value = Common.Get_Session_Int(SessionKey.ROLE_ID), Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("Operated_MenuId", SqlDbType.BigInt) { Value = Common.Get_Session_Int(SessionKey.CURRENT_MENU_ID), Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("Action", SqlDbType.BigInt) { Value = obj.Id > 0 ? "UPDATE" : "INSERT", Direction = ParameterDirection.Input, IsNullable = true });

					var response = ExecuteStoredProcedure("SP_User_CUD", parameters.ToArray());

					var msgtype = response.Split('|').Length > 0 ? response.Split('|')[0] : "";
					var message = response.Split('|').Length > 1 ? response.Split('|')[1].Replace("\"", "") : "";
					var strid = response.Split('|').Length > 2 ? response.Split('|')[2].Replace("\"", "") ?? "0" : "0";

					return (msgtype.Contains("S"), message, Convert.ToInt64(strid));

				}
				catch (Exception ex) { /*LogService.LogInsert(GetCurrentAction(), "", ex);*/ }

			return (false, ResponseStatusMessage.Error, 0);
		}

		public static List<Employee> Employee_Get(long id = 0)
		{
			var listObj = new List<Employee>();

			try
			{
				var parameters = new List<SqlParameter>();
				parameters.Add(new SqlParameter("Id", SqlDbType.BigInt) { Value = id, Direction = ParameterDirection.Input, IsNullable = true });

				parameters.Add(new SqlParameter("Operated_By", SqlDbType.BigInt) { Value = Common.Get_Session_Int(SessionKey.USER_ID), Direction = ParameterDirection.Input, IsNullable = true });
				parameters.Add(new SqlParameter("Operated_RoleId", SqlDbType.BigInt) { Value = Common.Get_Session_Int(SessionKey.ROLE_ID), Direction = ParameterDirection.Input, IsNullable = true });
				parameters.Add(new SqlParameter("Operated_MenuId", SqlDbType.BigInt) { Value = Common.Get_Session_Int(SessionKey.CURRENT_MENU_ID), Direction = ParameterDirection.Input, IsNullable = true });

				var dt = ExecuteStoredProcedure_DataTable("SP_Employee_GET", parameters.ToArray());

				if (dt != null && dt.Rows.Count > 0)
					foreach (DataRow dr in dt.Rows)
						listObj.Add(new Employee()
						{
							Id = dr["Id"] != DBNull.Value ? Convert.ToInt64(dr["Id"]) : 0,
							RoleId = dr["RoleId"] != DBNull.Value ? Convert.ToInt64(dr["RoleId"]) : 0,
							UserId = dr["UserId"] != DBNull.Value ? Convert.ToInt64(dr["UserId"]) : 0,
							UserName = dr["UserName"] != DBNull.Value ? Convert.ToString(dr["UserName"]) : "",
							MiddleName = dr["MiddleName"] != DBNull.Value ? Convert.ToString(dr["MiddleName"]) : "",
							FirstName = dr["FirstName"] != DBNull.Value ? Convert.ToString(dr["FirstName"]) : "",
							UserType = dr["UserType"] != DBNull.Value ? Convert.ToString(dr["UserType"]) : "",
							BirthDate = dr["BirthDate"] != DBNull.Value ? Convert.ToDateTime(dr["BirthDate"]) : nullDateTime,
							BirthDate_Text = dr["BirthDate_Text"] != DBNull.Value ? Convert.ToString(dr["BirthDate_Text"]) : "",
							IsActive = dr["IsActive"] != DBNull.Value ? Convert.ToBoolean(dr["IsActive"]) : false
						});
			}
			catch (Exception ex) { /*LogService.LogInsert(GetCurrentAction(), "", ex);*/ }

			return listObj;
		}

		public static (bool, string, long) Employee_Save(Employee obj = null)
		{
			if (obj != null)
				try
				{
					var parameters = new List<SqlParameter>();

					parameters.Add(new SqlParameter("Id", SqlDbType.BigInt) { Value = obj.Id, Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("UserId", SqlDbType.BigInt) { Value = obj.UserId, Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("RoleId", SqlDbType.BigInt) { Value = obj.RoleId, Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("UserName", SqlDbType.NVarChar) { Value = obj.UserName, Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("Password", SqlDbType.NVarChar) { Value = obj.Password, Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("FirstName", SqlDbType.NVarChar) { Value = obj.FirstName, Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("MiddleName", SqlDbType.NVarChar) { Value = obj.MiddleName, Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("LastName", SqlDbType.NVarChar) { Value = obj.LastName, Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("UserType", SqlDbType.NVarChar) { Value = obj.UserType, Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("BirthDate", SqlDbType.NVarChar) { Value = obj.BirthDate?.ToString("dd/MM/yyyy"), Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("IsActive", SqlDbType.NVarChar) { Value = obj.IsActive, Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("Operated_By", SqlDbType.BigInt) { Value = Common.Get_Session_Int(SessionKey.USER_ID), Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("Operated_RoleId", SqlDbType.BigInt) { Value = Common.Get_Session_Int(SessionKey.ROLE_ID), Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("Operated_MenuId", SqlDbType.BigInt) { Value = Common.Get_Session_Int(SessionKey.CURRENT_MENU_ID), Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("Action", SqlDbType.NVarChar) { Value = obj.Id > 0 ? "UPDATE" : "INSERT", Direction = ParameterDirection.Input, IsNullable = true });

					var response = ExecuteStoredProcedure("SP_Employee_Save", parameters.ToArray());

					var msgtype = response.Split('|').Length > 0 ? response.Split('|')[0] : "";
					var message = response.Split('|').Length > 1 ? response.Split('|')[1].Replace("\"", "") : "";
					var strid = response.Split('|').Length > 2 ? response.Split('|')[2].Replace("\"", "") ?? "0" : "0";

					return (msgtype.Contains("S"), message, Convert.ToInt64(strid));

				}
				catch (Exception ex) { /*LogService.LogInsert(GetCurrentAction(), "", ex);*/ }

			return (false, ResponseStatusMessage.Error, 0);
		}

		public static (bool, string) Employee_Status(long Id = 0, bool IsActive = false, bool IsDelete = false)
		{
			if (Id > 0)
				try
				{
					var parameters = new List<SqlParameter>();

					parameters.Add(new SqlParameter("Id", SqlDbType.BigInt) { Value = Id, Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("IsActive", SqlDbType.NVarChar) { Value = IsActive, Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("Operated_By", SqlDbType.BigInt) { Value = Common.Get_Session_Int(SessionKey.USER_ID), Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("Operated_RoleId", SqlDbType.BigInt) { Value = Common.Get_Session_Int(SessionKey.ROLE_ID), Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("Operated_MenuId", SqlDbType.BigInt) { Value = Common.Get_Session_Int(SessionKey.CURRENT_MENU_ID), Direction = ParameterDirection.Input, IsNullable = true });
					parameters.Add(new SqlParameter("Action", SqlDbType.NVarChar) { Value = IsDelete ? "DELETE" : "STATUS", Direction = ParameterDirection.Input, IsNullable = true });

					var response = ExecuteStoredProcedure("SP_Employee_Status", parameters.ToArray());

					var msgtype = response.Split('|').Length > 0 ? response.Split('|')[0] : "";
					var message = response.Split('|').Length > 1 ? response.Split('|')[1].Replace("\"", "") : "";
					var strid = response.Split('|').Length > 2 ? response.Split('|')[2].Replace("\"", "") ?? "0" : "0";

					return (msgtype.Contains("S"), message);

				}
				catch (Exception ex) { /*LogService.LogInsert(GetCurrentAction(), "", ex);*/ }

			return (false, ResponseStatusMessage.Error);
		}

	}
}