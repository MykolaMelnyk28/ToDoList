using ToDoList.Shared.Entity;

namespace ToDoList.Servicies
{
    public interface ITaskService
    {
        Task<bool> CreateTaskAsync(TaskEntity t);
        Task<bool> CreateTaskAsync(TaskEntity t, UserEntity u);
        Task<bool> DeleteTaskAsync(TaskEntity t);
        Task<bool> DeleteTaskByIdAsync(int id);
        Task<bool> UpdateTaskAsync(TaskEntity t);
        IEnumerable<TaskEntity> GetTasks();
        IEnumerable<TaskEntity> GetTasksByUserId(int userId);
        IEnumerable<TaskEntity> GetTasks(Func<TaskEntity, bool> p);
        TaskEntity? FirstOrDefalut(Func<TaskEntity, bool> p);
    }
}
