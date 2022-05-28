﻿namespace Logistics.WebApi.Client.Implementations;

internal class ApiClient : ApiClientBase, IApiClient
{
    public ApiClient(ApiClientOptions options) : base(options.Host!)
    {
        SetTenantId(options.TenantId);
    }

    public string? TenantId { get; private set; }

    public void SetTenantId(string? tenantId)
    {
        TenantId = tenantId;
        SetRequestHeader("X-TenantId", tenantId);
    }

    #region Cargo API

    public async Task<CargoDto?> GetCargoAsync(string id)
    {
        var result = await GetRequestAsync<DataResult<CargoDto>>($"api/cargo/{id}");
        return result.Value;
    }

    public async Task<PagedDataResult<CargoDto>> GetCargoesAsync(string searchInput = "", int page = 1, int pageSize = 10)
    {
        var query = new Dictionary<string, string>
        {
            {"page", page.ToString() },
            {"pageSize", pageSize.ToString() }
        };

        if (!string.IsNullOrEmpty(searchInput))
        {
            query.Add("search", searchInput);
        }
        var result = await GetRequestAsync<PagedDataResult<CargoDto>>("api/cargo/list", query);
        return result;
    }

    public Task CreateCargoAsync(CargoDto cargo)
    {
        return PostRequestAsync("api/cargo/create", cargo);
    }
    
    public Task UpdateCargoAsync(CargoDto cargo)
    {
        return PutRequestAsync($"api/cargo/update/{cargo.Id}", cargo);
    }

    public Task DeleteCargoAsync(string id)
    {
        return DeleteRequestAsync($"api/cargo/{id}");
    }

    #endregion


    #region Truck API

    public async Task<TruckDto?> GetTruckAsync(string id)
    {
        var result = await GetRequestAsync<DataResult<TruckDto>>($"api/truck/{id}");
        return result.Value;
    }

    public async Task<TruckDto?> GetTruckByDriverAsync(string driverId)
    {
        var result = await GetRequestAsync<DataResult<TruckDto>>($"api/truck/driver/{driverId}");
        return result.Value;
    }

    public async Task<PagedDataResult<TruckDto>> GetTrucksAsync(string searchInput = "", int page = 1, int pageSize = 10, bool includeCargoIds = false)
    {
        var query = new Dictionary<string, string>
        {
            {"page", page.ToString() },
            {"pageSize", pageSize.ToString() },
            {"includeCargoIds", includeCargoIds.ToString() }
        };

        if (!string.IsNullOrEmpty(searchInput))
        {
            query.Add("search", searchInput);
        }
        var result = await GetRequestAsync<PagedDataResult<TruckDto>>("api/truck/list", query);
        return result;
    }

    public Task CreateTruckAsync(TruckDto truck)
    {
        return PostRequestAsync("api/truck/create", truck);
    }

    public Task UpdateTruckAsync(TruckDto truck)
    {
        return PutRequestAsync($"api/truck/update/{truck.Id}", truck);
    }

    public Task DeleteTruckAsync(string id)
    {
        return DeleteRequestAsync($"api/truck/{id}");
    }

    #endregion


    #region User API

    public async Task<UserDto?> GetUserAsync(string id)
    {
        var result = await GetRequestAsync<DataResult<UserDto>>($"api/user/{id}");
        return result.Value;
    }

    public async Task<PagedDataResult<UserDto>> GetUsersAsync(string searchInput = "", int page = 1, int pageSize = 10)
    {
        var query = new Dictionary<string, string>
        {
            {"page", page.ToString() },
            {"pageSize", pageSize.ToString() }
        };

        if (!string.IsNullOrEmpty(searchInput))
        {
            query.Add("search", searchInput);
        }
        var result = await GetRequestAsync<PagedDataResult<UserDto>>("api/user/list", query);
        return result;
    }

    public async Task<bool> UserExistsAsync(string externalId)
    {
        try
        {
            var result = await GetUserAsync(externalId);
            return result != null;
        }
        catch (ApiException)
        {
            return false;
        }
    }

    public Task CreateUserAsync(UserDto user)
    {
        return PostRequestAsync("api/user/create", user);
    }

    public async Task<bool> TryCreateUserAsync(UserDto user)
    {
        if (string.IsNullOrEmpty(user.ExternalId))
        {
            throw new ApiException("ExternalId is null or empty");
        }

        var userExists = await UserExistsAsync(user.ExternalId);

        if (!userExists)
        {
            await CreateUserAsync(user);
            return true;
        }

        return false;
    }

    public Task UpdateUserAsync(UserDto user)
    {
        return PutRequestAsync($"api/user/update/{user.Id}", user);
    }

    #endregion


    #region Tenant API

    public async Task<TenantDto?> GetTenantAsync(string id)
    {
        var result = await GetRequestAsync<DataResult<TenantDto>>($"api/tenant/{id}");
        return result.Value;
    }

    public async Task<PagedDataResult<TenantDto>> GetTenantsAsync(string searchInput = "", int page = 1, int pageSize = 10)
    {
        var query = new Dictionary<string, string>
        {
            {"page", page.ToString() },
            {"pageSize", pageSize.ToString() }
        };

        if (!string.IsNullOrEmpty(searchInput))
        {
            query.Add("search", searchInput);
        }
        var result = await GetRequestAsync<PagedDataResult<TenantDto>>("api/tenant/list", query);
        return result;
    }

    public Task CreateTenantAsync(TenantDto tenant)
    {
        return PostRequestAsync("api/tenant/create", tenant);
    }

    public Task UpdateTenantAsync(TenantDto tenant)
    {
        return PutRequestAsync($"api/tenant/update/{tenant.Id}", tenant);
    }

    public Task DeleteTenantAsync(string id)
    {
        return DeleteRequestAsync($"api/tenant/{id}");
    }

    #endregion
}
