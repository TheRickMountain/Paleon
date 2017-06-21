package com.paleon.toolbox;

import java.util.ArrayList;
import java.util.List;

public class Utils {

	public static float getAverageOfList(List<Float> numbers) {
        float total = 0;
        for (Float number : numbers) {
            total += number;
        }
        return total / numbers.size();
    }
	
	public static String [] removeEmptyStrings(String[] data) {
        ArrayList<String> result = new ArrayList<String>();

        for (int i = 0; i < data.length; i++)
            if(!data[i].equals(""))
                result.add(data[i]);

        String[] res = new String[result.size()];
        result.toArray(res);

        return res;
    }
	
}
