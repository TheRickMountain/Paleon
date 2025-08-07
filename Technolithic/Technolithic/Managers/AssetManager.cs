using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Technolithic
{
    public static class AssetManager
    {
        private static Dictionary<string, MyTexture> textures = new Dictionary<string, MyTexture>();

        private static List<string> allFilesPaths = new List<string>();

        public static void LoadContent(GraphicsDevice graphicsDevice, ContentManager contentManager, string contentDirectory)
        {
            DirectoryInfo rootDirectoryInfo = new DirectoryInfo(contentDirectory);

            SearchAllFiles(rootDirectoryInfo.FullName, rootDirectoryInfo);

            LoadAllFiles(graphicsDevice, contentManager);
        }

        public static MyTexture GetTexture(params string[] path)
        {
            string finalPath = Path.Combine(path);

            if (!textures.ContainsKey(finalPath))
            {
                Debug.WriteLine($"Texture not found: {finalPath}");
                return null;
            }

            return textures[finalPath];
        }

        private static void SearchAllFiles(string rootDirectory, DirectoryInfo dir)
        {
            FileInfo[] files = dir.GetFiles("*.xnb");
            foreach (FileInfo file in files)
            {
                string relativeFilePath = Path.GetRelativePath(rootDirectory, file.FullName);
                string relativeFilePathWithoutExtension = relativeFilePath.Replace(".xnb", "");
                allFilesPaths.Add(relativeFilePathWithoutExtension);
            }

            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                SearchAllFiles(rootDirectory, subDir);
            }
        }

        private static void LoadAllFiles(GraphicsDevice graphicsDevice, ContentManager contentManager)
        {
            Debug.WriteLine("Loading: textures, sound effects");
            foreach (var filePath in allFilesPaths)
            {
                object file = contentManager.Load<object>(filePath);

                if (file is Texture2D rawTexture)
                {
                    LoadTexture(rawTexture, filePath);
                }
            }
        }

        private static void LoadTexture(Texture2D rawTexture, string texturePath)
        {
            textures.Add(texturePath, new MyTexture(rawTexture));
        }
    }
}
