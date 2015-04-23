cpdeploy
========

A copy deployer that tries not to copy unchanged content, written in C#.

Usage
-----

    cpdeploy  1.1.4 - Copyright ©2015 Rafael 'Monoman' Teixeira, Managed Commons Team
    A copy deployer that tries not to copy unchanged content.

    License: MIT License - See http://opensource.org/licenses/MIT

    Usage: cpdeploy [options] <path of directory to deploy into>
    Options:
      -b -backup:subdirectory  Name of subdirectory to copy older versions with -o (default: _OLDER).
      -c -clean                Clean target directory
      -d -dontoverwrite        Do not overwrite if there is a target directory
      -f -from:directory       Path to directory to copy from (default: current directory)
      -? -help                 Show this help list
      -o -onlylatest           Move older versions to backup folder (use -b to choose backup folder).
                               Skips pinned dirs (those ending in '-PINNED')
      -q -quiet                Quiet mode
      -s -summary              Summary mode - Only the summary line
      -t -test                 Test if there is anything to update
      -v -verbose              Verbose output
      -V -version              Display version and licensing information

    Please report bugs at <https://github.com/managed-commons/cpdeploy/issues>

License: MIT
------------

Copyright (c) 2015 Managed Commons

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
