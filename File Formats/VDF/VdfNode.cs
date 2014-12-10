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
