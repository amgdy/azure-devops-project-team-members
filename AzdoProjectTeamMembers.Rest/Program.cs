using CsvHelper;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AzdoProjectTeamMembers.Rest
{
     internal class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Please enter the following details to list all of the team members in all of the projects in the collection/instance");
            Console.WriteLine();
            Console.Write("Azure DevOps Instance/Collection URL: ");
            var url = Console.ReadLine();

            Console.Write("Personal Access Token (PAT): ");
            var pat = Console.ReadLine();

            VssBasicCredential vssBasicCredential = new VssBasicCredential(pat, pat);

            using (VssConnection connection = new VssConnection(new Uri(url), vssBasicCredential))
            {
                var projectClient = connection.GetClient<ProjectHttpClient>();
                var teamClient = connection.GetClient<TeamHttpClient>();

                var projects = await projectClient.GetProjects();
                var data = new List<ProjectTeamMember>();

                foreach (var project in projects)
                {
                    Console.WriteLine(project.Name);

                    var projectTeamMember = new ProjectTeamMember();

                    projectTeamMember.ProjectId = project.Id;
                    projectTeamMember.ProjectName = project.Name;
                    projectTeamMember.ProjectDescription = project.Description;
                    
                    var teams = await teamClient.GetTeamsAsync(project.Id.ToString());
                    List<Team> tmmbrs = new List<Team>();
                    foreach (var team in teams)
                    {

                        //Console.WriteLine($"\t {team.Name}");

                        //projectTeamMember.TeamId = team.Id;
                        //projectTeamMember.TeamName = team.Name;

                        var members = await teamClient.GetTeamMembersWithExtendedPropertiesAsync(project.Id.ToString(), team.Id.ToString());
                        List<Member> mmbers = new List<Member>();

                        foreach (var member in members)
                        {
                            Console.WriteLine($"\t\t {member.Identity.DisplayName} - {member.IsTeamAdmin}");
                            mmbers.Add(new Member { MemberId  = member.Identity.Id , MemberName = member.Identity.DisplayName, IsTeamAdmin = member.IsTeamAdmin});
                        }
                        tmmbrs.Add(new Team { TeamId = team.Id, TeamName = team.Name, Members = mmbers });
                        projectTeamMember.Teams = tmmbrs;
                    }
                    data.Add(projectTeamMember);
                }

                using (var writer = new StreamWriter($"data.{DateTime.Now.ToString("yyyyMMddhhmmss")}.csv"))
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteRecords(data);
                }
            }
            Console.WriteLine("Exported! In Folder:\n"+ System.Environment.CurrentDirectory);
            Console.ReadLine();
        }
    }
}
