using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzdoProjectTeamMembers.Rest
{
    public class ProjectTeamMember
    {
        public Guid ProjectId { get; set; }

        public string ProjectName { get; set; }

        public string ProjectDescription { get; set; }

        public Guid TeamId { get; set; }

        public string TeamName { get; set; }

        public string MemberId { get; set; }

        public string MemberName { get; set; }

        public bool IsTeamAdmin { get; set; }
    }
}
