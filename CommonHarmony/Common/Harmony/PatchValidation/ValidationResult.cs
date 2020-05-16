using System.Collections.Generic;

namespace Common.Harmony.PatchValidation
{
    internal class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Information { get; } = new List<string>();
    }
}