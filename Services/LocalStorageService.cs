using Microsoft.JSInterop;
using System.Text.Json;
using DiarioEscolar.Models;

namespace DiarioEscolar.Services;

public class LocalStorageService
{
    private readonly IJSRuntime _jsRuntime;

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<List<Turma>> GetTurmasAsync()
    {
        try
        {
            var json = await GetItemAsync("turmas");
            return string.IsNullOrEmpty(json) ? new List<Turma>() : JsonSerializer.Deserialize<List<Turma>>(json) ?? new List<Turma>();
        }
        catch (JsonException)
        {
            await ClearAllDataAsync();
            return new List<Turma>();
        }
    }

    public async Task SaveTurmasAsync(List<Turma> turmas)
    {
        var json = JsonSerializer.Serialize(turmas);
        await SetItemAsync("turmas", json);
    }

    public async Task UpdateTurmaAsync(Turma turma)
    {
        var turmas = await GetTurmasAsync();
        var index = turmas.FindIndex(t => t.Id == turma.Id);
        if (index >= 0)
        {
            turmas[index] = turma;
            await SaveTurmasAsync(turmas);
        }
    }

    public async Task DeleteTurmaAsync(string turmaId)
    {
        var turmas = await GetTurmasAsync();
        turmas.RemoveAll(t => t.Id == turmaId);
        await SaveTurmasAsync(turmas);
    }

    // Métodos base do localStorage (persistência bruta)
    public async Task<string> GetItemAsync(string key)
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
    }

    public async Task SetItemAsync(string key, string value)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, value);
    }

    public async Task RemoveItemAsync(string key)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
    }

    public async Task ClearAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.clear");
    }

    public async Task ClearAllDataAsync()
    {
        await RemoveItemAsync("turmas");
        await RemoveItemAsync("estudantes");
        await RemoveItemAsync("diarios");
        await RemoveItemAsync("horarios");
    }
}
