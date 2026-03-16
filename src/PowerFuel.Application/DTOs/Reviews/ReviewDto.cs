namespace PowerFuel.Application.DTOs.Reviews;

public record ReviewDto(int Id, int Rating, string? Comment, string? UserName, DateTime CreatedAt);
