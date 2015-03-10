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
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Doto_Unlocker.VDF
{
    class VDFParserException : Exception, ISerializable
    {
        public VDFParserException():base() { }
        public VDFParserException(string message) : base() { }
        public VDFParserException(string message, Exception inner) : base() { }
        protected VDFParserException(SerializationInfo info, StreamingContext context) : base() { }
    }

    static class VdfParser
    {
        enum Token { none, Key, Val, GroupStart, GroupEnd }

        /// <summary>
        /// Removed supporting unquoted tokens
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static VdfNode Parse(string data)
        {
            Stack<VdfNode> hierarchy = new Stack<VdfNode>();
            VdfNode rootNode = null; // volvo always uses only 1 root node
            VdfNode parentNode = null;
            VdfNode currentNode = null;
            Token lastToken = Token.none;
            int len = data.Length;

            for (int pos = 0; pos < len; )
            {
                char el = data[pos];
                pos++; // early increment (little optimization)

                switch (el)
                {
                    case '"':
                        if (lastToken != Token.Key)
                        {
                            // new key-value node
                            currentNode = new VdfNode();

                            if (parentNode != null)
                            {
                                // is it first childnode?
                                if (parentNode.ChildNodes == null) parentNode.ChildNodes = new NodeList();
                                // add this node to the list
                                parentNode.ChildNodes.Add(currentNode);
                            }
                            else
                            {   // it's first node in the tree
                                rootNode = currentNode;
                            }

                            int endOfKey = data.IndexOf('"', pos);
                            if (endOfKey == -1) throw new VDFParserException(); // unclosed token

                            currentNode.Key = data.Substring(pos, endOfKey - pos);
                            pos = endOfKey + 1;
                            lastToken = Token.Key;
                        }
                        else // add value to the node
                        {
                            int endOfVal;
                            for (int offset = 0; ;offset = endOfVal-pos+1)
                            {
                                endOfVal = data.IndexOf('"', pos + offset);
                                if (endOfVal == -1) throw new VDFParserException(); // unclosed token
                                
                                // skip \" sequence
                                if (data[endOfVal - 1] != '\\') break;
                            }

                            currentNode.Val = data.Substring(pos, endOfVal - pos);
                            pos = endOfVal + 1;
                            lastToken = Token.Val;
                        }
                        break;

                    case '{':
                        hierarchy.Push(parentNode); // save parent
                        parentNode = currentNode;
                        lastToken = Token.GroupStart;
                        break;

                    case '}':
                        if (parentNode != null)
                            parentNode = hierarchy.Pop(); // restore parent
                        lastToken = Token.GroupEnd;
                        break;

                    case '\n':
                        lastToken = Token.none;
                        break;
                }
            }

            return rootNode;
        }
    }
}
