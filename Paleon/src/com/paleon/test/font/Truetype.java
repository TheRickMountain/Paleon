package com.paleon.test.font;
import static org.lwjgl.glfw.GLFW.glfwPollEvents;
import static org.lwjgl.glfw.GLFW.glfwSwapBuffers;
import static org.lwjgl.glfw.GLFW.glfwWindowShouldClose;
import static org.lwjgl.opengl.GL11.GL_ALPHA;
import static org.lwjgl.opengl.GL11.GL_BLEND;
import static org.lwjgl.opengl.GL11.GL_COLOR_BUFFER_BIT;
import static org.lwjgl.opengl.GL11.GL_LINEAR;
import static org.lwjgl.opengl.GL11.GL_ONE_MINUS_SRC_ALPHA;
import static org.lwjgl.opengl.GL11.GL_QUADS;
import static org.lwjgl.opengl.GL11.GL_SRC_ALPHA;
import static org.lwjgl.opengl.GL11.GL_TEXTURE_2D;
import static org.lwjgl.opengl.GL11.GL_TEXTURE_MAG_FILTER;
import static org.lwjgl.opengl.GL11.GL_TEXTURE_MIN_FILTER;
import static org.lwjgl.opengl.GL11.GL_UNSIGNED_BYTE;
import static org.lwjgl.opengl.GL11.glBegin;
import static org.lwjgl.opengl.GL11.glBindTexture;
import static org.lwjgl.opengl.GL11.glBlendFunc;
import static org.lwjgl.opengl.GL11.glClear;
import static org.lwjgl.opengl.GL11.glClearColor;
import static org.lwjgl.opengl.GL11.glColor3f;
import static org.lwjgl.opengl.GL11.glEnable;
import static org.lwjgl.opengl.GL11.glEnd;
import static org.lwjgl.opengl.GL11.glGenTextures;
import static org.lwjgl.opengl.GL11.glPopMatrix;
import static org.lwjgl.opengl.GL11.glPushMatrix;
import static org.lwjgl.opengl.GL11.glScalef;
import static org.lwjgl.opengl.GL11.glTexCoord2f;
import static org.lwjgl.opengl.GL11.glTexImage2D;
import static org.lwjgl.opengl.GL11.glTexParameteri;
import static org.lwjgl.opengl.GL11.glTranslatef;
import static org.lwjgl.opengl.GL11.glVertex2f;
import static org.lwjgl.stb.STBTruetype.stbtt_BakeFontBitmap;
import static org.lwjgl.stb.STBTruetype.stbtt_GetBakedQuad;
import static org.lwjgl.system.MemoryStack.stackPush;

import java.io.IOException;
import java.nio.ByteBuffer;
import java.nio.FloatBuffer;

import org.lwjgl.BufferUtils;
import org.lwjgl.stb.STBTTAlignedQuad;
import org.lwjgl.stb.STBTTBakedChar;
import org.lwjgl.system.MemoryStack;

import com.paleon.engine.toolbox.IOUtil;

/** STB Truetype demo. */
public final class Truetype extends FontDemo {

	private Truetype(String filePath) {
		super(24, filePath);
	}

	public static void main(String[] args) {
		String filePath;
		if ( args.length == 0 ) {
			System.out.println("Use 'ant demo -Dclass=org.lwjgl.demo.stb.Truetype -Dargs=<path>' to load a different text file (must be UTF8-encoded).\n");
			filePath = "/fonts/README.md";
		} else
			filePath = args[0];

		new Truetype(filePath).run("STB Truetype Demo");
	}

	private STBTTBakedChar.Buffer init(int BITMAP_W, int BITMAP_H) {
		int texID = glGenTextures();
		STBTTBakedChar.Buffer cdata = STBTTBakedChar.malloc(96);

		try {
			ByteBuffer ttf = IOUtil.ioResourceToByteBuffer("/fonts/ArialMT.ttf", 160 * 1024);

			ByteBuffer bitmap = BufferUtils.createByteBuffer(BITMAP_W * BITMAP_H);
			stbtt_BakeFontBitmap(ttf, getFontHeight(), bitmap, BITMAP_W, BITMAP_H, 32, cdata);

			glBindTexture(GL_TEXTURE_2D, texID);
			glTexImage2D(GL_TEXTURE_2D, 0, GL_ALPHA, BITMAP_W, BITMAP_H, 0, GL_ALPHA, GL_UNSIGNED_BYTE, bitmap);
			glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR);
			glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR);
		} catch (IOException e) {
			throw new RuntimeException(e);
		}

		glClearColor(43f / 255f, 43f / 255f, 43f / 255f, 0f); // BG color
		glColor3f(169f / 255f, 183f / 255f, 198f / 255f); // Text color

		glEnable(GL_TEXTURE_2D);
		glEnable(GL_BLEND);
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);

		return cdata;
	}

	@Override
	protected void loop() {
		int BITMAP_W = 512;
		int BITMAP_H = 512;

		STBTTBakedChar.Buffer cdata = init(BITMAP_W, BITMAP_H);

		while ( !glfwWindowShouldClose(getWindow()) ) {
			glfwPollEvents();

			glClear(GL_COLOR_BUFFER_BIT);

			float scaleFactor = 1.0f + getScale() * 0.25f;

			glPushMatrix();
			// Zoom
			glScalef(scaleFactor, scaleFactor, 1f);
			// Scroll
			glTranslatef(4.0f, getFontHeight() * 0.5f + 4.0f - getLineOffset() * getFontHeight(), 0f);

			renderText(cdata, BITMAP_W, BITMAP_H);

			glPopMatrix();

			glfwSwapBuffers(getWindow());
		}

		cdata.free();
	}

	private void renderText(STBTTBakedChar.Buffer cdata, int BITMAP_W, int BITMAP_H) {
		try ( MemoryStack stack = stackPush() ) {
			FloatBuffer x = stack.floats(0.0f);
			FloatBuffer y = stack.floats(0.0f);

			STBTTAlignedQuad q = STBTTAlignedQuad.mallocStack(stack);

			glBegin(GL_QUADS);
			for ( int i = 0; i < text.length(); i++ ) {
				char c = text.charAt(i);
				if ( c == '\n' ) {
					y.put(0, y.get(0) + getFontHeight());
					x.put(0, 0.0f);
					continue;
				} else if ( c < 32 || 128 <= c )
					continue;
				stbtt_GetBakedQuad(cdata, BITMAP_W, BITMAP_H, c - 32, x, y, q, true);

				glTexCoord2f(q.s0(), q.t0());
				glVertex2f(q.x0(), q.y0());

				glTexCoord2f(q.s1(), q.t0());
				glVertex2f(q.x1(), q.y0());

				glTexCoord2f(q.s1(), q.t1());
				glVertex2f(q.x1(), q.y1());

				glTexCoord2f(q.s0(), q.t1());
				glVertex2f(q.x0(), q.y1());
			}
			glEnd();
		}
	}

}