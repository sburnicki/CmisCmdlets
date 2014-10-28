// CmisCmdlets - Cmdlets to use CMIS from Powershell and Pash
// Copyright (C) GRAU DATA 2013-2014
//
// Author(s): Stefan Burnicki <stefan.burnicki@graudata.com>
//
// This Source Code Form is subject to the terms of the
// Mozilla Public License, v. 2.0. If a copy of the MPL was
// not distributed with this file, You can obtain one at
//  http://mozilla.org/MPL/2.0/.
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CmisCmdlets.Test
{
    public class FileSystemTestHelper
    {
        private List<string> _createdFiles;

        public FileSystemTestHelper()
        {
            _createdFiles = new List<string>();
        }

        public void RegisterTempFile(params string[] paths)
        {
            _createdFiles.AddRange(paths);
        }

        public void ForgetTempFiles()
        {
            _createdFiles.Clear();
        }

        public void CleanUp()
        {
            for (int i = _createdFiles.Count; i >= 0; i--)
            {
                File.Delete(_createdFiles[i]);
            }
            _createdFiles.Clear();
        }

        public void CreateTempFile(string path, string content)
        {
            CreateTempFile(path, content, Encoding.UTF8);
        }

        public void CreateTempFile(string path, string content, Encoding encoding)
        {
            File.WriteAllText(path, content, encoding);
            _createdFiles.Add(path);
        }
    }
}

