package com.paleon.engine.toolbox;
public final class ReflectionUtils
{
    private ReflectionUtils()
    {
    }

    public static boolean isInstanceOf(Class<?> clazz, Object o)
    {
        if ((clazz == null) || (o == null))
            return false;

        if (clazz.isInterface())
            throw new UnsupportedOperationException();

        Class<?> oClazz = o.getClass();

        while (oClazz != null)
        {
            if (oClazz == clazz)
                return true;

            oClazz = oClazz.getSuperclass();
        }

        return false;
    }
}