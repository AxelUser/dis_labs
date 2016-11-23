using MessageEncryptionService.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Data.Entity;

namespace MessageEncryptionService.Handlers.Data
{
    public class DataTransformHandler
    {
        public static TaskInfoViewModel FromXML(string xml)
        {
            var formatter = new XmlSerializer(typeof(TaskInfoViewModel));
            using(StringReader sr = new StringReader(xml))
            {
                return (TaskInfoViewModel)formatter.Deserialize(sr);
            }
        }

        public static string ToXML(TaskInfoViewModel model)
        {
            var formatter = new XmlSerializer(typeof(TaskInfoViewModel));
            using (StringWriter sw = new StringWriter())
            {
                formatter.Serialize(sw, model);
                return sw.ToString();
            }
        }

        public static async void SaveToDb(TaskInfoViewModel model)
        {
            using(TaskManagerContext db = new TaskManagerContext())
            {
                PlannedTask task = new PlannedTask()
                {
                    Title = model.Title,
                    Description = model.Description,
                    CreationDate = model.CreationDate,
                    UpdateDate = model.UpdateDate,
                    EstimatedCompletionDate = model.EstimatedCompletionDate,
                    EstimatedDifficulty = model.EstimatedDifficulty
                };
                db.PlannedTasks.Add(task);
                await db.SaveChangesAsync();

                var existedTags = await db.Tags.Include(t=>t.PlannedTasks)
                    .Where(t => model.Tags.Contains(t.Name)).ToListAsync();
                var newTags = model.Tags.Where(t => !existedTags.Select(g => g.Name).Contains(t)).ToList();
                foreach(var tag in newTags)
                {
                    Tag newTag = new Tag()
                    {
                        Name = tag
                    };
                    newTag.PlannedTasks.Add(task);
                    db.PlannedTasks.Add(task);                    
                }
                var user = await db.Users.SingleOrDefaultAsync(u => u.Login == model.ExecutorLogin);
                //if (user != null)
                //{
                //    user = new User()
                //    {
                //        Login = 
                //    }
                //}
                db.SaveChanges();
            }
        }
    }
}
