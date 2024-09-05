using System.ComponentModel.DataAnnotations.Schema;

namespace Clinic_Management_System
{
	public partial class Service : EntitiesBase
	{
        public override long Id { get; set; }
        public string Name { get; set; }
        public string Heading1 { get; set; }
        public string Heading2 { get; set; }
        public string SubHeading1 { get; set; }
        public string SubHeading2 { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string ImagePath { get; set; }
        public long AttachmentId_Primary { get; set; }
        public long AttachmentId_Secondry { get; set; }
        public int DisplayOrder { get; set; }
        public string GetImagePath() { return (ImagePath.StartsWith("/") ? "Content/images/services/" : "/Content/images/services/") + ImagePath; }
    }
}
