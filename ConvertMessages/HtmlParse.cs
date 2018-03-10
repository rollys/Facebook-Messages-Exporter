using System;
using System.IO;
using System.Collections.Generic;
using DataStructs;
using MyFileParser;

namespace FBConversation
{
    class Conversation : HtmlFile
    {
        SimpleTable Data;

        public double progress;

        public IList<sMessage> messages => _messages;
        public string filename => _filename;
        string htmlcontent;
        public string name => _name;
        private int _zeroindex;
        private string _name;
        private string _filename;
        private IList<sMessage> _messages;

        public int zeroindex => _zeroindex;

        public int size { get; }

        public Conversation(string Path)
        {
            if (string.IsNullOrEmpty(Path)) throw new Exception("Path can not be empty.");

            StreamReader sr = new StreamReader(Path);
            htmlcontent = sr.ReadToEnd();
            _messages = new List<sMessage>();
            _zeroindex = 0;
            size = htmlcontent.Length;
            _filename = System.IO.Path.GetFileName(Path);
        }

        public void Read()
        {
            _name = rReadBetween(@"<title>", @"</title>", htmlcontent)[0];
            _messages = new List<sMessage>();
            IList<string> r = rReadBetween(@"<div class=""message_header"">", @"<div class=""message"">", htmlcontent);
            for (int i = 0; i < r.Count; i++)
            {
                sMessage mes = new sMessage(rReadBetween(@"<span class=""user"">", @"</span>", r[i])[0], rReadBetween(@"<span class=""meta"">", @"</span>", r[i])[0], rReadBetween(@"<p>", @"</p>", r[i])[0]);
                messages.Add(mes);
            }
        }

        public double Read(int count)
        {
            if (zeroindex == 0) _name = rReadBetween(@"<title>", @"</title>", htmlcontent)[0];

            IList<string> r = rReadBetween(@"<div class=""message_header"">", @"<div class=""message"">", htmlcontent, count, ref _zeroindex);
            if (r.Count == 0) return 1.0;
            for (int i = 0; i < r.Count; i++)
            {
                sMessage mes = new sMessage(rReadBetween(@"<span class=""user"">", @"</span>", r[i])[0], rReadBetween(@"<span class=""meta"">", @"</span>", r[i])[0], rReadBetween(@"<p>", @"</p>", r[i])[0]);
                messages.Add(mes);
            }
            progress = (double)zeroindex / (double)size;
            return progress;
        }

        public SimpleTable GetTable()
        {
            if (messages != null)
            {
                Data = new SimpleTable(messages);
                return Data;
            }
            return null;
        }
    }
}