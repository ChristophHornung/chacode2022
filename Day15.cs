using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Chacode2022;

internal class Day15 : DayX
{
	private static bool part1;

	public void Solve(int part)
	{
		part1 = part == 1;
		using var reader = GetInput();
		List<Sensor> sensors = new();
		while (!reader.EndOfStream)
		{
			var line = reader.ReadLine()!;
			sensors.Add(Sensor.Parse(line, null));
		}

		var beacons = sensors.Select(s => s.ClosestBeacon).Distinct().ToHashSet();
		var maxX = sensors.SelectMany(s => new[] {s.Position.X + s.BeaconDistance, s.ClosestBeacon.X}).Max();
		var minX = sensors.SelectMany(s => new[] {s.Position.X - s.BeaconDistance, s.ClosestBeacon.X}).Min();

		var lineToCheck = 2_000_000;
		var coveredPositions = 0;
		for (var x = minX; x < maxX; x++)
		{
			var p = new Point(x, lineToCheck);
			var found = sensors.Any(sensor => sensor.BeaconDistance >= sensor.Position.GetDistance(p));
			if (found && !beacons.Contains(p)) coveredPositions++;
		}

		Point? beacon = null;

		for (var y = 0; y < 4_000_000; y++)
		for (var x = 0; x < 4_000_000; x++)
		{
			var pBeacon = new Point(x, y);
			var inRangeOfSensor =
				sensors.FirstOrDefault(sensor => sensor.BeaconDistance >= sensor.Position.GetDistance(pBeacon));
			if (inRangeOfSensor == null)
			{
				beacon = pBeacon;
				break;
			}
			else
			{
				// We are in range of a sensor, and we try to skip it
				// We will always have the same Y-distance
				var yDistance = Math.Abs(inRangeOfSensor.Position.Y - pBeacon.Y);

				// Since we are in range now we can skip until the max X range
				var newX = inRangeOfSensor.Position.X + inRangeOfSensor.BeaconDistance - yDistance;
				Debug.Assert(newX >= x);
				x = newX;
			}
		}

		ReportResult($"{coveredPositions} | {beacon!.X * 4_000_000L + beacon.Y}");
	}

	private record Sensor(Point Position, Point ClosestBeacon) : IParsable<Sensor>
	{
		public int BeaconDistance => Position.GetDistance(ClosestBeacon);

		public static Sensor Parse(string s, IFormatProvider? provider)
		{
			var reg = new Regex(
				"Sensor at x=(-?[0-9]*), y=(-?[0-9]*): closest beacon is at x=(-?[0-9]*), y=(-?[0-9]*)");
			var match = reg.Match(s);
			var sensor = new Sensor(
				new Point(int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value)),
				new Point(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[4].Value))
			);
			return sensor;
		}

		public static bool TryParse(string? s, IFormatProvider? provider, out Sensor result)
		{
			throw new NotImplementedException();
		}
	}

	internal record Point(int X, int Y);
}

internal static class DistanceHelper
{
	public static int GetDistance(this Day15.Point a, Day15.Point b)
	{
		return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
	}
}