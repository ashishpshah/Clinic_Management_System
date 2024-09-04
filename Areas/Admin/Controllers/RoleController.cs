using Clinic_Management_System.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Clinic_Management_System.Areas.Admin.Controllers
{
	[RouteArea("Admin")]
	public class RoleController : BaseController<ResponseModel<Role>>
	{
		// GET: Admin/Role
		public ActionResult Index()
		{
			if (Common.IsSuperAdmin())
				CommonViewModel.ObjList = (from x in _context.Roles.AsNoTracking().ToList()
										   where x.Id > 1
										   select new Role { Id = x.Id, Name = x.Name, IsActive = x.IsActive, IsAdmin = x.IsAdmin }).Distinct().ToList();

			else if (!Common.IsSuperAdmin() && Common.IsAdmin())
				CommonViewModel.ObjList = (from x in _context.Roles.AsNoTracking().ToList()
										   where x.Id > 1 
										   select new Role { Id = x.Id, Name = x.Name, IsActive = x.IsActive, IsAdmin = x.IsAdmin }).Distinct().ToList();

			return View(CommonViewModel);
		}

		//[CustomAuthorizeAttribute(AccessType_Enum.Read)]
		public ActionResult Partial_AddEditForm(long Id = 0)
		{
			CommonViewModel.Obj = new Role();

			if (Common.IsAdmin() && Id > 1)
				CommonViewModel.Obj = _context.Roles.AsNoTracking().ToList().Where(x => x.Id > 1 && x.Id == Id).FirstOrDefault();


			var listMenu = _context.Menus.AsNoTracking().ToList();

			foreach (var item in listMenu.Where(x => x.ParentId > 0).ToList())
				item.ParentMenuName = listMenu.Where(x => x.Id == item.ParentId).Select(x => x.Name).FirstOrDefault();

			CommonViewModel.SelectListItems = new List<SelectListItem_Custom>();

			var menus = new List<SelectListItem_Custom>();

			if (Common.IsSuperAdmin())
			{
				menus = (from x in listMenu.ToList()
						 where !x.Name.ToLower().Contains("menu") //x.IsSuperAdmin == Common.IsSuperAdmin() && x.IsAdmin == Common.IsAdmin()
						 orderby x.ParentId, x.DisplayOrder
						 select new SelectListItem_Custom(Convert.ToString(x.Id + "_" + x.ParentId), Convert.ToString(x.Name) + (x.ParentId > 0 ? " (" + Convert.ToString(x.ParentMenuName) + " )" : ""), "M")).ToList();
			}
			else
			{
				menus = (from x in listMenu.ToList()
						 where x.IsSuperAdmin == false && !x.Name.ToLower().Contains("menu")
						 orderby x.ParentId, x.DisplayOrder
						 select new SelectListItem_Custom(Convert.ToString(x.Id + "_" + x.ParentId), Convert.ToString(x.Name) + (x.ParentId > 0 ? " (" + Convert.ToString(x.ParentMenuName) + " )" : ""), "M")).ToList();
			}

			if (menus != null && menus.Count > 0) CommonViewModel.SelectListItems.AddRange(menus);

			var list = _context.RoleMenuAccesses.AsNoTracking().ToList().Where(x => x.RoleId == CommonViewModel.Obj.Id).ToList();

			if (list != null && list.Count() > 0)
			{
				string[] selected = (from x in list
									 join y in listMenu on x.MenuId equals y.Id
									 where !y.Name.ToLower().Contains("menu") && !y.Name.ToLower().Contains("menu")
									 select Convert.ToString(x.MenuId + "_" + y.ParentId)).ToArray();

				if (selected != null && selected.Length > 0)
					CommonViewModel.Obj.CreatedDate_Text = string.Join(",", selected) + ",";
			}

			if (CommonViewModel.SelectListItems == null) CommonViewModel.SelectListItems = new List<SelectListItem_Custom>();

			return PartialView("_Partial_AddEditForm", CommonViewModel);
		}

		[HttpPost]
		//[CustomAuthorizeAttribute(AccessType_Enum.Write)]
		public ActionResult Save(Role viewModel)
		{
			try
			{
				viewModel = viewModel != null && !Common.IsSuperAdmin() && viewModel.Id == Common.LoggedUser_RoleId() ? null : viewModel;

				if (viewModel != null)
				{
					#region Validation

					if (!Common.IsAdmin())
					{
						CommonViewModel.IsSuccess = false;
						CommonViewModel.StatusCode = ResponseStatusCode.Error;
						CommonViewModel.Message = ResponseStatusMessage.UnAuthorize;

						return Json(CommonViewModel);
					}

					if (string.IsNullOrEmpty(viewModel.Name))
					{
						CommonViewModel.IsSuccess = false;
						CommonViewModel.StatusCode = ResponseStatusCode.Error;
						CommonViewModel.Message = "Please enter Role name.";

						return Json(CommonViewModel);
					}

					if (_context.Roles.AsNoTracking().Any(x => x.Name.ToLower().Replace(" ", "") == viewModel.Name.ToLower().Replace(" ", "") && x.Id != viewModel.Id) || viewModel.Id == 1)
					{
						CommonViewModel.IsSuccess = false;
						CommonViewModel.StatusCode = ResponseStatusCode.Error;
						CommonViewModel.Message = "Role already exist. Please try another Role.";

						return Json(CommonViewModel);
					}

					#endregion

					#region Database-Transaction

					using (var transaction = _context.Database.BeginTransaction())
					{
						try
						{
							Role obj = _context.Roles.AsNoTracking().ToList().Where(x => x.Id > 1 && x.Id == viewModel.Id).FirstOrDefault();

							if (viewModel != null && !(viewModel.DisplayOrder > 0))
								viewModel.DisplayOrder = (_context.Roles.AsNoTracking().Max(x => x.DisplayOrder) ?? 0) + 1;

							if (Common.IsAdmin() && obj != null)
							{
								obj.Name = viewModel.Name;
								obj.DisplayOrder = viewModel.DisplayOrder;
								obj.IsAdmin = Common.IsSuperAdmin() ? viewModel.IsAdmin : false;
								obj.IsActive = viewModel.IsActive;

								_context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
								_context.SaveChanges();
							}
							else if (Common.IsAdmin())
							{
								viewModel.IsAdmin = Common.IsSuperAdmin() ? viewModel.IsAdmin : false;

								_context.Roles.Add(viewModel);
								_context.SaveChanges();
								_context.Entry(viewModel).Reload();
							}

							try
							{
								var listRoleMenuAccesses = _context.RoleMenuAccesses.AsNoTracking().ToList().Where(x => x.RoleId == viewModel.Id).ToList();

								if (listRoleMenuAccesses != null && listRoleMenuAccesses.Count() > 0)
								{
									foreach (var access in listRoleMenuAccesses)
									{
										_context.Entry(access).State = System.Data.Entity.EntityState.Deleted;
										_context.SaveChanges();
									}
								}

								if (!string.IsNullOrEmpty(viewModel.CreatedDate_Text))
								{
									var list = viewModel.CreatedDate_Text.Split(',');

									foreach (var item in list.Where(x => !string.IsNullOrEmpty(x)))
									{
										try
										{
											var roleMenuAccess = new RoleMenuAccess()
											{
												MenuId = Convert.ToInt64(item.Split('_')[0]),
												RoleId = viewModel.Id,
												IsCreate = true,
												IsUpdate = true,
												IsRead = true,
												IsDelete = true,
												IsActive = true,
												IsDeleted = false,
												IsSetDefault = true
											};

											_context.RoleMenuAccesses.Add(roleMenuAccess);
											_context.SaveChanges();
										}
										catch (Exception) { continue; }
									}


								}

							}
							catch (Exception) { }

							CommonViewModel.IsConfirm = true;
							CommonViewModel.IsSuccess = true;
							CommonViewModel.StatusCode = ResponseStatusCode.Success;
							CommonViewModel.Message = ResponseStatusMessage.Success;
							CommonViewModel.RedirectURL = Url.Action("Index", "Role", new { area = "Admin" });

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
				if (!Common.IsSuperAdmin() || !Common.IsAdmin())
				{
					CommonViewModel.IsSuccess = false;
					CommonViewModel.StatusCode = ResponseStatusCode.Error;
					CommonViewModel.Message = ResponseStatusMessage.UnAuthorize;

					return Json(CommonViewModel);
				}

				if (Common.IsSuperAdmin() && Common.IsAdmin() && !_context.UserRoleMappings.AsNoTracking().Any(x => x.Id > 1 && x.RoleId == Id)
					&& _context.Roles.AsNoTracking().Any(x => x.Id > 1 && x.Id == Id))
				{
					var obj = _context.Roles.AsNoTracking().ToList().Where(x => x.Id == Id).FirstOrDefault();

					_context.Entry(obj).State = System.Data.Entity.EntityState.Deleted;
					_context.SaveChanges();

					CommonViewModel.IsConfirm = true;
					CommonViewModel.IsSuccess = true;
					CommonViewModel.StatusCode = ResponseStatusCode.Success;
					CommonViewModel.Message = ResponseStatusMessage.Delete;

					CommonViewModel.RedirectURL = Url.Action("Index", "Role", new { area = "Admin" });

					return Json(CommonViewModel);
				}
			}
			catch (Exception ex)
			{ }

			CommonViewModel.IsSuccess = false;
			CommonViewModel.StatusCode = ResponseStatusCode.Error;
			CommonViewModel.Message = ResponseStatusMessage.Unable_Delete;

			return Json(CommonViewModel);
		}

	}
}