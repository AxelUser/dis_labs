using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using MessageEncryptionService.Model;
using System.Data.Entity;
using System.Linq.Expressions;

namespace WebToDoList.Models
{
    public class DALService
    {
        public async Task<List<TaskViewModel>> GetAllTasks()
        {
            using(TaskManagerContext db = new TaskManagerContext())
            {
                List<TaskViewModel> models = new List<TaskViewModel>();
                var tasks = await db.PlannedTasks
                    .Include(t=>t.Tag)
                    .ToListAsync();
                return tasks.Select(t => t.ToViewModel()).ToList();
            }
        }

        public async Task AddRecord(TaskViewModel model)
        {
            using (TaskManagerContext db = new TaskManagerContext())
            {
                PlannedTask task = new PlannedTask()
                {
                    Title = model.Title,
                    Description = model.Description,
                    CreationDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    EstimatedCompletionDate = model.EstimatedCompletionDate,
                    EstimatedDifficulty = model.EstimatedDifficulty
                };

                var tagQuery = db.Tags.AsQueryable();
                if(model.TagId != null)
                {
                    tagQuery = tagQuery.(tg => tg.Id == model.TagId);
                }
                else
                {
                    tagQuery = tagQuery.Where(tg => tg.Name == model.TagName);
                }
                var tag = await db.Tags.SingleOrDefaultAsync();                  
                if (tag == null)
                {
                    tag = new Tag()
                    {
                        Name = model.TagName
                    };
                }
                task.Tag = tag;

                //Hardcoded for speed
                task.Executor = new User()
                {
                    FullName = "Alexey Maltsev",
                    Login = "axeluser"
                };

                db.PlannedTasks.Add(task);
                await db.SaveChangesAsync();
            }
        }

        public async Task<List<string>> GetAllTags()
        {
            using(TaskManagerContext db = new TaskManagerContext())
            {
                return await db.Tags.Select(tg=>tg.Name).ToListAsync();
            }
        }
    }
}