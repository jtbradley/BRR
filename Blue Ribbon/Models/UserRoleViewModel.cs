using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Blue_Ribbon.Models
{
    public class UserRoleViewModel
    {
        public string FirstName { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string AmazonID { get; set; }
        public List<string> SelectedRoleIds { get; set; }
        public IEnumerable<SelectListItem> RoleChoices { get; set; }
    }
}
