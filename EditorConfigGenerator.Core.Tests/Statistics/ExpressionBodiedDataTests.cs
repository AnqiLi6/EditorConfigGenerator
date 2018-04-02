﻿using EditorConfigGenerator.Core.Statistics;
using NUnit.Framework;
using Spackle;
using System;

namespace EditorConfigGenerator.Core.Tests.Statistics
{
	[TestFixture]
	public static class ExpressionBodiedDataTests
	{
		[Test]
		public static void Create()
		{
			var generator = new RandomObjectGenerator();
			var totalOccurences = generator.Generate<uint>();
			var arrowSingleLineOccurences = generator.Generate<uint>();
			var arrowMultiLineOccurences = generator.Generate<uint>();
			var blockOccurences = generator.Generate<uint>();

			var data = new ExpressionBodiedData(totalOccurences, arrowSingleLineOccurences, arrowMultiLineOccurences,
				blockOccurences);

			Assert.That(data.TotalOccurences, Is.EqualTo(totalOccurences), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(arrowSingleLineOccurences), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(arrowMultiLineOccurences), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockOccurences, Is.EqualTo(blockOccurences), nameof(data.BlockOccurences));
		}

		[Test]
		public static void CreateDefault()
		{
			var data = new ExpressionBodiedData();
			Assert.That(data.TotalOccurences, Is.EqualTo(default(uint)), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(default(uint)), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(default(uint)), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockOccurences, Is.EqualTo(default(uint)), nameof(data.BlockOccurences));
		}

		[Test]
		public static void Add()
		{
			var data1 = new ExpressionBodiedData(10u, 1u, 2u, 3u);
			var data2 = new ExpressionBodiedData(100u, 10u, 20u, 30u);
			var data3 = data1.Add(data2);

			Assert.That(data3.TotalOccurences, Is.EqualTo(110u), nameof(data3.TotalOccurences));
			Assert.That(data3.ArrowSingleLineOccurences, Is.EqualTo(11u), nameof(data3.ArrowSingleLineOccurences));
			Assert.That(data3.ArrowMultiLineOccurences, Is.EqualTo(22u), nameof(data3.ArrowMultiLineOccurences));
			Assert.That(data3.BlockOccurences, Is.EqualTo(33u), nameof(data3.BlockOccurences));
		}

		[Test]
		public static void AddWithNull()
		{
			var data = new ExpressionBodiedData();
			Assert.That(() => data.Add(null), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public static void GetSettingWithNoOccurrences()
		{
			var data = new ExpressionBodiedData(0u, 0u, 0u, 0u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo(string.Empty), nameof(data.GetSetting));
		}

		[Test]
		public static void GetSettingWithArrows()
		{
			var data = new ExpressionBodiedData(2u, 1u, 1u, 0u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = true:error"), nameof(data.GetSetting));
		}

		[Test]
		public static void GetSettingWithBlocks()
		{
			var data = new ExpressionBodiedData(1u, 0u, 0u, 1u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = false:error"), nameof(data.GetSetting));
		}

		[Test]
		public static void GetSettingWithSingleLine()
		{
			var data = new ExpressionBodiedData(4u, 3u, 1u, 0u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = true:error"), nameof(data.GetSetting));
		}

		[Test]
		public static void GetSettingWithFavoritingArrows()
		{
			var data = new ExpressionBodiedData(4u, 3u, 1u, 1u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = true:error"), nameof(data.GetSetting));
		}

		[Test]
		public static void GetSettingWithFavoritingBlocks()
		{
			var data = new ExpressionBodiedData(4u, 1u, 0u, 3u);
			Assert.That(data.GetSetting("x", Severity.Error), Is.EqualTo("x = false:error"), nameof(data.GetSetting));
		}

		[Test]
		public static void UpdateWithArrowSingleLine()
		{
			var data = new ExpressionBodiedData();
			data = data.Update(ExpressionBodiedDataOccurence.ArrowSingleLine);

			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(1u), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(0u), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockOccurences, Is.EqualTo(0u), nameof(data.BlockOccurences));
		}

		[Test]
		public static void UpdateWithArrowMultiLine()
		{
			var data = new ExpressionBodiedData();
			data = data.Update(ExpressionBodiedDataOccurence.ArrowMultiLine);

			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(0u), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(1u), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockOccurences, Is.EqualTo(0u), nameof(data.BlockOccurences));
		}

		[Test]
		public static void UpdateWithBlock()
		{
			var data = new ExpressionBodiedData();
			data = data.Update(ExpressionBodiedDataOccurence.Block);

			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.ArrowSingleLineOccurences, Is.EqualTo(0u), nameof(data.ArrowSingleLineOccurences));
			Assert.That(data.ArrowMultiLineOccurences, Is.EqualTo(0u), nameof(data.ArrowMultiLineOccurences));
			Assert.That(data.BlockOccurences, Is.EqualTo(1u), nameof(data.BlockOccurences));
		}
	}
}
