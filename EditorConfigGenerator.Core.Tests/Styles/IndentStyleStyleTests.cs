﻿using EditorConfigGenerator.Core.Statistics;
using EditorConfigGenerator.Core.Styles;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NUnit.Framework;
using System;

namespace EditorConfigGenerator.Core.Tests.Styles
{
	[TestFixture]
	public static class IndentStyleStyleTests
	{
		[Test]
		public static void CreateWithNoData()
		{
			var data = new TabSpaceData();
			var style = new IndentStyleStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo(string.Empty), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithMoreSpaceData()
		{
			var data = new TabSpaceData(1u, 0u, 1u);
			var style = new IndentStyleStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo("indent_style = space"), nameof(style.GetSetting));
		}

		[Test]
		public static void CreateWithMoreTabData()
		{
			var data = new TabSpaceData(1u, 1u, 0u);
			var style = new IndentStyleStyle(data);
			Assert.That(style.Data, Is.SameAs(data), nameof(style.Data));
			Assert.That(style.GetSetting(), Is.EqualTo("indent_style = tab"), nameof(style.GetSetting));
		}

		[Test]
		public static void Add()
		{
			var style1 = new IndentStyleStyle(new TabSpaceData(1u, 2u, 3u));
			var style2 = new IndentStyleStyle(new TabSpaceData(10u, 20u, 30u));
			var style3 = style1.Add(style2);

			var data = style3.Data;
			Assert.That(data.TotalOccurences, Is.EqualTo(11u), nameof(data.TotalOccurences));
			Assert.That(data.TabOccurences, Is.EqualTo(22u), nameof(data.TabOccurences));
			Assert.That(data.SpaceOccurences, Is.EqualTo(33u), nameof(data.SpaceOccurences));
		}

		[Test]
		public static void AddWithNull()
		{
			var style = new IndentStyleStyle(new TabSpaceData());
			Assert.That(() => style.Add(null), Throws.TypeOf<ArgumentNullException>());
		}

		[Test]
		public static void UpdateWithNull()
		{
			var data = new TabSpaceData(default, default, default);
			var style = new IndentStyleStyle(data);

			Assert.That(() => style.Update(null), Throws.TypeOf<ArgumentNullException>(), nameof(style.Update));
		}

		[Test]
		public static void UpdateWithTabIndentation()
		{
			var style = new IndentStyleStyle(new TabSpaceData(default, default, default));

			var statement = SyntaxFactory.ParseStatement("	var x = 0;", options: Shared.ParseOptions) as LocalDeclarationStatementSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TabOccurences, Is.EqualTo(1u), nameof(data.TabOccurences));
			Assert.That(data.SpaceOccurences, Is.EqualTo(0u), nameof(data.SpaceOccurences));
		}

		[Test]
		public static void UpdateWithSpaceIndentation()
		{
			var style = new IndentStyleStyle(new TabSpaceData(default, default, default));

			var statement = SyntaxFactory.ParseStatement("   var x = 0;", options: Shared.ParseOptions) as LocalDeclarationStatementSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(1u), nameof(data.TotalOccurences));
			Assert.That(data.TabOccurences, Is.EqualTo(0u), nameof(data.TabOccurences));
			Assert.That(data.SpaceOccurences, Is.EqualTo(1u), nameof(data.SpaceOccurences));
		}

		[Test]
		public static void UpdateWithNoIndentation()
		{
			var style = new IndentStyleStyle(new TabSpaceData(default, default, default));

			var statement = SyntaxFactory.ParseStatement("var x = 0;", options: Shared.ParseOptions) as LocalDeclarationStatementSyntax;
			var newStyle = style.Update(statement);

			var data = newStyle.Data;
			Assert.That(newStyle, Is.Not.SameAs(style), nameof(newStyle));
			Assert.That(data.TotalOccurences, Is.EqualTo(0u), nameof(data.TotalOccurences));
			Assert.That(data.TabOccurences, Is.EqualTo(0u), nameof(data.TabOccurences));
			Assert.That(data.SpaceOccurences, Is.EqualTo(0u), nameof(data.SpaceOccurences));
		}
	}
}