
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Clinic_Management_System
{
    public partial class Lov_Master : EntitiesBase
    {
        public string Lov_Column { get; set; }
        public string Lov_Code { get; set; }
        public string Lov_Desc { get; set; }
        public Nullable<int> Display_Seq_No { get; set; }
        public string Column_For { get; set; }
    }
}