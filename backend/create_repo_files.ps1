$interfaces = @(
    "IUserRepository",
    "INoteRepository",
    "IAtomRepository",
    "IReviewRepository",
    "IAnalyticsRepository",
    "ISearchRepository",
    "IIntegrationRepository",
    "INotificationRepository"
)

$implementations = @(
    "UserRepository",
    "NoteRepository",
    "AtomRepository",
    "ReviewRepository",
    "AnalyticsRepository",
    "SearchRepository",
    "IntegrationRepository",
    "NotificationRepository"
)

foreach ($interface in $interfaces) {
    $content = @"
namespace Common.Repositories;

public interface $interface
{
}
"@
    $content | Out-File -Encoding utf8 -FilePath "backend/Common/Repositories/$interface.cs"
}

foreach ($implementation in $implementations) {
    $content = @"
namespace Common.Repositories;

public class $implementation
{
}
"@
    $content | Out-File -Encoding utf8 -FilePath "backend/Common/Repositories/$implementation.cs"
}

Write-Host "Repository files created successfully!" 