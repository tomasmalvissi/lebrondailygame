using LBJ.Models.Nba;
using System.Globalization;
using System.Text;

namespace LBJ.Helpers;

public static class TweetMessageFormatHelper
{
    public static string FormatResponseForNewGame(Game match)
    {
        var builder = new StringBuilder();

        builder.AppendLine("👑 ¡Hoy juegan los Lakers de LeBron! 👑");

        builder.AppendLine();

        builder.AppendLine($"🏀 {match.HomeTeam.FullName} vs {match.VisitorTeam.FullName}");

        builder.AppendLine($"📅 {DateFormatter(match.Datetime)}");

        return builder.ToString();
    }

    public static string FormatResponseForFinishGame(Game match)
    {
        bool lakersWon = DidLosLakersWin(match);

        var builder = new StringBuilder();
        builder.AppendLine(lakersWon ? "👑 ¡WIN WIN WIN! 👑" : "Perdimos...");
        builder.AppendLine();

        var (winner, winnerScore, loser, loserScore) =
            match.HomeTeamScore > match.VisitorTeamScore
            ? (match.HomeTeam.FullName, match.HomeTeamScore, match.VisitorTeam.FullName, match.VisitorTeamScore)
            : (match.VisitorTeam.FullName, match.VisitorTeamScore, match.HomeTeam.FullName, match.HomeTeamScore);

        builder.AppendLine($"🏀 {winner} {winnerScore} - {loser} {loserScore}");
        builder.AppendLine();
        builder.AppendLine(lakersWon ? "DAAAALE LAKER" : "Nos vemos el próximo partido. GANEN");

        return builder.ToString();
    }

    private static bool DidLosLakersWin(Game match)
    {
        string lakersAbbreviation = "LAL";
        string homeTeam = match.HomeTeam.Abbreviation;
        string visitorTeam = match.VisitorTeam.Abbreviation;

        int homeScore = match.HomeTeamScore;
        int visitorScore = match.VisitorTeamScore;

        bool lakersWon = (homeTeam == lakersAbbreviation && homeScore > visitorScore) ||
                         (visitorTeam == lakersAbbreviation && visitorScore > homeScore);

        return lakersWon;
    }

    private static string DateFormatter(string datetime)
    {
        DateTime utcDateTime = DateTime.Parse(datetime, null, DateTimeStyles.RoundtripKind);

        TimeZoneInfo argentinaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Argentina/Buenos_Aires");

        DateTime argentinaDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, argentinaTimeZone);

        var culture = new CultureInfo("es-ES");

        string dayName = culture.TextInfo.ToTitleCase(argentinaDateTime.ToString("dddd", culture));
        string dayNumber = argentinaDateTime.Day.ToString();
        string monthName = argentinaDateTime.ToString("MMMM", culture).ToLower();
        string time = argentinaDateTime.ToString("HH:mm", culture);

        string formattedDate = $"{dayName} {dayNumber} de {monthName} a las {time}";

        return formattedDate;
    }

    public static string NormalizeTextForComparison(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        string normalized = text.Replace("\r", "").Replace("\n", "");
        normalized = System.Text.RegularExpressions.Regex.Replace(normalized, @"\s+", " ");
        normalized = normalized.Trim();

        return normalized;
    }
}
