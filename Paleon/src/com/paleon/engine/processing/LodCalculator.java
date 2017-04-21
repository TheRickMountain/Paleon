package com.paleon.engine.processing;

import com.paleon.engine.items.Camera;
import com.paleon.engine.terrain.Terrain;
import com.paleon.engine.terrain.TerrainBlock;
import com.paleon.engine.toolbox.MathUtils;

public class LodCalculator {

	private final static float TERRAIN_MIN = 200;
	private final static float TERRAIN_LOG_MIN = (float) Math.log(TERRAIN_MIN);
	private final static float TERRAIN_LOG_INCREASE = (float) Math.log(2);
	
	public static void calculateTerrainLOD(TerrainBlock block, Camera camera) {
		float distance = MathUtils.getDistanceBetweenPoints(camera.getPosition().x, camera.getPosition().z, block.getX(),
				block.getZ());
		block.setDisFromCamera(distance);
		int lod;
		if (distance > TERRAIN_MIN) {
			float lodFloat = (float) ((Math.log(distance) - TERRAIN_LOG_MIN) / TERRAIN_LOG_INCREASE);
			lod = (int) (Math.floor(lodFloat) + 1);
		} else {
			lod = 0;
		}
		if (lod >= Terrain.NUM_OF_LODS) {
			block.setLOD(Terrain.NUM_OF_LODS - 1);
		}else{
			block.setLOD(lod);
		}
	}
	
}
