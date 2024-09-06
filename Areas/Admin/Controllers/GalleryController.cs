using Clinic_Management_System.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Clinic_Management_System.Areas.Admin.Controllers
{
	[RouteArea("Admin")]
	public class GalleryController : BaseController<ResponseModel<Gallery>>
	{
		// GET: Admin/Gallery
		public ActionResult Index()
		{
			CommonViewModel.ObjList = _context.Gallery.AsNoTracking().ToList().Where(x => x.Id > 0).ToList();

			return View(CommonViewModel);
		}

		//[CustomAuthorizeAttribute(AccessType_Enum.Read)]
		public ActionResult Partial_AddEditForm(long Id = 0)
		{
			CommonViewModel.Obj = new Gallery();

			if (Common.IsAdmin() && Id > 0)
            {
                CommonViewModel.Obj = _context.Gallery.AsNoTracking().ToList().Where(x => x.Id == Id).FirstOrDefault();
                CommonViewModel.Data1 = _context.Attachments.AsNoTracking().ToList().Where(x => x.GalleryId == Id).ToList();
            }
				

			return PartialView("_Partial_AddEditForm", CommonViewModel);
		}

		[HttpPost]
		//[CustomAuthorizeAttribute(AccessType_Enum.Write)]
		public ActionResult Save(Gallery viewModel, List<HttpPostedFileBase> files)
		{
			try
			{
				if (viewModel != null && viewModel != null)
				{
                    #region Validation

                    if (!Common.IsAdmin() || !Common.IsAdmin())
					{
						CommonViewModel.IsSuccess = false;
						CommonViewModel.StatusCode = ResponseStatusCode.Error;
						CommonViewModel.Message = ResponseStatusMessage.UnAuthorize;

						return Json(CommonViewModel);
					}

					if (string.IsNullOrEmpty(viewModel.Title))
					{
						CommonViewModel.IsSuccess = false;
						CommonViewModel.StatusCode = ResponseStatusCode.Error;
						CommonViewModel.Message = "Please enter Gallery name.";

						return Json(CommonViewModel);
					}

					if (_context.Gallery.AsNoTracking().Any(x => x.Title.ToLower().Replace(" ", "") == viewModel.Title.ToLower().Replace(" ", "") && x.Id != viewModel.Id))
					{
						CommonViewModel.IsSuccess = false;
						CommonViewModel.StatusCode = ResponseStatusCode.Error;
						CommonViewModel.Message = "Gallery already exist. Please try another Gallery name.";

						return Json(CommonViewModel);
					}

					#endregion

					#region Database-Transaction

					using (var transaction = _context.Database.BeginTransaction())
					{
						try
						{
                            
							Gallery obj = null;
                            Attachment Attachment = new Attachment();

							if (Common.IsAdmin() && viewModel.Id > 0)
								obj = _context.Gallery.AsNoTracking().ToList().Where(x => x.Id == viewModel.Id).FirstOrDefault();                            

                            if (Common.IsAdmin() && obj != null)
							{
								obj.Title = viewModel.Title;
								obj.SubTitle = viewModel.SubTitle;
								obj.Description = viewModel.Description;
								obj.Type = viewModel.Type;
								obj.IsActive = viewModel.IsActive;

								_context.Entry(obj).State = System.Data.Entity.EntityState.Modified;
								_context.SaveChanges();
							}
							else if (Common.IsAdmin())
							{
								_context.Gallery.Add(viewModel);
								_context.SaveChanges();
								_context.Entry(viewModel).Reload();
                            }

                            if (files != null && files.Count > 0)
                            {
                                foreach (HttpPostedFileBase file in files)
                                {
                                    if (file != null)
                                    {
                                        string fileName = Path.GetFileNameWithoutExtension(DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + file.FileName); // Get file name without extension
                                        string extension = Path.GetExtension(file.FileName); // Get file extension
                                        long size = file.ContentLength; // Get file size in bytes
                                        string type = file.ContentType; // Get MIME type
                                        string path = "/images/Gallery/" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + file.FileName; // path 
                                        string fullpath = Path.Combine(Server.MapPath("~/Content/images/Gallery/"), DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + file.FileName); // Full path  

                                        try
                                        {
                                            Attachment.GalleryId = viewModel.Id;
                                            Attachment.Name = fileName;
                                            Attachment.Extension = extension;
                                            Attachment.Size = size;
                                            Attachment.Type = type;
                                            Attachment.Path = path;
                                            _context.Attachments.Add(Attachment);
                                            _context.SaveChanges();

                                            if (!Directory.Exists(Server.MapPath("~/Content/images/Gallery/")))
                                            {
                                                Directory.CreateDirectory(Server.MapPath("~/Content/images/Gallery/"));
                                            }

                                            file.SaveAs(fullpath);
                                        }
                                        catch (Exception ex) { }
                                    }
                                }
                            }


                            CommonViewModel.IsConfirm = true;
							CommonViewModel.IsSuccess = true;
							CommonViewModel.StatusCode = ResponseStatusCode.Success;
							CommonViewModel.Message = ResponseStatusMessage.Success;
							CommonViewModel.RedirectURL = Url.Action("Index", "Gallery", new { area = "Admin" });

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
				if (!Common.IsAdmin())
				{
					CommonViewModel.IsSuccess = false;
					CommonViewModel.StatusCode = ResponseStatusCode.Error;
					CommonViewModel.Message = ResponseStatusMessage.UnAuthorize;

					return Json(CommonViewModel);
				}

				if (Common.IsAdmin() && _context.Gallery.AsNoTracking().Any(x => x.Id == Id))
				{

                    var obj = _context.Gallery.AsNoTracking().ToList().Where(x => x.Id == Id).FirstOrDefault();

					_context.Entry(obj).State = System.Data.Entity.EntityState.Deleted;
					_context.SaveChanges();

                    var AttachList = _context.Attachments.AsNoTracking().ToList().Where(x => x.GalleryId == Id).ToList();

                    foreach (var item in AttachList)
                    {
                        _context.Entry(item).State = System.Data.Entity.EntityState.Deleted;
                        _context.SaveChanges();

                        if (System.IO.File.Exists(item.Path))
                        {
                            System.IO.File.Delete(item.Path);
                        }
                    }


                    CommonViewModel.IsConfirm = true;
					CommonViewModel.IsSuccess = true;
					CommonViewModel.StatusCode = ResponseStatusCode.Success;
					CommonViewModel.Message = ResponseStatusMessage.Delete;

					CommonViewModel.RedirectURL = Url.Action("Index", "Gallery", new { area = "Admin" });

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


        [HttpPost]
        //[CustomAuthorizeAttribute(AccessType_Enum.Delete)]
        public ActionResult DeleteAttchmentConfirmed(long Id)
        {
            try
            {
                if (!Common.IsAdmin())
                {
                    CommonViewModel.IsSuccess = false;
                    CommonViewModel.StatusCode = ResponseStatusCode.Error;
                    CommonViewModel.Message = ResponseStatusMessage.UnAuthorize;

                    return Json(CommonViewModel);
                }

                if (Common.IsAdmin() && _context.Attachments.AsNoTracking().Any(x => x.Id == Id))
                {
                    var obj = _context.Attachments.AsNoTracking().ToList().Where(x => x.Id == Id).FirstOrDefault();

                    _context.Entry(obj).State = System.Data.Entity.EntityState.Deleted;
                    _context.SaveChanges();

                    if (System.IO.File.Exists(obj.Path))
                    {
                        System.IO.File.Delete(obj.Path);
                    }

                    CommonViewModel.IsConfirm = true;
                    CommonViewModel.IsSuccess = true;
                    CommonViewModel.StatusCode = ResponseStatusCode.Success;
                    CommonViewModel.Message = ResponseStatusMessage.Delete;

                    CommonViewModel.RedirectURL = Url.Action("Index", "Gallery", new { area = "Admin" });

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
        private void DeleteFile(string fileName)
        {
            var filePath = Path.Combine(Server.MapPath("~/Content/images/Gallery/"), fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
        //public ActionResult GetData_Table(JqueryDatatableParam param)
        //{
        //    List<TrsUnitMst> list = new List<TrsUnitMst>();

        //    var queryCondition = "";

        //    var sqlquery = "WITH TBL_DATA AS (SELECT ZZ.*, ROWNUM RNUM FROM (SELECT Z.* FROM (SELECT (count(*) OVER())ROW_COUNT, X.UNIT_CODE, X.FAS_UNIT_CODE, X.UNIT_HEAD, X.FOR_TEXT, X.REP_SERVER_PATH, " +
        //        "X.UPLOAD_PATH, X.MIS_SERVER_PATH, X.ORGANIZATION_NAME, X.UNIT_NAME, X.LETTER_HEAD, X.BASE_UNIT_CODE, X.EMAIL_DOMAIN, X.SOFTWARE_GLOBAL_PATH, X.MOBILE_NOS_FOR_SMS, X.BOARD_INTERFACE_PRG, " +
        //        "X.BOARD_INTERFACE_FILE_PATH, X.SMS_URL_TEXT " +
        //        "FROM TRS_UNIT_MST X ) Z WHERE #queryCondition# ";

        //    //if (!string.IsNullOrEmpty(Status) && Status != "0")
        //    //    queryCondition = queryCondition + "AND X.STATUS = " + Status.ToLower() + " ";

        //    if (!string.IsNullOrEmpty(param.sSearch))
        //    {
        //        queryCondition = queryCondition + "AND (LOWER(Z.UNIT_CODE) LIKE # OR LOWER(Z.UNIT_NAME) LIKE # OR LOWER(Z.ORGANIZATION_NAME) LIKE # OR LOWER(Z.BASE_UNIT_CODE) LIKE # OR LOWER(Z.EMAIL_DOMAIN) LIKE # ) ";

        //        queryCondition = queryCondition.Replace("#", "'" + param.sSearch.ToLower() + "%'");
        //    }


        //    var sortColumnIndex = Convert.ToInt32(HttpContext.Request.Query["iSortCol_0"]);
        //    var sortDirection = HttpContext.Request.Query["sSortDir_0"];

        //    if (sortColumnIndex == 0)
        //        queryCondition = queryCondition + "ORDER BY Z.UNIT_CODE <>";
        //    else if (sortColumnIndex == 1)
        //        queryCondition = queryCondition + "ORDER BY Z.UNIT_NAME <>";
        //    else if (sortColumnIndex == 2)
        //        queryCondition = queryCondition + "ORDER BY Z.ORGANIZATION_NAME <>";
        //    else if (sortColumnIndex == 3)
        //        queryCondition = queryCondition + "ORDER BY Z.BASE_UNIT_CODE <>";
        //    else if (sortColumnIndex == 4)
        //        queryCondition = queryCondition + "ORDER BY Z.EMAIL_DOMAIN <>";
        //    else
        //        queryCondition = queryCondition + "ORDER BY Z.UNIT_CODE ";

        //    if (sortDirection != "asc")
        //        queryCondition = queryCondition.Replace("<>", "DESC ");
        //    else
        //        queryCondition = queryCondition.Replace("<>", " ASC ");


        //    sqlquery = sqlquery.Replace("#queryCondition#", queryCondition) + " ) ZZ ) " + Environment.NewLine;

        //    sqlquery = sqlquery + "SELECT * FROM TBL_DATA WHERE RNUM <= " + (param.iDisplayLength + param.iDisplayStart) + " AND RNUM > " + param.iDisplayStart;

        //    sqlquery = sqlquery.Replace("WHERE AND", "WHERE ").Replace("WHERE WHERE", "WHERE ").Replace("WHERE OR ", "WHERE ").Replace("WHERE ORDER", "ORDER ");

        //    DataTable dt = _context.GetSQLQuery(sqlquery);

        //    if (dt != null && dt.Rows.Count > 0)
        //    {
        //        foreach (DataRow dr in dt.Rows)
        //        {
        //            list.Add(new TrsUnitMst()
        //            {
        //                UnitCode = dr["UNIT_CODE"] != DBNull.Value ? Convert.ToInt32(Convert.ToString(dr["UNIT_CODE"])) : 0,
        //                //FasUnitCode = dr["FAS_UNIT_CODE"] != DBNull.Value ? Convert.ToString(dr["FAS_UNIT_CODE"]) : "",
        //                //UnitHead = dr["UNIT_HEAD"] != DBNull.Value ? Convert.ToString(dr["UNIT_HEAD"]) : "",
        //                //ForText = dr["FOR_TEXT"] != DBNull.Value ? Convert.ToString(dr["FOR_TEXT"]) : "",
        //                //RepServerPath = dr["REP_SERVER_PATH"] != DBNull.Value ? Convert.ToString(dr["REP_SERVER_PATH"]) : "",
        //                //UploadPath = dr["UPLOAD_PATH"] != DBNull.Value ? Convert.ToString(dr["UPLOAD_PATH"]) : "",
        //                //MisServerPath = dr["MIS_SERVER_PATH"] != DBNull.Value ? Convert.ToString(dr["MIS_SERVER_PATH"]) : "",
        //                OrganizationName = dr["ORGANIZATION_NAME"] != DBNull.Value ? Convert.ToString(dr["ORGANIZATION_NAME"]) : "",
        //                UnitName = dr["UNIT_NAME"] != DBNull.Value ? Convert.ToString(dr["UNIT_NAME"]) : "",
        //                BaseUnitCode = dr["BASE_UNIT_CODE"] != DBNull.Value ? Convert.ToString(dr["BASE_UNIT_CODE"]) : "",
        //                EmailDomain = dr["EMAIL_DOMAIN"] != DBNull.Value ? Convert.ToString(dr["EMAIL_DOMAIN"]) : "",
        //                //SoftwareGlobalPath = dr["SOFTWARE_GLOBAL_PATH"] != DBNull.Value ? Convert.ToString(dr["SOFTWARE_GLOBAL_PATH"]) : "",
        //                //MobileNosForSms = dr["MOBILE_NOS_FOR_SMS"] != DBNull.Value ? Convert.ToString(dr["MOBILE_NOS_FOR_SMS"]) : "",
        //                //BoardInterfacePrg = dr["BOARD_INTERFACE_PRG"] != DBNull.Value ? Convert.ToString(dr["BOARD_INTERFACE_PRG"]) : "",
        //                //BoardInterfaceFilePath = dr["BOARD_INTERFACE_FILE_PATH"] != DBNull.Value ? Convert.ToString(dr["BOARD_INTERFACE_FILE_PATH"]) : "",
        //                //SmsUrlText = dr["SMS_URL_TEXT"] != DBNull.Value ? Convert.ToString(dr["SMS_URL_TEXT"]) : ""
        //            });
        //        }

        //    }

        //    return Json(new
        //    {
        //        param.sEcho,
        //        iTotalRecords = list.Count(),
        //        iTotalDisplayRecords = dt != null && dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[0]["ROW_COUNT"]?.ToString()) : 0,
        //        aaData = list
        //    }, new JsonSerializerSettings() { DateFormatString = "dd/MM/yyyy" });

        //}
    }
}