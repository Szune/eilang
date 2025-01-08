using System.Text;

namespace eilang.Extensions
{
    public static class StringBuilderExtensions
    {

        public static StringBuilder AppendIndentedLine(this StringBuilder sb, int level, string text)
        {
            sb.Append(' ', level * 4);
            sb.AppendLine(text);
            return sb;
        }
    }
}
