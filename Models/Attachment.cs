using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_Management_System
{
	public partial class Attachment : EntitiesBase
	{
		public override long Id { get; set; }
		public string Name { get; set; }
		public string Extension { get; set; }
		public long Size { get; set; }
		public string Type { get; set; }
		public string Path { get; set; }
		public string Remarks { get; set; }
		[NotMapped] public string File_Base64Str { get; set; }

	}
}
