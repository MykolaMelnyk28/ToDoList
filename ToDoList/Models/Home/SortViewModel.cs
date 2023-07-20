using ToDoList.Shared;

namespace ToDoList.Models.Home
{
	public class SortViewModel
	{
		public TaskFields CurrentState { get; set; } = TaskFields.Name;
        public bool AscSort { get; set; } = true;
        public SortViewModel(TaskFields field, bool ascSort)
        {
            this.CurrentState = field;
            this.AscSort = ascSort;
        }
    }
}
