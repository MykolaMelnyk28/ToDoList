using ToDoList.Shared.Entity;

namespace ToDoList.Models.Home
{
    public class IndexViewModel
    {   
        public IEnumerable<TaskEntity> Tasks { get; set; }
        public string SortBy { get; set; }
        public bool IsDescending { get; set; }
        public string SearchBy { get; set; }
		public string SearchValue { get; set; }

		
		public IndexViewModel(IEnumerable<TaskEntity> tasks, string sortBy = "Name", bool isDescending = false, string searchBy = "Name", string searchValue = "")
		{
			this.SortBy = sortBy;
			this.IsDescending = isDescending;
			this.SearchBy = searchBy;
			this.SearchValue = searchValue;
            this.Tasks = Sort(Search(tasks)).ToList();
		}

        public IndexViewModel() : this(new List<TaskEntity>()) { }

        public IEnumerable<TaskEntity> Sort(IEnumerable<TaskEntity> tasks)
        {
            IEnumerable<TaskEntity> sortedTasks = null;

            switch(SortBy)
            {
                case "Priority":
                    sortedTasks = IsDescending ? tasks.OrderByDescending(t => t.PriorityId) : tasks.OrderBy(t => t.PriorityId);
                    break;
                case "State":
                    sortedTasks = IsDescending ? tasks.OrderByDescending(t => t.StateId) : tasks.OrderBy(t => t.StateId);
                    break;
                default:
                    sortedTasks = IsDescending ? tasks.OrderByDescending(t => t.Name) : tasks.OrderBy(t => t.Name);
                    break;
            }

            return sortedTasks;
        }

        public IEnumerable<TaskEntity> Search(IEnumerable<TaskEntity> tasks)
        {
            IEnumerable<TaskEntity> foundTasks = null;

            if(!string.IsNullOrWhiteSpace(SearchValue))
            {
                switch(SearchBy)
                {
                    case "Priority":
                        foundTasks = tasks.Where(x => x.Priority.Name.StartsWith(SearchValue));
                        break;
                    case "State":
                        foundTasks = tasks.Where(x => x.State.Name.StartsWith(SearchValue));
                        break;
                    default:
                        foundTasks = tasks.Where(x => x.Name.StartsWith(SearchValue));
                        break;
                }

                return foundTasks;
            }

            return tasks;
        }
    }
}
