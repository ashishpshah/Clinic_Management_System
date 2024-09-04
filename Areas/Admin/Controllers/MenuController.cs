using Clinic_Management_System.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages.Html;

namespace Clinic_Management_System.Areas.Admin.Controllers
{
	[RouteArea("Admin")]
	public class MenuController : BaseController<ResponseModel<Menu>>
	{
		// GET: Admin/Menu
		public ActionResult Index()
		{
			var list = new List<Menu>();

			if (Common.IsSuperAdmin())
			{
				list = _context.Menus.AsNoTracking().ToList();

				if (Common.IsSuperAdmin())
					CommonViewModel.ObjList = list;

				foreach (Menu obj in list)
					obj.ParentMenuName = list.Where(x => x.Id == obj.ParentId).Select(x => x.Name).FirstOrDefault();

			}

			CommonViewModel.ObjList = list;

			return View(CommonViewModel);
		}

		//[CustomAuthorizeAttribute(AccessType_Enum.Read)]
		public ActionResult Partial_AddEditForm(long Id = 0)
		{
			CommonViewModel.Obj = new Menu();

			if (Common.IsSuperAdmin())
			{
				var list = _context.Menus.AsNoTracking().ToList();

				if (Common.IsSuperAdmin() && Id > 0)
					CommonViewModel.Obj = list.Where(x => x.Id == Id).FirstOrDefault();

				CommonViewModel.SelectListItems = list.Where(x => x.IsActive == true && x.IsDeleted == false).ToList()
					.Select(x => new SelectListItem_Custom(Convert.ToString(x.Id), Convert.ToString(x.Name))).ToList();
			}

			return PartialView("_Partial_AddEditForm", CommonViewModel);
		}

		[HttpPost]
		//[CustomAuthorizeAttribute(AccessType_Enum.Write)]
		public ActionResult Save(Menu viewModel)
		{
			try
			{
				if (viewModel != null && viewModel != null)
				{
					#region Validation

					if (!Common.IsSuperAdmin())
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
						CommonViewModel.Message = "Please enter Menu name.";

						return Json(CommonViewModel);
					}

					if (_context.Menus.AsNoTracking().Any(x => x.Name.ToLower().Replace(" ", "") == viewModel.Name.ToLower().Replace(" ", "") && x.Id != viewModel.Id))
					{
						CommonViewModel.IsSuccess = false;
						CommonViewModel.StatusCode = ResponseStatusCode.Error;
						CommonViewModel.Message = "Menu already exist. Please try another Menu.";

						return Json(CommonViewModel);
					}

					#endregion

					#region Database-Transaction

					//using (var transaction = _context.Database.BeginTransaction())
					//{
					try
					{
						Menu obj = _context.Menus.AsNoTracking().ToList().Where(x => x.Id == viewModel.Id).FirstOrDefault();

						if (viewModel != null && !(viewModel.DisplayOrder > 0))
							viewModel.DisplayOrder = (_context.Roles.AsNoTracking().Max(x => x.DisplayOrder) ?? 0) + 1;

						if (Common.IsSuperAdmin() && obj != null)
						{
							obj.Name = viewModel.Name;
							obj.ParentId = viewModel.ParentId;
							obj.Area = viewModel.Area;
							obj.Controller = viewModel.Controller;

							obj.DisplayOrder = viewModel.DisplayOrder;
							obj.IsActive = viewModel.IsActive;

							_context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
							_context.SaveChanges();
						}
						else if (Common.IsSuperAdmin())
						{
							_context.Menus.Add(viewModel);
							_context.SaveChanges();
						}

						CommonViewModel.IsConfirm = true;
						CommonViewModel.IsSuccess = true;
						CommonViewModel.StatusCode = ResponseStatusCode.Success;
						CommonViewModel.Message = ResponseStatusMessage.Success;
						CommonViewModel.RedirectURL = Url.Action("Index", "Menu", new { area = "Admin" });

						//transaction.Commit();

						return Json(CommonViewModel);
					}
					catch (Exception ex)
					{ /*transaction.Rollback();*/ }
					//}

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
				if (!Common.IsSuperAdmin())
				{
					CommonViewModel.IsSuccess = false;
					CommonViewModel.StatusCode = ResponseStatusCode.Error;
					CommonViewModel.Message = ResponseStatusMessage.UnAuthorize;

					return Json(CommonViewModel);
				}

				if (Common.IsSuperAdmin() && !_context.UserMenuAccesses.AsNoTracking().Any(x => x.MenuId == Id)
					&& _context.Menus.AsNoTracking().Any(x => x.Id == Id))
				{
					var obj = _context.Menus.AsNoTracking().ToList().Where(x => x.Id == Id).FirstOrDefault();

					_context.Entry(obj).State = System.Data.Entity.EntityState.Deleted;
					_context.SaveChanges();

					CommonViewModel.IsConfirm = true;
					CommonViewModel.IsSuccess = true;
					CommonViewModel.StatusCode = ResponseStatusCode.Success;
					CommonViewModel.Message = ResponseStatusMessage.Delete;

					CommonViewModel.RedirectURL = Url.Action("Index", "Menu", new { area = "Admin" });

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