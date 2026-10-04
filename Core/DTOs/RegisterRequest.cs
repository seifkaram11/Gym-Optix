using Core.Enum;

namespace Core.DTOs;

public record RegisterRequest(
  string? Email,
  string Password,
  string? PersonName,
  Genders Gender);
