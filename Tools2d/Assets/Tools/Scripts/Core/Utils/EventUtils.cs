using System;

namespace Scripts.Core.Utils
{
    public static class EventUtils
    {
        public static void Call(this Action action)
        {
            if (action != null)
                action();
        }

        public static void Call<T>(this Action<T> action, T argument)
        {
            if (action != null)
                action(argument);
        }

        public static void Call<T, K>(this Action<T, K> action, T argument1, K argument2)
        {
            if (action != null)
                action(argument1, argument2);
        }
    }
}
