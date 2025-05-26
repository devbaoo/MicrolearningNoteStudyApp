$projects = @(
    "Common",
    "AuthenticationFunction",
    "UserManagementFunction",
    "NoteManagementFunction",
    "AtomManagementFunction",
    "KnowledgeGraphFunction",
    "ReviewSystemFunction",
    "LearningAnalyticsFunction",
    "SearchDiscoveryFunction",
    "IntegrationFunction",
    "NotificationFunction",
    "AuthenticationFunction.Tests",
    "UserManagementFunction.Tests",
    "NoteManagementFunction.Tests",
    "AtomManagementFunction.Tests",
    "KnowledgeGraphFunction.Tests",
    "ReviewSystemFunction.Tests",
    "LearningAnalyticsFunction.Tests",
    "SearchDiscoveryFunction.Tests",
    "IntegrationFunction.Tests",
    "NotificationFunction.Tests"
)

$solutionContent = @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17
VisualStudioVersion = 17.14.36121.58
MinimumVisualStudioVersion = 10.0.40219.1
"@

foreach ($project in $projects) {
    $projectGuid = [Guid]::NewGuid().ToString("B").ToUpper()
    $projectPath = "backend\$project\$project.csproj"
    
    $solutionContent += @"

Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "$project", "$project\$project.csproj", "{$projectGuid}"
EndProject
"@
}

$solutionContent += @"

Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
"@

foreach ($project in $projects) {
    $projectGuid = [Guid]::NewGuid().ToString("B").ToUpper()
    
    $solutionContent += @"

		{$projectGuid}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{$projectGuid}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{$projectGuid}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{$projectGuid}.Release|Any CPU.Build.0 = Release|Any CPU
"@
}

$solutionContent += @"

	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
	GlobalSection(ExtensibilityGlobals) = postSolution
		SolutionGuid = {8C944352-2C0E-4036-828E-756589F4CE71}
	EndGlobalSection
EndGlobal
"@

$solutionContent | Out-File -Encoding utf8 -FilePath "backend/backend.sln"

Write-Host "Solution file updated successfully!" 