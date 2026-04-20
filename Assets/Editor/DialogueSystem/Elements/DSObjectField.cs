using System;
using DS.Windows;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace DS.Elements
{
    public class DSObjectField : ObjectField
    {
        public DSObjectField(string label, Type objectType, Action<ChangeEvent<Object>> registerValueCallback = null, Object value = null)
        {
            this.objectType = objectType;
            this.label = label;
            this.value = value;
            this.RegisterValueChangedCallback(eval =>
            {
                registerValueCallback?.Invoke(eval);
            });
        }

        public void Reset()
        {
            this.value = null;
        }
    }
}