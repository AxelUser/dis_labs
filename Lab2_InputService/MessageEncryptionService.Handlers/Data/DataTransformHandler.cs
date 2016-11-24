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
        public static TaskInfoViewModel[] FromXML(string xml)
        {
            if (!string.IsNullOrEmpty(xml))
            {
                var formatter = new XmlSerializer(typeof(TaskInfoViewModel[]));
                using (StringReader sr = new StringReader(xml))
                {
                    return (TaskInfoViewModel[])formatter.Deserialize(sr);
                }
            }
            return new TaskInfoViewModel[0];
        }

        public static string ToXML(TaskInfoViewModel[] model)
        {
            var formatter = new XmlSerializer(typeof(TaskInfoViewModel[]));
            using (StringWriter sw = new StringWriter())
            {
                formatter.Serialize(sw, model);
                return sw.ToString();
            }
        }

        public static async Task SaveToDb(IEnumerable<TaskInfoViewModel> models)
        {
            using(TaskManagerContext db = new TaskManagerContext())
            {
                foreach (var model in models)
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

                    var tag = await db.Tags.Include(t => t.PlannedTasks)
                        .SingleOrDefaultAsync(t => model.Tag == t.Name);
                    if(tag == null)
                    {
                        tag = new Tag()
                        {
                            Name = model.Tag
                        };
                    }
                    task.Tag = tag;

                    var user = await db.Users.SingleOrDefaultAsync(u => u.Login == model.ExecutorLogin);
                    if (user == null)
                    {
                        user = new User()
                        {
                            Login = model.ExecutorLogin,
                            FullName = model.ExecutorFullName
                        };
                    }
                    task.Executor = user;

                    db.PlannedTasks.Add(task);
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}
