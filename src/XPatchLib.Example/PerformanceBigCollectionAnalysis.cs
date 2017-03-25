using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using XPatchLib;
using XmlSerializer = System.Xml.Serialization.XmlSerializer;

namespace XmlSerializerExample
{
    [PrimaryKey("Code")]
    public abstract class CodeName
    {
        public CodeName()
        {
        }

        public CodeName(string pCode, string pName)
            : this()
        {
            Code = pCode.Trim();
            Name = pName.Trim();
        }

        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class Director : CodeName
    {
        public Director()
        {
        }

        public Director(string pCode, string pName)
            : base(pCode, pName)
        {
        }
    }

    public class Genre : CodeName
    {
        public Genre()
        {
        }

        public Genre(string pCode, string pName)
            : base(pCode, pName)
        {
        }
    }

    public class Label : CodeName
    {
        public Label()
        {
        }

        public Label(string pCode, string pName)
            : base(pCode, pName)
        {
        }
    }

    [PrimaryKey("Id")]
    public class Movie
    {
        public Movie()
        {
            Genres = new List<Genre>();
            Series = new List<Series>();
            Previews = new List<string>();
            Stars = new List<Star>();
        }

        public string Code { get; set; }

        public string CoverFigure { get; set; }

        public Director Director { get; set; }

        public List<Genre> Genres { get; set; }

        public string Id { get; set; }

        public Label Label { get; set; }

        public string Length { get; set; }

        public string Name { get; set; }

        public List<string> Previews { get; set; }

        public DateTime? ReleaseDate { get; set; }

        public List<Series> Series { get; set; }

        public List<Star> Stars { get; set; }

        public override bool Equals(object obj)
        {
            Movie m = obj as Movie;
            if (m == null)
            {
                return false;
            }
            return Id == m.Id;
        }

        public override int GetHashCode()
        {
            return string.IsNullOrEmpty(Id) ? 0 : Id.GetHashCode();
        }
    }

    public class MovieCollection : Collection<Movie>
    {
    }

    public class PerformanceBigCollectionAnalysis
    {
        private static String LOGFILE = Path.GetTempFileName();

        /// <summary>
        /// 测试XPatchLib 
        /// </summary>
        public static void Main()
        {
            //测试XPatchLib（不覆盖原有对象）
            TestXPatchLib(false);

            //测试不同序列化方法间的性能差异
            //MainTest();
            //Console.ReadLine();
        }

        /// <summary>
        /// 测试XmlSerializer与XPatchLib之间的性能区别 
        /// </summary>
        public static void MainTest()
        {
            Console.WriteLine("日志文件:" + LOGFILE);
            for (int i = 0; i < 10; i++)
            {
                TestXPatchLib(false);
                TestXPatchLib(true);
                TestXPatchLibNull();
                TestXmlSerializer();
            }
        }

        private static MovieCollection CreateMovieCollection(int pMovieNum, int pStarNum, int pGenreNum, int pSeriesNum)
        {
            MovieCollection result = new MovieCollection();

            for (int i = 0; i < pMovieNum; i++)
            {
                result.Add(new Movie { Id = i.ToString(), Code = i.ToString() });
                for (int j = 0; j < pStarNum; j++)
                {
                    result[i].Stars.Add(new Star { Name = j.ToString(), Code = j.ToString() });
                }
                for (int j = 0; j < pGenreNum; j++)
                {
                    result[i].Genres.Add(new Genre { Name = j.ToString(), Code = j.ToString() });
                }
                for (int j = 0; j < pSeriesNum; j++)
                {
                    result[i].Series.Add(new Series { Name = j.ToString(), Code = j.ToString() });
                }
            }
            return result;
        }

        private static void Test()
        {
            DateTime BeginTime = DateTime.Now;
            DateTime EndTime = DateTime.Now;
            TimeSpan ts;
            int max = 500000000;

            Movie m = new Movie { Id = "1" };

            //Write("PropertyInfo.Invoke");
            //BeginTime = System.DateTime.Now;
            //for (int i = 0; i < max; i++)
            //{
            //    System.Reflection.PropertyInfo pi = m.GetType().GetProperty("Id");
            //    object o = pi.GetGetMethod().Invoke(m, null);
            //}
            //EndTime = System.DateTime.Now;
            //ts = EndTime.Subtract(BeginTime);
            //Write(ts.ToString());
            //Write("----------------------------");

            //Write("PropertyInfo.GetValue");
            //BeginTime = System.DateTime.Now;
            //for (int i = 0; i < max; i++)
            //{
            //    System.Reflection.PropertyInfo pi = m.GetType().GetProperty("Id");
            //    object o=pi.GetValue(m);
            //}
            //EndTime = System.DateTime.Now;
            //ts = EndTime.Subtract(BeginTime);
            //Write(ts.ToString());
            //Write("----------------------------");

            Write("Cached PropertyInfo.GetValue");
            BeginTime = DateTime.Now;
            PropertyInfo cpi = m.GetType().GetProperty("Id");
            for (int i = 0; i < max; i++)
            {
                object o = cpi.GetValue(m);
            }
            EndTime = DateTime.Now;
            ts = EndTime.Subtract(BeginTime);
            Write(ts.ToString());
            Write("----------------------------");

            //Write("PropertyDescriptor.GetValue");
            //BeginTime = System.DateTime.Now;
            //for (int i = 0; i < max; i++)
            //{
            //    System.ComponentModel.PropertyDescriptor desc = System.ComponentModel.TypeDescriptor.GetProperties(m.GetType())["Id"];
            //    object o = desc.GetValue(m);
            //}
            //EndTime = System.DateTime.Now;
            //ts = EndTime.Subtract(BeginTime);
            //Write(ts.ToString());
            //Write("----------------------------");

            //Write("Cached PropertyDescriptor.GetValue");
            //BeginTime = System.DateTime.Now;
            //System.ComponentModel.PropertyDescriptor cdesc = System.ComponentModel.TypeDescriptor.GetProperties(m.GetType())["Id"];
            //for (int i = 0; i < max; i++)
            //{
            //    object o = cdesc.GetValue(m);
            //}
            //EndTime = System.DateTime.Now;
            //ts = EndTime.Subtract(BeginTime);
            //Write(ts.ToString());
            //Write("----------------------------");

            //Write("Cached Expression.GetValue");
            //BeginTime = System.DateTime.Now;
            //System.Linq.Expressions.ParameterExpression carg = System.Linq.Expressions.Expression.Parameter(m.GetType(), "x");
            //System.Linq.Expressions.Expression cexpr = System.Linq.Expressions.Expression.Property(carg, "Id");

            //var cex = System.Linq.Expressions.Expression.Lambda<Func<Movie,object>>(cexpr, carg);
            //var cpropertyResolver = cex.Compile();

            //for (int i = 0; i < max; i++)
            //{
            //    object o = cpropertyResolver(m);
            //}
            //EndTime = System.DateTime.Now;
            //ts = EndTime.Subtract(BeginTime);
            //Write(ts.ToString());
            //Write("----------------------------");

            Write("Cached CastDelegate.GetValue");
            BeginTime = DateTime.Now;

            var instance = Expression.Parameter(typeof(object), "instance");
            PropertyInfo pi1 = m.GetType().GetProperty("Id");

            UnaryExpression instanceCast = Expression.TypeAs(instance, pi1.DeclaringType);
            Func<object, object> GetDelegate = Expression.Lambda<Func<object, object>>(Expression.TypeAs(Expression.Call(instanceCast, pi1.GetGetMethod()), typeof(object)), instance).Compile();

            for (int i = 0; i < max; i++)
            {
                object o = GetDelegate(m);
            }
            EndTime = DateTime.Now;
            ts = EndTime.Subtract(BeginTime);
            Write(ts.ToString());
            Write("----------------------------");
        }

        private static void TestXmlSerializer()
        {
            DateTime BeginTime = DateTime.Now;
            DateTime EndTime = DateTime.Now;
            TimeSpan ts;

            MovieCollection col1 = CreateMovieCollection(5000, 4, 5, 6);
            XmlSerializer serializer =
                new XmlSerializer(typeof(MovieCollection));

            string context = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, col1);

                stream.Position = 0;
                using (StreamReader stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    context = stremReader.ReadToEnd();
                }
            }

            EndTime = DateTime.Now;
            ts = EndTime.Subtract(BeginTime);
            Write(string.Format("XmlSerializer Serialize:{0}.", ts));

            string fileName = Path.GetTempFileName();
            FileStream fs = File.Create(fileName);  //创建文件
            fs.Close();
            StreamWriter sw = new StreamWriter(fileName);  //创建写入流
            sw.Write(context);

            sw.Flush();
            sw.Close();

            BeginTime = DateTime.Now;
            using (StringReader reader = new StringReader(context))
            {
                MovieCollection col3 = serializer.Deserialize(reader) as MovieCollection;
            }
            EndTime = DateTime.Now;
            ts = EndTime.Subtract(BeginTime);
            Write(string.Format("XmlSerializer Deserialize:{0}.", ts));
            Write("----------------------------");
        }

        private static void TestXPatchLib(Boolean pOverride)
        {
            DateTime BeginTime = DateTime.Now;
            DateTime EndTime = DateTime.Now;
            TimeSpan ts;

            MovieCollection col1 = CreateMovieCollection(5000, 4, 5, 6);
            MovieCollection col2 = CreateMovieCollection(5005, 5, 6, 7);

            XPatchLib.XmlSerializer serializer = new XPatchLib.XmlSerializer(typeof(MovieCollection));

            string context = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, col1, col2);

                stream.Position = 0;
                using (StreamReader stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    context = stremReader.ReadToEnd();
                }
            }

            EndTime = DateTime.Now;
            ts = EndTime.Subtract(BeginTime);
            Write(string.Format("XPatchLib Divide:{0}.", ts));

            string fileName = Path.GetTempFileName();
            FileStream fs = File.Create(fileName);  //创建文件
            fs.Close();
            StreamWriter sw = new StreamWriter(fileName);  //创建写入流
            sw.Write(context);

            sw.Flush();
            sw.Close();

            BeginTime = DateTime.Now;
            using (StringReader reader = new StringReader(context))
            {
                MovieCollection col3 = serializer.Combine(reader, col1, pOverride) as MovieCollection;
            }
            EndTime = DateTime.Now;
            ts = EndTime.Subtract(BeginTime);
            Write(string.Format("XPatchLib Combine Override:{1}:{0}.", ts, pOverride));
            Write("----------------------------");
        }

        private static void TestXPatchLibNull()
        {
            DateTime BeginTime = DateTime.Now;
            DateTime EndTime = DateTime.Now;
            TimeSpan ts;

            MovieCollection col1 = CreateMovieCollection(5000, 4, 5, 6);

            XPatchLib.XmlSerializer serializer = new XPatchLib.XmlSerializer(typeof(MovieCollection));

            string context = string.Empty;
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Divide(stream, null, col1);

                stream.Position = 0;
                using (StreamReader stremReader = new StreamReader(stream, Encoding.UTF8))
                {
                    context = stremReader.ReadToEnd();
                }
            }

            EndTime = DateTime.Now;
            ts = EndTime.Subtract(BeginTime);
            Write(string.Format("XPatchLib Divide Null:{0}.", ts));

            string fileName = Path.GetTempFileName();
            FileStream fs = File.Create(fileName);  //创建文件
            fs.Close();
            StreamWriter sw = new StreamWriter(fileName);  //创建写入流
            sw.Write(context);

            sw.Flush();
            sw.Close();

            BeginTime = DateTime.Now;
            using (StringReader reader = new StringReader(context))
            {
                MovieCollection col3 = serializer.Combine(reader, null) as MovieCollection;
            }
            EndTime = DateTime.Now;
            ts = EndTime.Subtract(BeginTime);
            Write(string.Format("XPatchLib Combine Null:{0}.", ts));
            Write("----------------------------");
        }

        private static void Write(String msg)
        {
            Console.WriteLine(msg);
            File.AppendAllLines(LOGFILE, new[] { msg, Environment.NewLine });
        }
    }

    public class Series : CodeName
    {
        public Series()
        {
        }

        public Series(string pCode, string pName)
            : base(pCode, pName)
        {
        }
    }

    public class Star : CodeName
    {
        public Star()
        {
        }

        public Star(string pCode, string pName)
            : base(pCode, pName)
        {
        }
    }
}