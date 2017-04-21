package com.paleon.engine.components;

import com.paleon.engine.toolbox.Color;

public class Image extends Component {

	public int textureId;
	public Color color;
	
	public Image(int textureId, Color color) {
		this.textureId = textureId;
		this.color = color;
	}
	
}
