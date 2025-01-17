using System;
using Microsoft.Data.SqlClient;
using ZorgRobotWebApp.Services.AgendaManager;

namespace ZorgRobotWebApp.Services.Datainterface;

public class SqlTaskRepo
{
    private readonly SqlInterface sqlInterface;

    public SqlTaskRepo(SqlInterface SqlInterface)
    {
        sqlInterface = SqlInterface;
    }

    public List<AgendaTask> GetAllTasks()
    {
        return sqlInterface.GetListOfData<AgendaTask>("", "[Title]");
    }

    public void SaveTask(AgendaTask task)
    {
        sqlInterface.SaveData("", task);
    }
}
