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

    // Métodos para Turmas
    public async Task<List<Turma>> GetTurmasAsync()
    {
        try
        {
            var json = await GetItemAsync("turmas");
            return string.IsNullOrEmpty(json) ? new List<Turma>() : JsonSerializer.Deserialize<List<Turma>>(json) ?? new List<Turma>();
        }
        catch (JsonException)
        {
            // Se houver erro de deserialização, limpa os dados antigos e retorna lista vazia
            await ClearAllDataAsync();
            return new List<Turma>();
        }
    }

    public async Task SaveTurmasAsync(List<Turma> turmas)
    {
        var json = JsonSerializer.Serialize(turmas);
        await SetItemAsync("turmas", json);
    }

    public async Task AddTurmaAsync(Turma turma)
    {
        var turmas = await GetTurmasAsync();
        turmas.Add(turma);
        await SaveTurmasAsync(turmas);
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

    // Métodos para Estudantes
    public async Task<List<Estudante>> GetEstudantesAsync()
    {
        var json = await GetItemAsync("estudantes");
        return string.IsNullOrEmpty(json) ? new List<Estudante>() : JsonSerializer.Deserialize<List<Estudante>>(json) ?? new List<Estudante>();
    }

    public async Task SaveEstudantesAsync(List<Estudante> estudantes)
    {
        var json = JsonSerializer.Serialize(estudantes);
        await SetItemAsync("estudantes", json);
    }

    public async Task AddEstudanteAsync(Estudante estudante)
    {
        var estudantes = await GetEstudantesAsync();
        estudantes.Add(estudante);
        await SaveEstudantesAsync(estudantes);
    }

    // Métodos para Diários
    public async Task<List<Diario>> GetDiariosAsync()
    {
        var json = await GetItemAsync("diarios");
        return string.IsNullOrEmpty(json) ? new List<Diario>() : JsonSerializer.Deserialize<List<Diario>>(json) ?? new List<Diario>();
    }

    public async Task SaveDiariosAsync(List<Diario> diarios)
    {
        var json = JsonSerializer.Serialize(diarios);
        await SetItemAsync("diarios", json);
    }

    public async Task AddDiarioAsync(Diario diario)
    {
        var diarios = await GetDiariosAsync();
        diarios.Add(diario);
        await SaveDiariosAsync(diarios);
    }

    // Métodos base do localStorage
    private async Task<string> GetItemAsync(string key)
    {
        return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
    }

    private async Task SetItemAsync(string key, string value)
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
