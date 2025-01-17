using ZorgRobotWebApp.Services.Datainterface;

namespace ZorgRobotWebApp.Services.AgendaManager
{
    public class TaskUtil
    {

        private readonly SqlTaskRepo sqlTaskRepo;


        public TaskUtil(SqlTaskRepo SqlTaskRepo)
        {
            sqlTaskRepo = SqlTaskRepo;
        }

        public List<AgendaTask> AddTaskToList(List<AgendaTask> agendaTasks, AgendaTask agendaTask)
        {
            var _agendaTasks = agendaTasks;

            _agendaTasks.Add(agendaTask);

            sqlTaskRepo.SaveTask(agendaTask);

            return _agendaTasks;
        }
    }
}
