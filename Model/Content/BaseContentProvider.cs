/*
 * Doto-Content-Unlocker, tool for unlocking custom dota2 content.
 * 
 *  Copyright (c) 2014 Teq, https://github.com/Teq2/Doto-Content-Unlocker
 * 
 *  Permission is hereby granted, free of charge, to any person obtaining a copy of 
 *  this software and associated documentation files (the "Software"), to deal in the 
 *  Software without restriction, including without limitation the rights to use, copy, 
 *  modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, 
 *  and to permit persons to whom the Software is furnished to do so, subject to the 
 *  following conditions:
 * 
 *  The above copyright notice and this permission notice shall be included in 
 *  all copies or substantial portions of the Software.
 *  
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
 *  INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
 *  PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
 *  HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 *  CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE 
 *  OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using Doto_Unlocker.VDF;
using Doto_Unlocker.VPK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doto_Unlocker.Model
{
    interface IContentProvider
    {
        string ID { get; }

        IEnumerable<IGraphicContent> GetContentList();
        System.Drawing.Image GetInstalledContentImage();
        IGraphicContent InstallContent(int ID);
    }

    abstract class ContentProvider : IContentProvider
    {
        protected readonly VpkArchive arc;
        protected readonly VdfNode schema;
        protected readonly string dotaDataPath;

        public abstract string ID { get; }
        public abstract IEnumerable<IGraphicContent> GetContentList();
        public abstract System.Drawing.Image GetInstalledContentImage();
        public abstract IGraphicContent InstallContent(int ID);

        public ContentProvider(VpkArchive vpk, VdfNode schema, string dotaDataPath)
        {
            this.arc = vpk;
            this.schema = schema;
            this.dotaDataPath = dotaDataPath;
        }
    }
}
