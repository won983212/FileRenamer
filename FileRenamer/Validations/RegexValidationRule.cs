using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FileRenamer.Validations
{
    public class RegexValidationRule : ValidationRule
    {
        public bool CheckReplacementRegex { get; set; } = false;

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            string pattern = (value ?? "") as string;

            if (pattern == null)
                return new ValidationResult(false, "문자열을 입력해야합니다.");

            if (string.IsNullOrWhiteSpace(pattern))
            {
                if (CheckReplacementRegex)
                    return new ValidationResult(false, "이 필드는 반드시 입력해야합니다.");
                else
                    return ValidationResult.ValidResult;
            }

            if (CheckReplacementRegex && pattern.StartsWith("inc"))
            {
                Match m = MatchIncreaseToken(pattern);
                if (!m.Success)
                    return new ValidationResult(false, "잘못된 inc문법입니다.");
                else
                    pattern = pattern.Substring(m.Value.Length);
            }

            try
            {
                Regex.Match("", pattern);
            }
            catch (ArgumentException e)
            {
                return new ValidationResult(false, e.Message);
            }

            return ValidationResult.ValidResult;
        }

        public static Match MatchIncreaseToken(string pattern)
        {
            return Regex.Match(pattern, "^inc\\(([+-]*\\d+),([+-]*\\d+)\\)");
        }
    }
}
