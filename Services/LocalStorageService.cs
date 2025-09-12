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

    public async Task UpdateDiarioAsync(Diario diario)
    {
        var diarios = await GetDiariosAsync();
        var index = diarios.FindIndex(d => d.Id == diario.Id);
        if (index >= 0)
        {
            diarios[index] = diario;
            await SaveDiariosAsync(diarios);
        }
    }

    public async Task SaveDiarioRecordAsync(string turmaId, string professorNome, string disciplina, DateTime data, string conteudo, string observacoes)
    {
        var turmas = await GetTurmasAsync();
        var turma = turmas.FirstOrDefault(t => t.Id == turmaId);
        
        if (turma != null)
        {
            var diario = turma.Diarios.FirstOrDefault(d => 
                d.ProfessorNome == professorNome && 
                d.Disciplina == disciplina);
            
            if (diario == null)
            {
                diario = new Diario
                {
                    ProfessorNome = professorNome,
                    Disciplina = disciplina,
                    TurmaId = turmaId,
                    Registros = new Dictionary<DateTime, string>()
                };
                turma.Diarios.Add(diario);
            }
            
            // Formato: conteudo|observacoes
            var recordData = $"{conteudo}|{observacoes}";
            diario.Registros[data.Date] = recordData;
            
            await SaveTurmasAsync(turmas);
        }
    }

    public async Task<(string conteudo, string observacoes)> GetDiarioRecordAsync(string turmaId, string professorNome, string disciplina, DateTime data)
    {
        var turmas = await GetTurmasAsync();
        var turma = turmas.FirstOrDefault(t => t.Id == turmaId);
        
        if (turma != null)
        {
            var diario = turma.Diarios.FirstOrDefault(d => 
                d.ProfessorNome == professorNome && 
                d.Disciplina == disciplina);
            
            if (diario?.Registros.ContainsKey(data.Date) == true)
            {
                var recordData = diario.Registros[data.Date];
                var parts = recordData.Split('|', 2);
                return (parts.Length > 0 ? parts[0] : "", parts.Length > 1 ? parts[1] : "");
            }
        }
        
        return ("", "");
    }

    public async Task<List<string>> GetEducadoresAsync()
    {
        var turmas = await GetTurmasAsync();
        return turmas
            .SelectMany(t => t.Horarios)
            .Select(h => h.Professor)
            .Where(p => !string.IsNullOrEmpty(p))
            .Distinct()
            .OrderBy(p => p)
            .ToList();
    }

    public async Task<List<Diario>> GetDiariosByEducadorAsync(string educador)
    {
        var turmas = await GetTurmasAsync();
        return turmas
            .SelectMany(t => t.Horarios
                .Where(h => h.Professor == educador)
                .Select(h => new Diario
                {
                    ProfessorNome = h.Professor,
                    Disciplina = h.Disciplina,
                    TurmaId = t.Id,
                    Registros = t.Diarios
                        .FirstOrDefault(d => d.ProfessorNome == h.Professor && d.Disciplina == h.Disciplina)?.Registros 
                        ?? new Dictionary<DateTime, string>()
                }))
            .GroupBy(d => new { d.TurmaId, d.Disciplina })
            .Select(g => g.First())
            .ToList();
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
