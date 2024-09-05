namespace Clinic_Management_System
{
	public partial class Gallery : EntitiesBase
	{
        public override long Id { get; set; }
        public long AttachmentId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int DisplayOrder { get; set; }
    }
}
