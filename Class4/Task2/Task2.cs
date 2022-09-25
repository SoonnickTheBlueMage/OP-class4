using System.Text;

namespace Task2
{
    public class Task2
    {
        private record class ParsedInput(string FilePath, Encoding ReadingEncoding, Encoding WritingEncoding);

        private static ParsedInput? ParseArgs(IReadOnlyList<string> args)
        {
            if (args.Count != 3)
            {
                Console.WriteLine("Wrong input format");
                return null;
            }

            if (!File.Exists(@args[0]))
            {
                Console.WriteLine("File does not exist");
                return null;
            }
            
            Encoding readingEncoding;
            try
            {
                readingEncoding = Encoding.GetEncoding(args[1]);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Reading encoding does not exist");
                return null;
            }

            Encoding writingEncoding;
            try
            {
                writingEncoding = Encoding.GetEncoding(args[2]);
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Writing encoding does not exist");
                return null;
            }

            return new ParsedInput(args[0], readingEncoding, writingEncoding);
        }

        private static string ReadText(string filePath, Encoding readingEncoding)
        {
            var streamReader = new StreamReader(@filePath, readingEncoding);
            var text = streamReader.ReadToEnd();
            streamReader.Close();
            
            return text;
        }

        private static void WriteText(string filePath, Encoding writingEncoding, string text)
        {
            var streamWriter = new StreamWriter(@filePath, false, writingEncoding);
            streamWriter.Write(text);
            streamWriter.Close();
        }

        public static void Main(string[] args)
        {
            var parsedInput = ParseArgs(args);
            if (parsedInput == null)
            {
                return;
            }

            var text = ReadText(parsedInput.FilePath, parsedInput.ReadingEncoding);
            WriteText(parsedInput.FilePath, parsedInput.WritingEncoding, text);
            
            Console.WriteLine("Done");
        }
    }
}
