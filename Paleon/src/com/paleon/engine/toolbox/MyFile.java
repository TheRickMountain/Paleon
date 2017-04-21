package com.paleon.engine.toolbox;

import java.io.BufferedReader;
import java.io.InputStream;
import java.io.InputStreamReader;

public class MyFile {
	
	private static final String FILE_SEPARATOR = "/";

	private String path;
	private String name;

	public MyFile(String path) {
		this.path = FILE_SEPARATOR + path;
		String[] dirs = path.split(FILE_SEPARATOR);
		this.name = dirs[dirs.length - 1];
	}

	public MyFile(String... paths) {
		this.path = "";
		for (String part : paths) {
			this.path += (FILE_SEPARATOR + part);
		}
		String[] dirs = path.split(FILE_SEPARATOR);
		this.name = dirs[dirs.length - 1];
	}
	
	public InputStream getInputStream() {
		return Class.class.getResourceAsStream(path);
	}
	
	public BufferedReader getReader() {
		try {
			InputStreamReader isr = new InputStreamReader(getInputStream());
			BufferedReader reader = new BufferedReader(isr);
			return reader;
		} catch (Exception e) {
			System.err.println("Couldn't get reader for " + path);
			throw e;
		}
	}

	public String getPath() {
		return path;
	}

	public String getName() {
		return name;
	}
	
}
