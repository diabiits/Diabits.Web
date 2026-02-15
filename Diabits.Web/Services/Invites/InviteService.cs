using Diabits.Web.DTOs;
using Diabits.Web.Infrastructure.Api;
using Diabits.Web.Models;

namespace Diabits.Web.Services.Invites;

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
        var result = await _apiClient.GetAsync<List<Invite>>("Invite");

        if (!result.IsSuccess)
            throw new InvalidOperationException(result.Error ?? "Something went wrong while getting invites");

        return result.Data?.ToList() ?? new List<Invite>();
    }

    public async Task<Invite> CreateInviteAsync(string email)
    {
        var result = await _apiClient.PostAsync<Invite>("Invite", new InviteRequest(email));

        if (result.Data == null)
            throw new HttpRequestException(result.Error ?? "Something went wrong while creating invite");

        return result.Data;
    }
}