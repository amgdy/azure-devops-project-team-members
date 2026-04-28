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

        public string TeamNames {
            get
            {
                return string.Join(";\n", Teams.Select(w => w.TeamName).ToArray());
            }
            set { TeamNames = value; }
        }

        public string TeamMembers
        {
            get
            {
                return string.Join("\n", Teams.Select(w => w.TeamMembersNames).ToArray());
            }
            set { TeamMembers = value; }
        }
        public List<Team> Teams { get; set; }
    }
}
