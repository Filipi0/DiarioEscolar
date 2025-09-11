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
