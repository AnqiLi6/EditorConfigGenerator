﻿using EditorConfigGenerator.Core.Statistics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace EditorConfigGenerator.Core.Extensions
{
	internal static class SyntaxNodeExtensions
	{
		internal static ExpressionBodiedData Examine(this SyntaxNode @this, ExpressionBodiedData current)
		{
			if (@this == null) { throw new ArgumentNullException(nameof(@this)); }
			if (current == null) { throw new ArgumentNullException(nameof(current)); }

			if (!@this.ContainsDiagnostics)
			{
				var data = current;
				var arrows = @this.ChildNodes().Where(_ => _.Kind() == SyntaxKind.ArrowExpressionClause)
					.Cast<ArrowExpressionClauseSyntax>().ToImmutableArray();

				foreach (var arrow in arrows)
				{
					var lines = arrow.DescendantTrivia().Count(
						_ => _.Kind() == SyntaxKind.EndOfLineTrivia);
					var occurence = lines >= 1 ? ExpressionBodiedDataOccurence.ArrowMultiLine :
						ExpressionBodiedDataOccurence.ArrowSingleLine;
					data = data.Update(occurence);
				}

				var singleLineBlockCount = (uint)@this.ChildNodes().Where(_ => _.Kind() == SyntaxKind.Block)
					.Cast<BlockSyntax>().Count(
						bs => bs.DescendantNodes().Count(_ => typeof(StatementSyntax).IsAssignableFrom(_.GetType())) == 1);

				for (var i = 0; i < singleLineBlockCount; i++)
				{
					data = data.Update(ExpressionBodiedDataOccurence.Block);
				}

				return data;
			}
			else
			{
				return current;
			}
		}

		internal static T FindParent<T>(this SyntaxNode @this)
			where T : SyntaxNode
		{
			var parent = @this.Parent;

			while (!(parent is T) && parent != null)
			{
				parent = parent.Parent;
			}

			return parent as T;
		}

		internal static bool HasParenthesisSpacing(this SyntaxNode @this)
		{
			var children = @this.ChildNodesAndTokens().ToArray();
			var openParen = (SyntaxToken?)children.FirstOrDefault(_ => _.IsKind(SyntaxKind.OpenParenToken));
			var closeParen = (SyntaxToken?)children.LastOrDefault(_ => _.IsKind(SyntaxKind.CloseParenToken));

			var hasSpaceAfterOpenParen = openParen != null && openParen.Value.HasTrailingTrivia &&
				openParen.Value.TrailingTrivia.Any(_ => _.IsKind(SyntaxKind.WhitespaceTrivia));

			if (hasSpaceAfterOpenParen && closeParen != null)
			{
				var previousNodeIndex = Array.IndexOf(children, closeParen.Value) - 1;
				var lastNodeOrToken = SyntaxNodeExtensions.GetLastNodeOrToken(children[previousNodeIndex]);

				var hasSpaceBeforeCloseParen = lastNodeOrToken.HasTrailingTrivia &&
					lastNodeOrToken.GetTrailingTrivia().Any(_ => _.IsKind(SyntaxKind.WhitespaceTrivia));

				return hasSpaceBeforeCloseParen;
			}

			return false;
		}

		private static SyntaxNodeOrToken GetLastNodeOrToken(SyntaxNodeOrToken nodeOrToken)
		{
			SyntaxNodeOrToken last = default;
			var children = nodeOrToken.ChildNodesAndTokens().ToArray();

			while (children.Length > 0)
			{
				last = children[children.Length - 1];
				children = last.ChildNodesAndTokens().ToArray();
			}

			return last;
		}
	}
}
