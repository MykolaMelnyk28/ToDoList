using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoList.DB;
using ToDoList.Shared.Entity;

namespace ToDoList.Servicies
{
    public class TaskManager : ITaskService
    {
        private readonly ILogger<TaskManager> _logger;
        private readonly ApplicationContext _db;

        public TaskManager(ILogger<TaskManager> logger, ApplicationContext context)
        {
            _logger = logger;
            _db = context;
        }

        public async Task<bool> CreateTaskAsync(TaskEntity t)
        {
            NotNullOrThrow(t);
            try
            {
                if(!_db.Tasks.Contains(t))
                {
                    await _db.Tasks.AddAsync(t);
                    await _db.SaveChangesAsync();
                    return true;
                }
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, "An error occurred while creating a task.");
            }
            return false;
        }

        public async Task<bool> CreateTaskAsync(TaskEntity t, UserEntity u)
        {
            NotNullOrThrow(t);
            NotNullOrThrow(u);
            if (u.Id <= 0)
            {
                throw new ArgumentException(nameof(u.Id));
            }
            t.UserId = u.Id;
            return await CreateTaskAsync(t);
        }

        public async Task<bool> DeleteTaskAsync(TaskEntity t)
        {
            NotNullOrThrow(t);
            try
            {
                TaskEntity? task = _db.Tasks.FirstOrDefault(t);

                if(task == null)
                {
                    return false;
                } 

                _db.Tasks.Remove(task);
                await _db.SaveChangesAsync();
                return true;
            }
            catch(Exception ex) {
                _logger.LogError(ex, "An error occurred while deleting a task.");
                return false;
            }
        }

        public async Task<bool> DeleteTaskByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException(nameof(id));
            }
            try
            {
                TaskEntity? task = _db.Tasks.FirstOrDefault(x => x.Id == id);
                if(task == null)
                {
                    return false;
                }
                _db.Tasks.Remove(task);
                await _db.SaveChangesAsync();
                return true;
            }
            catch(Exception ex) {
                _logger.LogError(ex, "An error occurred while deleting a task by Id.");
                return false;
            }
        }

        public TaskEntity? FirstOrDefalut(Func<TaskEntity, bool> p)
        {
            try
            {
                return _db.Tasks
                .Include(x => x.State)
                .Include(x => x.Priority)
                .Include(x => x.User)
                .FirstOrDefault(p);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting a task.");
                return null;
            }
        }

        public IEnumerable<TaskEntity> GetTasks()
        {
            return GetTasks(x => true);
        }

        public IEnumerable<TaskEntity> GetTasks(Func<TaskEntity, bool> p)
        {
            try
            {
                return _db.Tasks
                .Include(x => x.State)
                .Include(x => x.Priority)
                .Include(x => x.User)
                .Where(p)
                .ToList();
            } 
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting tasks.");
                return Enumerable.Empty<TaskEntity>();
            }
        }

        public IEnumerable<TaskEntity> GetTasksByUserId(int userId)
        {
            if(userId <= 0)
            {
                throw new ArgumentException(nameof(userId));
            }
            return GetTasks(x => x.UserId == userId);
        }

        public async Task<bool> UpdateTaskAsync(TaskEntity t)
        {
            NotNullOrThrow(t);
            try
            {
                TaskEntity? task = await _db.Tasks.FirstOrDefaultAsync(x => x.Id == t.Id);

                if(task == null)
                {
                    return false;
                }

                _db.Entry(task).CurrentValues.SetValues(t);
                await _db.SaveChangesAsync();

                return true;
            }
            catch(Exception ex) {
                _logger.LogError(ex, "An error occurred while updating a task.");
                return false;
            }
        }

        private Object NotNullOrThrow(Object t)
        {
            if(t == null)
            {
                throw new ArgumentNullException(nameof(t));
            }
            return t;
        }
    }
}
