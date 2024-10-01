<Query Kind="Program">
  <NuGetReference>Microsoft.TeamFoundationServer.Client</NuGetReference>
  <NuGetReference>Microsoft.TeamFoundationServer.ExtendedClient</NuGetReference>
  <Namespace>Microsoft.TeamFoundation.Core.WebApi</Namespace>
  <Namespace>Microsoft.TeamFoundation.SourceControl.WebApi</Namespace>
  <Namespace>Microsoft.VisualStudio.Services.Account.Client</Namespace>
  <Namespace>Microsoft.VisualStudio.Services.Common</Namespace>
  <Namespace>Microsoft.VisualStudio.Services.Identity.Client</Namespace>
  <Namespace>Microsoft.VisualStudio.Services.Profile</Namespace>
  <Namespace>Microsoft.VisualStudio.Services.Profile.Client</Namespace>
  <Namespace>Microsoft.VisualStudio.Services.WebApi</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>Microsoft.VisualStudio.Services.Identity</Namespace>
  <Namespace>Microsoft.VisualStudio.Services.Organization.Client</Namespace>
</Query>

async Task Main()
{
	{
		using var connection = Login();
		//using var connection = OnBehalfOfLogin();
		
		var identityClient = await connection.GetClientAsync<IdentityHttpClient>();
		var groups = await identityClient.ListGroupsAsync(recurse: true);
		
		await PullRequestsByProject(connection);
		//await PullRequests(connection);
		//await Groups(connection);
		//await PullRequestsThreads(connection);

		//await Profile(connection);
		//await Tenants(connection);
		//await OrganizationsOrprojects(connection);
	}
}

private async Task PullRequestsByProject(VssConnection connection)
{
	var adoUserId = connection.AuthorizedIdentity.Id.ToString();
	adoUserId.Dump();
	var client = await connection.GetClientAsync<ThrottledGitHttpClient>();
	var projectClient = await connection.GetClientAsync<ProjectHttpClient>();
	var aPR = await client.GetPullRequestByIdAsync(1234);
	var result = new List<GitPullRequest>(1000);
	var prCriteria = new GitPullRequestSearchCriteria() { ReviewerId = Guid.Parse("<guid>"), Status = PullRequestStatus.Active };

	var projects = await projectClient.GetProjects(ProjectState.WellFormed);
	foreach (var project in projects.OrderBy(p => p.Name))
	{
		var prs = await client.GetPullRequestsByProjectAsync(project.Id, prCriteria, top: 1_000);
		result.AddRange(prs);
		
		if(project.Name == "<My_Project>"){
			var pr = prs.Where(pr => pr.PullRequestId == 5678);
		}

		//$"{project.Name}\t{prs.Count}".Dump();

		int index = 1;
		while (prs.Count == 1_000)
		{
			prs = await client.GetPullRequestsByProjectAsync(project.Id, prCriteria, skip: 1_000 * index++, top: 1_000);
			foreach (var pr in prs)
			{
				//var members = await teamClient.GetTeamMembersWithExtendedPropertiesAsync(pr.Repository.ProjectReference.Id.ToString(), "<guid>");
				pr.CreationDate.ToShortDateString().Dump();
			}
			result.AddRange(prs);
			//$"{project.Name}\t{prs.Count}".Dump();
		}
	}

	result.Count.Dump();
}

private async Task PullRequests(VssConnection connection)
{
	connection.AuthorizedIdentity.Id.Dump();
	var client = await connection.GetClientAsync<ThrottledGitHttpClient>();
	var identityClient = await connection.GetClientAsync<IdentityHttpClient>();
	var projectClient = await connection.GetClientAsync<ProjectHttpClient>();

	var result = new List<GitPullRequest>(1000);
	var result2 = new List<GitPullRequest>(1000);

	var prCriteria = new GitPullRequestSearchCriteria() { Status = PullRequestStatus.Active };

	var projects = await projectClient.GetProjects(ProjectState.WellFormed);

	foreach (var project in projects.OrderBy(p => p.Name))
	{
		var prs = await client.GetPullRequestsByProjectAsync(project.Id, prCriteria, top: 1_000);
		result2.AddRange(prs);

		//$"{project.Name}\t{prs.Count}".Dump();

		int index = 1;
		while (prs.Count == 1_000)
		{
			prs = await client.GetPullRequestsByProjectAsync(project.Id, prCriteria, skip: 1_000 * index++, top: 1_000);
			result2.AddRange(prs);
			//$"{project.Name}\t{prs.Count}".Dump();
		}
	}

	var byRepo2 = result2.GroupBy(p => p.Repository.Name).ToList();

	var repos = await client.GetRepositoriesAsync();
	var reposByProject = repos.GroupBy(r => r.ProjectReference.Name).ToList();
	//
	//	foreach (var g in reposByProject.OrderBy(g => g.Key))
	//	{
	//		if (g.Key != "<My_Project>") continue;
	//
	//		var prsByProject = new List<GitPullRequest>();
	//		foreach (var repo in g)
	//		{
	//			try
	//			{
	//				var prs = await client.GetPullRequestsAsync(repo.Id, prCriteria, top: 1_000);
	//				$"{repo.Name}\t{prs.Count}".Dump();
	//				prsByProject.AddRange(prs);
	//			}
	//			catch { }
	//		}
	//
	//		$"{g.Key}\t{prsByProject.Count}".Dump();
	//	}
	//
	//	var myProject = await client.GetPullRequestsByProjectAsync("<My_Project>", default);


	foreach (var repo in repos)
	{
		//repo.Name.Dump();
		try
		{
			var gitPrs = await client.GetPullRequestsAsync(repo.Id, prCriteria, top: 1_000);
			result.AddRange(gitPrs);
			//result.Count().Dump();

			int index = 1;
			while (gitPrs.Count == 1_000)
			{
				gitPrs = await client.GetPullRequestsAsync(repo.Id, prCriteria, skip: 1_000 * index++, top: 1_000);
				result.AddRange(gitPrs);
			}

			//gitPrs.Dump();
			//foreach (var pr in gitPrs)
			//{
			//	pr.Title.Dump();
			//	var groups = pr.Reviewers.Where(r => r.IsContainer);
			//	//groups.Dump();
			//	foreach (var group in pr.Reviewers)
			//	{
			//		if (group.IsContainer)
			//		{
			//			var groupIdentity = await identityClient.ReadIdentityAsync(group.Id, queryMembership: QueryMembership.Expanded);
			//			foreach (var u in groupIdentity.MemberIds)
			//			{
			//				u.Dump();
			//			}
			//		}
			//		//(await client.GetPullRequestReviewerAsync(repo.Id.ToString(), pr.PullRequestId, group.Id)).Dump();
			//	}
			//}
			//gitPrs[0].Reviewers.Dump();		
		}
		catch (Exception e) { e.Dump(); }
	}

	result.Count.Dump();
	result2.Count.Dump();
}

private async Task PullRequestsThreads(VssConnection connection)
{
	using var client = await connection.GetClientAsync<ThrottledGitHttpClient>();
	foreach (var repo in await client.GetRepositoriesAsync())
	{
		foreach (var pr in await client.GetPullRequestsAsync(repo.Id, default))
		{
			foreach (var thread in await client.GetThreadsAsync(repo.ProjectReference.Id, repo.Id, pr.PullRequestId, default))
			{
				//thread.Dump();
				var comments = await client.GetCommentsAsync(repo.Id, pr.PullRequestId, thread.Id, default);
				comments.First().Dump();
				//comments.Dump();
			}
		}
	}
}

private async Task Groups(VssConnection connection)
{
	//using var teamsClient = await connection.GetClientAsync<TeamHttpClient>();
	//(await teamsClient.GetAllTeamsAsync()).Dump();

	using var idClient = await connection.GetClientAsync<IdentityHttpClient>();

	var res = await idClient.ListGroupsAsync(recurse: true);
	res.Dump();
	(await idClient.ReadIdentityAsync("<guid>", queryMembership: QueryMembership.Expanded)).Dump();

	//var result = await idClient.GetDescriptorByIdAsync(Guid.Parse("<guid>"));
	//result.Dump();


	//(await client.GetUserIdentityIdsByDomainIdAsync(Guid.Parse("<guid>"))).Dump();
	//(await client.GetIdentitySnapshotAsync(Guid.Parse("<guid>"))).Dump();
	//(await client.ListUsersAsync("<guid>")).Dump();


	//using var client2 = await connection.GetClientAsync<TeamHttpClient>();
	//var teams = await client2.GetTeamsAsync("<my_project>", mine: true);
	//teams.Dump();

	//foreach (var repo in new [];
	//{
	//	foreach (var pr in await client.GetPullRequestsAsync(repo.Id, default))
	//	{
	//		foreach (var thread in await client.GetThreadsAsync(repo.ProjectReference.Id, repo.Id, pr.PullRequestId, default))
	//		{
	//			//thread.Dump();
	//			var comments = await client.GetCommentsAsync(repo.Id, pr.PullRequestId, thread.Id, default);
	//			comments.First().Dump();
	//			//comments.Dump();
	//		}
	//	}
	//}
}

private async Task Profile(VssConnection connection)
{
	using var profileClient = await connection.GetClientAsync<ProfileHttpClient>();
	(await profileClient.GetProfileAsync(new(AttributesScope.Core))).Dump();
	var me = (await profileClient.GetProfileAsync(new(AttributesScope.Core))).PublicAlias;
}

private async Task Tenants(VssConnection connection)
{
	using var projectCollectionsClient = await connection.GetClientAsync<ProjectCollectionHttpClient>();
	(await projectCollectionsClient.GetProjectCollections()).Dump();
}

private async Task OrganizationsOrprojects(VssConnection connection)
{

	using var projectClient = await connection.GetClientAsync<ProjectHttpClient>();
	var project = await projectClient.GetProjects(ProjectState.All);
	project.Dump();
}

private VssConnection Login()
{
	var baseUrl = "https://dev.azure.com";
	var projectName = "<my_project>";

	var token = Util.GetPassword("<my_password>");
	return new VssConnection(new Uri($"{baseUrl}/{projectName}/"), new VssBasicCredential(string.Empty, token));
}

private VssConnection OnBehalfOfLogin()
{
	var baseUrl = "https://dev.azure.com";
	var projectName = "<my_project>";

	var token = "<token>";
	return new VssConnection(new Uri($"{baseUrl}/{projectName}/"), new VssBasicCredential(string.Empty, token));
}