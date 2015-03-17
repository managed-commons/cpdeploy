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
using Commons.GetOptions;

namespace cpdeploy
{
	internal class CopyDeployerOptions : Options
	{
		public CopyDeployerOptions(string[] args)
			: base(args, OptionsParsingMode.Linux | OptionsParsingMode.GNU_DoubleDash, false, true, true)
		{
		}

		[Option("Clean target directory", "clean", ShortForm = 'c')]
		public bool CleanTarget { get; set; }

		[Option("Path to {directory} to copy from (default: current directory)", "from", ShortForm = 'f')]
		public string From { get; set; }

		public bool IsOk
		{
			get
			{
				if (string.IsNullOrWhiteSpace(FirstArgument)) {
					DoHelp();
					return false;
				}
				return true;
			}
		}

		[Option("Quiet mode", "quiet", ShortForm = 'q')]
		public bool Quiet { get; set; }

		[Option("Summary mode - Only the summary line", "summary", ShortForm = 's')]
		public bool Summary { get; set; }

		public string To { get { return FirstArgument; } }

		[Option("Verbose output", "verbose", ShortForm = 'v')]
		public bool Verbose { get; set; }

		[KillInheritedOption]
		public override WhatToDoNext DoHelp2()
		{
			return base.DoHelp2();
		}

		[KillInheritedOption]
		public override WhatToDoNext DoUsage()
		{
			return base.DoUsage();
		}

		protected override void InitializeOtherDefaults()
		{
			base.InitializeOtherDefaults();
			From = Directory.GetCurrentDirectory();
			Verbose = false;
		}
	}
}