#nullable enable

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dfe.PrepareTransfers.Web.Models.ProjectList;

public class ProjectListFilters
{
    public const string FilterTitle = nameof(FilterTitle);

    private IDictionary<string, object?> _store = null!;

    [BindProperty]
    public string? Title { get; set; }

    public bool IsFiltered => string.IsNullOrWhiteSpace(Title) is false;

    public ProjectListFilters PersistUsing(IDictionary<string, object?> store)
    {
        _store = store;

        Title = Get(FilterTitle).FirstOrDefault()?.Trim();

        return this;
    }

    public void PopulateFrom(IEnumerable<KeyValuePair<string, StringValues>> requestQuery)
    {
        Dictionary<string, StringValues>? query = new(requestQuery, StringComparer.OrdinalIgnoreCase);

        if (query.ContainsKey("clear"))
        {
            ClearFilters();

            Title = default;

            return;
        }

        Title = query.ContainsKey(nameof(Title))
            ? Cache(FilterTitle, GetFromQuery(nameof(Title))).FirstOrDefault()?.Trim()
            : Get(FilterTitle, true).FirstOrDefault()?.Trim();


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
        Cache(FilterTitle, default);
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
