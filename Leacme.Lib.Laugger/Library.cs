// Copyright (c) 2017 Leacme (http://leac.me). View LICENSE.md for more information.
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Dasync.Collections;

namespace Leacme.Lib.Laugger {

	public class Library {

		public Library() {

		}

		/// <summary>
		/// Retrieve the machine system logs.
		/// </summary>
		/// <param name="logName"></param>
		/// <param name="logMessages"></param>
		/// <returns></returns>
		public async Task<IEnumerable<(string logName, IEnumerable<string> logMessages)>> GetLocalLogs() {
			var logs = new ConcurrentBag<(string logName, IEnumerable<string> logMessages)>();
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
				Parallel.ForEach(EventLog.GetEventLogs(), z => {
					try {
						logs.Add((z.LogDisplayName, z.Entries.Cast<EventLogEntry>().Select(zz => zz.TimeGenerated + ": " + zz.Message)));
					} catch {

					}
				});
			} else {
				await Directory.GetFiles("/var/log", "*", SearchOption.AllDirectories).ParallelForEachAsync(
					async z => {
						try {
							logs.Add((z, await File.ReadAllLinesAsync(z)));
						} catch {

						}
					}, 5);
			}
			return logs.ToList().OrderBy(z => z.logName);
		}

	}
}