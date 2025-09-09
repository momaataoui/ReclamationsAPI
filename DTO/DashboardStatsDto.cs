// Dans DTO/DashboardStatsDto.cs
namespace ReclamationsAPI.DTO
{
    public class StatSummary
    {
        public string StatutNom { get; set; }
        public int Count { get; set; }
    }

    public class DailyTrend
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }

    public class DashboardStatsDto
    {
        public int TotalReclamations { get; set; }
        public int EnCours { get; set; }
        public int Resolues { get; set; }
        public List<StatSummary> RepartitionParStatut { get; set; }
        public List<DailyTrend> Tendances7Jours { get; set; }
    }
}