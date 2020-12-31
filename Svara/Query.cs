using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Svara
{
    public record Query(string filePath, bool doCleanup=false, char listDelim=';')
    {
        private const string commentChar = "#";
        public async Task<(bool answered, string answer)> GetUserInputAsync()
        {
            (bool ans, string answ) res = (false, "");
            await Task.Run(() => { res = GetUserInput(); });

            return res;
        }

        public (bool answered, string answer) GetUserInput(string message)
        {
            var comment = $"{commentChar} This is a comment, every line starting with '{commentChar}' will be ignored! {Environment.NewLine}";

            createTempFile(filePath,"", $"# Message: {message}{Environment.NewLine}{comment}");
            return queryUserInput(filePath);
        }

        public (bool anaswered, IEnumerable<string> answers) GetUserInput(IEnumerable<string> list)
        {
            if (!list.Any())
                return (false, new List<string>());

            var comment = $"{commentChar} This is a comment, every line starting with '{commentChar}' or '{listDelim}' will be ignored! {Environment.NewLine}" +
                $"{commentChar} Remove the first {listDelim} character from the line/s you want to select in the list above";

            createTempFile(filePath, string.Join(Environment.NewLine, list.Select(line => listDelim + line)), comment);
            var input = queryUserInput(filePath);
            var ans = input.answer.Split(Environment.NewLine).Select(x => x.Trim()).Where(line => !line.StartsWith(listDelim) && !line.StartsWith(commentChar) && !string.IsNullOrWhiteSpace(line));
            return (ans.Any(), ans);
        }

        public (bool answered, string answer) GetUserInput()
        {
            createTempFile(filePath, "");
            return queryUserInput(filePath);
        }

        private void createTempFile(string path, string text="",string comment="")
        {
            File.WriteAllText(path,$"{text}{(string.IsNullOrWhiteSpace(comment) ? comment : Environment.NewLine)}{(string.IsNullOrWhiteSpace(comment) ? comment : Environment.NewLine)}{comment}");
        }

        private (bool answered, string answer) queryUserInput(string path)
        {
            var str = string.Empty;
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = "notepad";
            startInfo.Arguments = $"/A {path}";

            var p = Process.Start(startInfo);
            p.WaitForExit();

            str = string.Join(Environment.NewLine, File.ReadAllLines(filePath).Where(line => !line.StartsWith(commentChar)));

            if (doCleanup)
                File.Delete(filePath);

            if (!string.IsNullOrWhiteSpace(str))
                return (true, str);

            return (false, "");
        }
    }
}
