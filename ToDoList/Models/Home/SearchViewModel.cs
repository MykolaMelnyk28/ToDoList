using ToDoList.Shared;

namespace ToDoList.Models.Home
{
    public class SearchViewModel
    {
        public TaskFields SearchBy { get; set; }
        public string SearchValue { get; set; }

        public SearchViewModel(TaskFields searchBy, string searchValue)
        {
            this.SearchBy = searchBy;
            this.SearchValue = searchValue;
        }
    }
}
