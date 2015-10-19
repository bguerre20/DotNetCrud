using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DotNetCrud
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            // 1. Add 2 students
            Student s1 = new Student();
            s1.StudentName = "aaa aaa";
            s1.StudentAddress = new StudentAddress();
            s1.StudentAddress.Address1 = "123 aaa";
            s1.StudentAddress.Address2 = "345 aaa";
            s1.StudentAddress.City = "Long Beach";
            s1.StudentAddress.State = "California";

            Student s2 = new Student();
            s2.StudentName = "bbb bbb";
            s2.StudentAddress = new StudentAddress();
            s2.StudentAddress.Address1 = "123 bbb";
            s2.StudentAddress.Address2 = "345 bbb";
            s2.StudentAddress.City = "Long Beach";
            s2.StudentAddress.State = "California";

            createStudent(s1);
            createStudent(s2);
            // 2. Display 2 students
            textBox1.Text += readStudent("*");
            // 3. update student a: adress
            updateStudent("aaa aaa");
            // 4. display student a
            printDivider();
            textBox1.Text += readStudent("aaa aaa");
            // 5. delete student a: adress
            deleteStudentAddress("aaa aaa");
            // 6. display student a
            printDivider();
            textBox1.Text += readStudent("aaa aaa");
            // 7. create 2 standard objects
            Standard std1 = new Standard();
            std1.StandardName = "FT";
            std1.Description = "Full-Time Instructor";
            Standard std2 = new Standard();
            std2.StandardName = "PT";
            std2.Description = "Part-Time Instructor";
            createStandard(std1);
            createStandard(std2);
            // 8. Create 3 teacher objects
            Teacher t1 = new Teacher();
            t1.TeacherName = "Teacher Name 1";
            Teacher t2 = new Teacher();
            t2.TeacherName = "Teacher Name 2";
            Teacher t3 = new Teacher();
            t3.TeacherName = "Teacher Name 3";
            createTeacher(t1);
            createTeacher(t2);
            createTeacher(t3);
            // 9. Add t1, t2 to std1, and t3 to std2
            updateStandard(t1, t2, std1);
            updateStandard2(t3, std2);
            // 10. Display t1, t2, t3, and their standard name and description
            printDivider();
            string output = "";
            using (var ctx = new SchoolDBEntities())
            {
                var teachers = (from teach in ctx.Teachers
                                select teach);

                
                foreach (Teacher t in teachers)
                    output += "\r\nStandard Name: " + t.Standard.StandardName +
                        "\r\nStandard Description: " + t.Standard.Description +
                        "\r\nTeacher Name: " + t.TeacherName;
            }

            textBox1.Text += output;

            printDivider();
            // 11. Update std1 description
            
            using (var ctx = new SchoolDBEntities())
            {
                //fetching existing standard from the db
                Standard std = (from s in ctx.Standards.Include("Teachers")
                       where s.StandardName == "FT"
                       select s).FirstOrDefault<Standard>();
                std.Description = "Full Time Intstructor Updated";
                ctx.SaveChanges();
            }



            // 12. Display all teachers who are full time
            using (var ctx = new SchoolDBEntities())
            {
                var results = (from t in ctx.Teachers
                               where t.Standard.StandardName == "FT"
                               select t);
                foreach (Teacher t in results)
                    textBox1.Text += "\r\nTeacher Name: " + t.TeacherName + 
                        "\r\nDescription: " + t.Standard.Description;
            }

            printDivider();
            // 13. change the standard id of t1, to standard id of part time instructor
            using (var ctx = new SchoolDBEntities())
            { 
                Teacher teacher1 = ctx.Teachers.Where(s => s.TeacherName == "Teacher Name 1").FirstOrDefault<Teacher>();
                Standard standard2 = ctx.Standards.Where(s => s.StandardName == "PT").FirstOrDefault<Standard>();
                int newID = standard2.StandardId;
                teacher1.StandardId = newID;

                ctx.Entry(teacher1).State = EntityState.Modified;
                ctx.SaveChanges();

                textBox1.Text += "NEW ID: " + teacher1.StandardId + "\r\nNAME: " + teacher1.TeacherName;
            }
            // 14. display t1, with description
            }



        private bool createStudent(Student s)
        {
            using (var dbCtx = new SchoolDBEntities())
            {
                //Add Student object into Students DBset
                dbCtx.Students.Add(s);
                // call SaveChanges method to save student into database
                dbCtx.SaveChanges();
            }
            return true;
        }

        private string readStudent(string s)
        {
            string returnValue = "Error Reading...";

            if (s.Equals("*")) //get all
            {
                using (var ctx = new SchoolDBEntities())
                {
                    var student = (from st in ctx.Students
                                   select st);

                    printDivider();

                    returnValue = "";
                    foreach (Student st in student)
                    {
                        returnValue += "\r\nName: " + st.StudentName +
                                        "\r\nAddress1: " + st.StudentAddress.Address1 +
                                        "\r\nAddress2: " + st.StudentAddress.Address2;
                    }
                }
            }
            else //get specific
            {
                using (var ctx = new SchoolDBEntities())
                {
                    var student = ctx.Students.Where(st => st.StudentName == s).FirstOrDefault<Student>();

                    printDivider();

                    returnValue = "";
                    if (student.StudentAddress != null)
                    {
                        returnValue += "\r\nName: " + student.StudentName +
                                        "\r\nAddress1: " + student.StudentAddress.Address1 +
                                        "\r\nAddress2: " + student.StudentAddress.Address2;
                    }
                    else
                    {
                        returnValue += "\r\nName: " + student.StudentName +
                                        "\r\nAddress1: NULL"+
                                        "\r\nAddress2: NULL";
                    }
                }
            }
            return returnValue;
        }

        private bool deleteStudentAddress(string sName)
        {
            Student studentToDelete;
            //1. Get student from DB
            using (var ctx = new SchoolDBEntities())
            {
                studentToDelete = ctx.Students.Where(s => s.StudentName ==  sName).FirstOrDefault<Student>();
                ctx.Entry(studentToDelete.StudentAddress).State = System.Data.Entity.EntityState.Deleted;
                ctx.SaveChanges();
            }
            
            return true;
        }

        private bool updateStudent(string sName)
        {
            Student stud;
            //1. Get student from DB
            using (var ctx = new SchoolDBEntities())
            {
                stud = ctx.Students.Where(s => s.StudentName == sName).FirstOrDefault<Student>();

                if (stud != null)
                {
                    stud.StudentAddress.Address1 = "123 aaa new";
                }

                //3. Mark entity as modified
                ctx.Entry(stud).State = System.Data.Entity.EntityState.Modified;

                //4. call SaveChanges
                ctx.SaveChanges();
            }
           

            return true;
        }

        private void printDivider()
        {
            textBox1.Text += "\r\n ---------------------------------------------- \r\n";
        }

        private void createStandard(Standard std)
        {
            using (var dbCtx = new SchoolDBEntities())
            {
                //Add Student object into Students DBset
                dbCtx.Standards.Add(std);
                // call SaveChanges method to save student into database
                dbCtx.SaveChanges();
            }
        }

        private void createTeacher(Teacher t)
        {
            using (var dbCtx = new SchoolDBEntities())
            {
                //Add Student object into Students DBset
                dbCtx.Teachers.Add(t);
                // call SaveChanges method to save student into database
                dbCtx.SaveChanges();
            }
        }

        private void updateStandard(Teacher t1, Teacher t2, Standard stnd)
        {
            Standard std = null;
            using (var ctx = new SchoolDBEntities())
            {
                //fetching existing standard from the db
                std = (from s in ctx.Standards.Include("Teachers")
                       where s.StandardName == stnd.StandardName
                       select s).FirstOrDefault<Standard>();

                Teacher teach1 = ctx.Teachers.Where(s => s.TeacherName == t1.TeacherName).FirstOrDefault<Teacher>();
                Teacher teach2 = ctx.Teachers.Where(s => s.TeacherName == t2.TeacherName).FirstOrDefault<Teacher>();

                std.Teachers.Add(teach1);
                std.Teachers.Add(teach2);
                ctx.SaveChanges();

            }

        }

        private void updateStandard2(Teacher t, Standard stnd)
        {
            Standard std = null;
            using (var ctx = new SchoolDBEntities())
            {
                //fetching existing standard from the db
                std = (from s in ctx.Standards.Include("Teachers")
                       where s.StandardName == stnd.StandardName
                       select s).FirstOrDefault<Standard>();
                Teacher teach1 = ctx.Teachers.Where(s => s.TeacherName == t.TeacherName).FirstOrDefault<Teacher>();

                std.Teachers.Add(teach1);
                ctx.SaveChanges();
            }

        }
    }
}
