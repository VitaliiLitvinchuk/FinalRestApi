namespace Api.Dtos.Users;

public record UserCreateDto(string Email, string FirstName, string LastName, string GoogleId, string AvatarUrl);
