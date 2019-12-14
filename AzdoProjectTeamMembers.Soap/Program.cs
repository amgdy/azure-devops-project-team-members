using CsvHelper;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzdoProjectTeamMembers.Soap
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
            using (TfsTeamProjectCollection tpc = new TfsTeamProjectCollection(new Uri(url), vssBasicCredential))
            {
                tpc.Authenticate();

                Console.WriteLine();
                Console.Write("Instance/Collection Display Name: ");
                Console.WriteLine(tpc.DisplayName);
                
                Console.Write("Instance/Collection ID: ");
                Console.WriteLine(tpc.InstanceId);
                Console.WriteLine();

                var data = new List<ProjectTeamMember>();

                var log = new StringBuilder();
                var workItemStore = tpc.GetService<WorkItemStore>();
                var teamService = tpc.GetService<TfsTeamService>();

                // Get security namespace for the project collection.
                ISecurityService securityService = tpc.GetService<ISecurityService>();
                SecurityNamespace securityNamespace = securityService.GetSecurityNamespace(FrameworkSecurity.IdentitiesNamespaceId);
                var projects = workItemStore.Projects;
                foreach (Project project in projects)
                {
                    log.AppendLine($"PROJECT: {project.Name}");
                    Console.WriteLine($"PROJECT: {project.Name}");

                    var teams = teamService.QueryTeams(project.Uri.ToString());

                    foreach (var team in teams)
                    {
                        log.AppendLine($"\tTEAM: {team.Name}");
                        Console.WriteLine($"\tTEAM: {team.Name}");

                        var members = team.GetMembers(tpc, MembershipQuery.Expanded);
                        string token = IdentityHelper.CreateSecurityToken(team.Identity);
                        // Retrieve an ACL object for all the team members.
                        AccessControlList acl = securityNamespace.QueryAccessControlList(token, members.Select(m => m.Descriptor), true);
                        // Retrieve the team administrator SIDs by querying the ACL entries.
                        var entries = acl.AccessControlEntries;
                        var admins = entries.Where(e => (e.Allow & 15) == 15).Select(e => e.Descriptor.Identifier);

                        // Finally, retrieve the actual TeamFoundationIdentity objects from the SIDs.
                        var adminIdentities = members.Where(m => admins.Contains(m.Descriptor.Identifier));

                        foreach (var member in members)
                        {
                            var isAdmin = adminIdentities.Any(i => i == member);
                            var isAdminText = isAdmin ? " [ADMIN]" : string.Empty;

                            log.AppendLine($"\t\tACCOUNT: {member.DisplayName} {isAdminText}");
                            Console.WriteLine($"\t\tACCOUNT: {member.DisplayName} {isAdminText}");

                            var projectTeamMember = new ProjectTeamMember
                            {
                                ProjectId = project.Id,
                                ProjectName = project.Name,
                                TeamId = team.Identity.TeamFoundationId,
                                TeamName = team.Name,
                                MemberId = member.TeamFoundationId,
                                MemberName = member.DisplayName,
                                IsTeamAdmin = isAdmin
                            };

                            data.Add(projectTeamMember);

                        }
                    }

                    Console.WriteLine();
                }

                File.AppendAllText($"{tpc.InstanceId}.{DateTime.Now.ToString("yyyyMMddhhmmss")}.txt", log.ToString());

                using (var writer = new StreamWriter($"{tpc.InstanceId}.{DateTime.Now.ToString("yyyyMMddhhmmss")}.csv"))
                using (var csv = new CsvWriter(writer))
                {
                    csv.WriteRecords(data);
                }
            }

            Console.ReadLine();
        }
    }
}
