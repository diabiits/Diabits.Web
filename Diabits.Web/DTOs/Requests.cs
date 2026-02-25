namespace Diabits.Web.DTOs;

// TODO Move DTOs from files
public record InviteRequest(string Email);
public record LoginRequest(string Username, string Password); 
public record UpdateAccountRequest(string CurrentPassword, string? NewUsername, string? NewPassword);