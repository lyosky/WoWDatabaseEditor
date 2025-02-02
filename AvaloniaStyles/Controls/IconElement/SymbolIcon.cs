﻿#nullable disable
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using TextBlock = Avalonia.Controls.TextBlock;

namespace AvaloniaStyles.Controls;

/// <summary>
/// Represents an icon that uses a glyph from the SymbolThemeFontFamily resource as its content.
/// </summary>
public class SymbolIcon : FAIconElement
{
    static SymbolIcon()
    {
        FontSizeProperty.OverrideDefaultValue<SymbolIcon>(18d);
        _symbolFontFamily = new FontFamily("avares://AvaloniaStyles/Fonts#Symbols");
    }

    /// <summary>
    /// Defines the <see cref="Symbol"/> property
    /// </summary>
    public static readonly StyledProperty<Symbol> SymbolProperty =
        AvaloniaProperty.Register<SymbolIcon, Symbol>(nameof(Symbol));

    /// <summary>
    /// Defines the <see cref="FontSize"/> property
    /// </summary>
    public static readonly StyledProperty<double> FontSizeProperty =
        TextBlock.FontSizeProperty.AddOwner<SymbolIcon>();

    /// <summary>
    /// Gets or sets the <see cref="Controls.Symbol"/> this icon displays
    /// </summary>
    public Symbol Symbol
    {
        get => GetValue(SymbolProperty);
        set => SetValue(SymbolProperty, value);
    }

    /// <summary>
    /// Gets or sets the font size this icon uses when rendering
    /// </summary>
    public double FontSize
    {
        get => GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TextBlock.FontSizeProperty ||
            change.Property == SymbolProperty)
        {
            _textLayout = null;
            InvalidateMeasure();
        }
        else if (change.Property == TextBlock.ForegroundProperty)
        {
            _textLayout = null;  
            // FAIconElement calls InvalidateVisual
        }
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        // Force invalidation of text for inherited properties now that we've attached to the tree
        if (_textLayout != null)
            GenerateText();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (_textLayout == null)
            GenerateText();

        return _textLayout?.Size ?? default;
    }

    public override void Render(DrawingContext context)
    {
        if (_textLayout == null)
            GenerateText();

        var dstRect = new Rect(Bounds.Size);
        using (context.PushClip(dstRect))
        {
            var pt = new Point(dstRect.Center.X - _textLayout.Size.Width / 2,
                dstRect.Center.Y - _textLayout.Size.Height / 2);
            using var _ = context.PushPostTransform(Matrix.CreateTranslation(pt));
            _textLayout.Draw(context); // , pt todo Avalonia 11
        }
    }

    private void GenerateText()
    {
        var glyph = char.ConvertFromUtf32((int)Symbol).ToString();

        _textLayout = new TextLayout(glyph,
            new Typeface(_symbolFontFamily),
           FontSize, Foreground, TextAlignment.Left);
    }

    private TextLayout _textLayout;
    private static FontFamily _symbolFontFamily;
}
