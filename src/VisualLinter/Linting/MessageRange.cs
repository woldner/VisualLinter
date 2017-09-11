namespace jwldnr.VisualLinter.Linting
{
    internal class MessageRange
    {
        public int ColumnEnd { get; set; }

        public int ColumnStart { get; set; }

        public int LineEnd { get; set; }

        public int LineStart { get; set; }
    }
}