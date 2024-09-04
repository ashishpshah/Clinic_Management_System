using Clinic_Management_System.Controllers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Linq;

namespace Clinic_Management_System.Areas.Admin.Controllers
{
	[RouteArea("Admin")]
	public class UserController : BaseController<ResponseModel<User>>
	{
		// GET: Admin/User
		//[CustomAuthorizeAttribute(AccessType_Enum.Read)]
		public ActionResult Index()
		{
			CommonViewModel.ObjList = new List<User>();

			CommonViewModel.ObjList = (from x in _context.Users.AsNoTracking().ToList()
									   join y in _context.UserRoleMappings.AsNoTracking().ToList() on x.Id equals y.UserId
									   join z in _context.Roles.AsNoTracking().ToList() on y.RoleId equals z.Id
									   where y.RoleId > 1 && y.UserId != Common.LoggedUser_Id()
									   select new User() { Id = x.Id, UserName = x.UserName, User_Role_Id = z.Id, User_Role = z.Name }).ToList();

			return View(CommonViewModel);
		}

		public ActionResult Partial_AddEditForm(long Id = 0)
		{
			CommonViewModel.Obj = new User();

			if (Id > 0)
			{
				CommonViewModel.Obj = (from x in _context.Users.AsNoTracking().ToList()
									   join y in _context.UserRoleMappings.AsNoTracking().ToList() on x.Id equals y.UserId
									   join z in _context.Roles.AsNoTracking().ToList() on y.RoleId equals z.Id
									   where x.Id == Id && x.Id > 1 && z.Id > 1 && y.RoleId > 1 && y.UserId != Common.LoggedUser_Id()
									   select new User() { Id = x.Id, UserName = x.UserName, User_Role_Id = z.Id, User_Role = z.Name, IsActive = x.IsActive }).FirstOrDefault();
			}

			//if (!string.IsNullOrEmpty(CommonViewModel.Obj.Password))
			//    //CommonViewModel.Obj.Password = Common.Decrypt(CommonViewModel.Obj.Password);
			//    CommonViewModel.Obj.Password = "";

			CommonViewModel.SelectListItems = new List<SelectListItem_Custom>();

			var listRole = _context.Roles.AsNoTracking().ToList().Where(x => x.Id > 1).Select(x => new SelectListItem_Custom(x.Id.ToString(), x.Name, "R")).Distinct().ToList();

			if (listRole != null && listRole.Count() > 0) CommonViewModel.SelectListItems.AddRange(listRole);

			CommonViewModel.Obj.User_Id_Str = CommonViewModel.Obj.Id > 0 ? Common.Encrypt(CommonViewModel.Obj.Id.ToString()) : null;
			CommonViewModel.Obj.Role_Id_Str = CommonViewModel.Obj.User_Role_Id > 0 ? Common.Encrypt(CommonViewModel.Obj.User_Role_Id.ToString()) : null;

			return PartialView("_Partial_AddEditForm", CommonViewModel);
		}

		[HttpPost]
		public ActionResult Save(ResponseModel<User> viewModel)
		{
			try
			{
				if (viewModel != null && viewModel.Obj != null)
				{
					if (!string.IsNullOrEmpty(viewModel.Obj.Date_Text)) { try { viewModel.Obj.Date = DateTime.ParseExact(viewModel.Obj.Date_Text, "yyyy-MM-dd", CultureInfo.InvariantCulture); } catch { } }

					#region Validation

					if (string.IsNullOrEmpty(viewModel.Obj.UserName))
					{

						CommonViewModel.Message = "Please enter Username.";
						CommonViewModel.IsSuccess = false;
						CommonViewModel.StatusCode = ResponseStatusCode.Error;

						return Json(CommonViewModel);
					}

					if (_context.Users.AsNoTracking().Any(x => x.UserName.ToLower() == viewModel.Obj.UserName.ToLower()))
					{

						CommonViewModel.Message = "Username already exist. Please try another Username.";
						CommonViewModel.IsSuccess = false;
						CommonViewModel.StatusCode = ResponseStatusCode.Error;

						return Json(CommonViewModel);
					}

					if (string.IsNullOrEmpty(viewModel.Obj.Password) && viewModel.Obj.Id == 0)
					{

						CommonViewModel.Message = "Please enter Password.";
						CommonViewModel.IsSuccess = false;
						CommonViewModel.StatusCode = ResponseStatusCode.Error;

						return Json(CommonViewModel);
					}

					if (viewModel.Obj.User_Role_Id == 0)
					{

						CommonViewModel.Message = "Please select Role.";
						CommonViewModel.IsSuccess = false;
						CommonViewModel.StatusCode = ResponseStatusCode.Error;

						return Json(CommonViewModel);
					}

					long Decrypt_Id = !string.IsNullOrEmpty(viewModel.Obj.User_Id_Str) ? Convert.ToInt64(Common.Decrypt(viewModel.Obj.User_Id_Str)) : 0;
					long Decrypt_RoleId = !string.IsNullOrEmpty(viewModel.Obj.Role_Id_Str) ? Convert.ToInt64(Common.Decrypt(viewModel.Obj.Role_Id_Str)) : 0;

					var objAvailable = (from x in _context.Users.AsNoTracking().ToList()
										join y in _context.UserRoleMappings.AsNoTracking().ToList() on x.Id equals y.UserId
										join z in _context.Roles.AsNoTracking().ToList() on y.RoleId equals z.Id
										where x.UserName.ToLower().Trim().Replace(" ", "") == viewModel.Obj.UserName.ToLower().Trim().Replace(" ", "")
										&& x.Id != Decrypt_Id
										select new User() { Id = x.Id, UserName = x.UserName, User_Role_Id = z.Id, User_Role = z.Name }).FirstOrDefault();

					if (objAvailable != null || viewModel.Obj.User_Role_Id == 1)
					{

						CommonViewModel.Message = "Username already exist. Please try another Username.";
						CommonViewModel.IsSuccess = false;
						CommonViewModel.StatusCode = ResponseStatusCode.Error;

						return Json(CommonViewModel);
					}

					#endregion


					#region Database-Transaction

					using (var transaction = _context.Database.BeginTransaction())
					{
						try
						{
							//User obj = _context.Users.Where(x => x.UserName.ToLower().Replace(" ", "") == viewModel.Obj.UserName.ToLower().Replace(" ", "")).FirstOrDefault();
							User obj = _context.Users.AsNoTracking().ToList().Where(x => x.Id == Decrypt_Id).FirstOrDefault();

							if (obj != null && Common.IsAdmin())
							{
								obj.UserName = viewModel.Obj.UserName;
								//obj.Password = Common.Encrypt(viewModel.Obj.Password);
								obj.IsActive = viewModel.Obj.IsActive;

								if (viewModel.Obj.IsPassword_Reset == true)
									obj.Password = Common.Encrypt("12345");

								_context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
								_context.SaveChanges();

							}
							else if (Common.IsAdmin())
							{
								viewModel.Obj.Password = Common.Encrypt(viewModel.Obj.Password);

								_context.Users.Add(viewModel.Obj);
								_context.SaveChanges();
								_context.Entry(viewModel.Obj).Reload();

							}


							var role = _context.Roles.AsNoTracking().ToList().Where(x => x.Id == viewModel.Obj.User_Role_Id).FirstOrDefault();

							if (role != null && (Decrypt_RoleId != viewModel.Obj.User_Role_Id))
							{
								try
								{
									UserRoleMapping UserRole = _context.UserRoleMappings.AsNoTracking().ToList().Where(x => x.UserId == Decrypt_Id && x.RoleId == Decrypt_RoleId).FirstOrDefault();

									if (UserRole != null)
									{
										UserRole.RoleId = viewModel.Obj.User_Role_Id;

										_context.Entry(UserRole).State = System.Data.Entity.EntityState.Modified;
										_context.SaveChanges();
									}
									else
									{
										_context.UserRoleMappings.Add(new UserRoleMapping() { UserId = viewModel.Obj.Id, RoleId = viewModel.Obj.User_Role_Id });
										_context.SaveChanges();
									}

									var listUserMenuAccess = _context.UserMenuAccesses.AsNoTracking().ToList().Where(x => x.UserId == Decrypt_Id && x.RoleId == Decrypt_RoleId).ToList();

									if (listUserMenuAccess != null && listUserMenuAccess.Count() > 0)
									{
										foreach (var access in listUserMenuAccess)
										{
											_context.Entry(access).State = System.Data.Entity.EntityState.Deleted;
											_context.SaveChanges();
										}
									}

									foreach (var item in _context.RoleMenuAccesses.AsNoTracking().ToList().Where(x => x.RoleId == viewModel.Obj.User_Role_Id).ToList())
									{
										var userMenuAccess = new UserMenuAccess()
										{
											MenuId = item.MenuId,
											UserId = viewModel.Obj.Id,
											RoleId = viewModel.Obj.User_Role_Id,
											IsCreate = item.IsCreate,
											IsUpdate = item.IsUpdate,
											IsRead = item.IsRead,
											IsDelete = item.IsDelete,
											IsActive = item.IsActive,
											IsDeleted = item.IsDelete,
											IsSetDefault = true
										};

										_context.UserMenuAccesses.Add(userMenuAccess);
										_context.SaveChanges();
									}

								}
								catch (Exception ex) { }
							}



							CommonViewModel.IsConfirm = true;
							CommonViewModel.IsSuccess = true;
							CommonViewModel.StatusCode = ResponseStatusCode.Success;
							CommonViewModel.Message = "Record saved successfully ! ";
							CommonViewModel.RedirectURL = Url.Action("Index", "User", new { area = "Admin" });

							transaction.Commit();

							return Json(CommonViewModel);
						}
						catch (Exception ex)
						{ transaction.Rollback(); }
					}

					#endregion
				}
			}
			catch (Exception ex) { }


			CommonViewModel.Message = ResponseStatusMessage.Error;
			CommonViewModel.IsSuccess = false;
			CommonViewModel.StatusCode = ResponseStatusCode.Error;

			return Json(CommonViewModel);
		}

		[HttpPost]
		//[CustomAuthorizeAttribute(AccessType_Enum.Delete)]
		public ActionResult DeleteConfirmed(long Id)
		{
			try
			{
				if (_context.Users.AsNoTracking().ToList().Any(x => x.Id == Id))
				{
					var UserRole = _context.UserRoleMappings.AsNoTracking().ToList().Where(x => x.UserId == Id).ToList();

					if (UserRole != null)
						foreach (var obj in UserRole)
						{
							_context.Entry(obj).State = System.Data.Entity.EntityState.Deleted;
							_context.SaveChanges();
						}

					var UserMenu = _context.UserMenuAccesses.AsNoTracking().ToList().Where(x => x.UserId == Id).ToList();

					if (UserMenu != null)
						foreach (var obj in UserMenu)
						{
							_context.Entry(obj).State = System.Data.Entity.EntityState.Deleted;
							_context.SaveChanges();
						}

					var user = _context.Users.AsNoTracking().ToList().Where(x => x.Id == Id).FirstOrDefault();

					if (user != null)
					{
						_context.Entry(user).State = System.Data.Entity.EntityState.Deleted;
						_context.SaveChanges();
					}



					CommonViewModel.IsConfirm = true;
					CommonViewModel.IsSuccess = true;
					CommonViewModel.StatusCode = ResponseStatusCode.Success;
					CommonViewModel.Message = "Data deleted successfully ! ";
					CommonViewModel.RedirectURL = Url.Action("Index", "User", new { area = "Admin" });

					return Json(CommonViewModel);
				}

			}
			catch (Exception ex)
			{ }


			CommonViewModel.Message = "Unable to delete User.";
			CommonViewModel.IsSuccess = false;
			CommonViewModel.StatusCode = ResponseStatusCode.Error;

			return Json(CommonViewModel);
		}

	}

}