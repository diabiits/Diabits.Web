using Diabits.Web.Infrastructure.Api;

namespace Diabits.Web.Features.Invites.Services;

/// <summary>
/// Handles authentication business logic and state management.
/// </summary>
public class InviteService
{
    private readonly ApiClient _apiClient;

    public InviteService(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<Invite>> GetInvitesAsync()
    {
        var result = await _apiClient.GetAsync<InviteResponse>("Invite");

        if (!result.IsSuccess)
            throw new NotImplementedException(); //TODO Handle error

        return result.Data?.Invites.ToList() ?? new List<Invite>();
    }

    public async Task<List<Invite>> CreateInviteAsync(Invite invite)
    {
        var result = await _apiClient.PostAsync<InviteResponse>("Invite", invite);

        if (!result.IsSuccess)
            throw new NotImplementedException(); //TODO Handle error

        return result.Data?.Invites.ToList() ?? new List<Invite>();
    }



    //TODO Move?
    public record InviteResponse(IEnumerable<Invite> Invites);

    //TODO Refactor UsedBy to be a user object instead of just a string username
    public record Invite(string Id, string Email, string Code, DateTime CreatedAt, string UsedBy, DateTime UsedAt);
}