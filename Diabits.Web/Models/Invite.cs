namespace Diabits.Web.Models;

// TODO Move models from files
public record Invite(string Email, string Code, DateTime CreatedAt, string? UsedBy, DateTime? UsedAt);