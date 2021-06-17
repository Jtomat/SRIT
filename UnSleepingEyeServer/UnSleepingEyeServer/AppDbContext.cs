using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using UnSleepingEyeServer.Support;
using System.Collections;
using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace UnSleepingEyeServer
{
    public class AppDbContext: DbContext
    {
        public static AppDbContext Connect;
        private DbSet<Task> Tasks;
        private DbSet<Stage> Stages;
        private DbSet<WorkerRole> WorkerRoles;
        private DbSet<OperationReport> OperationsReports;
        private DbSet<Project> Projects;
        private DbSet<Worker> Workers;
        private DbSet<AppUser> Users;
        private DbSet<ReportData> ReportDatas;
        public AppDbContext():
            base("DC") {
            Database.Connection.ConnectionString = "workstation id=DataStorage.mssql.somee.com;packet size=4096;user id=secondAcy_SQLLogin_1;pwd=2o8gynzbz3;data source=DataStorage.mssql.somee.com;persist security info=False;initial catalog=DataStorage";
            Tasks = Set<Task>();
            Stages = Set<Stage>();
            Projects = Set<Project>();
            Workers = Set<Worker>();
            WorkerRoles = Set<WorkerRole>();
            OperationsReports = Set<OperationReport>();
            Users = Set<AppUser>();
            ReportDatas = Set<ReportData>();
            if (Connect == null)
            {
                Connect = this;
            }
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<AppDbContext>(null);
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Task>().ToTable("Task");
            modelBuilder.Entity<Worker>().ToTable("Worker");
            modelBuilder.Entity<Project>().ToTable("Project");
            modelBuilder.Entity<WorkerRole>().ToTable("WorkerRole");
            modelBuilder.Entity<ReportData>().ToTable("ReportData");
            modelBuilder.Entity<AppUser>().ToTable("AppUser");
            modelBuilder.Entity<OperationReport>().ToTable("OperationReport");
        }
        public Project GetProjectByID(long id)
        {
            return Projects.FirstOrDefault(i => i.ID == id);
        }
        public AppUser GetAppUserByAuthPair(string login, string pass)
        {
            var hash = Keeper.Encrypt(pass);
            return Users.FirstOrDefault(i=>i.Email == login && i.PasswordHash == hash);
        }
        public WorkerRole[] GetWorkerRolesByID(string id) 
        {
            var prep = Workers.Where(i => i.ID_user.ToString() == id).ToArray();
            var result = new List<WorkerRole>();
            foreach(var pr in prep)
            {
                result.Add(pr.WorkerRole);
            }
            return result.ToArray();
        }
        public Task CreateTask(int worker, int stage, string name,
            string info )
        {
            try
            {
                var pr = new Task()
                {
                    Name = name,
                    Info = info,
                    ID_Stage = stage,
                    ID_Worker = worker,
                };
                Tasks.Add(pr);
                SaveChanges();
                return pr;
            }
            catch
            {
                return null;
            }
        }
        public Project CreateProject(string name, string info) 
        {
            try
            {
                var pr = new Project()
                {
                    Name = name,
                    Info = info,
                };
                Projects.Add(pr);
                SaveChanges();
                return pr;
            }
            catch
            {
                return null;
            }
        }
        public Stage CreateStage(long idPr, string name,
            string dateS, string dateE) 
        {
            try
            {
                var st = new Stage()
                {
                    Name = name,
                    ID_Project = idPr,
                    Date_Start = DateTime.Parse(dateS),
                    Date_End = DateTime.Parse(dateE),
                };
                Stages.Add(st);
                SaveChanges();
                var opR = new OperationReport()
                {
                    ID_Stage = st.ID,
                    Type = "INFO",
                    Info = "Данные о ходе работ",
                    Date = DateTime.Now
                };
                SaveChanges();
                opR.ReportData.Add(new ReportData() {
                    ID_Report = opR.ID,
                    Name = "%",
                    Value = "0"
                });
                SaveChanges();
                return st;
            }
            catch
            {
                return null;
            }
        }
        public bool UpdStage(long id, string name,
            string dateS, string dateE)
        {
            try
            {
                var st = Stages.FirstOrDefault(i=>i.ID==id);
                st.Name = name;
                    st.Date_Start = DateTime.Parse(dateS);
                    st.Date_End = DateTime.Parse(dateE);
                SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public OperationReport CreateExceptionReport(Stage cur_stage, string doc)
        {
            var rep = CreateOperationReport(cur_stage, "ERROR",
                $"Ошибка при выполнении задачи на стадии {cur_stage.Name} проекта {cur_stage.Project.Name}.",
                doc);
            var fromTask = new ReportData()
            {
                Name = "Solution",
                Value = null
            };
            rep = UpdateOperationReportData(rep, new List<ReportData>() { fromTask });
            return rep;
        }
        public OperationReport AddSolution(OperationReport report, Task solution)
        {
            var fromTask = new ReportData()
            {
                Name = "Solution",
                Value = solution.ID.ToString()
            };
            return UpdateOperationReportData(report, new List<ReportData>() { fromTask });
        }
        public OperationReport UpdateOperationReportData(OperationReport currentOpRep, IEnumerable<ReportData> toUpd)
        {
            var sourse = OperationsReports.FirstOrDefault(i => i.ID == currentOpRep.ID);
            if (sourse == null)
            {
                OperationsReports.Add(currentOpRep);
                SaveChanges();
                sourse = OperationsReports.FirstOrDefault(i => i.ID == currentOpRep.ID);
            }
            sourse.Date = DateTime.Now;
            foreach (var record in toUpd)
            {
                var nowInfo = sourse.ReportData.FirstOrDefault(i => i.Name == record.Name);
                if (nowInfo != null)
                {
                    nowInfo.Value = record.Value;
                }
                else
                {
                    record.ID_Report = sourse.ID;
                    ReportDatas.Add(record);
                }
            }
            SaveChanges();
            return currentOpRep;
        }
        public Task FihishTask(long id) {
            var t = Tasks.FirstOrDefault(i=>i.ID == id);
            t.Finished = true;
            t.Act_Date = DateTime.Now;
            var rep = t.Stage.OperationReport.FirstOrDefault(i=>i.Type=="INFO");
            var newRD = rep.ReportData.FirstOrDefault(i=>i.Name=="%");
            newRD.Value = Math.Round((100.0 * t.Stage.Task.Count /
                (Stages.Where(i => ((bool)i.Finished) && i.ID == t.ID_Stage)).Count()), 3).ToString();
            rep.Date = DateTime.Now;
            SaveChanges();
            return t;
        }
        public OperationReport CreateOperationReport(Stage cur_task, string type, string info, string doc)
        {
            var bytes = Convert.FromBase64String(doc);
            var keywords = (new DataAnalyzer()).
                GetKeyWordsForText(FileFormat.GetContentFrom(bytes,
                FileFormat.GetMimeFromBytes(bytes)[1]));
            var keysData = "";
            foreach (var keysRow in keywords)
            {
                keysData += "[";
                foreach (var key in keysRow)
                {
                    keysData += key + " ";
                }
                keysData = keysData.Trim() + "]";
            }
            var newRep = new OperationReport()
            {
                ID_Stage = cur_task.ID,
                Type = type,
                Info = info,
                Date = DateTime.Now,
                Doc = Convert.FromBase64String(doc)
            };
            OperationsReports.Add(newRep);
            SaveChanges();

            newRep.ReportData.Add(new ReportData() { Name = "Keywords", Value = keysData });
            SaveChanges();

            return newRep;
        }
        private object UpdateDataTo(object toUpd, object current)
        {
            var props = current.GetType().GetProperties();
            foreach (var pr in props)
            {
                if (pr.PropertyType == typeof(string) ||
                    !typeof(IEnumerable).IsAssignableFrom(pr.PropertyType))
                {
                    if (pr.GetValue(current).GetHashCode() !=
                        pr.GetValue(toUpd).GetHashCode())
                    {
                        pr.SetValue(current, pr.GetValue(toUpd));
                    }
                }
            }
            return current;
        }
        public Stage FinishStage(long id) 
        {
            var stage = Stages.FirstOrDefault(i=>i.ID == id);
            if (stage == null)
                return null;
            stage.Finished = true;
            stage.Actual_Date_End = DateTime.Now;
            var rep = stage.OperationReport.FirstOrDefault(i=>i.Type=="INFO");
            var endProp = rep.ReportData.FirstOrDefault(i=>i.Name=="%");
            endProp.Value = "100";
            UpdateOperationReportData(rep, new List<ReportData>(){endProp});
            SaveChanges();
            return stage;
        }
        public AppUser CreateUser(string login, string pass, string FIO)
        {
            if (Users.FirstOrDefault(i => i.Email == login) != null)
                return null;
            var hash = Keeper.Encrypt(pass);
            var newUser = new AppUser() {Email = login, FIO = FIO, PasswordHash = hash};
            Users.Add(newUser);
            SaveChanges();
            return newUser;
        }
        public Task GetTaskByID(long id)
        {
            return Tasks.FirstOrDefault(i => i.ID == id);
        }
        public Task GetTasksForStage(long id)
        {
            return Tasks.FirstOrDefault(i => i.ID_Stage == id);
        }
        public Stage GetStageByID(long id)
        {
            return Stages.FirstOrDefault(i => i.ID == id);
        }
        public ReportData GetReportDataByID(long id)
        {
            return ReportDatas.FirstOrDefault(i => i.ID == id);
        }
        public ReportData GetReportDataByNameFor(string name, OperationReport current_rep)
        {
            return ReportDatas.FirstOrDefault(i => i.Name == name && i.ID_Report == current_rep.ID);
        }
        public AppUser GetUserByID(string id)
        {
            return Users.FirstOrDefault(i => i.ID.ToString() == id);
        }
        public Project[] GetAllProjects()
        {
            return Projects.ToArray();
        }
        public bool RemoveProject(long id) 
        {
            try
            {
                Projects.Remove(Projects.FirstOrDefault(i=>i.ID==id));
                SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool RemoveStage(long id)
        {
            try
            {
                Stages.Remove(Stages.FirstOrDefault(i => i.ID == id));
                SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Список задач для пользователя по id
        /// </summary>
        /// <param name="ID">ID пользоваеля (AspNetUsers.Id)</param>
        /// <returns>Dictionary<int, List<Task>>, int - приоритет, List<Task> - список задач.</returns>
        public Dictionary<int, List<Task>> GetTasksWithPriorityFor(string ID)
        {
            var worker = Workers.Where(i => i.ID_user.ToString() == ID).ToArray();
            var result = new Dictionary<int, List<Task>>();
            foreach (var w in worker)
            {
                var data = GetTasksWithPriority(w.ID);
                foreach (var p in data)
                    if (result.ContainsKey(p.Key))
                    {
                        foreach (var d in p.Value)
                            result[p.Key].Add(d);
                    }
                    else
                        result.Add(p.Key, p.Value);
            }
            result = result.OrderBy(g => g.Key).ToDictionary(g => g.Key, g => g.Value);
            return result;
        }
        /// <summary>
        /// Список задач для сотрудника по id должнисти
        /// </summary>
        /// <param name="id_worker">ID сотрудника (Worker.ID)</param>
        /// <returns>Dictionary<int, List<Task>>, int - приоритет, List<Task> - список задач.</returns>
        private Dictionary<int, List<Task>> GetTasksWithPriority(long id_worker)
        {
            var unsorted = Tasks.Where(i => i.Finished != true && i.ID_Worker == id_worker).ToList();
            var sorted = unsorted.GroupBy(i => ((i.Date_End - DateTime.Now).Value.Days) % 3);
            return sorted.ToDictionary(g => g.Key, g => g.ToList());
        }
        public void AddWorkerRole(WorkerRole newRole)
        {
            WorkerRoles.Add(newRole);
            SaveChanges();
        }

        public string[][] getExcelStream()
        {
            var result = new List<string[]>();
            result.Add(new string[]{ "Проект", "Стадии", "Задачи","Сотрудник", "%"});
            foreach(var pr in GetAllProjects())
            {
                foreach(var st in pr.Stage)
                {
                    foreach(var task in st.Task)
                    {
                        result.Add(new string[] { pr.Name, st.Name, task.Name, task.Worker != null ? task.Worker.User.FIO : "Отсутствует", task.Finished.Value ? "100" : "0" });
                    }
                }
            }
            return result.ToArray();
     
        }

        public bool updProject(long id, string name, string info) 
        {
            try 
            {
                var pr = Projects.FirstOrDefault(i=>i.ID==id);
                pr.Name = name;
                pr.Info = info;
                SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }



        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
