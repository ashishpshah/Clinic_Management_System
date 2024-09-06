using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_Management_System
{
	public partial class Attachment : EntitiesBase
	{
		public override long Id { get; set; }
		public long GalleryId { get; set; }
		public string Name { get; set; }
		public string Extension { get; set; }
		public long Size { get; set; }
		public string Type { get; set; }
		public string Path { get; set; }
		public string Remarks { get; set; }
        public string GetImagePath() { return (Path.StartsWith("/") ? "Content/images/Gallery/" : "~/Content/images/Gallery/") + Name+"."+ Extension; }
        [NotMapped] public string File_Base64Str { get; set; }

	}
}
