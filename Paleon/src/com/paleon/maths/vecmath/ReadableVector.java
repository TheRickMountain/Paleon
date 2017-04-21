/*
 * Decompiled with CFR 0_114.
 */
package com.paleon.maths.vecmath;

import java.nio.FloatBuffer;

public interface ReadableVector {
    public float length();

    public float lengthSquared();

    public Vector store(FloatBuffer var1);
}

