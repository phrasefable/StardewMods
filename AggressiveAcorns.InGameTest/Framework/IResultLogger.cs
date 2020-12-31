using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework.Model;
using StardewModdingAPI;

namespace Phrasefable.StardewMods.AggressiveAcorns.InGameTest.Framework
{
    public interface IResultLogger
    {
        public void Log(ITestResult resultRoot);
    }

    public class SmapiConsoleLogger : IResultLogger
    {
        private readonly string _indent = "    ";

        private readonly IMonitor _monitor;
        private readonly Func<IIdentifiable, string> _getIdString;


        public SmapiConsoleLogger(IMonitor monitor, Func<IIdentifiable, string> getIdString)
        {
            this._monitor = monitor;
            this._getIdString = getIdString;
        }


        public void Log(ITestResult resultRoot)
        {
            IDictionary<ITestResult, int> failedChildren = new Dictionary<ITestResult, int>();
            IDictionary<ITestResult, int> numLeaves = new Dictionary<ITestResult, int>();

            void Preprocess(ITestResult result)
            {
                if (!result.IsAggregate) return;

                foreach (ITestResult child in result.Children)
                {
                    Preprocess(child);
                }

                numLeaves[result] = result.Children.Sum(
                    child => numLeaves.ContainsKey(child) ? numLeaves[child] : 1
                );
                failedChildren[result] = result.Children.Sum(
                    child => failedChildren.ContainsKey(child)
                        ? failedChildren[child]
                        : child.Result.Status == Status.Pass
                            ? 0
                            : 1
                );
            }

            static string TestOrTests(int n) => n == 1 ? "test" : "tests";

            string GetLogLine(ITestResult result)
            {
                var builder = new StringBuilder();
                builder.Append(this._getIdString(result));

                if (result.IsAggregate)
                {
                    // Number of tests
                    builder.Append(" (");
                    builder.Append(numLeaves[result]);
                    builder.Append(" ");
                    builder.Append(TestOrTests(numLeaves[result]));
                    builder.Append(")");

                    // Number of failures
                    if (failedChildren[result] > 0)
                    {
                        builder.Append(" Failed: ");
                        builder.Append(failedChildren[result]);
                        builder.Append(" ");
                        builder.Append(TestOrTests(failedChildren[result]));
                        builder.Append("failed");
                    }
                }

                builder.Append(" ").Append(result.Result.Status);
                if (result.Result.Message != null) builder.Append(" ").Append(result.Result.Message);

                return builder.ToString();
            }

            void LogRecursively(ITestResult result, int level)
            {
                void Print(string message, LogLevel logLevel)
                {
                    this._monitor.Log(
                        new StringBuilder(this._indent.Length * level)
                            .Insert(0, this._indent, level)
                            .Append(message)
                            .ToString(),
                        logLevel
                    );
                }

                if (result.IsAggregate)
                {
                    Print(GetLogLine(result), failedChildren[result] > 0 ? LogLevel.Error : LogLevel.Info);
                    foreach (ITestResult child in result.Children)
                    {
                        LogRecursively(child, level + 1);
                    }
                }
            }

            Preprocess(resultRoot);
            LogRecursively(resultRoot, 0);
        }
    }
}
