using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using WPFInk.tool;
using System.Windows.Ink;

namespace WPFInk.ink
{
    public class ImageConnector
    {
        private MyImage source;
        private MyImage target;
        private MyStroke stroke;

        public ImageConnector(MyImage source, MyImage target)
        {
            this.source = source;
            this.target = target;
            StylusPointCollection collection = Connector.getInstance().getImageConnector(source, target);
            stroke = new MyStroke(new Stroke(collection));
            source.addConnector(this);
            target.addConnector(this);
        }


        

        public void setStyluspointCollection(StylusPointCollection collection)
        {
            stroke.Stroke.StylusPoints = collection;
            //stroke.Stroke.wi
            
        }

        public void adjustConnector()
        {
            StylusPointCollection collection = Connector.getInstance().getImageConnector(source, target);
            setStyluspointCollection(collection);
        }

        public MyStroke MYSTROKE
        {
            get
            {
                return stroke;
            }
        }

        public MyImage Source
        {
            get
            {
                return source;
            }
        }
        public MyImage Target
        {
            get
            {
                return target;
            }
        }
    }
}
