using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

            // 8. Create 3 teacher objects

            // 9. Add t1, t2 to std1, and t3 to std2

            // 10. Display t1, t2, t3, and their standard name and description

            // 11. Update std1 description

            // 12. Display all teachers who are full time

            // 13. change the standard id of t1, to standard id of part time instructor

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
    }
}
