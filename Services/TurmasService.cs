using DiarioEscolar.Dtos;
using DiarioEscolar.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiarioEscolar.Services
{
    public class TurmasService
    {
        private readonly LocalStorageService _localStorage;

        public TurmasService(LocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        // Operações básicas de Turmas
        public async Task<List<Turma>> GetTurmasAsync()
        {
            return await _localStorage.GetTurmasAsync();
        }

        public async Task<Turma?> GetTurmaByIdAsync(string id)
        {
            var turmas = await GetTurmasAsync();
            return turmas.FirstOrDefault(t => t.Id == id);
        }

        public async Task AddTurmaAsync(Turma turma)
        {
            var turmas = await GetTurmasAsync();
            turmas.Add(turma);
            await _localStorage.SaveTurmasAsync(turmas);
        }

        public async Task UpdateTurmaAsync(Turma turma)
        {
            await _localStorage.UpdateTurmaAsync(turma);
        }

        public async Task DeleteTurmaAsync(string id)
        {
            await _localStorage.DeleteTurmaAsync(id);
        }

        // Operações de Estudantes
        public async Task AddEstudanteAsync(string turmaId, string nomeEstudante)
        {
            var turmas = await GetTurmasAsync();
            var turma = turmas.FirstOrDefault(t => t.Id == turmaId);
            if (turma != null && !string.IsNullOrWhiteSpace(nomeEstudante))
            {
                var estudante = new Estudante
                {
                    Nome = nomeEstudante.Trim(),
                    TurmaId = turmaId
                };

                turma.Estudantes.Add(estudante);
                turma.Estudantes = turma.Estudantes.OrderBy(e => e.Nome).ToList();
                await UpdateTurmaAsync(turma);
            }
        }

        public async Task UpdateEstudanteAsync(string turmaId, string estudanteId, string novoNome)
        {
            var turmas = await GetTurmasAsync();
            var turma = turmas.FirstOrDefault(t => t.Id == turmaId);
            if (turma != null)
            {
                var estudante = turma.Estudantes.FirstOrDefault(e => e.Id == estudanteId);
                if (estudante != null)
                {
                    estudante.Nome = novoNome.Trim();
                    turma.Estudantes = turma.Estudantes.OrderBy(e => e.Nome).ToList();
                    await UpdateTurmaAsync(turma);
                }
            }
        }

        public async Task DeleteEstudanteAsync(string turmaId, string estudanteId)
        {
            var turmas = await GetTurmasAsync();
            var turma = turmas.FirstOrDefault(t => t.Id == turmaId);
            if (turma != null)
            {
                var estudante = turma.Estudantes.FirstOrDefault(e => e.Id == estudanteId);
                if (estudante != null)
                {
                    turma.Estudantes.Remove(estudante);
                    await UpdateTurmaAsync(turma);
                }
            }
        }

        // Operações de Horários
        public async Task AddHorarioAsync(string turmaId, NovoHorarioDto horarioDto)
        {
            var turmas = await GetTurmasAsync();
            var turma = turmas.FirstOrDefault(t => t.Id == turmaId);
            if (turma != null)
            {
                var horario = new Horario
                {
                    DiaSemana = int.Parse(horarioDto.DiaSemana),
                    HoraInicio = horarioDto.HoraInicio,
                    HoraFim = horarioDto.HoraFim,
                    Disciplina = horarioDto.Disciplina,
                    Professor = horarioDto.Professor,
                    TurmaId = turmaId
                };

                turma.Horarios.Add(horario);
                await UpdateTurmaAsync(turma);
            }
        }

        public async Task UpdateHorarioAsync(string turmaId, string horarioId, NovoHorarioDto horarioDto)
        {
            var turmas = await GetTurmasAsync();
            var turma = turmas.FirstOrDefault(t => t.Id == turmaId);
            if (turma != null)
            {
                var horario = turma.Horarios.FirstOrDefault(h => h.Id == horarioId);
                if (horario != null)
                {
                    horario.DiaSemana = int.Parse(horarioDto.DiaSemana);
                    horario.HoraInicio = horarioDto.HoraInicio;
                    horario.HoraFim = horarioDto.HoraFim;
                    horario.Disciplina = horarioDto.Disciplina;
                    horario.Professor = horarioDto.Professor;
                    await UpdateTurmaAsync(turma);
                }
            }
        }

        public async Task DeleteHorarioAsync(string turmaId, string horarioId)
        {
            var turmas = await GetTurmasAsync();
            var turma = turmas.FirstOrDefault(t => t.Id == turmaId);
            if (turma != null)
            {
                var horario = turma.Horarios.FirstOrDefault(h => h.Id == horarioId);
                if (horario != null)
                {
                    turma.Horarios.Remove(horario);
                    await UpdateTurmaAsync(turma);
                }
            }
        }

        // Operações de Diários
        public async Task AddDiarioAsync(string turmaId, NovoDiarioDto diarioDto)
        {
            var turmas = await GetTurmasAsync();
            var turma = turmas.FirstOrDefault(t => t.Id == turmaId);
            if (turma != null)
            {
                var diario = new Diario
                {
                    ProfessorNome = diarioDto.ProfessorNome,
                    Disciplina = diarioDto.Disciplina,
                    TurmaId = turmaId,
                    TurmaNome = turma.Nome
                };

                turma.Diarios.Add(diario);
                await UpdateTurmaAsync(turma);
            }
        }

        public async Task UpdateDiarioAsync(string turmaId, string diarioId, NovoDiarioDto diarioDto)
        {
            var turmas = await GetTurmasAsync();
            var turma = turmas.FirstOrDefault(t => t.Id == turmaId);
            if (turma != null)
            {
                var diario = turma.Diarios.FirstOrDefault(d => d.Id == diarioId);
                if (diario != null)
                {
                    diario.ProfessorNome = diarioDto.ProfessorNome;
                    diario.Disciplina = diarioDto.Disciplina;
                    await UpdateTurmaAsync(turma);
                }
            }
        }

        public async Task DeleteDiarioAsync(string turmaId, string diarioId)
        {
            var turmas = await GetTurmasAsync();
            var turma = turmas.FirstOrDefault(t => t.Id == turmaId);
            if (turma != null)
            {
                var diario = turma.Diarios.FirstOrDefault(d => d.Id == diarioId);
                if (diario != null)
                {
                    turma.Diarios.Remove(diario);
                    await UpdateTurmaAsync(turma);
                }
            }
        }

        // Operações de Professores (para autocomplete)
        public async Task<List<string>> GetProfessoresAsync()
        {
            var turmas = await GetTurmasAsync();
            var professoresUnicos = new HashSet<string>();

            foreach (var turma in turmas)
            {
                foreach (var horario in turma.Horarios)
                {
                    if (!string.IsNullOrWhiteSpace(horario.Professor))
                    {
                        professoresUnicos.Add(horario.Professor);
                    }
                }

                foreach (var diario in turma.Diarios)
                {
                    if (!string.IsNullOrWhiteSpace(diario.ProfessorNome))
                    {
                        professoresUnicos.Add(diario.ProfessorNome);
                    }
                }
            }

            return professoresUnicos.OrderBy(p => p).ToList();
        }

        public async Task<Dictionary<string, string>> GetProfessoresDisciplinasAsync()
        {
            var turmas = await GetTurmasAsync();
            var professoresDisciplinas = new Dictionary<string, string>();

            foreach (var turma in turmas)
            {
                foreach (var horario in turma.Horarios)
                {
                    if (!string.IsNullOrWhiteSpace(horario.Professor) && 
                        !professoresDisciplinas.ContainsKey(horario.Professor))
                    {
                        professoresDisciplinas[horario.Professor] = horario.Disciplina;
                    }
                }

                foreach (var diario in turma.Diarios)
                {
                    if (!string.IsNullOrWhiteSpace(diario.ProfessorNome) && 
                        !professoresDisciplinas.ContainsKey(diario.ProfessorNome))
                    {
                        professoresDisciplinas[diario.ProfessorNome] = diario.Disciplina;
                    }
                }
            }

            return professoresDisciplinas;
        }

        // Métodos auxiliares
        public async Task<List<string>> FilterProfessoresAsync(string input)
        {
            var professores = await GetProfessoresAsync();
            
            if (string.IsNullOrWhiteSpace(input))
            {
                return professores;
            }

            return professores
                .Where(p => p.Contains(input, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}