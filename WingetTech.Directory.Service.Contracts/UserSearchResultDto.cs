namespace WingetTech.Directory.Service.Contracts;

public record UserSearchResultDto(
    IReadOnlyCollection<UserDto> Users,
    int TotalCount
);
