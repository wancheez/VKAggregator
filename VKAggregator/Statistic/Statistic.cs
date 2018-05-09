using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace VKAggregator.Statistic
{
    public class Statistic
    {
        //Словарь, хранящий объем полученных данных на каждом уровне рекурсии
        static Dictionary<int, int> layerSizeVertex = new Dictionary<int, int>();
        static Dictionary<int, int> layerSizeEdgeVal = new Dictionary<int, int>();
        static DateTime beginTime, endTime;
        static List<String>  errList = new List<string>();
        static int errorCount = 0;
        //Объявляем событие, со стандартным дотнетовским делегатом.
        public static event EventHandler CountEdgeChanged;      
        //Свойство для реализации event - изменение переменной. Не получилось
        public static Dictionary<int, int> layerSizeEdge
        {
            get { return layerSizeVertex; }
            set
            {                         
                layerSizeEdgeVal = value;
            }
        }

        //Создаём класс аргумента события, для передачи дополнительной информации
        //связанной с событием.
        public class MyEventArgs : EventArgs
        {
            public Dictionary<int, int> CountEdgeField
            {
                get
                {
                    return layerSizeVertex;
                }
                set
                {
                }
            }
        }


        public static void newLayer(int depth)
        {
            for (int i = 0; i <= depth + 1; i++)
            {
                //костыли
                try
                {
                    layerSizeEdge.Add(i, 0);
                    layerSizeVertex.Add(i, 0);
                }
                catch { }
            }
        }

        public static void logBegin()
        {
            beginTime = DateTime.Now;
        }

        public static void LogVertex(int layer, int count)
        {
            layerSizeVertex[layer]+=count;
            CountEdgeChanged(null, new MyEventArgs());
        }

        public static void LogEdge(int layer, int count)
        {
            //if (layer > 0)
            layerSizeEdge[layer]+=count;
            //Вызываем делегат события по изменению статистики для вывода на форму
            CountEdgeChanged(null, new MyEventArgs());
        }

        public static void logError(string Error)
        {
            errList.Add(DateTime.Now + ": " + Error);

        }

        public static void LogEnd()
        {
            endTime = DateTime.Now;
            File.Create("log/log_" + DateTime.Now.Day + "_" + DateTime.Now.Month + ".log");
            using (StreamWriter file =
              new StreamWriter("D:/log_" + DateTime.Now.Day + "_" + DateTime.Now.Month + ".log"))
            {
                file.WriteLine(string.Format("Begin Time: {0}", beginTime));
                foreach (KeyValuePair<int, int> kvp in layerSizeVertex)
                {
                    file.WriteLine("Layer: {0}, Vertex = {1}, Edges = {2}", kvp.Key, kvp.Value, layerSizeEdge[kvp.Key]);
                }

                foreach (String err in errList)
                {
                    file.WriteLine("ERROR: {0}", errList);
                }

                file.WriteLine(string.Format("End Time: {0}", endTime));
            }
        }

        public static int GetEdgesCount(int depth)
        {
            int sum = 0;
            foreach(KeyValuePair<int, int> i in layerSizeEdge)
            {
                sum += i.Value;
            }
            return sum;
        }

        public static int GetVertexCount(int depth)
        {
            int sum = 0;
            foreach (KeyValuePair<int, int> i in layerSizeVertex)
            {
                sum += i.Value;
            }
            return sum;
        }

        public static TimeSpan GetTimeElapsed()
        {
            return DateTime.Now - beginTime;
        }
    }
}

