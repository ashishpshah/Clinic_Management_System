using Clinic_Management_System.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Clinic_Management_System.Areas.Admin.Controllers
{
	[RouteArea("Admin")]
	public class HomeController : BaseController<ResponseModel<LoginViewModel>>
	{
		public ActionResult Index()
		{
			if (Common.LoggedUser_Id() == 0)
				return RedirectToAction("Login", "Home", new { Area = "Admin" });

			return View();
		}

		public ActionResult Login()
		{
			Common.Clear_Session();

			return View(new ResponseModel<LoginViewModel>());
		}


		[HttpPost]
		//[ValidateAntiForgeryToken]
		public JsonResult Login(LoginViewModel viewModel)
		{
			try
			{
				if (!string.IsNullOrEmpty(viewModel.UserName) && viewModel.UserName.Length > 0 && _context.Users.AsNoTracking().Any(x => x.UserName == viewModel.UserName))
				{
					viewModel.Password = Common.Encrypt(viewModel.Password);

					var obj = _context.Users.AsNoTracking().ToList().Where(x => x.UserName == viewModel.UserName && x.Password == viewModel.Password).FirstOrDefault();

					if (obj != null && obj.IsActive == true && obj.IsDeleted == false)
					{
						var userRole = _context.UserRoleMappings.AsNoTracking().ToList().Where(x => x.UserId == obj.Id).FirstOrDefault();

						obj.RoleId = userRole != null ? userRole.RoleId : 0;

						List<UserMenuAccess> listMenuAccess = new List<UserMenuAccess>();
						List<UserMenuAccess> listMenuPermission = new List<UserMenuAccess>();

						Role role = _context.Roles.AsNoTracking().ToList().Where(x => x.Id == obj.RoleId).FirstOrDefault();

						if (role != null && role.Id == 1)
						{
							listMenuAccess = (from y in _context.Menus.AsNoTracking().ToList()
											  where y.IsActive == true && y.IsDeleted == false
											  select new UserMenuAccess() { Id = y.Id, ParentMenuId = y.ParentId, Area = y.Area, Controller = y.Controller, MenuName = y.Name, IsCreate = true, IsUpdate = true, IsRead = true, IsDelete = true, DisplayOrder = y.DisplayOrder, IsActive = y.IsActive, IsDeleted = y.IsDeleted }).ToList();
						}
						else if (role != null && role.IsAdmin)
						{

							listMenuAccess = (from x in _context.UserMenuAccesses.AsNoTracking().ToList()
											  join y in _context.Menus.AsNoTracking().ToList() on x.MenuId equals y.Id
											  where x.UserId == obj.Id && x.RoleId == obj.RoleId
											  && y.IsActive == true && y.IsDeleted == false && x.IsActive == true && x.IsDeleted == false && y.Name != "Menu"
											  && x.IsRead == true
											  select new UserMenuAccess() { Id = y.Id, ParentMenuId = y.ParentId, Area = y.Area, Controller = y.Controller, MenuName = y.Name, DisplayOrder = y.DisplayOrder, IsActive = x.IsActive, IsDeleted = x.IsDeleted }).ToList();
						}
						else if (role != null && !role.IsAdmin && role.IsActive && !role.IsDeleted)
						{
							listMenuAccess = (from x in _context.UserMenuAccesses.AsNoTracking().ToList()
											  join y in _context.Menus.AsNoTracking().ToList() on x.MenuId equals y.Id
											  where x.UserId == obj.Id && x.RoleId == obj.RoleId
											  && y.IsActive == true && y.IsDeleted == false && x.IsActive == true && x.IsDeleted == false && y.Id != 1 && y.ParentId != 1 && y.Name != "Menu"
											  && x.IsRead == true
											  select new UserMenuAccess() { Id = y.Id, ParentMenuId = y.ParentId, Area = y.Area, Controller = y.Controller, MenuName = y.Name, DisplayOrder = y.DisplayOrder, IsActive = x.IsActive, IsDeleted = x.IsDeleted }).ToList();
						}

						if (role != null && role.Id == 1)
							listMenuPermission = listMenuAccess;
						else
							listMenuPermission = (from x in _context.UserMenuAccesses.AsNoTracking().ToList()
												  join y in _context.Menus.AsNoTracking().ToList() on x.MenuId equals y.Id
												  where x.UserId == obj.Id && y.IsActive == true && y.IsDeleted == false && x.IsActive == true && x.IsDeleted == false
												  && listMenuAccess.Any(z => z.Id == y.Id)
												  select new UserMenuAccess() { MenuId = y.Id, ParentMenuId = y.ParentId, Area = y.Area, Controller = y.Controller, MenuName = y.Name, IsCreate = x.IsCreate, IsUpdate = x.IsUpdate, IsRead = x.IsRead, IsDelete = x.IsDelete, IsActive = x.IsActive, IsDeleted = x.IsDeleted }).ToList();

						Common.Configure_UserMenuAccess(listMenuAccess.Where(x => x.IsActive == true && x.IsDeleted == false).ToList(), listMenuPermission.Where(x => x.IsActive == true && x.IsDeleted == false).ToList());

						Common.Set_Session_Int(SessionKey.USER_ID, obj.Id);
						Common.Set_Session_Int(SessionKey.ROLE_ID, obj.RoleId);

						Common.Set_Session(SessionKey.USER_NAME, obj.UserName);
						Common.Set_Session(SessionKey.ROLE_NAME, role.Name);
						Common.Set_Session_Int(SessionKey.ROLE_ADMIN, (role.IsAdmin ? 1 : 0));
						Common.Set_Session_Int(SessionKey.IS_SUPER_USER_KEY, (obj.RoleId == 1 ? 1 : 0));


						CommonViewModel.IsSuccess = true;
						CommonViewModel.StatusCode = ResponseStatusCode.Success;
						CommonViewModel.Message = ResponseStatusMessage.Success;

						CommonViewModel.RedirectURL = Url.Content("~/") + "Admin/" + this.ControllerContext.RouteData.Values["Controller"].ToString() + "/Index";

						return Json(CommonViewModel);
					}

					//DataTable dt = _context.GetSQLQuery("SELECT USER_ID, 'C' USER_TYPE, '' IS_ADMIN FROM APR_REGISTRATION_MASTER where USER_ID = '" + viewModel.UserId + "' " +
					//	"UNION ALL SELECT TO_CHAR(USER_ID)USER_ID, 'I' USER_TYPE, IS_ADMIN FROM APR_EMP_ACCESS where TO_CHAR(USER_ID) = '" + viewModel.UserId + "' ");

					//if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
					//{
					//	var user_type = dt.Rows[0]["USER_TYPE"] != DBNull.Value ? Convert.ToString(dt.Rows[0]["USER_TYPE"]) : null;
					//	var is_admin = dt.Rows[0]["IS_ADMIN"] != DBNull.Value ? Convert.ToString(dt.Rows[0]["IS_ADMIN"]) : null;

					//	dt = _context.GetSQLQuery("select HRMSADM.FC_CHECK_PASSWORD('" + viewModel.UserId + "', '" + viewModel.Password + "') ISVALID from dual");

					//	if (dt != null && dt.Rows != null && dt.Rows.Count > 0 && !string.IsNullOrEmpty(user_type))
					//	{
					//		string check = dt.Rows[0]["ISVALID"] != DBNull.Value ? Convert.ToString(dt.Rows[0]["ISVALID"]) : null;

					//		if (!string.IsNullOrEmpty(check) && check.Equals("1"))
					//		{
					//			AppHttpContextAccessor.SetSession(SessionKey.USER_ID, viewModel.UserId);
					//			AppHttpContextAccessor.SetSession(SessionKey.USER_TYPE, user_type ?? "");
					//			AppHttpContextAccessor.SetSession(SessionKey.USER_IS_ADMIN, is_admin ?? "");

					//			CommonViewModel.IsSuccess = true;
					//			CommonViewModel.StatusCode = ResponseStatusCode.Success;
					//			CommonViewModel.Message = ResponseStatusMessage.Success;

					//			if (user_type.Equals("I"))
					//			{
					//				CommonViewModel.RedirectURL = Url.Content("~/") + "Admin/" + this.ControllerContext.RouteData.Values["Controller"].ToString() + "/Index";
					//			}
					//			else
					//			{
					//				CommonViewModel.RedirectURL = Url.Content("~/") + this.ControllerContext.RouteData.Values["Controller"].ToString() + "/Profile";
					//			}

					//			return Json(CommonViewModel);
					//		}
					//	}
					//}
				}

				CommonViewModel.IsSuccess = false;
				CommonViewModel.StatusCode = ResponseStatusCode.Error;
				CommonViewModel.Message = "User Id and Password does not Match";

			}
			catch (Exception ex)
			{
				CommonViewModel.IsSuccess = false;
				CommonViewModel.StatusCode = ResponseStatusCode.Error;
				CommonViewModel.Message = ResponseStatusMessage.Error + " | " + ex.Message;
			}

			return Json(CommonViewModel);
		}

		public ActionResult Logout()
		{
			Common.Clear_Session();

			return RedirectToAction("Login", "Home", new { Area = "Admin" });
		}

	}
}