namespace MyCompany.Visitors.Data.Infrastructure.Initializers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using MyCompany.Visitors.Model;
    using System.Configuration;

    /// <summary>
    /// The default initializer for context. You can learn more 
    /// about EF initializers here
    /// http://msdn.microsoft.com/en-us/library/gg696323(v=VS.103).aspx
    /// </summary>
    class MyCompanyContextInitializer :
        DropCreateDatabaseIfModelChanges<MyCompanyContext>
    {

        private static readonly Random _randomize = new Random();
        private static string tenant = ConfigurationManager.AppSettings["ida:Domain"];
        private string _picturePath = "FakeImages\\{0}.jpg";
        private string _smallPicturePath = "FakeImages\\{0} - small.jpg";
        private List<string> _employeeEmails = new List<string>() 
        {
            String.Format("Andrew.Davis@{0}", tenant),
            String.Format("Christen.Anderson@{0}", tenant),
            String.Format("David.Alexander@{0}", tenant),
            String.Format("Robin.Counts@{0}", tenant),
            String.Format("Thomas.Andersen@{0}", tenant),
            String.Format("Josh.Bailey@{0}", tenant),
            String.Format("Adam.Barr@{0}", tenant),
            String.Format("Christa.Geller@{0}", tenant),
            String.Format("Carole.Poland@{0}", tenant),
            String.Format("Cristina.Potra@{0}", tenant),
        };
        private List<string> _employeeNames = new List<string>() 
        {
            "Andrew Davis",
            "Christen Anderson", 
            "David Alexander",
            "Robin Counts",
            "Thomas Andersen", 
            "Josh Bailey", 
            "Adam Barr",
            "Christa Geller",
            "Carole Poland",
            "Cristina Potra",
        };

        private List<string> _visitorNames = new List<string>()
        {
            "Scott Hunter",
            "Brian Moore",
            "Scott Woodgate",
            "Craig Kitterman",
            "Violeta Arroyo",
            "Phil Webb",
            "Rob Caron",
            "Matt Mostad",
            "Adriana Bors",
            "Beth Massi",
            "Carlos Zimmermann",
            "Mitra Azizirad"
        };

        /// <summary>
        /// Seed
        /// </summary>
        /// <param name="context"></param>
        protected override void Seed(MyCompanyContext context)
        {
            CreateTeamManagers(context);

            CreateEmployees(context, 2);

            CreateTeamManagersNYC(context);
            CreateEmployeesForNYCTeams(context);

            CreateEmployeePictures(context);

            CreateVisitors(context);
            CreateVisits(context);
            CreateQuestions(context);
        }


        private void CreateQuestions(MyCompanyContext context)
        {
            context.Questions.Add(new Question()
            {
                Text = "Why are software updates necessary?",
                Answer = "Microsoft is committed to providing its customers with software that has been tested for safety and security. Although no system is completely secure, we use processes, technology, and several specially focused teams to investigate, fix, and learn from security issues to help us meet this goal and to provide guidance to customers on how to help protect their PCs."
            });

            context.Questions.Add(new Question()
            {
                Text = "How can I keep my software up to date?",
                Answer = "Microsoft offers a range of online services to help you keep your computer up to date. Windows Update finds updates that you might not even be aware of and provides you with the simplest way to install updates that help prevent or fix problems, improve how your computer works, or enhance your computing experience. Visit Windows Update to learn more."
            });

            context.Questions.Add(new Question()
            {
                Text = "How do I find worldwide downloads?",
                Answer = "Microsoft delivers downloads in more than 118 languages worldwide. The Download Center now combines all English downloads into a single English Download Center. We no longer offer separate downloads for U.S. English, U.K. English, Australian English, or Canadian English."
            });

            context.Questions.Add(new Question()
            {
                Text = "How do I install downloaded software?",
                Answer = "Before you can use any software that you download, you must install it. For example, if you download a security update but do not install it, the update will not provide any protection for your computer."
            });

            context.Questions.Add(new Question()
            {
                Text = "Can I try Office before I buy?",
                Answer = "Yes. You can sign up to try Office 365 Home Premium for one month free."
            });

            context.Questions.Add(new Question()
            {
                Text = "What do I get with the Office 365 Home Premium trial? ",
                Answer = "The Office trial gives you access to all the features of Office 365 Home Premium except the additional. 20 GB of SkyDrive storage. You can install the Office trial alongside your existing version of Office."
            });

            context.Questions.Add(new Question()
            {
                Text = "How do I make sure I always have the latest Office applications?",
                Answer = "Office 365 customers with an active subscription always get the newest versions of the Office applications when they are available. When we release a new version of Office, you will be notified that you have the option to update your software to the latest version. "
            });

            context.Questions.Add(new Question()
            {
                Text = "Can I install the new Office on my Mac?",
                Answer = " you have an active Office 365 Home Premium or Office 365 University subscription, and available installs, you can install Office applications including Word, Excel, PowerPoint and Outlook on your Mac. The applications available for Mac users and the version numbers may be different from those available for PC users."
            });
            context.SaveChanges();
        }

        private void CreateTeamManagers(MyCompanyContext context)
        {
            int managersCount = 1;

            for (int i = 0; i < managersCount; i++)
            {
                int id = i + 1;
                var name = _employeeNames[i];
                var split = name.Split(' ');
                context.Employees.Add(new Employee()
                {
                    EmployeeId = id,
                    FirstName = split[0],
                    LastName = split[1],
                    Email = _employeeEmails[i],
                    JobTitle = "Team Lead",
                });

                context.Teams.Add(new Team() { TeamId = id, ManagerId = id });
            }

            context.SaveChanges();
        }


        private void CreateEmployees(MyCompanyContext context, int employeesCount)
        {
            int initialId = context.Employees.Count() + 1;

            int teamOneId = context.Teams.OrderBy(t => t.TeamId).First().TeamId;

            for (int i = initialId; i <= employeesCount; i++)
            {
                int index = i - 1;
                var name = _employeeNames[index];

                var split = name.Split(' ');
                context.Employees.Add(new Employee()
                {
                    EmployeeId = i,
                    FirstName = split[0],
                    LastName = split[1],
                    Email = _employeeEmails[index],
                    JobTitle = GetPosition(i),
                    TeamId = teamOneId
                });
            }

            context.SaveChanges();
        }

        private void CreateVisitors(MyCompanyContext context)
        {
            int visitorsCount = _visitorNames.Count;
            int visitorPictureId = 1;
            string domain;

            string cs = ConfigurationManager.ConnectionStrings["MyCompany.Visitors"].ConnectionString; //QuickStart

            for (int i = 0; i < visitorsCount; i++)
            {
                int id = i + 1;
                var name = _visitorNames[i];
                var split = name.Split(' ');

                if (!cs.Contains("localdb") && i == 1) //QuickStart - for production db simulate a data error
                {
                    domain = "#mycompanydemos.com"; 
                }
                else
                {
                    domain = "@mycompanydemos.com"; 
                }
                
                context.Visitors.Add(new Visitor()
                    {
                        VisitorId = id,
                        FirstName = split[0],
                        LastName = split[1],
                        Email = name.Replace(" ", ".") + domain,
                        Position = GetPosition(i),
                        Company = "Microsoft",
                        CreatedDateTime = DateTime.UtcNow,
                        LastModifiedDateTime = DateTime.UtcNow
                    }
                );

                string path = string.Format(_smallPicturePath, name);
                context.VisitorPictures.Add(new VisitorPicture()
                {
                    VisitorPictureId = visitorPictureId,
                    VisitorId = id,
                    PictureType = PictureType.Small,
                    Content = GetPicture(path)
                });
                visitorPictureId++;

                path = string.Format(_picturePath, name);
                context.VisitorPictures.Add(new VisitorPicture()
                {
                    VisitorPictureId = visitorPictureId,
                    VisitorId = id,
                    PictureType = PictureType.Big,
                    Content = GetPicture(path)
                });
                visitorPictureId++;
            }

            context.SaveChanges();
        }

        private void CreateVisits(MyCompanyContext context)
        {
            var employeesIds = context.Employees.Select(e => e.EmployeeId).ToList();

            int cesardlId = 101;
            int jayschId = 102;
            int orvilleId = 103;
            int egamma = 104 ; 

            List<int> _employeeNYCIds = new List<int>() 
            {
                cesardlId, jayschId, orvilleId, egamma
            };

            foreach (var visitor in context.Visitors)
            {
                int visits = _randomize.Next(5, 10);

                DateTime visitDate = new DateTime(2017, 05, 17, 12, 0, 0).ToUniversalTime();
                DateTime launchDt = new DateTime(2017, 05, 10, 19, 0, 0).ToUniversalTime();
                DateTime createdDateTime = new DateTime(2016, 11, 13);
                for (int i = 0; i < visits; i++)
                {
                    int index = _randomize.Next(0, 3);
                    int employeeId = _employeeNYCIds.ElementAt(index);
                    int days = _randomize.Next(-2, 15);
                    int minutes = _randomize.Next(5, 480);

                    var visit = new Visit()
                    {
                        CreatedDateTime = DateTime.UtcNow,
                        VisitDateTime = i == 0 
                                    && employeeId == cesardlId ? launchDt : visitDate.AddDays(days).AddMinutes(minutes),
                        Comments = string.Empty,
                        EmployeeId = employeeId,
                        VisitorId = visitor.VisitorId,
                        Status = VisitStatus.Pending
                    };

                    if (i % 3 == 0)
                    {
                        visit.HasCar = true;
                        visit.Plate = string.Format("B{0}E-6610", visitor.FirstName.Substring(0, 1));
                    }

                    context.Visits.Add(visit);
                }
            }

            context.SaveChanges();
        }

        private void CreateEmployeePictures(MyCompanyContext context)
        {
            int employeePictureId = 1;

            foreach (var employee in context.Employees)
            {
                string employeeName = string.Format("{0} {1}", employee.FirstName, employee.LastName);
                string path = string.Format(_smallPicturePath, employeeName);
                context.EmployeePictures.Add(new EmployeePicture()
                {
                    EmployeePictureId = employeePictureId,
                    EmployeeId = employee.EmployeeId,
                    PictureType = PictureType.Small,
                    Content = GetPicture(path)
                });
                employeePictureId++;

                path = string.Format(_picturePath, employeeName);
                context.EmployeePictures.Add(new EmployeePicture()
                {
                    EmployeePictureId = employeePictureId,
                    EmployeeId = employee.EmployeeId,
                    PictureType = PictureType.Big,
                    Content = GetPicture(path)
                });
                employeePictureId++;
            }

            context.SaveChanges();
        }

        private static byte[] GetPicture(string fileName)
        {
            string path = new Uri(Assembly.GetAssembly(typeof(MyCompanyContextInitializer)).CodeBase).LocalPath;
            FileStream fs = new FileStream(Path.Combine(Path.GetDirectoryName(path),
                                                         fileName), FileMode.Open, FileAccess.Read);

            using (BinaryReader br = new BinaryReader(fs))
            {
                return br.ReadBytes((int)fs.Length);
            }
        }

        private static string GetPosition(int index)
        {
            return "Developer Division";
        }

        private static string GetComments()
        {
            List<string> comments = new List<string>() {
                "Work meeting", 
                "Sprint Planning Meeting", 
                "Analyze the project status", 
                "Personal meeting", 
                "Monthly meeting with partners", 
                "First meeting before starting the project", 
            };

            return comments[_randomize.Next(0, 5)];
        }

        #region NYC

        private List<string> _employeeEmailsNYC = new List<string>()
        {
            // Managers
            String.Format("cesardl@{0}", tenant),
            String.Format("Jaysch@{0}", tenant),
            String.Format("orvillem@{0}", tenant),
            String.Format("egamma@{0}", tenant),

            // Equipo 1
            String.Format("bharry@{0}", tenant),
            String.Format("somase@{0}", tenant),
            String.Format("scottgu@{0}", tenant),
            String.Format("nichers@{0}", tenant),


            // Equipo 2
            String.Format("bethma@{0}", tenant),
            String.Format("epadrino@{0}", tenant),
            String.Format("seanla@{0}", tenant),
            String.Format("jnak@{0}", tenant),

            // Equipo 3
            String.Format("cesardl@{0}", tenant),
            String.Format("davidcsa@{0}", tenant),
            String.Format("dmitryly@{0}", tenant),
            String.Format("briankel@{0}", tenant),


            // Equipo 4
            String.Format("habibh@{0}", tenant),
            String.Format("Brandon.Bray@{0}", tenant),
            String.Format("David.Salgado@{0}", tenant),
            String.Format("ed.blankenship@{0}", tenant),
        };

        private List<string> _employeeNamesNYC = new List<string>() 
        {
            "Cesar de_la_Torre",
            "Jay Schmelzer",
            "Orville McDonald",
            "Erich Gamma",
            "Brian Harry",
            "S. Somasegar",
            "Scott Guthrie",
            "Nicole Herskowitz",
            "Beth Massi",
            "Evelyn Padrino",
            "Sean Laberee",
            "Jim Nakashima",
            "Cesar de_la_Torre",
            "David Carmona",
            "Dmitry Lyalin",
            "Brian Keller",
            "Habib Heydarian",
            "Brandon Bray",
            "David Salgado",
            "Ed Blankenship",
            };

        int startId = 100;

        void CreateTeamManagersNYC(MyCompanyContext context)
        {
            int managersCount = 4;

            for (int i = 0; i < managersCount; i++)
            {
                int id = startId + 1 + i;
                var name = _employeeNamesNYC[i];
                var split = name.Split(' ');
                context.Employees.Add(new Employee()
                {
                    EmployeeId = id,
                    FirstName = split[0],
                    LastName = split[1],
                    Email = _employeeEmailsNYC[i],
                    JobTitle = "Team Lead",
                });

                context.Teams.Add(new Team() { TeamId = id, ManagerId = id });
            }

            context.SaveChanges();

        }

        void CreateEmployeesForNYCTeams(MyCompanyContext context)
        {
            int teamOneId = context.Teams.OrderBy(t => t.TeamId).Skip(1).First().TeamId; // Cesar de_la_Torre
            int teamTwoId = context.Teams.OrderBy(t => t.TeamId).Skip(2).First().TeamId; // Jay Schmelzer
            int teamThreeId = context.Teams.OrderBy(t => t.TeamId).Skip(3).First().TeamId; // Orville McDonald
            int teamFourId = context.Teams.OrderBy(t => t.TeamId).Skip(4).First().TeamId; // Erich Gamma

            CreateEmployeesNYC(context, teamOneId, 5, 8);
            CreateEmployeesNYC(context, teamTwoId, 9, 12);
            CreateEmployeesNYC(context, teamThreeId, 13, 16);
            CreateEmployeesNYC(context, teamFourId, 17, 20);
        }

        void CreateEmployeesNYC(MyCompanyContext context, int teamId, int startIndex, int endIndex)
        {
            int initialId = 300 * startIndex;
            for (int i = startIndex; i <= endIndex; i++)
            {
                int index = i - 1;
                var name = _employeeNamesNYC[index];

                var split = name.Split(' ');
                context.Employees.Add(new Employee()
                {
                    EmployeeId = initialId,
                    FirstName = split[0],
                    LastName = split[1],
                    Email = _employeeEmailsNYC[index],
                    JobTitle = GetPosition(i),
                    TeamId = teamId,
                });
              
                initialId++;
            }

            context.SaveChanges();
        }

        #endregion 
    }
}