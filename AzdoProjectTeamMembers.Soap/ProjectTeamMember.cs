using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzdoProjectTeamMembers.Soap
{
    public class ProjectTeamMember
    {
        public int ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string ProjectDescription { get; set; }

        public Guid TeamId { get; set; }

        public string TeamName { get; set; }

        public Guid MemberId { get; set; }

        public string MemberName { get; set; }

        public bool IsTeamAdmin { get; set; }
    }
}
