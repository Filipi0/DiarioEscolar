using DiarioEscolar.Models;
using DiarioEscolar.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiarioEscolar.Services
{
    public class DiarioService
    {
        private readonly LocalStorageService _localStorage;
        private readonly TurmasService _turmasService;

        public DiarioService(LocalStorageService localStorage, TurmasService turmasService)
        {
            _localStorage = localStorage;
            _turmasService = turmasService;
        }

        // Método principal para obter todos os diários e educadores
        public async Task<(List<string> Educadores, List<Diario> Diarios)> GetAllDiariosWithEducatorsAsync()
        {
            var turmas = await _turmasService.GetTurmasAsync();
            var todosDiarios = turmas.SelectMany(t => t.Diarios).ToList();
            var educadores = todosDiarios.Select(d => d.ProfessorNome)
                                        .Distinct()
                                        .Where(p => !string.IsNullOrWhiteSpace(p))
                                        .OrderBy(p => p)
                                        .ToList();
            return (educadores, todosDiarios);
        }

        // Obter todos os educadores únicos
        public async Task<List<string>> GetEducadoresAsync()
        {
            var (educadores, _) = await GetAllDiariosWithEducatorsAsync();
            return educadores;
        }

        // Obter diários de um educador específico
        public async Task<List<Diario>> GetDiariosByEducatorAsync(string educatorName)
        {
            var (_, todosDiarios) = await GetAllDiariosWithEducatorsAsync();
            return todosDiarios.Where(d => d.ProfessorNome == educatorName).ToList();
        }

        // Filtrar educadores por termo de busca
        public async Task<List<string>> FilterEducatorsAsync(string searchTerm)
        {
            var educadores = await GetEducadoresAsync();
            
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return educadores;
            }

            return educadores.Where(e => e.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                             .ToList();
        }

        // Obter turma associada a um diário
        public async Task<Turma?> GetTurmaForDiarioAsync(string diarioId)
        {
            var turmas = await _turmasService.GetTurmasAsync();
            var diario = turmas.SelectMany(t => t.Diarios).FirstOrDefault(d => d.Id == diarioId);
            return turmas.FirstOrDefault(t => t.Id == diario?.TurmaId);
        }

        // Obter diário por ID
        public async Task<Diario?> GetDiarioByIdAsync(string diarioId)
        {
            var turmas = await _turmasService.GetTurmasAsync();
            return turmas.SelectMany(t => t.Diarios).FirstOrDefault(d => d.Id == diarioId);
        }

        // Salvar registro de aula com DTO
        public async Task SaveDiarioRecordAsync(string diarioId, DiaryRecordDto recordDto)
        {
            var turmas = await _turmasService.GetTurmasAsync();
            var diario = turmas.SelectMany(t => t.Diarios).FirstOrDefault(d => d.Id == diarioId);
            
            if (diario != null)
            {
                if (diario.Registros == null)
                    diario.Registros = new Dictionary<DateTime, string>();

                var recordData = $"{recordDto.Content}|{recordDto.Observations}";
                diario.Registros[recordDto.Date.Date] = recordData;

                // Encontra a turma pai para salvar usando TurmasService
                var turmaPai = turmas.FirstOrDefault(t => t.Id == diario.TurmaId);
                if (turmaPai != null)
                {
                    await _turmasService.UpdateTurmaAsync(turmaPai);
                }
            }
        }

        // Obter registro de aula para uma data específica
        public async Task<(string Content, string Observations)> GetDiarioRecordAsync(string diarioId, DateTime date)
        {
            var diario = await GetDiarioByIdAsync(diarioId);
            
            if (diario?.Registros?.ContainsKey(date.Date) == true)
            {
                var recordData = diario.Registros[date.Date];
                var parts = recordData.Split('|', 2);
                var content = parts.Length > 0 ? parts[0] : "";
                var observations = parts.Length > 1 ? parts[1] : "";
                return (content, observations);
            }

            return ("", "");
        }

        // Verificar se há registro para uma data específica
        public async Task<bool> HasRecordAsync(string diarioId, DateTime date)
        {
            var diario = await GetDiarioByIdAsync(diarioId);
            return diario?.Registros?.ContainsKey(date.Date) ?? false;
        }

        // Métodos auxiliares para informações de turma (evita duplicação de código)
        public async Task<string> GetTurmaNameAsync(string turmaId)
        {
            var turma = await _turmasService.GetTurmaByIdAsync(turmaId);
            return turma?.Nome ?? "Turma";
        }

        public async Task<string> GetTurmaSerieAsync(string turmaId)
        {
            var turma = await _turmasService.GetTurmaByIdAsync(turmaId);
            return turma?.Serie ?? "";
        }

        public async Task<int> GetTurmaStudentCountAsync(string turmaId)
        {
            var turma = await _turmasService.GetTurmaByIdAsync(turmaId);
            return turma?.Estudantes?.Count ?? 0;
        }

        // Obter estudantes ordenados de uma turma
        public async Task<List<Estudante>> GetTurmaStudentsAsync(string turmaId)
        {
            var turma = await _turmasService.GetTurmaByIdAsync(turmaId);
            return turma?.Estudantes?.OrderBy(e => e.Nome).ToList() ?? new List<Estudante>();
        }

        // Obter horários de uma turma para verificar dias de aula
        public async Task<List<Horario>> GetTurmaHorariosAsync(string turmaId)
        {
            var turma = await _turmasService.GetTurmaByIdAsync(turmaId);
            return turma?.Horarios ?? new List<Horario>();
        }

        // Verificar se há aula em um dia específico para um diário
        public async Task<bool> HasClassOnDayAsync(string diarioId, DateTime date)
        {
            var diario = await GetDiarioByIdAsync(diarioId);
            if (diario == null) return false;

            var horarios = await GetTurmaHorariosAsync(diario.TurmaId);
            var dayOfWeek = (int)date.DayOfWeek;
            
            return horarios.Any(h => h.Professor == diario.ProfessorNome && 
                                   h.Disciplina == diario.Disciplina && 
                                   h.DiaSemana == dayOfWeek);
        }

        // Gerenciamento de estudantes (reutilizando TurmasService)
        public async Task AddEstudanteToTurmaAsync(string turmaId, string nomeEstudante)
        {
            await _turmasService.AddEstudanteAsync(turmaId, nomeEstudante);
        }

        public async Task UpdateEstudanteInTurmaAsync(string turmaId, string estudanteId, string novoNome)
        {
            await _turmasService.UpdateEstudanteAsync(turmaId, estudanteId, novoNome);
        }

        public async Task DeleteEstudanteFromTurmaAsync(string turmaId, string estudanteId)
        {
            await _turmasService.DeleteEstudanteAsync(turmaId, estudanteId);
        }

        // Método para criar DTOs completos com todos os dados necessários
        public async Task<List<DiarioComTurmaDto>> GetDiariosComTurmaCompletosAsync(string educatorName)
        {
            var turmas = await _turmasService.GetTurmasAsync();
            var diarios = turmas.SelectMany(t => t.Diarios)
                               .Where(d => d.ProfessorNome == educatorName)
                               .ToList();

            var diariosCompletos = new List<DiarioComTurmaDto>();

            foreach (var diario in diarios)
            {
                var turma = turmas.FirstOrDefault(t => t.Id == diario.TurmaId);
                if (turma != null)
                {
                    var diarioDto = new DiarioComTurmaDto
                    {
                        Id = diario.Id,
                        TurmaId = diario.TurmaId,
                        TurmaNome = turma.Nome,
                        TurmaSerie = turma.Serie,
                        Disciplina = diario.Disciplina,
                        ProfessorNome = diario.ProfessorNome,
                        NumeroEstudantes = turma.Estudantes?.Count ?? 0,
                        TurmaStudents = turma.Estudantes?.OrderBy(e => e.Nome).ToList() ?? new List<Estudante>(),
                        TurmaHorarios = turma.Horarios ?? new List<Horario>(),
                        Registros = diario.Registros ?? new Dictionary<DateTime, string>()
                    };
                    diariosCompletos.Add(diarioDto);
                }
            }

            return diariosCompletos;
        }
    }
}