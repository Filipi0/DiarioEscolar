using System;

namespace DiarioEscolar.Dtos
{
    public class DiaryRecordDto
    {
        public DateTime Date { get; set; }
        public string Content { get; set; } = string.Empty;
        public string Observations { get; set; } = string.Empty;
    }
}