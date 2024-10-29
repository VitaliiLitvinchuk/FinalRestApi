namespace Api.Dtos.Users;

public record UserUpdateDto(Guid Id, string FirstName, string LastName, string AvatarUrl);
