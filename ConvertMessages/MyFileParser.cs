using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using DataStructs;

namespace MyFileParser
{
    class HtmlFile
    {
        string file;
        string path;

        public HtmlFile(string Path, Encoding e)
        {
            if (string.IsNullOrEmpty(Path)) throw new ArgumentNullException("Path mustnot be null.");

            path = Path;
            StreamReader sr = new StreamReader(Path, e);

            try
            {
                file = sr.ReadToEnd();
            }
            catch (Exception E)
            {
                throw E;
            }
        }

        public HtmlFile(string File)
        {
            if (string.IsNullOrEmpty(File)) throw new ArgumentNullException("Path mustnot be null.");
            file = File.ToString();
        }

        public HtmlFile()
        {

        }

        bool IsSame(string word, string source, int index)
        {
            for (int i = index, k = 0; k < word.Length && i < source.Length; ++i, ++k)
            {
                if (word[k] != source[i])
                {
                    return false;
                }
            }

            return true;
        }

        protected IList<string> rReadBetween(string Start, string Stop, string source)
        {
            IList<string> readed = new List<string>();

            for (int i = 0; i < source.Length; i++)
            {
                if (source[i] == Start[0])
                {
                    if (IsSame(Start, source, i))
                    {
                        i += Start.Length;
                        string content = "";

                        for (; !IsSame(Stop, source, i); ++i)
                        {
                            content += source[i].ToString();
                        }
                        readed.Add(content);
                    }
                }
            }
            return readed;
        }

        protected IList<string> rReadBetween(string Start, string Stop, string source, int c, ref int startIndex)
        {
            IList<string> readed = new List<string>();
            int i = startIndex;
            for (; i < source.Length && readed.Count < c; i++)
            {
                if (source[i] == Start[0])
                {
                    if (IsSame(Start, source, i))
                    {
                        i += Start.Length;
                        string content = "";

                        for (; !IsSame(Stop, source, i); ++i)
                        {
                            content += source[i].ToString();
                        }
                        readed.Add(content);
                    }
                }
            }
            startIndex = i;
            return readed;
        }

        public IList<string> ReadBeetween(string Start, string Stop) => rReadBetween(Start, Stop, file);

        public IList<string> ReadBeetween(string Start, string Stop, int count, ref int startIndex) => rReadBetween(Start, Stop, file, count, ref startIndex);
    }

    class CSVFile
    {
        SimpleTable data;
        public CSVFile(SimpleTable Source)
        {
            data = Source;
        }

        public void Save(string path, string separator, bool header)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("Path mustnot be null.");

            try
            {
                FileStream sf = new FileStream(path.Replace(".html", ".csv"), FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter sw = new StreamWriter(sf, Encoding.UTF8);

                try
                {


                    if (header)
                    {
                        string towrite = "";
                        for (int k = 0; k < data.columnanames.Count; k++)
                        {
                            if (k == 0) towrite += data.columnanames[k];
                            else towrite += " ; " + data.columnanames[k];
                        }
                    }

                    for (int i = 0; i < data.data.Count; i++)
                    {
                        string towrite = "";

                        for (int k = 0; k < data.columnanames.Count; k++)
                        {
                            if (k == 0) towrite += data.data[i][k];
                            else towrite += " ; " + data.data[i][k];
                        }

                        sw.WriteLine(towrite);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    sw.Close();
                    sf.Close();
                }
            }
            catch (Exception s)
            {
                throw s;
            }
        }

        public void Save(string path, string[] tooverwrite, string separator)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentNullException("Path mustnot be null.");

            try
            {
                FileStream sf = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter sw = new StreamWriter(sf, Encoding.UTF8);

                try
                {
                    for (int i = 0; i < tooverwrite.Length; i++)
                    {
                        string towrite = "";


                        if (i == 0) towrite += tooverwrite[i];
                        else towrite += " ; " + tooverwrite[i];


                        sw.WriteLine(towrite);
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    sw.Close();
                }
            }
            catch (Exception s)
            {
                throw s;
            }
        }
    }
}