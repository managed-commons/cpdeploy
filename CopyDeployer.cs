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
                if (!Directory.Exists(ParentDir(to))) {
                    Quiet("Path to deploy into is not a valid directory: {0}", to);
                    return 3;
                }
                if (_options.Test) {
                    if (HaveEqualContents(from, to)) {
                        Summary("No files need to be updated");
                    } else {
                        Summary("Some files need to be updated");
                        return 4;
                    }
                } else if (_options.DontOverWrite && Directory.Exists(to)) {
                    Summary("Skipping existing target directory: {0}", to);
                } else {
                    Quiet("Deploying from {0} into {1}", from, to);
                    if (_options.CleanTarget && Directory.Exists(to)) {
                        Quiet("Removing previous content at: {0}", to);
                        Directory.Delete(to, true);
                    }
                    if (_options.OnlyLatest)
                        MoveSimilarFolders(to, _options.BackupDir);
                    DeployDir(from, to);
                    if (_filesSkipped == 0)
                        Summary("{0} files copied", _filesCopied);
                    else
                        Summary("{0} files copied, {1} files skipped", _filesCopied, _filesSkipped);
                }
                return 0;
            } catch (Exception e) {
                Quiet("Exception: {0}", e);
                return 255;
            }
        }

        private int _filesCopied = 0;

        private int _filesSkipped = 0;

        private CopyDeployerOptions _options;

        private static bool IsPinned(string dir)
        {
            return dir.ToLower().EndsWith("-pinned");
        }

        private static string LastDir(string dir)
        {
            return Path.GetFileName(dir);
        }

        private static string ParentDir(string to)
        {
            return Path.GetDirectoryName(to);
        }

        private static string RemoveVersion(string path)
        {
            return new string(path.Reverse().SkipWhile(c => char.IsDigit(c) || c == '.').Reverse().ToArray());
        }

        private void DeployDir(string from, string to)
        {
            if (!Directory.Exists(to)) {
                Directory.CreateDirectory(to);
            }
            foreach (var dir in Directory.EnumerateDirectories(from)) {
                var toSubDir = Path.Combine(to, Path.GetFileName(dir));
                Verbose("Copying directory {0} to {1}", dir, toSubDir);
                DeployDir(dir, toSubDir);
            }
            foreach (var file in Directory.EnumerateFiles(from)) {
                var toFile = Path.Combine(to, Path.GetFileName(file));
                if (File.Exists(toFile)) {
                    if (FilesMatch(file, toFile)) {
                        _filesSkipped++;
                        continue;
                    } else {
                        File.Delete(toFile);
                    }
                }
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

        private bool HaveEqualContents(string from, string to)
        {
            if (!Directory.Exists(to)) {
                return false;
            }
            foreach (var dir in Directory.EnumerateDirectories(from)) {
                var toSubDir = Path.Combine(to, Path.GetFileName(dir));
                Verbose("Comparing directory {0} to {1}", dir, toSubDir);
                if (!HaveEqualContents(dir, toSubDir))
                    return false;
            }
            foreach (var file in Directory.EnumerateFiles(from)) {
                var toFile = Path.Combine(to, Path.GetFileName(file));
                if (File.Exists(toFile) && FilesMatch(file, toFile)) {
                    continue;
                }
                return false;
            }
            return true;
        }

        private void MoveDir(string dir, string target)
        {
            try {
                Directory.Move(dir, target);
            } catch (Exception e) {
                Quiet("Failed to move '{0}' to '{1}' - Continuing...", dir, target);
            }
        }

        private void MoveSimilarFolders(string latestPath, string backupDir)
        {
            var baseDir = ParentDir(latestPath);
            var backupPath = Path.Combine(baseDir, backupDir);
            var mask = RemoveVersion(LastDir(latestPath)) + ".*";
            foreach (var dir in Directory.EnumerateDirectories(baseDir, mask))
                if (!(dir.Equals(latestPath) || IsPinned(dir))) {
                    if (!Directory.Exists(backupPath)) {
                        Quiet("Creating backup dir '{0}'", backupPath);
                        Directory.CreateDirectory(backupPath);
                    }
                    var dirName = LastDir(dir);
                    var target = Path.Combine(backupPath, dirName);
                    Quiet("Moving older version '{0}' to '{1}'", dirName, backupDir);
                    MoveDir(dir, target);
                }
        }

        private void Quiet(string format, params object[] parameters)
        {
            WriteLineIf(!_options.Quiet, format, parameters);
        }

        private void Summary(string format, params object[] parameters)
        {
            WriteLineIf(_options.Summary || !_options.Quiet, format, parameters);
        }

        private void Verbose(string format, params object[] parameters)
        {
            WriteLineIf(_options.Verbose, format, parameters);
        }

        private void WriteLineIf(bool condition, string format, object[] parameters)
        {
            if (condition)
                Console.WriteLine(string.Format(format, parameters));
        }
    }
}