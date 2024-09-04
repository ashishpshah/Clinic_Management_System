namespace Clinic_Management_System
{
	public static class SessionKey
	{
		public static string USER_AUTH { get; set; } = "USER_AUTH";
		public static string USER_ID { get; set; } = "USER_ID";
		public static string USER_NAME { get; set; } = "USER_NAME";
		public static string ROLE_ID { get; set; } = "ROLE_ID";
		public static string ROLE_NAME { get; set; } = "ROLE_NAME";
		public static string ROLE_ADMIN { get; set; } = "ROLE_ADMIN";
		public static string LOGGED_USER_MENU_ACCESS { get; set; } = "LOGGED_USER_MENU_ACCESS";
		public static string IS_SUPER_USER_KEY { get; set; } = "IS_SUPER_USER_KEY";
		public static string CURRENT_MENU_ID { get; set; } = "CURRENT_MENU_ID";
	}

	public static class ResponseStatusCode
	{
		public static int Success { get; set; } = 1;
		public static int Error { get; set; } = 0;
		public static int NotFound { get; set; } = 2;
		public static int Exist { get; set; } = 3;
		public static int Failed { get; set; } = 4;
	}

	public static class Doc_Type
	{
		public static string GST { get { return "GST"; } }
		public static string PanCard { get { return "PANCARD"; } }
		public static string Company_Doc { get { return "COMPANY_DOC"; } }
		public static string Company_Logo { get { return "COMPANY_LOGO"; } }
		public static string Profile_Pic { get { return "PROFILE_PIC"; } }

		public static string DocmentType(string type)
		{
			string f = type.ToUpper();

			if (f.Equals(GST))
				return "GST";
			else if (f.Equals(PanCard))
				return "Pan Card";
			else if (f.Equals(Company_Doc))
				return "Company Docment";
			else if (f.Equals(Company_Logo))
				return "Company Logo";
			else if (f.Equals(Profile_Pic))
				return "Profile Picture";
			else
				return "";
		}

		public static string ContentType(string FileName)
		{
			string f = FileName.ToLower();

			if (f.IndexOf(".pdf") != -1)

				return "pdf/application";

			else if (f.IndexOf(".csv") != -1)

				return "text/comma-separated-values";

			else if (f.IndexOf(".txt") != -1)

				return "text/plain";

			else if (f.IndexOf(".jpg") != -1)

				return "image/jpeg";

			else if (f.IndexOf(".png") != -1)

				return "image/png";

			else

				return "";
		}
	}


	public static class ResponseStatusMessage
	{
		public static string Success { get { return "Record saved successfully !..."; } }
		public static string Delete { get { return "Record deleted successfully !..."; } }
		public static string Unable_Delete { get { return "Unable to delete record(s)."; } }
		public static string Error { get { return "Opps!... Something went wrong."; } }
		public static string Exist { get { return "Record allready available."; } }
		public static string NotFound { get { return "No any record found."; } }
		public static string UnAuthorize { get { return "You are not authorized to perform this action."; } }
	}


}