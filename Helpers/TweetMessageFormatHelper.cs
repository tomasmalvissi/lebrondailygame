using LBJ.Models.Nba;
using System.Globalization;
using System.Text;

namespace LBJ.Helpers;

public static class TweetMessageFormatHelper
{
    public static string FormatResponseForTweet(Game match)
    {
        var builder = new StringBuilder();

        builder.AppendLine("👑 ¡Hoy juegan los Lakers de LeBron! 👑");

        builder.AppendLine();

        builder.AppendLine($"🏀 {match.HomeTeam.FullName} vs {match.VisitorTeam.FullName}");

        builder.AppendLine($"📅 {DateFormatter(match.Datetime)}");

        return builder.ToString();
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
}
