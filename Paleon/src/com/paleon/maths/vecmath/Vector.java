/*
 * Decompiled with CFR 0_114.
 */
package com.paleon.maths.vecmath;

import java.io.Serializable;
import java.nio.FloatBuffer;

public abstract class Vector
implements Serializable,
ReadableVector {
    protected Vector() {
    }

    public final float length() {
        return (float)Math.sqrt(this.lengthSquared());
    }

    public abstract float lengthSquared();

    public abstract Vector load(FloatBuffer var1);

    public abstract Vector negate();

    public final Vector normalise() {
        float len = this.length();
        if (len != 0.0f) {
            float l = 1.0f / len;
            return this.scale(l);
        }
        throw new IllegalStateException("Zero length vector");
    }

    public abstract Vector store(FloatBuffer var1);

    public abstract Vector scale(float var1);
}

