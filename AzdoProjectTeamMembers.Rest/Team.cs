using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzdoProjectTeamMembers.Rest
{
    public class Team
    {
        public Guid TeamId { get; set; }

        public string TeamName { get; set; }
        public List<Member> Members { get; set; }
        public string TeamMembersNames
        {
            get
            {
                return TeamName+": \n"+string.Join(";\n", Members.Select(w => w.MemberName).ToArray());
            }
            set { TeamMembersNames = value; }
        }
    }
}
