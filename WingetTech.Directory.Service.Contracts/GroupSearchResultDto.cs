namespace WingetTech.Directory.Service.Contracts;

public record GroupSearchResultDto(
    IReadOnlyCollection<GroupDto> Groups,
    int TotalCount
);
