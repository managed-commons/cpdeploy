// cpdeploy
//
// Copyright (c) 2015 Rafael 'Monoman' Teixeira, Managed Commons Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace cpdeploy
{
	internal class CopyDeployer
	{
		public CopyDeployer(CopyDeployerOptions options)
		{
			_options = options;
			if (_options.Summary)
				_options.Quiet = true;
			if (_options.Quiet)
				_options.Verbose = false;
		}

		public int Execute()
		{
			try {
				var from = Path.GetFullPath(_options.From);
				var to = Path.GetFullPath(_options.To);
				if (!Directory.Exists(from)) {
					Quiet("Path to copy from is not a valid directory: {0}", from);
					return 2;
				}
				if (!Directory.Exists(Path.GetDirectoryName(to))) {
					Quiet("Path to deploy into is not a valid directory: {0}", to);
					return 3;
				}
				Quiet("Deploying from {0} into {1}", from, to);
				if (_options.CleanTarget && Directory.Exists(to)) {
					Quiet("Removing previous content at: {0}", to);
					Directory.Delete(to, true);
				}
				DeployDir(from, to);
				Summary("{0} files copied, {1} files skipped", _filesCopied, _filesSkipped);
				return 0;
			} catch (Exception e) {
				Quiet("Exception: {0}", e);
				return 255;
			}
		}

		private int _filesCopied = 0;
		private int _filesSkipped = 0;
		private CopyDeployerOptions _options;

		private void DeployDir(string from, string to)
		{
			if (!Directory.Exists(to))
				Directory.CreateDirectory(to);
			foreach (var dir in Directory.EnumerateDirectories(from)) {
				var toSubDir = Path.Combine(to, Path.GetFileName(dir));
				Verbose("Copying directory {0} to {1}", dir, toSubDir);
				DeployDir(dir, toSubDir);
			}
			foreach (var file in Directory.EnumerateFiles(from)) {
				var toFile = Path.Combine(to, Path.GetFileName(file));
				if (File.Exists(toFile))
					if (FilesMatch(file, toFile)) {
						_filesSkipped++;
						continue;
					} else
						File.Delete(toFile);
				Verbose("Copying file {0} to {1}", file, toFile);
				File.Copy(file, toFile);
				_filesCopied++;
			}
		}

		private bool FilesMatch(string file, string toFile)
		{
			return HashFile(file).SequenceEqual(HashFile(toFile));
		}

		private byte[] HashFile(string file)
		{
			using (var md5 = MD5.Create())
			using (var stream = File.OpenRead(file))
				return md5.ComputeHash(stream);
		}

		private void Summary(string format, params object[] parameters)
		{
			WriteLine(_options.Summary, format, parameters);
		}

		private void Quiet(string format, params object[] parameters)
		{
			WriteLine(!_options.Quiet, format, parameters);
		}

		private void Verbose(string format, params object[] parameters)
		{
			WriteLine(_options.Verbose, format, parameters);
		}

		private void WriteLine(bool condition, string format, object[] parameters)
		{
			if (condition)
				Console.WriteLine(string.Format(format, parameters));
		}
	}
}