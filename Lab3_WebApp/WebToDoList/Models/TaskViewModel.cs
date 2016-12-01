using MessageEncryptionService.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebToDoList.Models
{
    public class TaskViewModel
    {
        //public string ExecutorLogin { get; set; }
        //public string ExecutorFullName { get; set; }
        public Guid? TaskId { get; set; }
        public string Title { get; set; }
        public Guid? TagId { get; set; }
        public string TagName { get; set; }
        public int EstimatedDifficulty { get; set; }        
        public string Description { get; set; }
        public DateTime EstimatedCompletionDate { get; set; }
    }

    public static class TaskModelExtention
    {
        public static TaskViewModel ToViewModel(this PlannedTask entity)
        {
            return new TaskViewModel()
            {
                TaskId = entity.Id,
                Title = entity.Title,
                Description = entity.Description,
                EstimatedCompletionDate = entity.EstimatedCompletionDate,
                EstimatedDifficulty = entity.EstimatedDifficulty,
                TagId = entity?.Tag.Id,
                TagName = entity?.Tag.Name
            };
        }
    }
}