using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace FunctionAnalyser
{
	class Program
	{
		static void Main(string[] args)
		{
			int linesTotal = 0;
			int commandsTotal = 0;
			int commentsTotal = 0;
			int emptyTotal = 0;
			int filesTotal = 0;
			int entitySelectorsTotal = 0;
			int nbtAccessTotal = 0;

			string path = "./";

			Dictionary<string, int> commandsUsed = new Dictionary<string, int>();
			Dictionary<string, int> commandsUsedAfterExecute = new Dictionary<string, int>();

			if (args.Length > 0)
			{
				path = args[0];
			}

			Console.WriteLine("Analysing all function files in path " + path + " ...\n\n");

			string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
			foreach (string file in files)
			{
				if (file.EndsWith(".mcfunction"))
				{
					filesTotal++;
					string[] lines = File.ReadAllLines(file);
					linesTotal += lines.Length;
					foreach (string line in lines)
					{
						if (line.Length == 0)
						{
							emptyTotal++;
						}
						else
						{
							if (Char.IsWhiteSpace(line, 0))
							{
								emptyTotal++;
							}
							else if (line[0] == '#')
							{
								commentsTotal++;
							}
							else
							{
								commandsTotal++;
								string command = line.Split(' ')[0];

								if (command == "execute")
								{
									string[] tmp = line.Split(new string[] {"run "}, StringSplitOptions.RemoveEmptyEntries);
									if (tmp.Length > 1)
									{
										command = tmp[1].Split(' ')[0];
										if (commandsUsedAfterExecute.ContainsKey(command))
										{
											commandsUsedAfterExecute[command]++;
										}
										else
										{
											commandsUsedAfterExecute.Add(command, 1);
										}
									}
								}
								if (commandsUsed.ContainsKey(command))
								{
									commandsUsed[command]++;
								}
								else
								{
									commandsUsed.Add(command, 1);
								}
								if (line.Contains("@e"))
								{
									entitySelectorsTotal++;
								}
								if (line.Contains("{") && (command != "tellraw" && command != "title") || command == "data" || line.StartsWith("execute store result entity"))
								{
									nbtAccessTotal++;
								}
							}
						}
					}
				}
			}

			string result = String.Format("Found {0} mcfunction files that include a total amount of {1} lines. Of these lines\n"
			+ "+ {2} are commands ({5}%)\n"
			+ "+ {3} are comments ({6}%)\n"
			+ "+ {4} are empty ({7}%)\n\n",
			new object[] { filesTotal, linesTotal, commandsTotal, commentsTotal, emptyTotal,
			Math.Round((float)commandsTotal * 100 / linesTotal,2), Math.Round((float)commentsTotal * 100 / linesTotal,2), Math.Round((float)emptyTotal * 100 / linesTotal,2) });

			result += "-------------------------------\n\nUsed Commands\n";

			List<KeyValuePair<string, int>> commandsList = commandsUsed.ToList();
			commandsList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));

			// List<KeyValuePair<string, int>> executeList = commandsUsedAfterExecute.ToList();
			// executeList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value));


			foreach (KeyValuePair<string, int> commandPair in commandsList)
			{
				result += "+ " + commandPair.Key + ": " + commandPair.Value + " (" + Math.Round((float)commandPair.Value * 100 / commandsTotal, 2) + "%)";
				if(commandsUsedAfterExecute.ContainsKey(commandPair.Key)){
					result += " (" + commandsUsedAfterExecute[commandPair.Key] + " behind execute)";

				}
				result += "\n";
			}
			result += "\n-------------------------------\n\nPerformance Information\n";

			float averageEntitySelector = (float)Math.Round((float)entitySelectorsTotal / filesTotal, 2);
			float averageNBTAccess = (float)Math.Round((float)nbtAccessTotal / filesTotal, 2);
			result += "Usage of @e selectors: " + entitySelectorsTotal + ", that's an average of " + averageEntitySelector + " per file.\n";
			result += "Amount of NBT access: " + nbtAccessTotal + ", that's an average of " + averageNBTAccess + " per file.\n";

			Console.WriteLine(result);
			Console.WriteLine("Press any key to close");
			Console.ReadKey();
		}
	}
}
