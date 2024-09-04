

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Clinic_Management_System
{
	public partial class UserRoleMapping : EntitiesBase
    {
        public override long Id { get; set; }
        public long UserId { get; set; }
        public long RoleId { get; set; }

        [NotMapped] public string RoleName { get; set; } = null;
        [NotMapped] public string UserName { get; set; } = null;
        [NotMapped] public long[] SelectedRoleId { get; set; } = null;
        [NotMapped] public long[] SeelectedUserId { get; set; } = null;
        [NotMapped] public List<SelectListItem> Users { get; set; }
        [NotMapped] public List<SelectListItem> Roles { get; set; }
        [NotMapped] public List<Menu> Menus { get; set; }

    }

}