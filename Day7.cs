using System.Text.RegularExpressions;

namespace Chacode2022;

internal class Day7 : DayX
{
	public void Solve()
	{
		using StreamReader reader = this.GetInput();

		ElfDirectory root = new ElfDirectory("/", null);
		ElfDirectory current = root;
		while (!reader.EndOfStream)
		{
			current = this.Parse(reader, root, current);
		}

		List<ElfDirectory> smallDirectories = root.GetRecursiveDirectories().Where(r => r.FullSize <= 100000).ToList();
		List<ElfDirectory> allDirectories = root.GetRecursiveDirectories().ToList();

		long sizeFree = 70_000_000 - root.FullSize;

		long sizeRequired = 30_000_000 - sizeFree;

		Console.WriteLine(
			$"Day 7: {smallDirectories.Sum(s => s.FullSize)} | {allDirectories.OrderBy(c => c.FullSize).First(c => c.FullSize >= sizeRequired).FullSize}");
	}

	private ElfDirectory Parse(StreamReader reader, ElfDirectory root, ElfDirectory current)
	{
		string line = reader.ReadLine()!;
		Regex cdRegex = new Regex("\\$ cd (.*)");
		Match cdMatch = cdRegex.Match(line);
		if (cdMatch.Success)
		{
			string target = cdMatch.Groups[1].Value;
			switch (target)
			{
				case "/":
					return root;
				case "..":
					return current.Parent ?? current;
				default:
				{
					ElfDirectory? subDir = current.SubDirectories.FirstOrDefault(n => n.Name == target);
					if (subDir == null)
					{
						subDir = new ElfDirectory(target, current);
						current.SubDirectories.Add(subDir);
					}

					return subDir;
				}
			}
		}

		Regex lsRegex = new Regex("\\$ ls");
		Match lsMatch = lsRegex.Match(line);
		if (!lsMatch.Success)
		{
			throw new InvalidOperationException();
		}

		while (!reader.EndOfStream && reader.Peek() != '$')
		{
			line = reader.ReadLine()!;
			if (line.StartsWith("dir "))
			{
				ElfDirectory subDir = new ElfDirectory(line[4..], current);
				current.SubDirectories.Add(subDir);
			}
			else
			{
				string[] fileInfo = line.Split(' ');
				ElfFile file = new(fileInfo[1], long.Parse(fileInfo[0]));
				current.Files.Add(file);
			}
		}

		return current;
	}

	private class ElfDirectory
	{
		public ElfDirectory(string name, ElfDirectory? parent)
		{
			this.Name = name;
			this.SubDirectories = new List<ElfDirectory>();
			this.Files = new List<ElfFile>();
			this.Parent = parent;
		}

		public string Name { get; }
		public List<ElfDirectory> SubDirectories { get; }
		public ElfDirectory? Parent { get; }
		public List<ElfFile> Files { get; }

		public long FullSize
		{
			get { return this.SubDirectories.Sum(s => s.FullSize) + this.Files.Sum(f => f.Size); }
		}

		public IEnumerable<ElfDirectory> GetRecursiveDirectories()
		{
			yield return this;
			foreach (ElfDirectory subDirectory in this.SubDirectories.SelectMany(s => s.GetRecursiveDirectories()))
			{
				yield return subDirectory;
			}
		}
	}

	private record ElfFile(string Name, long Size);
}