using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFInk
{
    class HyperNode
    {
        public HyperNode()
        {
        }
        Node sourceNode;
        public void setSourceNode(Node sourceNode)
        {
            this.sourceNode = sourceNode;
        }
        public Node getSourceNode()
        {
            return sourceNode;
        }
    }
}
