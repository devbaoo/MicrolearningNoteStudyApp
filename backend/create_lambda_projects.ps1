$lambdaProjects = @(
    "AuthenticationFunction",
    "NoteManagementFunction",
    "AtomManagementFunction",
    "KnowledgeGraphFunction",
    "ReviewSystemFunction",
    "LearningAnalyticsFunction",
    "SearchDiscoveryFunction",
    "IntegrationFunction",
    "NotificationFunction"
)

# Create Lambda Function projects
foreach ($project in $lambdaProjects) {
    # Create the main project directory if it doesn't exist
    if (-not (Test-Path "backend/$project")) {
        New-Item -Path "backend/$project" -ItemType Directory
    }

    # Create the project file
    $projectContent = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <GenerateRuntimeConfigurationFiles>true</GenerateRuntimeConfigurationFiles>
    <AWSProjectType>Lambda</AWSProjectType>
    <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
    <PublishReadyToRun>true</PublishReadyToRun>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Amazon.Lambda.Core" Version="2.5.0" />
    <PackageReference Include="Amazon.Lambda.Serialization.SystemTextJson" Version="2.4.4" />
    <PackageReference Include="Amazon.Lambda.APIGatewayEvents" Version="2.7.0" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\Common\Common.csproj" />
  </ItemGroup>
</Project>
"@
    $projectContent | Out-File -Encoding utf8 -FilePath "backend/$project/$project.csproj"

    # Create Function.cs
    $functionContent = @"
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace $project;

public class Function
{
    public async Task<APIGatewayHttpApiV2ProxyResponse> FunctionHandler(APIGatewayHttpApiV2ProxyRequest request, ILambdaContext context)
    {
        context.Logger.LogInformation("Processing request in $project");

        return new APIGatewayHttpApiV2ProxyResponse
        {
            StatusCode = 200,
            Body = JsonSerializer.Serialize(new { Message = "$project is working!" }),
            Headers = new Dictionary<string, string> { { "Content-Type", "application/json" } }
        };
    }
}
"@
    $functionContent | Out-File -Encoding utf8 -FilePath "backend/$project/Function.cs"

    # Create the Handlers directory
    if (-not (Test-Path "backend/$project/Handlers")) {
        New-Item -Path "backend/$project/Handlers" -ItemType Directory
    }

    # Create the Services directory
    if (-not (Test-Path "backend/$project/Services")) {
        New-Item -Path "backend/$project/Services" -ItemType Directory
    }
}

# Create test projects
foreach ($project in $lambdaProjects) {
    $testProject = "$project.Tests"
    
    # Create the test project directory if it doesn't exist
    if (-not (Test-Path "backend/$testProject")) {
        New-Item -Path "backend/$testProject" -ItemType Directory
    }

    # Create the project file
    $testProjectContent = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.9.0" />
    <PackageReference Include="xunit" Version="2.7.0" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.7">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="coverlet.collector" Version="6.0.1">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Moq" Version="4.20.70" />
    <PackageReference Include="Amazon.Lambda.Core" Version="2.5.0" />
    <PackageReference Include="Amazon.Lambda.TestUtilities" Version="2.0.0" />
    <PackageReference Include="Amazon.Lambda.APIGatewayEvents" Version="2.7.0" />
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\$project\$project.csproj" />
  </ItemGroup>
</Project>
"@
    $testProjectContent | Out-File -Encoding utf8 -FilePath "backend/$testProject/$testProject.csproj"

    # Create FunctionTests.cs
    $testContent = @"
using Xunit;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;
using System.Text.Json;

namespace $testProject;

public class FunctionTests
{
    [Fact]
    public async Task TestFunctionHandler()
    {
        var request = new APIGatewayHttpApiV2ProxyRequest();
        var context = new TestLambdaContext();
        var function = new $project.Function();

        var response = await function.FunctionHandler(request, context);
        
        Assert.Equal(200, response.StatusCode);
        Assert.Contains("$project is working!", response.Body);
    }
}
"@
    $testContent | Out-File -Encoding utf8 -FilePath "backend/$testProject/FunctionTests.cs"
}

Write-Host "Lambda projects and test projects created successfully!" 