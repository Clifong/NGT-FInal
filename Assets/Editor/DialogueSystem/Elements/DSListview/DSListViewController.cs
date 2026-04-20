using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements.DSListview
{
    public class DSListViewController : ListViewController
    {
        public override void RemoveItem(int index)
        {
            // foreach (var visualElement in view.Children().ToArray())
            // {
            //     Debug.Log(visualElement);  
            // }
            // foreach (var visualElement in listView.Children())
            // {
            //     Debug.Log(visualElement);
            // }
            base.RemoveItem(index);
        }

        public override void RemoveItems(List<int> indices)
        {
            // foreach (var index in indices)
            // {
            //     Debug.Log(index); 
            // }
            base.RemoveItems(indices);
        }
        
    }
}