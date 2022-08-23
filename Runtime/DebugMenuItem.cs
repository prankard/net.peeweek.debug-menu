using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DebugMenuUtility
{
    public abstract class DebugMenuItem
    {
        public abstract string label { get; }
        public virtual string value => string.Empty;

        public virtual Action OnValidate => null;
        public virtual Action OnLeft => null;
        public virtual Action OnRight => null;
    }

    public class DebugMenuItemAction : DebugMenuItem
    {
        public override string label => _label;
        public override Action OnValidate => _action;

        private Action _action;
        private string _label;
        public DebugMenuItemAction(string label, Action action)
        {
            this._label = label;
            this._action = action;
        }
    }

    public class DebugMenuItemString : DebugMenuItem
    {
        public override string value => Getter?.Invoke();

        public override string label => _label;

        private string _label;

        public Func<float> Getter;

        public DebugMenuItemFloat(string label, Func<string> Getter)
        {
            this._label = label;
            this.Getter = Getter;
        }
    }

    public class DebugMenuItemInt : DebugMenuItem
    {
        public override string value => Getter?.Invoke().ToString();

        public override string label => _label;

        private string _label;
        private int increase, decrease, min, max;

        public Action<int> Setter;
        public Func<int> Getter;

        public DebugMenuItemFloat(string label, Func<int> Getter, Action<int> Setter, int increase = 1, int decrease = -1, int min = 0, int max = int.MaxValue)
        {
            this._label = label;
            this.Setter = Setter;
            this.Getter = Getter;
            this.increase = increase;
            this.decrease = decrease;
            this.min = min;
            this.max = max;
        }

        public override Action OnValidate => Increase;
        public override Action OnLeft => Decrease;
        public override Action OnRight => Increase;

        void Increase() => Toggle(increase);
        void Decrease() => Toggle(decrease);
        void Toggle(int pad = 1)
        {
            var value = (int)Mathf.Clamp(pad + Getter.Invoke(), min, max);
            Setter.Invoke(value);
        }
    }

    public class DebugMenuItemFloat : DebugMenuItem
    {
        public override string value => Getter?.Invoke().ToString();

        public override string label => _label;

        private string _label;
        private float increase, decrease, min, max;

        public Action<float> Setter;
        public Func<float> Getter;

        public DebugMenuItemFloat(string label, Func<float> Getter, Action<float> Setter, float increase = 1, float decrease = -1, float min = 0, float max = Mathf.Infinity)
        {
            this._label = label;
            this.Setter = Setter;
            this.Getter = Getter;
            this.increase = increase;
            this.decrease = decrease;
            this.min = min;
            this.max = max;
        }

        public override Action OnValidate => Increase;
        public override Action OnLeft => Decrease;
        public override Action OnRight => Increase;

        void Increase() => Toggle(increase);
        void Decrease() => Toggle(decrease);
        void Toggle(float pad = 1)
        {
            var value = Mathf.Clamp(pad + Getter.Invoke(), min, max);
            Setter.Invoke(value);
        }
    }

    public class DebugMenuItemBool : DebugMenuItem
    {
        public override string value => Getter?.Invoke().ToString();

        public override string label => _label;

        private string _label;

        public Action<bool> Setter;
        public Func<bool> Getter;

        public DebugMenuItemBool(string label, Func<bool> Getter, Action<bool> Setter, bool loop = true)
        {
            this._label = label;
            this.Setter = Setter;
            this.Getter = Getter;
        }

        public override Action OnValidate => Toggle;
        public override Action OnLeft => Toggle;
        public override Action OnRight => Toggle;

        void Toggle()
        {
            var value = Getter.Invoke();
            Setter.Invoke(!value);
        }
    }

    public class DebugMenuItemEnum<T> : DebugMenuItem where T : Enum
    {
        public override string value => Getter?.Invoke().ToString();
        public override string label => _label;

        private string _label;

        public Action<T> Setter;
        public Func<T> Getter;

        private string[] names;
        private Array values;
        private int length;

        private bool loop;

        public DebugMenuItemEnum(string label, Func<T> Getter, Action<T> Setter, bool loop = true)
        {
            names = Enum.GetNames(typeof(T));
            values = Enum.GetValues(typeof(T));
            length = names.Length;
            this._label = label;
            this.Setter = Setter;
            this.Getter = Getter;
            this.loop = loop;
        }

        public override Action OnValidate => Increase;
        public override Action OnLeft => Decrease;
        public override Action OnRight => Increase;

        void Increase() => Toggle(1);
        void Decrease() => Toggle(-1);
        void Toggle(int pad = 1)
        {
            int index = Array.IndexOf(values, Getter.Invoke());
            index += pad;
            if (!loop)
            {
                if (index < 0 || index >= length)
                    return; // invalid
            }
            index = (int)Mathf.Repeat(index, length);
            Setter?.Invoke((T)values.GetValue(index));
        }
    }
}
