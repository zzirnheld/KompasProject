using Godot;
using Kompas.Cards.Loading;

namespace Kompas.Test.Integration;

public class TestFileLoader : IFileLoader
{
	public string? LoadFileAsText(string path)
	{
		var localPath = path.Replace("res://", "./../../../../../Kompas/");

		return File.ReadAllText(localPath);
	}

	public Texture2D? LoadSprite(string cardFileName) => null;
}