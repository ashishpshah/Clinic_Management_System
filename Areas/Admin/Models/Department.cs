namespace Clinic_Management_System
{
	public partial class Department : EntitiesBase
	{
        public override long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
