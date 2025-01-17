using System;

namespace ZorgRobotWebApp.Services.AgendaManager;

public class AgendaTask
{
    public string Title { get; set; }
    public DateTime Date { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Interval { get; set; }

}
