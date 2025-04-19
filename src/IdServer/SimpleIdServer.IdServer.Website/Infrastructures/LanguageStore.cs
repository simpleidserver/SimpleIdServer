// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using System.Globalization;

namespace SimpleIdServer.IdServer.Website.Infrastructures;

public interface ILanguageStore
{
    IReadOnlyList<CultureInfo> Cultures { get; }
    Task InitialLoadAsync(CancellationToken cancellationToken = default);
    void Update(IList<string> codes);
    event Action<IList<string>>? LanguagesUpdated;
}

public class LanguageStore : ILanguageStore
{
    private readonly object _lock = new();
    private List<CultureInfo> _cultures = new();
    private readonly ILanguageService _languageService;

    public LanguageStore(ILanguageService languageService)
    {
        _languageService = languageService;
    }

    public event Action<IList<string>>? LanguagesUpdated;

    public IReadOnlyList<CultureInfo> Cultures
    {
        get
        {
            lock (_lock) { return _cultures.ToList().AsReadOnly(); }
        }
    }

    public async Task InitialLoadAsync(CancellationToken cancellationToken = default)
    {
        var codes = await _languageService.GetSupportedLanguagesAsync();
        Update(codes);
    }

    public void Update(IList<string> codes)
    {
        var cultures = codes
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Select(code => new CultureInfo(code.Trim()))
            .ToList();

        if (!cultures.Any())
        {
            return;
        }

        lock (_lock)
        {
            _cultures = cultures;
        }

        LanguagesUpdated?.Invoke(codes);
    }
}
