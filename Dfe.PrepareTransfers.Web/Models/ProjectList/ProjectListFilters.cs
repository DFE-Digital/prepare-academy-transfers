#nullable enable

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dfe.PrepareTransfers.Web.Models.ProjectList;

public class ProjectListFilters
{
    public const string TransfersFilterTitle = nameof(TransfersFilterTitle);
    public const string TransfersFilterStatuses = nameof(TransfersFilterStatuses);
    public const string TransfersFilterOfficers = nameof(TransfersFilterOfficers);

    private IDictionary<string, object?> _store = null!;

    public List<string> AvailableStatuses { get; set; } = new();
    public List<string> AvailableDeliveryOfficers { get; set; } = new();

    [BindProperty]
    public string? Title { get; set; }

    [BindProperty]
    public string[] SelectedStatuses { get; set; } = Array.Empty<string>();

    [BindProperty]
    public string[] SelectedOfficers { get; set; } = Array.Empty<string>();

    public bool IsVisible => string.IsNullOrWhiteSpace(Title) is false ||
                             SelectedStatuses.Length > 0 ||
                             SelectedOfficers.Length > 0;

    public ProjectListFilters PersistUsing(IDictionary<string, object?> store)
    {
        _store = store;

        Title = Get(TransfersFilterTitle).FirstOrDefault()?.Trim();
        SelectedStatuses = Get(TransfersFilterStatuses);
        SelectedOfficers = Get(TransfersFilterOfficers);

        return this;
    }

    public void PopulateFrom(IEnumerable<KeyValuePair<string, StringValues>> requestQuery)
    {
        Dictionary<string, StringValues>? query = new(requestQuery, StringComparer.OrdinalIgnoreCase);

        if (query.ContainsKey("clear"))
        {
            ClearFilters();

            Title = default;
            SelectedStatuses = Array.Empty<string>();
            SelectedOfficers = Array.Empty<string>();

            return;
        }

        if (query.ContainsKey("remove"))
        {
            SelectedStatuses = GetAndRemove(TransfersFilterStatuses, GetFromQuery(nameof(SelectedStatuses)), true);
            SelectedOfficers = GetAndRemove(TransfersFilterOfficers, GetFromQuery(nameof(SelectedOfficers)), true);

            return;
        }

        bool activeFilterChanges = query.ContainsKey(nameof(Title)) ||
                                   query.ContainsKey(nameof(SelectedStatuses)) ||
                                   query.ContainsKey(nameof(SelectedOfficers)); ;

        if (activeFilterChanges)
        {
            Title = Cache(TransfersFilterTitle, GetFromQuery(nameof(Title))).FirstOrDefault()?.Trim();
            SelectedStatuses = Cache(TransfersFilterStatuses, GetFromQuery(nameof(SelectedStatuses)));
            SelectedOfficers = Cache(TransfersFilterOfficers, GetFromQuery(nameof(SelectedOfficers)));
        }
        else
        {
            Title = Get(TransfersFilterTitle, true).FirstOrDefault()?.Trim();
            SelectedStatuses = Get(TransfersFilterStatuses, true);
            SelectedOfficers = Get(TransfersFilterOfficers, true);
        }

        string[] GetFromQuery(string key)
        {
            return query.ContainsKey(key) ? query[key]! : Array.Empty<string>();
        }
    }

    private string[] Get(string key, bool persist = false)
    {
        if (_store.ContainsKey(key) is false) return Array.Empty<string>();

        string[]? value = (string[]?)_store[key];
        if (persist) Cache(key, value);

        return value ?? Array.Empty<string>();
    }

    private string[] GetAndRemove(string key, string[]? value, bool persist = false)
    {
        if (_store.ContainsKey(key) is false) return Array.Empty<string>();

        string[]? currentValues = (string[]?)_store[key];

        if (value is not null && value.Length > 0 && currentValues is not null)
        {
            currentValues = currentValues.Where(x => !value.Contains(x)).ToArray();
        }

        if (persist) Cache(key, currentValues);

        return currentValues ?? Array.Empty<string>();
    }

    private string[] Cache(string key, string[]? value)
    {
        if (value is null || value.Length == 0)
            _store.Remove(key);
        else
            _store[key] = value;

        return value ?? Array.Empty<string>();
    }

    private void ClearFilters()
    {
        Cache(TransfersFilterTitle, default);
        Cache(TransfersFilterStatuses, default);
        Cache(TransfersFilterOfficers, default);
    }

    /// <summary>
    ///    Removes all project list filters from the store
    /// </summary>
    /// <param name="store">the store used to cache filters between pages</param>
    /// <remarks>
    ///    Note that, when using TempData, this won't take effect until after the next request context that returns a 2xx response!
    /// </remarks>
    public static void ClearFiltersFrom(IDictionary<string, object?> store)
    {
        new ProjectListFilters().PersistUsing(store).ClearFilters();
    }
}
