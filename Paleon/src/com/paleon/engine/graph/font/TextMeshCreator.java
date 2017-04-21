package com.paleon.engine.graph.font;

import java.io.File;
import java.util.ArrayList;
import java.util.List;

import com.paleon.engine.components.Text;
import com.paleon.engine.toolbox.MathUtils;

public class TextMeshCreator {

	protected static final double LINE_HEIGHT = 0.03f;
    protected static final int SPACE_ASCII = 32;
 
    private MetaFile metaData;
 
    protected TextMeshCreator(File metaFile) {
        metaData = new MetaFile(metaFile);
    }
 
    protected TextMeshData createTextMesh(Text text) {
        List<Line> lines = createStructure(text);
        TextMeshData data = createQuadVertices(text, lines);
        return data;
    }
 
    private List<Line> createStructure(Text text) {
        char[] chars = text.text.toCharArray();
        List<Line> lines = new ArrayList<Line>();
        Line currentLine = new Line(metaData.getSpaceWidth(), text.size, text.lineMaxSize);
        Word currentWord = new Word(text.size);
        for (char c : chars) {
            int ascii = (int) c;
            if (ascii == SPACE_ASCII) {
                boolean added = currentLine.attemptToAddWord(currentWord);
                if (!added) {
                    lines.add(currentLine);
                    currentLine = new Line(metaData.getSpaceWidth(), text.size, text.lineMaxSize);
                    currentLine.attemptToAddWord(currentWord);
                }
                currentWord = new Word(text.size);
                continue;
            }
            Character character = metaData.getCharacter(ascii);
            currentWord.addCharacter(character);
        }
        completeStructure(lines, currentLine, currentWord, text);
        return lines;
    }
 
    private void completeStructure(List<Line> lines, Line currentLine, Word currentWord, Text text) {
        boolean added = currentLine.attemptToAddWord(currentWord);
        if (!added) {
            lines.add(currentLine);
            currentLine = new Line(metaData.getSpaceWidth(), text.size, text.lineMaxSize);
            currentLine.attemptToAddWord(currentWord);
        }
        lines.add(currentLine);
    }
 
    private TextMeshData createQuadVertices(Text text, List<Line> lines) {
        text.numberOfLines = lines.size();
        double curserX = 0f;
        double curserY = 0f;
        List<Float> vertices = new ArrayList<Float>();
        List<Float> textureCoords = new ArrayList<Float>();
        for (Line line : lines) {
            if (text.centered) {
                curserX = (line.getMaxLength() - line.getLineLength()) / 2;
            }
            for (Word word : line.getWords()) {
                for (Character letter : word.getCharacters()) {
                    addVerticesForCharacter(curserX, curserY, letter, text.size, vertices);
                    addTexCoords(textureCoords, letter.getxTextureCoord(), letter.getyTextureCoord(),
                            letter.getXMaxTextureCoord(), letter.getYMaxTextureCoord());
                    curserX += letter.getxAdvance() * text.size;
                }
                curserX += metaData.getSpaceWidth() * text.size;
            }
            curserX = 0;
            curserY += LINE_HEIGHT * text.size;
        }      
        
        return new TextMeshData(MathUtils.joinArrays(listToArray(vertices), listToArray(textureCoords)));
    }
 
    private void addVerticesForCharacter(double curserX, double curserY, Character character, double fontSize,
            List<Float> vertices) {
        double x = curserX + (character.getxOffset() * fontSize);
        double y = curserY + (character.getyOffset() * fontSize);
        double maxX = x + (character.getSizeX() * fontSize);
        double maxY = y + (character.getSizeY() * fontSize);
        double properX = (2 * x) - 1;
        double properY = (-2 * y) + 1;
        double properMaxX = (2 * maxX) - 1;
        double properMaxY = (-2 * maxY) + 1;
        addVertices(vertices, properX, properY, properMaxX, properMaxY);
    }
 
    private static void addVertices(List<Float> vertices, double x, double y, double maxX, double maxY) {
        vertices.add((float) x);
        vertices.add((float) y);
        vertices.add((float) x);
        vertices.add((float) maxY);
        vertices.add((float) maxX);
        vertices.add((float) maxY);
        vertices.add((float) maxX);
        vertices.add((float) maxY);
        vertices.add((float) maxX);
        vertices.add((float) y);
        vertices.add((float) x);
        vertices.add((float) y);
    }
 
    private static void addTexCoords(List<Float> texCoords, double x, double y, double maxX, double maxY) {
        texCoords.add((float) x);
        texCoords.add((float) y);
        texCoords.add((float) x);
        texCoords.add((float) maxY);
        texCoords.add((float) maxX);
        texCoords.add((float) maxY);
        texCoords.add((float) maxX);
        texCoords.add((float) maxY);
        texCoords.add((float) maxX);
        texCoords.add((float) y);
        texCoords.add((float) x);
        texCoords.add((float) y);
    }
 
     
    private static float[] listToArray(List<Float> listOfFloats) {
        float[] array = new float[listOfFloats.size()];
        for (int i = 0; i < array.length; i++) {
            array[i] = listOfFloats.get(i);
        }
        return array;
    }
	
}
