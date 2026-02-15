namespace Diabits.Web.Models;

//TODO Move to DTO?
public record Invite(string Email, string Code, DateTime CreatedAt, string? UsedBy, DateTime? UsedAt);