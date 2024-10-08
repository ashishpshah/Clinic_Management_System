﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_Management_System
{
	public partial class Gallery : EntitiesBase
	{
        public override long Id { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        [NotMapped]public string ImagePath { get; set; }
        public string GetImagePath() { return (ImagePath.StartsWith("/") ? "Content/images/Gallery/" : "/Content/images/Gallery/") + ImagePath; }
        public int DisplayOrder { get; set; }
    }
}
