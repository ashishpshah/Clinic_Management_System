using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;

namespace Clinic_Management_System
{
	public partial class DataContext : DbContext
	{
		public DataContext() : base("name=DbConnectionString") { }

		public virtual DbSet<Attachment> Attachments { get; set; }
		public virtual DbSet<Department> Departments { get; set; }
		public virtual DbSet<Employee> Employees { get; set; }
		public virtual DbSet<Menu> Menus { get; set; }
		public virtual DbSet<Role> Roles { get; set; }
		public virtual DbSet<User> Users { get; set; }
		public virtual DbSet<UserLog> UserLogs { get; set; }
		public virtual DbSet<UserMenuAccess> UserMenuAccesses { get; set; }
		public virtual DbSet<UserRoleMapping> UserRoleMappings { get; set; }
		public virtual DbSet<RoleMenuAccess> RoleMenuAccesses { get; set; }


		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Attachment>().HasKey(e => new { e.Id }).ToTable("Attachments");
			modelBuilder.Entity<Department>().HasKey(e => new { e.Id }).ToTable("Department");
			modelBuilder.Entity<Employee>().HasKey(e => new { e.Id, e.UserId, e.RoleId }).ToTable("Employee");
			modelBuilder.Entity<User>().HasKey(e => new { e.Id, e.UserName }).ToTable("Users");
			modelBuilder.Entity<Role>().HasKey(e => new { e.Id }).ToTable("Roles");
			modelBuilder.Entity<UserRoleMapping>().HasKey(e => new { e.Id, e.UserId, e.RoleId }).ToTable("UserRoleMapping");
			modelBuilder.Entity<Menu>().HasKey(e => new { e.Id, e.ParentId }).ToTable("Menu");
			modelBuilder.Entity<UserMenuAccess>().HasKey(e => new { e.UserId, e.RoleId, e.MenuId, e.IsCreate, e.IsUpdate, e.IsRead, e.IsDelete }).ToTable("UserMenuAccess");
			modelBuilder.Entity<RoleMenuAccess>().HasKey(e => new { e.RoleId, e.MenuId, e.IsCreate, e.IsUpdate, e.IsRead, e.IsDelete }).ToTable("RoleMenuAccess");

			modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
			base.OnModelCreating(modelBuilder);
		}

		public int SaveChanges(CancellationToken cancellationToken = default)
		{
			var entities = (from entry in ChangeTracker.Entries()
							where entry.State == EntityState.Modified || entry.State == EntityState.Added
							select entry).ToList();

			var user = Common.LoggedUser_Id();
			var ipAddress = "";

			foreach (var entity in entities)
			{
				if (entity.State == EntityState.Added)
				{
					((EntitiesBase)entity.Entity).IsActive = true;
					((EntitiesBase)entity.Entity).IsDeleted = false;
					((EntitiesBase)entity.Entity).CreatedDate = DateTime.Now;
					((EntitiesBase)entity.Entity).CreatedBy = ((EntitiesBase)entity.Entity).CreatedBy == 0 ? user : ((EntitiesBase)entity.Entity).CreatedBy;
					((EntitiesBase)entity.Entity).LastModifiedDate = DateTime.Now;
					((EntitiesBase)entity.Entity).LastModifiedBy = ((EntitiesBase)entity.Entity).CreatedBy == 0 ? user : ((EntitiesBase)entity.Entity).CreatedBy;
				}

				if (entity.State == EntityState.Modified)
				{
					((EntitiesBase)entity.Entity).LastModifiedDate = DateTime.Now;
					((EntitiesBase)entity.Entity).LastModifiedBy = user;
				}

				if (entity.State == EntityState.Deleted)
				{
					((EntitiesBase)entity.Entity).IsActive = false;
					((EntitiesBase)entity.Entity).IsDeleted = true;
					((EntitiesBase)entity.Entity).LastModifiedDate = DateTime.Now;
					((EntitiesBase)entity.Entity).LastModifiedBy = user;
				}

				//entity.Entity.IPAddress = ipAddress;

			}

			return base.SaveChanges();
		}


		public DataTable GetData(string sqlquery)
		{
			try
			{
				DataTable dt = new DataTable();

				SqlConnection connection = new SqlConnection(Common.DbConnectionString);

				SqlDataAdapter oraAdapter = new SqlDataAdapter(sqlquery, connection);

				SqlCommandBuilder oraBuilder = new SqlCommandBuilder(oraAdapter);

				oraAdapter.Fill(dt);

				return dt;
			}
			catch
			{
				return null;
			}
		}

		public string ExecuteNonQuery(string query)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(Common.DbConnectionString))
				{
					using (SqlCommand cmd = new SqlCommand(query, conn))
					{
						//cmd.CommandText = query;
						cmd.CommandType = CommandType.Text;

						conn.Open();
						cmd.ExecuteNonQuery();
						conn.Close();

						return "S|Successfully done.";
					}
				}

			}
			catch (SqlException ex)
			{
				StringBuilder errorMessages = new StringBuilder();
				for (int i = 0; i < ex.Errors.Count; i++)
				{
					errorMessages.Append("Index #......" + i.ToString() + Environment.NewLine +
										 "Message:....." + ex.Errors[i].Message + Environment.NewLine +
										 "LineNumber:.." + ex.Errors[i].LineNumber + Environment.NewLine);
				}
				//Activity_Log.SendToDB("Database Oparation", "Error: " + "StoredProcedure: " + sp, ex);
				return "E|" + errorMessages.ToString();
			}
			catch (Exception ex)
			{
				//Activity_Log.SendToDB("Database Oparation", "Error: " + "StoredProcedure: " + sp, ex);
				return "E|" + ex.Message.ToString();
			}
		}

		public string ExecuteStoredProcedure(string sp, SqlParameter[] spCol)
		{
			try
			{
				using (SqlConnection conn = new SqlConnection(Common.DbConnectionString))
				{
					using (SqlCommand cmd = new SqlCommand(sp, conn))
					{
						cmd.CommandType = CommandType.StoredProcedure;

						SqlParameter returnParameter = new SqlParameter("@response", SqlDbType.NVarChar, 100);

						returnParameter.Direction = ParameterDirection.Output;

						if (spCol != null && spCol.Length > 0)
							cmd.Parameters.AddRange(spCol);

						cmd.Parameters.Add(returnParameter);

						conn.Open();
						cmd.ExecuteNonQuery();
						conn.Close();

						return returnParameter.Value.ToString();
					}
				}

			}
			catch (SqlException ex)
			{
				StringBuilder errorMessages = new StringBuilder();
				for (int i = 0; i < ex.Errors.Count; i++)
				{
					errorMessages.Append("Index #......" + i.ToString() + Environment.NewLine +
										 "Message:....." + ex.Errors[i].Message + Environment.NewLine +
										 "LineNumber:.." + ex.Errors[i].LineNumber + Environment.NewLine);
				}
				//Activity_Log.SendToDB("Database Oparation", "Error: " + "StoredProcedure: " + sp, ex);
				return "E|" + errorMessages.ToString();
			}
			catch (Exception ex)
			{
				//Activity_Log.SendToDB("Database Oparation", "Error: " + "StoredProcedure: " + sp, ex);
				return "E|" + ex.Message.ToString();
			}
		}


	}
}
