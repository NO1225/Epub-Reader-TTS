using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Epub_Reader_TTS
{

    [TemplatePart(Name = IndexhighlighttextblockName, Type = typeof(TextBlock))]
    public class IndexHighlightTextBlock : Control
    {
        private const string IndexhighlighttextblockName = "PART_IndexhighlightTextblock";

        public static readonly DependencyProperty HighlightIndexProperty =
            DependencyProperty.Register("HighlightIndex", typeof(int), typeof(IndexHighlightTextBlock),
                new PropertyMetadata(0, OnHighlightIndexPropertyChanged));

        public static readonly DependencyProperty HighlightLengthProperty =
            DependencyProperty.Register("HighlightLength", typeof(int), typeof(IndexHighlightTextBlock),
                new PropertyMetadata(0, OnHighlightIndexPropertyChanged));

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register("IsActive", typeof(bool), typeof(IndexHighlightTextBlock),
                new PropertyMetadata(false, IsActivePropertyChanged));

        public static readonly DependencyProperty TextProperty =
            TextBlock.TextProperty.AddOwner(
                typeof(IndexHighlightTextBlock),
                new PropertyMetadata(string.Empty, OnTextPropertyChanged));

        public static readonly DependencyProperty TextWrappingProperty = TextBlock.TextWrappingProperty.AddOwner(
            typeof(IndexHighlightTextBlock),
            new PropertyMetadata(TextWrapping.NoWrap));
        
        public static readonly DependencyProperty TextTrimmingProperty = TextBlock.TextTrimmingProperty.AddOwner(
            typeof(IndexHighlightTextBlock),
            new PropertyMetadata(TextTrimming.None));

        public static readonly DependencyProperty TextAlignmentProperty =
            DependencyProperty.Register("TextAlignment", typeof(TextAlignment),
                typeof(IndexHighlightTextBlock), 
                new PropertyMetadata(TextAlignment.Left));

        public static readonly DependencyProperty HighlightForegroundProperty =
            DependencyProperty.Register("HighlightForeground", typeof(Brush),
                typeof(IndexHighlightTextBlock),
                new PropertyMetadata(Brushes.White));

        public static readonly DependencyProperty HighlightBackgroundProperty =
            DependencyProperty.Register("HighlightBackground", typeof(Brush),
                typeof(IndexHighlightTextBlock),
                new PropertyMetadata(Brushes.Blue));

        private TextBlock highlightTextBlock;

        static IndexHighlightTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(IndexHighlightTextBlock),
                new FrameworkPropertyMetadata(typeof(IndexHighlightTextBlock)));
        }

        public Brush HighlightBackground
        {
            get => (Brush)GetValue(HighlightBackgroundProperty);
            set => SetValue(HighlightBackgroundProperty, value);
        }

        public Brush HighlightForeground
        {
            get => (Brush)GetValue(HighlightForegroundProperty);
            set => SetValue(HighlightForegroundProperty, value);
        }

        public int HighlightIndex
        {
            get => (int)GetValue(HighlightIndexProperty);
            set => SetValue(HighlightIndexProperty, value);
        }

        public int HighlightLength
        {
            get => (int)GetValue(HighlightLengthProperty);
            set => SetValue(HighlightLengthProperty, value);
        }
        
        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public TextWrapping TextWrapping
        {
            get => (TextWrapping)GetValue(TextWrappingProperty);
            set => SetValue(TextWrappingProperty, value);
        }

        public TextTrimming TextTrimming
        {
            get => (TextTrimming)GetValue(TextTrimmingProperty);
            set => SetValue(TextTrimmingProperty, value);
        }
        
        public TextAlignment TextAlignment
        {
            get => (TextAlignment)GetValue(TextAlignmentProperty);
            set => SetValue(TextAlignmentProperty, value);
        }

        private static void IsActivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textblock = (IndexHighlightTextBlock)d;

            if ((bool)e.NewValue)
                textblock.BringIntoView();

            textblock.ProcessIndexChanged(textblock.Text);

        }

        private static void OnHighlightIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textblock = (IndexHighlightTextBlock)d;

            textblock.ProcessIndexChanged(textblock.Text);
        }

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var textblock = (IndexHighlightTextBlock)d;
            textblock.ProcessIndexChanged(e.NewValue as string);
        }

        private void ProcessIndexChanged(string mainText)
        {
            var highlightIndex = HighlightIndex;

            var highlightLength = HighlightLength;

            var isActive = IsActive;

            if (highlightTextBlock == null)
                return;
            highlightTextBlock.Inlines.Clear();

            if (highlightTextBlock == null || string.IsNullOrWhiteSpace(mainText)) return;

            if (highlightLength == 0 || highlightIndex > mainText.Length - highlightLength)
            {
                var completeRun = new Run(mainText);
                highlightTextBlock.Inlines.Add(completeRun);
                return;
            }

            highlightTextBlock.Inlines.Add(GetRunForText(
                mainText.Substring(0, highlightIndex), false));

            highlightTextBlock.Inlines.Add(GetRunForText(
                mainText.Substring(highlightIndex, highlightLength), true));

            highlightTextBlock.Inlines.Add(GetRunForText(
                mainText.Substring(highlightIndex + highlightLength, mainText.Length - (highlightIndex + highlightLength)), false));

            if (isActive)
                BringIntoView();
        }

        private Run GetRunForText(string text, bool isHighlighted)
        {
            var textRun = new Run(text)
            {
                Foreground = isHighlighted ? HighlightForeground : Foreground,
                Background = isHighlighted ? HighlightBackground : Background
            };
            return textRun;
        }

        public override void OnApplyTemplate()
        {
            highlightTextBlock = GetTemplateChild(IndexhighlighttextblockName) as TextBlock;
            if (highlightTextBlock == null)
                return;
            ProcessIndexChanged(Text);
        }
    }
}
