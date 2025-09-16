using DiarioEscolar.Models;

namespace DiarioEscolar.Dtos;

public class NovoHorarioDto
{
    public string DiaSemana { get; set; } = string.Empty;
    public string HoraInicio { get; set; } = string.Empty;
    public string HoraFim { get; set; } = string.Empty;
    public string Disciplina { get; set; } = string.Empty;
    public string Professor { get; set; } = string.Empty;
}

public class NovoDiarioDto
{
    public string ProfessorNome { get; set; } = string.Empty;
    public string Disciplina { get; set; } = string.Empty;
}

public class DiarioComTurmaDto
{
    public string Id { get; set; } = string.Empty;
    public string TurmaId { get; set; } = string.Empty;
    public string TurmaNome { get; set; } = string.Empty;
    public string TurmaSerie { get; set; } = string.Empty;
    public string Disciplina { get; set; } = string.Empty;
    public string ProfessorNome { get; set; } = string.Empty;
    public int NumeroEstudantes { get; set; }
    
    // Dados completos para uso no DiarioDetail
    public List<Estudante> TurmaStudents { get; set; } = new();
    public List<Horario> TurmaHorarios { get; set; } = new();
    public Dictionary<DateTime, string> Registros { get; set; } = new();
}
