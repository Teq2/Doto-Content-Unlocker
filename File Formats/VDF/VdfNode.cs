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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Doto_Unlocker.VDF
{
    class VdfNode 
    {
        public String Key;
        public String Val;
        public NodeList ChildNodes;

        public static implicit operator String(VdfNode node)
        {
            return node.Val;
        }

        public VdfNode this[int Idx]
        {
            get
            {
                return ChildNodes[Idx];
            }
        }

        public VdfNode this[string Key]
        {
            get
            {
                return ChildNodes.SingleOrDefault((cNode) => cNode.Key.Equals(Key));
            }
        }
    }

    class NodeList : List<VdfNode>
    {
        public NodeList(): base() {   }
        
        public NodeList(IEnumerable<VdfNode> src): base (src) {   }

        //public VDFNode this[string Key]
        //{
        //    get
        //    {
        //        // query form: "from cNode in this from cNodeList in cNode[Key] select cNodeList"
        //        return new NodeList(this.Where((cNode) => cNode.ChildNodes != null).SelectMany(cNode => cNode[Key]));
        //    }
        //}
    }
}
