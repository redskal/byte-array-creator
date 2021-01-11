/*
 * ByteArrayCreator (BAC) v1.1
 * by Dorian Warboys / Red Skäl
 * https://github.com/redskal
 * 
 * A small tool I originally cobbled together in 2012 when I was trying
 * to move my toolset over to .NET framework.
 * 
 * Typically if I write my own shellcode I use NASM and generate a .BIN
 * file. This tool was used to generate exploit skeletons from those
 * shellcode files. It doesn't have the catchiest name but it wasn't
 * ever really supposed to be public and 'bac' is quick to type!
 */
using System;
using System.Linq;
using System.IO;

namespace ByteArrayCreator
{
    class Program
    {
        static void Main(string[] args)
        {
            byte[] buffer = null;
            // banner skilzZz
            Console.WriteLine(" [*] ByteArrayCreator - v1.1");
            Console.WriteLine(" [*] by Dorian Warboys / Red Skäl");
            Console.WriteLine(" [*] https://github.com/redskal");
            Console.WriteLine(" [*]");

            if (args.Count() < 2)
            {
                Usage();
                return;
            }

            Console.WriteLine(" [*] Creating outfile: {0}", args[0].ToString());
            StreamWriter outFile = new StreamWriter(args[0].ToString(), false);

            outFile.WriteLine("// [B]yte[A]rray[C]reator - v1.1");
            outFile.WriteLine("// by Dorian Warboys / Red Skäl");
            outFile.WriteLine("// https://github.com/redskal");
            outFile.WriteLine("");
            outFile.WriteLine("namespace ByteArrayCreator");
            outFile.WriteLine("{");
            outFile.WriteLine("   class FileArrays");
            outFile.WriteLine("   {");

            int i = 0;

            // nom, nom, nom...
            foreach (string file in args)
            {
                // avoid args[0]... messy, I know
                if (i == 0)
                {
                    i++;
                    continue;
                }

                if (!(File.Exists(file)))
                {
                    Usage();
                    return;
                }


                string fileName = Path.GetFileNameWithoutExtension(file);

                FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                long n = new FileInfo(file).Length;

                buffer = br.ReadBytes((Int32)n);

                Console.WriteLine(" [*] Adding file: {0} - {1} bytes", file, n.ToString());
                
                string _hex = BitConverter.ToString(buffer).Replace("-","\\x");
                _hex = "\\x" + _hex;


                for (int j = 80; j < (_hex.Length-4); j += 87)
                {
                    _hex = _hex.Insert(j, "\" +\r\n\t\"");
                }

                outFile.WriteLine("      // {0} - {1} bytes", Path.GetFileName(file), n.ToString());
                outFile.WriteLine("      public byte[] {0} =\r\n\t\"{1}\";", fileName, _hex);
                outFile.WriteLine("");
                i++;
            }

            Console.WriteLine(" [*] Closing outfile...");

            outFile.WriteLine("   }");
            outFile.WriteLine("}");
            outFile.Close();

            Console.WriteLine(" [*]");
            Console.WriteLine(" [*] Exiting...");
        }

        static void Usage()
        {
            Console.WriteLine(" [!] Usage:");
            Console.WriteLine(" [!]    bac.exe <out_file> <file1> [file2] [file3] ...");
            Console.WriteLine(" [*]");
            Console.WriteLine(" [*] Exiting...");
            return;
        }
    }
}
