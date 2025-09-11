namespace DiarioEscolar.Models;

public class Estudante
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Nome { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public string Email { get; set; } = string.Empty;
    public string TurmaId { get; set; } = string.Empty;
}

public class Horario
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int DiaSemana { get; set; } // 0=Domingo, 1=Segunda, etc.
    public string HoraInicio { get; set; } = string.Empty;
    public string HoraFim { get; set; } = string.Empty;
    public string Disciplina { get; set; } = string.Empty;
    public string Professor { get; set; } = string.Empty;
    public string TurmaId { get; set; } = string.Empty;
}

public class Diario
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ProfessorId { get; set; } = Guid.NewGuid().ToString();
    public string ProfessorNome { get; set; } = string.Empty;
    public string Disciplina { get; set; } = string.Empty;
    public string TurmaId { get; set; } = string.Empty;
    public Dictionary<DateTime, string> Registros { get; set; } = new();
}

public class Turma
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Nome { get; set; } = string.Empty;
    public string Serie { get; set; } = string.Empty;
    public List<Estudante> Estudantes { get; set; } = new();
    public List<Diario> Diarios { get; set; } = new();
    public List<Horario> Horarios { get; set; } = new();
}

public enum ViewType
{
    Landing,
    Escola,
    Turmas,
    Diario
}
