using System.Diagnostics;
using System.IO;
using System.Text;

namespace SignalR.ConnectionManager
{
    internal class DebugWriter : TextWriter
    {
        public override void WriteLine(string value)
        {
            Debug.WriteLine(value);
        }

        public override void WriteLine(string format, params object[] args)
        {
            Debug.WriteLine(string.Format(format, args));
        }

        public override void Write(char value)
        {
            throw new System.NotImplementedException();
        }

        public override Encoding Encoding
        {
            get { throw new System.NotImplementedException(); }
        }
    }
}