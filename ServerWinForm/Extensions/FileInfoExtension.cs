using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerWinForm.Extensions
{
    public static class FileInfoExtension
    {
        public static bool IsEmpty(this FileInfo f)
        {
            return f.Length == 0 || f.Length < 6 && File.ReadAllText(f.FullName).Length == 0;
        }
    }
}
